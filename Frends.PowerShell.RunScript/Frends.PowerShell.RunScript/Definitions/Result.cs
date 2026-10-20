namespace Frends.PowerShell.RunScript.Definitions;

/// <summary>
/// Result.
/// </summary>
public class Result
{
    internal Result(bool success, IList<dynamic> output, IList<string> errors, string log, Error error = null)
    {
        Success = success;
        Error = error;
        Output = output;
        Errors = errors;
        Log = log;
    }

    /// <summary>
    /// Indicates whether the script was executed successfully.
    /// </summary>
    /// <example>true</example>
    public bool Success { get; private set; }

    /// <summary>
    /// Output of the script.
    /// </summary>
    /// <example>{Property1=Value1; Property2=Value2}</example>
    public IList<dynamic> Output { get; private set; }

    /// <summary>
    /// Error(s).
    /// </summary>
    /// <example>"at &lt;ScriptBlock&gt;, C:\temp\sample.ps1: line 1: The term 'foo' is not recognized..."</example>
    public IList<string> Errors { get; private set; }

    /// <summary>
    /// Log.
    /// </summary>
    /// <example>foo bar</example>
    public string Log { get; private set; }

    /// <summary>
    /// Error details. Null when Success is true.
    /// </summary>
    /// <example>null</example>
    public Error Error { get; private set; }
}
