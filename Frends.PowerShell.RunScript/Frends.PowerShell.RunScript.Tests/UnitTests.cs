using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using Frends.PowerShell.RunScript.Definitions;
using Frends.PowerShell.RunScript.Helpers;
using NUnit.Framework;

namespace Frends.PowerShell.RunScript.Tests;

[TestFixture]
public class UnitTests
{
    [Test]
    public void RunScript_ShouldRunScriptWithParameter()
    {
        var script = @"param([string]$testParam)
$testParam
write-output ""my test param: $testParam""";

        var result = PowerShell.RunScript(new RunScriptInput
        {
            Parameters = new[] { new PowerShellParameter { Name = "testParam", Value = "my test param" } },
            ReadFromFile = false,
            Script = script,
            LogInformationStream = true
        }, new RunOptions(), default);

        Assert.That(result.Result.Count, Is.EqualTo(2));
        Assert.That(result.Result.Last(), Is.EqualTo("my test param: my test param"));
    }

    private readonly string script =
        @"
new-timespan -hours 1
new-timespan -hours 2";
    private static readonly string[] expected = new[] { "-ExecutionPolicy", "RemoteSigned" };

    [Test]
    public void RunScript_ShouldRunScriptFromFile()
    {
        var scriptFilePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.ps1");
        PowerShellResult result;
        try
        {
            File.WriteAllText(scriptFilePath, script);
            result = PowerShell.RunScript(new RunScriptInput
            {
                ReadFromFile = true,
                ScriptFilePath = scriptFilePath,
                LogInformationStream = true
            }, new RunOptions(), default);
        }
        finally
        {
            File.Delete(scriptFilePath);
        }

        Assert.That(result.Result.Count, Is.EqualTo(2));
        Assert.That(result.Result.Last(), Is.EqualTo(TimeSpan.FromHours(2)));
    }

    [Test]
    public void RunScript_ShouldPreserveScriptFileContext()
    {
        var scriptDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(scriptDirectory);
        var scriptFilePath = Path.Combine(scriptDirectory, "main.ps1");
        var helperFilePath = Path.Combine(scriptDirectory, "helper.ps1");

        try
        {
            File.WriteAllText(helperFilePath, "'dot-sourced'");
            File.WriteAllText(scriptFilePath,
                ". \"$PSScriptRoot/helper.ps1\"\n[PSCustomObject]@{ ScriptPath = $PSCommandPath; ScriptRoot = $PSScriptRoot }");

            var result = PowerShell.RunScript(new RunScriptInput
            {
                ReadFromFile = true,
                ScriptFilePath = scriptFilePath,
            }, new RunOptions(), default);

            Assert.That(result.Result[0], Is.EqualTo("dot-sourced"));
            Assert.That(result.Result[1].ScriptPath, Is.EqualTo(scriptFilePath));
            Assert.That(result.Result[1].ScriptRoot, Is.EqualTo(scriptDirectory));
        }
        finally
        {
            Directory.Delete(scriptDirectory, recursive: true);
        }
    }

    [Test]
    public void RunScript_ShouldRunScriptFromParameter()
    {
        PowerShellResult result;

        result = PowerShell.RunScript(new RunScriptInput
        {
            ReadFromFile = false,
            Script = script,
            LogInformationStream = true
        }, new RunOptions(), default);

        Assert.That(result.Result.Last(), Is.EqualTo(TimeSpan.FromHours(2)));
    }

    [Test]
    public void RunScript_ShouldUseCustomPowerShell()
    {
        var result = PowerShell.RunScript(new RunScriptInput
        {
            Parameters = [new PowerShellParameter { Name = "testParam", Value = "custom powershell" }],
            ReadFromFile = false,
            Script = "param([string]$testParam)\nWrite-Information 'custom information'\nWrite-Error 'custom error'\n$testParam\n[System.Diagnostics.Process]::GetCurrentProcess().Id",
            LogInformationStream = true,
        }, new RunOptions
        {
            PowerShellType = PowerShellType.Custom,
            PathToCustomPowerShell = FindPowerShellExecutable(),
            ExecutionPolicy = PowerShellExecutionPolicy.Bypass
        }, CancellationToken.None);

        Assert.That(result.Result.First(), Is.EqualTo("custom powershell"));
        Assert.That(result.Result.Last(), Is.Not.EqualTo(Process.GetCurrentProcess().Id));
        Assert.That(result.Errors.Single(), Does.Contain("custom error"));
        Assert.That(result.Log, Is.EqualTo("custom information"));
    }

    [Test]
    public void RunScript_ShouldUseSelectedExecutionPolicy()
    {
        if (!OperatingSystem.IsWindows())
            Assert.Ignore("PowerShell execution policies are only supported on Windows.");

        var result = PowerShell.RunScript(new RunScriptInput
        {
            ReadFromFile = false,
            Script = "$env:PSExecutionPolicyPreference",
        }, new RunOptions
        {
            PowerShellType = PowerShellType.Custom,
            PathToCustomPowerShell = FindPowerShellExecutable(),
            ExecutionPolicy = PowerShellExecutionPolicy.Bypass,
        }, CancellationToken.None);

        Assert.That(result.Result.Single().ToString(), Is.EqualTo("Bypass"));
    }

    [Test]
    public void ExecutionPolicy_SystemDefault_ShouldNotAddCommandLineArgument()
    {
        var startInfo = new ProcessStartInfo();

        CustomPowerShellHandler.AddExecutionPolicyArguments(startInfo, PowerShellExecutionPolicy.SystemDefault);

        Assert.That(startInfo.ArgumentList, Is.Empty);
    }

    [Test]
    public void ExecutionPolicy_ExplicitValue_ShouldAddCommandLineArgument()
    {
        var startInfo = new ProcessStartInfo();

        CustomPowerShellHandler.AddExecutionPolicyArguments(startInfo, PowerShellExecutionPolicy.RemoteSigned);

        Assert.That(startInfo.ArgumentList, Is.EqualTo(expected));
    }

    private static string FindPowerShellExecutable()
    {
        var executableNames = OperatingSystem.IsWindows()
            ? new[] { "pwsh.exe", "powershell.exe" }
            : new[] { "pwsh", "powershell" };

        foreach (var directory in (Environment.GetEnvironmentVariable("PATH") ?? "").Split(Path.PathSeparator))
        {
            foreach (var executableName in executableNames)
            {
                var path = Path.Combine(directory.Trim('"'), executableName);
                if (File.Exists(path))
                    return path;
            }
        }

        throw new InvalidOperationException("No PowerShell executable was found on PATH.");
    }

    [Test]
    public void RunCommandAndScript_ShouldUseSharedSession()
    {
        var session = PowerShell.CreateSession();
        _ = PowerShell.RunScript(new RunScriptInput
        {
            ReadFromFile = false,
            Script = "$timespan = $timespan + (new-timespan -hours 1)",
            LogInformationStream = true
        },
            new RunOptions
            {
                Session = session
            }, default);

        var result2 = PowerShell.RunScript(new RunScriptInput
        {
            ReadFromFile = false,
            Script = "(new-timespan -hours 1) + $timespan",
            LogInformationStream = true
        },
            new RunOptions
            {
                Session = session
            }, default);

        Assert.That(result2.Result.Single(), Is.EqualTo(TimeSpan.FromHours(2)));
    }

    [Test]
    public void RunScript_ShouldListErrors()
    {
        var script =
@"
This-DoesNotExist
$Source = @""
using System;
namespace test {
    public static class pstest {
        public static void test`(`) {
        throw new Exception(""Argh"");
        }
    }
}
""@

Add-Type -TypeDefinition $Source -Language CSharp
[test.pstest]::test()
get-process -name doesnotexist -ErrorAction Stop
";

        var resultError = Assert.Throws<Exception>(() => PowerShell.RunScript(new RunScriptInput { ReadFromFile = false, Script = script, LogInformationStream = true }, null, default));

        Assert.That(resultError.Message, Is.Not.Null);
    }

    [Test]
    public void RunScript_ShouldOutputCustomPowershellObjects()
    {
        var script =
@"$test = New-Object pscustomobject
$test | Add-Member -type NoteProperty -name Property1 -Value 'Value1'
$test | Add-Member -type NoteProperty -name Property2 -Value 'Value2'
$test
";
        var result = PowerShell.RunScript(new RunScriptInput
        {
            ReadFromFile = false,
            Script = script,
            LogInformationStream = true
        }, null, default);

        Assert.That(result.Result[0].Property1, Is.EqualTo("Value1"));
        Assert.That(result.Result[0].Property2, Is.EqualTo("Value2"));
    }
}
