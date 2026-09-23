using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.PowerShell.RunScript.Definitions;

/// <summary>
/// Options.
/// </summary>
public class RunOptions
{
    /// <summary>
    /// Session parameter.
    /// </summary>
    /// <example>null</example>
    [DefaultValue(null)]
    [DisplayFormat(DataFormatString = "Expression")]
    public SessionWrapper Session { get; set; }
}
