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
                }
            },
                new RunOptions());

            Assert.That(result.Result, Is.Not.Null);
            Assert.That(result.Result.Single(), Is.EqualTo(TimeSpan.FromHours(1)));
        }

        [Test]
        public void RunScript_ShouldRunScriptWithParameter()
        {
            var script = @"param([string]$testParam)
$testParam
write-output ""my test param: $testParam""";

            var result = PowerShell.RunScript(new RunScriptInput
            {
                Parameters = new[] {new PowerShellParameter {Name = "testParam", Value = "my test param"}},
                ReadFromFile = false,
                Script = script
            }, new RunOptions());

            Assert.That(result.Result.Count, Is.EqualTo(2));
            Assert.That(result.Result.Last(), Is.EqualTo("my test param: my test param"));
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
                    }
            },
                new RunOptions
                {
                    Session = session
                });

            Assert.That(result.Result.Single(), Is.EqualTo(switchParameterValue));
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
                result = PowerShell.RunScript(new RunScriptInput
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

            result = PowerShell.RunScript(new RunScriptInput
            {
                ReadFromFile = false,
                Script = script
            }, new RunOptions());


            Assert.That(result.Result.Last(), Is.EqualTo(TimeSpan.FromHours(2)));
        }

        [Test]
        public void RunCommandAndScript_ShouldUseSharedSession()
        {
            var session = PowerShell.CreateSession();

            var result1 = PowerShell.RunScript(new RunScriptInput
            {
                ReadFromFile = false,
                Script = "$timespan = $timespan + (new-timespan -hours 1)"
            },
                new RunOptions
                {
                    Session = session
                });

            var result2 = PowerShell.RunScript(new RunScriptInput
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

        [Test]
        public void RunScript_ShouldListErrors()
        {
            var script = 
@"This-DoesNotExist
get-process -name doesnotexist -ErrorAction Stop
";

            var resultError = Assert.Throws<Exception>(() => PowerShell.RunScript(new RunScriptInput {ReadFromFile = false, Script = script}, null));

            Assert.That(resultError.Message, Is.Not.Null);
        }
    }
}
