namespace Frends.PowerShell.RunScript.Definitions;

/// <summary>
/// Error details.
/// </summary>
public class Error
{
    /// <summary>
    /// Summary of the error.
    /// </summary>
    /// <example>Encountered terminating error while executing powershell</example>
    public string Message { get; set; }

    /// <summary>
    /// Additional information about the error.
    /// </summary>
    /// <example>System.Exception: Encountered terminating error while executing powershell</example>
    public dynamic AdditionalInfo { get; set; }
}
