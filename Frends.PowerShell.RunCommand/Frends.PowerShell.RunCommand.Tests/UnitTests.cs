using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using Frends.PowerShell.RunCommand.Definitions;
using Frends.PowerShell.RunCommand.Helpers;
using NUnit.Framework;
namespace Frends.PowerShell.RunCommand.Tests;

[TestFixture]
public class UnitTests
{
    [Test]
    public void RunCommand_ShouldRunCommandWithParameter()
    {
        var result = PowerShell.RunCommand(new Input
        {
            Command = "New-TimeSpan",
            Parameters = new[]
            {
                new PowerShellParameter
                {
                    Name = "Hours",
                    Value = "1"
                },
            },
            LogInformationStream = true
        },
            new Options(), CancellationToken.None);

        Assert.That(result.Results, Is.Not.Null);
        Assert.That(result.Results.Single(), Is.EqualTo(TimeSpan.FromHours(1)));
    }

    [TestCase(true)]
    [TestCase(false)]
    public void RunCommand_ShouldRunCommandWithSwitchParameter(object switchParameterValue)
    {
        var session = PowerShell.CreateSession();
        session.PowerShell.AddScript(@"
function Test-Switch { 
    param([switch] $switchy) 
    $switchy.IsPresent 
}", false);
        session.PowerShell.Invoke();
        session.PowerShell.Commands.Clear();

        var result = PowerShell.RunCommand(new Input
        {
            Command = "Test-Switch",
            Parameters = new[]
                {
                    new PowerShellParameter
                    {
                        Name = "switchy",
                        Value = switchParameterValue
                    }
                },
            LogInformationStream = true
        },
            new Options
            {
                Session = session
            }, CancellationToken.None);

        Assert.That(result.Results.Single(), Is.EqualTo(switchParameterValue));
    }

    [Test]
    public void RunCommand_ShouldUseCustomPowerShell()
    {
        var result = PowerShell.RunCommand(new Input
        {
            Command = "Write-Output",
            Parameters = [new PowerShellParameter { Name = "InputObject", Value = "custom powershell" }],
            LogInformationStream = true,
        }, new Options
        {
            PowerShellType = PowerShellType.Custom,
            PathToCustomPowerShell = FindPowerShellExecutable(),
            ExecutionPolicy = PowerShellExecutionPolicy.Bypass,
        }, CancellationToken.None);

        Assert.That(result.Results.Single(), Is.EqualTo("custom powershell"));
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

        Assert.That(startInfo.ArgumentList, Is.EqualTo(new[] { "-ExecutionPolicy", "RemoteSigned" }));
    }

    [Test]
    public void RunCommand_CustomPowerShellShouldRejectSharedSession()
    {
        using var session = PowerShell.CreateSession();

        var exception = Assert.Throws<InvalidOperationException>(() => PowerShell.RunCommand(new Input
        {
            Command = "Write-Output",
        }, new Options
        {
            PowerShellType = PowerShellType.Custom,
            PathToCustomPowerShell = FindPowerShellExecutable(),
            Session = session,
        }, CancellationToken.None));

        Assert.That(exception.Message, Does.Contain("shared session"));
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
}