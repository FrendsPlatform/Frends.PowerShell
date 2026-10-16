using System.ComponentModel.DataAnnotations;

namespace Frends.PowerShell.RunScript.Definitions;

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
