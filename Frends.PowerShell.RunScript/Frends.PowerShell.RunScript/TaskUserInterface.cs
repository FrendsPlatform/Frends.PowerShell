using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Management.Automation;
using System.Management.Automation.Host;
using System.Security;

namespace Frends.PowerShell.RunScript;

internal class TaskUserInterface : PSHostUserInterface
{
    /// <summary>
    /// Returns an empty string because user input cannot be provided.
    /// </summary>
    public override string ReadLine()
    {
        return ""; // We cannot provide user input
    }

    /// <summary>
    /// Returns an empty secure string because user input cannot be provided.
    /// </summary>
    public override SecureString ReadLineAsSecureString()
    {
        return new SecureString();
    }

    /// <summary>
    /// Writes the value to the trace output.
    /// </summary>
    /// <param name="value">Not used.</param>
    public override void Write(string value)
    {
        Trace.Write(value);
    }

    /// <summary>
    /// Writes the value to the trace output, ignoring the colors.
    /// </summary>
    /// <param name="foregroundColor">Not used.</param>
    /// <param name="backgroundColor">Not used.</param>
    /// <param name="value">Not used.</param>
    public override void Write(ConsoleColor foregroundColor, ConsoleColor backgroundColor, string value)
    {
        Write(value);
    }

    /// <summary>
    /// Writes the line to the trace output.
    /// </summary>
    /// <param name="value">Not used.</param>
    public override void WriteLine(string value)
    {
        Trace.WriteLine(value);//_messages.AppendLine(value);
    }

    /// <summary>
    /// Writes the error line to the trace output.
    /// </summary>
    /// <param name="value">Not used.</param>
    public override void WriteErrorLine(string value)
    {
        Trace.WriteLine(value);
    }

    /// <summary>
    /// Writes the debug line to the trace output.
    /// </summary>
    /// <param name="message">Not used.</param>
    public override void WriteDebugLine(string message)
    {
        Trace.WriteLine(message);
    }

    /// <summary>
    /// Ignores progress records.
    /// </summary>
    /// <param name="sourceId">Not used.</param>
    /// <param name="record">Not used.</param>
    public override void WriteProgress(long sourceId, ProgressRecord record)
    {
        // We don't handle progress
    }

    /// <summary>
    /// Writes the verbose line to the trace output.
    /// </summary>
    /// <param name="message">Not used.</param>
    public override void WriteVerboseLine(string message)
    {
        Trace.WriteLine(message);
    }

    /// <summary>
    /// Writes the warning line to the trace output.
    /// </summary>
    /// <param name="message">Not used.</param>
    public override void WriteWarningLine(string message)
    {
        Trace.WriteLine(message);
    }

    /// <summary>
    /// Answers every prompted field with an empty string.
    /// </summary>
    /// <param name="caption">Not used.</param>
    /// <param name="message">Not used.</param>
    /// <param name="descriptions">Fields to answer.</param>
    public override Dictionary<string, PSObject> Prompt(string caption, string message, Collection<FieldDescription> descriptions)
    {
        return descriptions?.ToDictionary(desc => desc.Name, desc => new PSObject("")); // Return empty string as answer
    }

    /// <summary>
    /// Throws because user credentials cannot be prompted.
    /// </summary>
    /// <param name="caption">Not used.</param>
    /// <param name="message">Not used.</param>
    /// <param name="userName">Not used.</param>
    /// <param name="targetName">Not used.</param>
    public override PSCredential PromptForCredential(string caption, string message, string userName, string targetName)
    {
        throw new NotImplementedException("We cannot prompt for user credentials");
    }

    /// <summary>
    /// Throws because user credentials cannot be prompted.
    /// </summary>
    /// <param name="caption">Not used.</param>
    /// <param name="message">Not used.</param>
    /// <param name="userName">Not used.</param>
    /// <param name="targetName">Not used.</param>
    /// <param name="allowedCredentialTypes">Not used.</param>
    /// <param name="options">Not used.</param>
    public override PSCredential PromptForCredential(string caption, string message, string userName, string targetName,
        PSCredentialTypes allowedCredentialTypes, PSCredentialUIOptions options)
    {
        throw new NotImplementedException("We cannot prompt for user credentials");
    }

    /// <summary>
    /// Always returns the default choice.
    /// </summary>
    /// <param name="caption">Not used.</param>
    /// <param name="message">Not used.</param>
    /// <param name="choices">Not used.</param>
    /// <param name="defaultChoice">Not used.</param>
    public override int PromptForChoice(string caption, string message, Collection<ChoiceDescription> choices, int defaultChoice)
    {
        //always go by default
        return defaultChoice;
    }

    private readonly PSHostRawUserInterface _rawUserInterface = null; // Not implemented

    /// <summary>
    /// Raw user interface is not implemented.
    /// </summary>
    /// <example>null</example>
    public override PSHostRawUserInterface RawUI
    {
        get { return _rawUserInterface; }
    }
}
