namespace Frends.PowerShell.RunCommand.Definitions;

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
    /// The exception that caused the error.
    /// </summary>
    /// <example>System.Exception: Encountered terminating error while executing powershell</example>
    public Exception AdditionalInfo { get; set; }
}
