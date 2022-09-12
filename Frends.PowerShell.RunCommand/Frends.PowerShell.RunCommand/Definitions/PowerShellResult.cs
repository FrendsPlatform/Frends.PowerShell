namespace Frends.PowerShell.RunCommand.Definitions;

/// <summary>
/// Task result.
/// </summary>
public class PowerShellResult
{
    /// <summary>
    /// Result values.
    /// </summary>
    /// <example>{ foo, bar }</example>
    public IList<dynamic> Result { get; private set; }

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

    internal PowerShellResult(IList<dynamic> result, IList<string> errors, string log)
    {
        Result = result;
        Errors = errors;
        Log = log;
    }
}