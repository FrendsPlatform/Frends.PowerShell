using System.Collections;
using System.Diagnostics;
using System.Management.Automation;
using System.Text;
using Frends.PowerShell.RunScript.Definitions;

namespace Frends.PowerShell.RunScript.Helpers;

internal static class CustomPowerShellHandler
{

    public static PowerShellResult ExecuteCustomPowerShell(
        string target,
        PowerShellParameter[] powerShellParameters,
        bool logInformationStream,
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

        var tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(tempDirectory);

        var wrapperPath = Path.Combine(
            Path.GetDirectoryName(typeof(PowerShell).Assembly.Location)
            ?? throw new InvalidOperationException("The task assembly directory could not be resolved."),
            "Helpers",
            "CustomPowerShellWrapper.ps1");
        var parametersPath = Path.Combine(tempDirectory, "parameters.xml");
        var resultPath = Path.Combine(tempDirectory, "result.xml");
        var errorsPath = Path.Combine(tempDirectory, "errors.xml");
        var informationPath = Path.Combine(tempDirectory, "information.xml");

        try
        {
            if (!File.Exists(wrapperPath))
                throw new FileNotFoundException("The custom PowerShell wrapper script was not found.", wrapperPath);

            var parameters = new Dictionary<string, object>();

            foreach (var parameter in powerShellParameters ?? [])
            {
                cancellationToken.ThrowIfCancellationRequested();
                parameters.Add(parameter.Name.Trim('-', ' '), parameter.Value);
            }

            File.WriteAllText(parametersPath, PSSerializer.Serialize(parameters), Encoding.UTF8);

            var startInfo = new ProcessStartInfo(options.PathToCustomPowerShell)
            {
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };
            startInfo.ArgumentList.Add("-NoLogo");
            startInfo.ArgumentList.Add("-NoProfile");
            startInfo.ArgumentList.Add("-NonInteractive");
            AddExecutionPolicyArguments(startInfo, options.ExecutionPolicy);
            startInfo.ArgumentList.Add("-File");
            startInfo.ArgumentList.Add(wrapperPath);
            startInfo.ArgumentList.Add("-Target");
            startInfo.ArgumentList.Add(target);
            startInfo.ArgumentList.Add("-ParametersPath");
            startInfo.ArgumentList.Add(parametersPath);
            startInfo.ArgumentList.Add("-ResultPath");
            startInfo.ArgumentList.Add(resultPath);
            startInfo.ArgumentList.Add("-ErrorsPath");
            startInfo.ArgumentList.Add(errorsPath);
            startInfo.ArgumentList.Add("-InformationPath");
            startInfo.ArgumentList.Add(informationPath);

            using var process = new Process();
            process.StartInfo = startInfo;
            process.Start();

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

            process.WaitForExitAsync(cancellationToken).GetAwaiter().GetResult();
            var output = standardOutput.GetAwaiter().GetResult();
            var errorOutput = standardError.GetAwaiter().GetResult();
            cancellationToken.ThrowIfCancellationRequested();

            var errors = DeserializeList(errorsPath).Select(item => item?.ToString())
                .Where(item => item != null).ToList();

            if (process.ExitCode != 0)
            {
                var errorDetails = string.IsNullOrWhiteSpace(errorOutput) ? output : errorOutput;

                throw new Exception(
                    $"Encountered terminating error while executing powershell: \n{errorDetails}\nErrors:\n{string.Join("\n", errors)}");
            }

            var results = DeserializeList(resultPath)
                .Select(item => item is PSObject powerShellObject
                        ? PowerShellHandler.GetResultObject(powerShellObject)
                        : item)
                .Cast<dynamic>()
                .ToList();
            var information = logInformationStream ? DeserializeList(informationPath) : [];

            return new PowerShellResult(results, errors, string.Join("\n", information));
        }
        finally
        {
            Directory.Delete(tempDirectory, recursive: true);
        }
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

    private static IList<object> DeserializeList(string path)
    {
        if (!File.Exists(path))
            return [];

        var value = PSSerializer.Deserialize(File.ReadAllText(path));

        if (value == null)
            return [];

        if (value is PSObject { BaseObject: not PSCustomObject } powerShellObject)
            value = powerShellObject.BaseObject;

        if (value is IEnumerable enumerable and not string)
            return [.. enumerable.Cast<object>()];

        return [value];
    }


}
