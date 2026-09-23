using System;
using System.IO;
using System.Linq;
using Frends.PowerShell.RunScriptDEV.Definitions;
using NUnit.Framework;

namespace Frends.PowerShell.RunScript.Tests;

[TestFixture]
public class UnitTests
{
    [OneTimeSetUp]
    public void Setup()
    {
        var command = "Set-ExecutionPolicy";
        PowerShellParameter[] parameters = new PowerShellParameter[]
        {
            new PowerShellParameter
            {
                Name = "ExecutionPolicy",
                Value = "Unrestricted"
            },
            new PowerShellParameter
            {
                Name = "Scope",
                Value = "CurrentUser"
            }
        };
        RunScriptDEV.PowerShell.RunCommand(command, parameters, new RunOptions(), default);
    }

    [Test]
    public void RunScript_ShouldRunScriptWithParameter()
    {
        var script = @"param([string]$testParam)
$testParam
write-output ""my test param: $testParam""";

        var result = RunScriptDEV.PowerShell.RunScriptDEV(new RunScriptInput
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

    [Test]
    public void RunScript_ShouldRunScriptFromFile()
    {
        var scriptFilePath = Path.GetTempFileName();
        PowerShellResult result;
        try
        {
            File.WriteAllText(scriptFilePath, script);
            result = RunScriptDEV.PowerShell.RunScriptDEV(new RunScriptInput
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
    public void RunScript_ShouldRunScriptFromParameter()
    {
        PowerShellResult result;

        result = RunScriptDEV.PowerShell.RunScriptDEV(new RunScriptInput
        {
            ReadFromFile = false,
            Script = script,
            LogInformationStream = true
        }, new RunOptions(), default);

        Assert.That(result.Result.Last(), Is.EqualTo(TimeSpan.FromHours(2)));
    }

    [Test]
    public void RunCommandAndScript_ShouldUseSharedSession()
    {
        var session = RunScriptDEV.PowerShell.CreateSession();
        _ = RunScriptDEV.PowerShell.RunScriptDEV(new RunScriptInput
        {
            ReadFromFile = false,
            Script = "$timespan = $timespan + (new-timespan -hours 1)",
            LogInformationStream = true
        },
            new RunOptions
            {
                Session = session
            }, default);

        var result2 = RunScriptDEV.PowerShell.RunScriptDEV(new RunScriptInput
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

        var resultError = Assert.Throws<Exception>(() => RunScriptDEV.PowerShell.RunScriptDEV(new RunScriptInput { ReadFromFile = false, Script = script, LogInformationStream = true }, null, default));

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
        var result = RunScriptDEV.PowerShell.RunScriptDEV(new RunScriptInput
        {
            ReadFromFile = false,
            Script = script,
            LogInformationStream = true
        }, null, default);

        Assert.That(result.Result[0].Property1, Is.EqualTo("Value1"));
        Assert.That(result.Result[0].Property2, Is.EqualTo("Value2"));
    }

    /// <summary>
    /// Reproduces the error reported by a customer trying to import Windows modules
    /// (e.g. ActiveDirectory) that ship type extension (`types.ps1xml`) data whose code
    /// property getter does not match PowerShell's required signature (public, static,
    /// non-void, single PSObject parameter). Instead of failing the module import outright,
    /// PowerShell reports the malformed member as a (non-terminating) error while the rest
    /// of the type data still loads, so the failure only surfaces once the offending member
    /// is actually accessed.
    /// </summary>
    [Test]
    public void RunScript_ShouldReportErrorForModuleWithInvalidCodePropertyGetter()
    {
        var typesFilePath = Path.Combine(Path.GetTempPath(), $"{Path.GetRandomFileName()}.types.ps1xml");
        var typesXml =
$@"<?xml version=""1.0"" encoding=""utf-8""?>
<Types>
  <Type>
    <Name>System.String</Name>
    <Members>
      <CodeProperty>
        <Name>InvalidGetter</Name>
        <GetCodeReference>
          <TypeName>{typeof(InvalidCodePropertyGetterProvider).AssemblyQualifiedName}</TypeName>
          <MethodName>{nameof(InvalidCodePropertyGetterProvider.GetValue)}</MethodName>
        </GetCodeReference>
      </CodeProperty>
    </Members>
  </Type>
</Types>";

        PowerShellResult result;
        try
        {
            File.WriteAllText(typesFilePath, typesXml);

            var script =
$@"Update-TypeData -AppendPath '{typesFilePath}'
""hello"".InvalidGetter";

            result = RunScriptDEV.PowerShell.RunScriptDEV(new RunScriptInput
            {
                ReadFromFile = false,
                Script = script,
                LogInformationStream = true
            }, null, default);
        }
        finally
        {
            File.Delete(typesFilePath);
        }

        Assert.That(result.Errors, Has.Some.Contains(
            "The getter method should be public, not void, static, and have one parameter of the type PSObject"));
    }

    /// <summary>
    /// Intentionally invalid CodeProperty getter (must be public, static, non-void and take a
    /// single PSObject parameter) used to reproduce the "getter method should be public, not
    /// void, static..." error surfaced by real-world modules such as ActiveDirectory.
    /// </summary>
    public static class InvalidCodePropertyGetterProvider
    {
        public static void GetValue(object psObject)
        {
        }
    }
}
