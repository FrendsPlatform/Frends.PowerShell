using System.Collections;
using System.Diagnostics;
using System.Management.Automation;
using System.Text;
using Frends.PowerShell.RunScript.Definitions;

namespace Frends.PowerShell.RunScript.Helpers;

internal static class CustomPowerShellHandler
{
    private const string WrapperScript = """
        $envelopeXml = [System.Text.Encoding]::UTF8.GetString([Convert]::FromBase64String([Console]::In.ReadToEnd()))
        $envelope = [System.Management.Automation.PSSerializer]::Deserialize($envelopeXml)
        $target = $envelope['Target']
        $isScript = [bool]$envelope['IsScript']
        $parameters = $envelope['Parameters']
        if ($null -eq $parameters) { $parameters = @{} }

        $errors = @()
        $information = @()
        $terminatingError = $false

        try {
            if ($isScript) {
                $scriptBlock = [scriptblock]::Create($target)
                $allOutput = @(& $scriptBlock @parameters *>&1)
            }
            else {
                $allOutput = @(& $target @parameters *>&1)
            }

            $results = @($allOutput | Where-Object {
                if ($_ -is [System.Management.Automation.ErrorRecord]) {
                    $errors += $_
                    return $false
                }

                if ($_ -is [System.Management.Automation.InformationRecord]) {
                    $information += $_
                    return $false
                }

                return $_ -isnot [System.Management.Automation.WarningRecord] -and
                    $_ -isnot [System.Management.Automation.VerboseRecord] -and
                    $_ -isnot [System.Management.Automation.DebugRecord] -and
                    $_ -isnot [System.Management.Automation.ProgressRecord]
            })
        }
        catch {
            $errors += $_
            $results = @()
            $terminatingError = $true
        }

        $errorMessages = @($errors | ForEach-Object { "$($_.ScriptStackTrace): $($_.Exception.Message)" })
        $informationMessages = @($information | ForEach-Object { "$($_.MessageData)" })

        $payload = @{
            Results = $results
            Errors = $errorMessages
            Information = $informationMessages
        }
        $payloadXml = [System.Management.Automation.PSSerializer]::Serialize($payload, 100)
        $payloadBase64 = [Convert]::ToBase64String([System.Text.Encoding]::UTF8.GetBytes($payloadXml))

        [Console]::Out.Write("$env:FRENDS_RESULT_BOUNDARY$payloadBase64")
        [Console]::Out.Flush()

        if ($terminatingError) {
            exit 1
        }
        """;

    public static PowerShellResult ExecuteCustomPowerShell(
        string target,
        PowerShellParameter[] powerShellParameters,
        bool logInformationStream,
        bool isScript,
        RunOptions options,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (options.Session != null)
            throw new InvalidOperationException("A shared session cannot be used with a custom PowerShell executable.");

        if (string.IsNullOrWhiteSpace(options.PathToCustomPowerShell))
            throw new ArgumentException("Path to custom PowerShell must be provided.", nameof(options));

        if (!File.Exists(options.PathToCustomPowerShell))
            throw new FileNotFoundException("The custom PowerShell executable was not found.",
                options.PathToCustomPowerShell);

        var parameters = new Dictionary<string, object>();

        foreach (var parameter in powerShellParameters ?? [])
        {
            cancellationToken.ThrowIfCancellationRequested();
            parameters.Add(parameter.Name.Trim('-', ' '), parameter.Value);
        }

        var envelope = new Hashtable
        {
            ["Target"] = target,
            ["IsScript"] = isScript,
            ["Parameters"] = parameters,
        };
        var envelopeBase64 = Convert.ToBase64String(
            Encoding.UTF8.GetBytes(PSSerializer.Serialize(envelope, 100)));

        var resultBoundary = $"##FRENDS_RESULT_{Guid.NewGuid():N}##";

        var startInfo = new ProcessStartInfo(options.PathToCustomPowerShell)
        {
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };
        startInfo.Environment["FRENDS_RESULT_BOUNDARY"] = resultBoundary;
        startInfo.ArgumentList.Add("-NoLogo");
        startInfo.ArgumentList.Add("-NoProfile");
        startInfo.ArgumentList.Add("-NonInteractive");
        AddExecutionPolicyArguments(startInfo, options.ExecutionPolicy);
        startInfo.ArgumentList.Add("-EncodedCommand");
        startInfo.ArgumentList.Add(Convert.ToBase64String(Encoding.Unicode.GetBytes(WrapperScript)));

        using var process = new Process();
        process.StartInfo = startInfo;
        process.Start();

        // Start draining stdout/stderr before writing to stdin to avoid a pipe deadlock on large payloads.
        var standardOutput = process.StandardOutput.ReadToEndAsync(cancellationToken);
        var standardError = process.StandardError.ReadToEndAsync(cancellationToken);
        using var cancellationRegistration = cancellationToken.Register(() =>
        {
            try
            {
                if (!process.HasExited)
                    process.Kill(entireProcessTree: true);
            }
            catch (InvalidOperationException)
            {
                // The process exited between checking its state and attempting to stop it.
            }
        });

        process.StandardInput.Write(envelopeBase64);
        process.StandardInput.Close();

        process.WaitForExitAsync(cancellationToken).GetAwaiter().GetResult();
        var output = standardOutput.GetAwaiter().GetResult();
        var errorOutput = standardError.GetAwaiter().GetResult();
        cancellationToken.ThrowIfCancellationRequested();

        var boundaryIndex = output.IndexOf(resultBoundary, StringComparison.Ordinal);

        if (boundaryIndex < 0)
        {
            var errorDetails = string.IsNullOrWhiteSpace(errorOutput) ? output : errorOutput;
            throw new Exception($"Encountered terminating error while executing powershell: \n{errorDetails}");
        }

        var payload = DeserializePayload(output[(boundaryIndex + resultBoundary.Length)..]);

        var errors = ToObjectList(payload["Errors"]).Select(item => item?.ToString())
            .Where(item => item != null).ToList();

        if (process.ExitCode != 0)
        {
            var errorDetails = string.IsNullOrWhiteSpace(errorOutput) ? output[..boundaryIndex] : errorOutput;

            throw new Exception(
                $"Encountered terminating error while executing powershell: \n{errorDetails}\nErrors:\n{string.Join("\n", errors)}");
        }

        var results = ToObjectList(payload["Results"])
            .Select(item => item is PSObject powerShellObject
                    ? PowerShellHandler.GetResultObject(powerShellObject)
                    : item)
            .Cast<dynamic>()
            .ToList();
        var information = logInformationStream ? ToObjectList(payload["Information"]) : [];

        return new PowerShellResult(results, errors, string.Join("\n", information));
    }

    internal static void AddExecutionPolicyArguments(
        ProcessStartInfo startInfo,
        PowerShellExecutionPolicy executionPolicy)
    {
        if (executionPolicy == PowerShellExecutionPolicy.SystemDefault)
            return;

        startInfo.ArgumentList.Add("-ExecutionPolicy");
        startInfo.ArgumentList.Add(executionPolicy.ToString());
    }

    private static IDictionary DeserializePayload(string payloadBase64)
    {
        var payloadXml = Encoding.UTF8.GetString(Convert.FromBase64String(payloadBase64));
        var value = PSSerializer.Deserialize(payloadXml);

        if (value is PSObject { BaseObject: not PSCustomObject } powerShellObject)
            value = powerShellObject.BaseObject;

        return value as IDictionary
            ?? throw new InvalidOperationException("The custom PowerShell process returned an unexpected result payload.");
    }

    private static IList<object> ToObjectList(object value)
    {
        if (value == null)
            return [];

        if (value is PSObject { BaseObject: not PSCustomObject } powerShellObject)
            value = powerShellObject.BaseObject;

        if (value is IEnumerable enumerable and not string)
            return [.. enumerable.Cast<object>()];

        return [value];
    }
}
