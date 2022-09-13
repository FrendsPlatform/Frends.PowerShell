using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace Frends.PowerShell.RunCommand.Definitions;

/// <summary>
/// Input parameters.
/// </summary>
public class RunCommandInput
{
    /// <summary>
    /// The PowerShell command to execute 
    /// </summary>
    /// <example>Example-Command</example>
    public string Command { get; set; }

    /// <summary>
    /// Parameters for the command, provided switch parameters need to have a boolean value
    /// </summary>
    /// <example>[ foo, bar ]</example>
    public PowerShellParameter[] Parameters { get; set; }

    /// <summary>
    /// Should the information stream be logged. If false, log will be an empty string.
    /// If set to true, a lot of string data may be logged. Use with caution.
    /// </summary>
    /// <example>false</example>
    [DefaultValue(false)]
    public bool LogInformationStream { get; set; }
}

/// <summary>
/// PowerShellParameter values.
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