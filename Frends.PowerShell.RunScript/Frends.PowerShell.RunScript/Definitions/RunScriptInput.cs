using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.PowerShell.RunScript.Definitions;

/// <summary>
/// Input parameters.
/// </summary>
public class RunScriptInput
{
    /// <summary>
    /// Parameters for the script, provided switch parameters need to have a boolean value
    /// </summary>
    /// <example>foo, bar</example>
    public PowerShellParameter[] Parameters { get; set; }

    /// <summary>
    /// Should the script be read from a file or from the Script parameter
    /// </summary>
    /// <example>false</example>
    [DefaultValue(false)]
    public bool ReadFromFile { get; set; }

    /// <summary>
    /// Location for the script file.
    /// </summary>
    /// <example>c:\temp\example.ps1</example>
    [UIHint(nameof(ReadFromFile), "", true)]
    public string ScriptFilePath { get; set; }

    /// <summary>
    /// The script to execute.
    /// </summary>
    /// <example>param([string]$testParam)$testParam write-output "my test param: $testParam"</example>
    [UIHint(nameof(ReadFromFile), "", false)]
    public string Script { get; set; }

    /// <summary>
    /// Should the information stream be logged. If false, log will be an empty string.
    /// If set to true, a lot of string data may be logged. Use with caution.
    /// </summary>
    /// <example>true</example>
    public bool LogInformationStream { get; set; }

    /// <summary>
    /// Define which version of PowerShell to use.
    /// Useful in more complex scripts, as default version supports only basic usage
    /// </summary>
    /// <example>false</example>
    [DefaultValue(false)]
    public bool ExecuteNativeShell { get; set; }
}

/// <summary>
/// PowerShell parameter.
/// </summary>
public class PowerShellParameter
{
    /// <summary>
    /// Parameter's name.
    /// </summary>
    /// <example>foo</example>
    public string Name { get; set; }

    /// <summary>
    /// Parameter's value.
    /// </summary>
    /// <example>bar</example>
    [DisplayFormat(DataFormatString = "Text")]
    public object Value { get; set; }
}