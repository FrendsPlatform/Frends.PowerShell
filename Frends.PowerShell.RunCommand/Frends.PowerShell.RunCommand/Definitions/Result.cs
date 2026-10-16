namespace Frends.PowerShell.RunCommand.Definitions;

/// <summary>
/// Task result.
/// </summary>
public class Result
{
    internal Result(bool success, IList<dynamic> results, IList<string> errors, string log, Error error = null)
    {
        Success = success;
        Results = results;
        Errors = errors;
        Log = log;
        Error = error;
    }

    /// <summary>
    /// Indicates whether the operation completed successfully.
    /// </summary>
    /// <example>true</example>
    public bool Success { get; private set; }

    /// <summary>
    /// Result values.
    /// </summary>
    /// <example>{ foo, bar }</example>
    public IList<dynamic> Results { get; private set; }

    /// <summary>
    /// Errors.
    /// </summary>
    /// <example>Encountered terminating error while executing powershell: Errors: "powershell.Streams.Error"</example>
    public IList<string> Errors { get; private set; }

    /// <summary>
    /// Log.
    /// </summary>
    /// <example>foobar.</example>
    public string Log { get; private set; }

    /// <summary>
    /// Error details. Null when Success is true.
    /// </summary>
    /// <example>null</example>
    public Error Error { get; private set; }
}
