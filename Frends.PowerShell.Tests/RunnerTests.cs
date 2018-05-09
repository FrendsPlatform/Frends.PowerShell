using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace Frends.PowerShell.Tests
{
    [TestFixture]
    public class PowerShellTests
    {
        [Test]
        public void RunCommand_ShouldRunCommandWithParameter()
        {
            var result = Runner.RunCommand(new RunCommandInput
            {
                Command = "New-TimeSpan",
                Parameters = new[]
                {
                    new PowerShellParameter
                    {
                        Name = "Hours",
                        Value = "1"
                    },
                }
            },
                new RunOptions());

            Assert.That(result.Result, Is.Not.Null);
            Assert.That(result.Result.Single(), Is.EqualTo(TimeSpan.FromHours(1)));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void RunCommand_ShouldRunCommandWithSwitchParameter(string switchParameterValue)
        {
            var result = Runner.RunCommand(new RunCommandInput
            {
                Command = "get-process",
                Parameters = new[]
                    {
                        new PowerShellParameter
                        {
                            Name = "Name",
                            Value = "Powershell"
                        },
                        new PowerShellParameter
                        {
                            Name = "FileVersionInfo",
                            Value = switchParameterValue
                        }
                    }
            },
                new RunOptions());

            Assert.That(result.Result, Is.Not.Null);
            Assert.That(result.Result, Is.All.TypeOf<FileVersionInfo>());
        }


        private string script =
            @"
new-timespan -hours 1
new-timespan -hours 2";

        [Test]
        public void RunScript_ShouldRunScriptFromFile()
        {
            var scriptFilePath = Path.GetTempFileName();
            PowerShellResult result;
            try
            {
                File.WriteAllText(scriptFilePath, script);
                result = Runner.RunScript(new RunScriptInput
                {
                    ReadFromFile = true,
                    ScriptFilePath = scriptFilePath
                }, new RunOptions());
            }
            finally
            {
                File.Delete(scriptFilePath);
            }

            Assert.That(result.Result.Count, Is.EqualTo(2));
            Assert.That(result.Result.Last(), Is.EqualTo(TimeSpan.FromHours(2)));
        }

        [Test]
        public void RunScript_ShouldRunScriptFromParameter()
        {
            PowerShellResult result;

            result = Runner.RunScript(new RunScriptInput
            {
                ReadFromFile = false,
                Script = script
            }, new RunOptions());


            Assert.That(result.Result.Last(), Is.EqualTo(TimeSpan.FromHours(2)));
        }

        [Test]
        public void RunCommandAndScript_ShouldUseSharedSession()
        {
            var session = Runner.CreateSession();

            var result1 = Runner.RunScript(new RunScriptInput
            {
                ReadFromFile = false,
                Script = "$timespan = $timespan + (new-timespan -hours 1)"
            },
                new RunOptions
                {
                    Session = session
                });

            var result2 = Runner.RunScript(new RunScriptInput
                {
                    ReadFromFile = false,
                    Script = "(new-timespan -hours 1) + $timespan"
                },
                new RunOptions
                {
                    Session = session
                });

            Assert.That(result2.Result.Single(), Is.EqualTo(TimeSpan.FromHours(2)));
        }
    }
}
