namespace Frends.PowerShell.RunCommand.Definitions;

/// <summary>
/// PowerShell executable selection.
/// </summary>
public enum PowerShellType
{
    /// <summary>
    /// Use the PowerShell version included with the task.
    /// </summary>
    Default,

    /// <summary>
    /// Use a custom PowerShell executable.
    /// </summary>
    Custom,
}
