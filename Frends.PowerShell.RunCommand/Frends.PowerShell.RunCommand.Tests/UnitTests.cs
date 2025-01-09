using System;
using System.Linq;
using Frends.PowerShell.RunCommand.Definitions;
using NUnit.Framework;
namespace Frends.PowerShell.RunCommand.Tests;

[TestFixture]
public class UnitTests
{
    [Test]
    public void RunCommand_ShouldRunCommandWithParameter()
    {
        var result = PowerShell.RunCommand(new RunCommandInput
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
            new RunOptions());

        Assert.That(result.Result, Is.Not.Null);
        Assert.That(result.Result.Single(), Is.EqualTo(TimeSpan.FromHours(1)));
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

        var result = PowerShell.RunCommand(new RunCommandInput
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
            new RunOptions
            {
                Session = session
            });

        Assert.That(result.Result.Single(), Is.EqualTo(switchParameterValue));
    }

    [Test]
    public void RunScript_ShouldRunFromNativeShell()
    {
        var result = PowerShell.RunCommand(new RunCommandInput
        {
            Command = "New-TimeSpan",
            Parameters = new[]
            {
                new PowerShellParameter
                {
                    Name = "Days",
                    Value = "1"
                },
            },
            ExecuteNativeShell = true
        },
        new RunOptions());

        StringAssert.Contains("1", result.Result.First());
        Assert.That(result.Result.Count, Is.EqualTo(11));

    }

}