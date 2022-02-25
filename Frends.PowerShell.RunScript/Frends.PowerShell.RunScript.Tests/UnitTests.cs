﻿using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace Frends.PowerShell.RunScript.Tests
{
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
            }, new RunOptions());

            Assert.That(result.Result.Count, Is.EqualTo(2));
            Assert.That(result.Result.Last(), Is.EqualTo("my test param: my test param"));
        }

        private readonly string script =
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
                    ScriptFilePath = scriptFilePath,
                    LogInformationStream = true
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
                Script = script,
                LogInformationStream = true
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
                Script = "$timespan = $timespan + (new-timespan -hours 1)",
                LogInformationStream = true
            },
                new RunOptions
                {
                    Session = session
                });

            var result2 = PowerShell.RunScript(new RunScriptInput
            {
                ReadFromFile = false,
                Script = "(new-timespan -hours 1) + $timespan",
                LogInformationStream = true
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

            var resultError = Assert.Throws<Exception>(() => PowerShell.RunScript(new RunScriptInput { ReadFromFile = false, Script = script, LogInformationStream = true }, null));

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
            }, null);

            Assert.That(result.Result[0].Property1, Is.EqualTo("Value1"));
            Assert.That(result.Result[0].Property2, Is.EqualTo("Value2"));
        }
    }
}
