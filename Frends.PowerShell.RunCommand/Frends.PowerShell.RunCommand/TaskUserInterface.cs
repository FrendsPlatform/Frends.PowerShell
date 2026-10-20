using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Management.Automation;
using System.Management.Automation.Host;
using System.Security;

namespace Frends.PowerShell.RunCommand;

internal class TaskUserInterface : PSHostUserInterface
{
    private readonly PSHostRawUserInterface rawUserInterface = null;

    /// <summary>
    /// Gets the raw user interface, which is not implemented.
    /// </summary>
    /// <example>null</example>
    public override PSHostRawUserInterface RawUI
    {
        get { return rawUserInterface; }
    }

    /// <summary>
    /// Returns an empty string because user input cannot be provided.
    /// </summary>
    /// <returns>An empty string.</returns>
    public override string ReadLine()
    {
        return string.Empty;
    }

    /// <summary>
    /// Returns an empty secure string because user input cannot be provided.
    /// </summary>
    /// <returns>An empty secure string.</returns>
    public override SecureString ReadLineAsSecureString()
    {
        return new SecureString();
    }

    /// <summary>
    /// Writes the value to the trace output.
    /// </summary>
    /// <param name="value">The value to write.</param>
    public override void Write(string value)
    {
        Trace.Write(value);
    }

    /// <summary>
    /// Writes the value to the trace output, ignoring the colors.
    /// </summary>
    /// <param name="foregroundColor">The foreground color, ignored by this implementation.</param>
    /// <param name="backgroundColor">The background color, ignored by this implementation.</param>
    /// <param name="value">The value to write.</param>
    public override void Write(ConsoleColor foregroundColor, ConsoleColor backgroundColor, string value)
    {
        Write(value);
    }

    /// <summary>
    /// Writes the line to the trace output.
    /// </summary>
    /// <param name="value">The value to write.</param>
    public override void WriteLine(string value)
    {
        Trace.WriteLine(value);
    }

    /// <summary>
    /// Writes the error line to the trace output.
    /// </summary>
    /// <param name="value">The value to write.</param>
    public override void WriteErrorLine(string value)
    {
        Trace.WriteLine(value);
    }

    /// <summary>
    /// Writes the debug line to the trace output.
    /// </summary>
    /// <param name="message">The debug message to write.</param>
    public override void WriteDebugLine(string message)
    {
        Trace.WriteLine(message);
    }

    /// <summary>
    /// Ignores progress records.
    /// </summary>
    /// <param name="sourceId">The source identifier of the progress record.</param>
    /// <param name="record">The progress record, ignored by this implementation.</param>
    public override void WriteProgress(long sourceId, ProgressRecord record)
    {
        // We don't handle progress
    }

    /// <summary>
    /// Writes the verbose line to the trace output.
    /// </summary>
    /// <param name="message">The verbose message to write.</param>
    public override void WriteVerboseLine(string message)
    {
        Trace.WriteLine(message);
    }

    /// <summary>
    /// Writes the warning line to the trace output.
    /// </summary>
    /// <param name="message">The warning message to write.</param>
    public override void WriteWarningLine(string message)
    {
        Trace.WriteLine(message);
    }

    /// <summary>
    /// Answers every field with an empty string.
    /// </summary>
    /// <param name="caption">The prompt caption.</param>
    /// <param name="message">The prompt message.</param>
    /// <param name="descriptions">The fields to answer.</param>
    /// <returns>A dictionary containing an empty string for each field.</returns>
    public override Dictionary<string, PSObject> Prompt(
        string caption,
        string message,
        Collection<FieldDescription> descriptions)
    {
        return descriptions?.ToDictionary(desc => desc.Name, desc => new PSObject(string.Empty));
    }

    /// <summary>
    /// Throws because credentials cannot be prompted.
    /// </summary>
    /// <param name="caption">The prompt caption.</param>
    /// <param name="message">The prompt message.</param>
    /// <param name="userName">The suggested user name.</param>
    /// <param name="targetName">The target for the credentials.</param>
    /// <returns>This method does not return because credentials cannot be prompted.</returns>
    public override PSCredential PromptForCredential(string caption, string message, string userName, string targetName)
    {
        throw new NotImplementedException("We cannot prompt for user credentials");
    }

    /// <summary>
    /// Throws because credentials cannot be prompted.
    /// </summary>
    /// <param name="caption">The prompt caption.</param>
    /// <param name="message">The prompt message.</param>
    /// <param name="userName">The suggested user name.</param>
    /// <param name="targetName">The target for the credentials.</param>
    /// <param name="allowedCredentialTypes">The allowed credential types.</param>
    /// <param name="options">The credential prompt options.</param>
    /// <returns>This method does not return because credentials cannot be prompted.</returns>
    public override PSCredential PromptForCredential(
        string caption,
        string message,
        string userName,
        string targetName,
        PSCredentialTypes allowedCredentialTypes,
        PSCredentialUIOptions options)
    {
        throw new NotImplementedException("We cannot prompt for user credentials");
    }

    /// <summary>
    /// Always selects the default choice.
    /// </summary>
    /// <param name="caption">The prompt caption.</param>
    /// <param name="message">The prompt message.</param>
    /// <param name="choices">The choices to display.</param>
    /// <param name="defaultChoice">The choice returned by this implementation.</param>
    /// <returns>The default choice index.</returns>
    public override int PromptForChoice(
        string caption,
        string message,
        Collection<ChoiceDescription> choices,
        int defaultChoice)
    {
        // Always use the default choice.
        return defaultChoice;
    }
}
