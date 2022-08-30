namespace Frends.PowerShell.RunScript;

/// <summary>
/// Result.
/// </summary>
public class PowerShellResult
{
    /// <summary>
    /// Result.
    /// </summary>
    /// <example>{Property1=Value1; Property2=Value2}</example>
    public IList<dynamic> Result { get; private set; }

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

    internal PowerShellResult(IList<dynamic> result, IList<string> errors, string log)
    {
        Result = result;
        Errors = errors;
        Log = log;
    }
}