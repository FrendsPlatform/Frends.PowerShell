using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Management.Automation;
using System.Management.Automation.Host;
using System.Security;

namespace Frends.PowerShell.RunScript;

internal class TaskUserInterface : PSHostUserInterface
{
    /// <summary>
    /// The raw user interface is not implemented.
    /// </summary>
    /// <example>null</example>
    public override PSHostRawUserInterface RawUI => null;

    /// <summary>
    /// Returns an empty line because user input cannot be provided.
    /// </summary>
    /// <returns>An empty string.</returns>
    public override string ReadLine()
    {
        return string.Empty;
    }

    /// <summary>
    /// Returns an empty secure string because user input cannot be provided.
    /// </summary>
    /// <returns>A new empty secure string.</returns>
    public override SecureString ReadLineAsSecureString()
    {
        return new SecureString();
    }

    /// <summary>
    /// Writes text to the trace output.
    /// </summary>
    /// <param name="value">Text to write.</param>
    public override void Write(string value)
    {
        Trace.Write(value);
    }

    /// <summary>
    /// Writes text to the trace output without applying the requested colors.
    /// </summary>
    /// <param name="foregroundColor">Ignored foreground color.</param>
    /// <param name="backgroundColor">Ignored background color.</param>
    /// <param name="value">Text to write.</param>
    public override void Write(ConsoleColor foregroundColor, ConsoleColor backgroundColor, string value)
    {
        Write(value);
    }

    /// <summary>
    /// Writes a line to the trace output.
    /// </summary>
    /// <param name="value">Line to write.</param>
    public override void WriteLine(string value)
    {
        Trace.WriteLine(value); // PowerShell output is not retained by this host.
    }

    /// <summary>
    /// Writes an error line to the trace output.
    /// </summary>
    /// <param name="value">Error line to write.</param>
    public override void WriteErrorLine(string value)
    {
        Trace.WriteLine(value);
    }

    /// <summary>
    /// Writes a debug line to the trace output.
    /// </summary>
    /// <param name="message">Debug message to write.</param>
    public override void WriteDebugLine(string message)
    {
        Trace.WriteLine(message);
    }

    /// <summary>
    /// Ignores progress records.
    /// </summary>
    /// <param name="sourceId">Identifier of the progress source, which is ignored.</param>
    /// <param name="record">Progress record, which is ignored.</param>
    public override void WriteProgress(long sourceId, ProgressRecord record)
    {
        // Progress is not handled by this host.
    }

    /// <summary>
    /// Writes a verbose line to the trace output.
    /// </summary>
    /// <param name="message">Verbose message to write.</param>
    public override void WriteVerboseLine(string message)
    {
        Trace.WriteLine(message);
    }

    /// <summary>
    /// Writes a warning line to the trace output.
    /// </summary>
    /// <param name="message">Warning message to write.</param>
    public override void WriteWarningLine(string message)
    {
        Trace.WriteLine(message);
    }

    /// <summary>
    /// Answers every prompted field with an empty string.
    /// </summary>
    /// <param name="caption">Prompt caption, which is ignored.</param>
    /// <param name="message">Prompt message, which is ignored.</param>
    /// <param name="descriptions">Fields to answer with empty strings.</param>
    /// <returns>A map from each field name to an empty PowerShell object.</returns>
    public override Dictionary<string, PSObject> Prompt(
        string caption,
        string message,
        Collection<FieldDescription> descriptions)
    {
        return descriptions?.ToDictionary(desc => desc.Name, _ => new PSObject(string.Empty));
    }

    /// <summary>
    /// Throws because user credentials cannot be prompted.
    /// </summary>
    /// <param name="caption">Caption for the credential request, which is ignored.</param>
    /// <param name="message">Message for the credential request, which is ignored.</param>
    /// <param name="userName">Suggested username, which is ignored.</param>
    /// <param name="targetName">Target name for the credentials, which is ignored.</param>
    /// <returns>This method does not return because credentials cannot be requested.</returns>
    public override PSCredential PromptForCredential(
        string caption,
        string message,
        string userName,
        string targetName)
    {
        throw new NotImplementedException("We cannot prompt for user credentials");
    }

    /// <summary>
    /// Throws because user credentials cannot be prompted.
    /// </summary>
    /// <param name="caption">Caption for the credential request with options, which is ignored.</param>
    /// <param name="message">Message for the credential request with options, which is ignored.</param>
    /// <param name="userName">Suggested username for the request with options, which is ignored.</param>
    /// <param name="targetName">Target name for the request with options, which is ignored.</param>
    /// <param name="allowedCredentialTypes">Permitted credential types, which are ignored.</param>
    /// <param name="options">Credential prompt options, which are ignored.</param>
    /// <returns>This method does not return because credentials cannot be requested.</returns>
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
    /// Always returns the default choice.
    /// </summary>
    /// <param name="caption">Choice prompt caption, which is ignored.</param>
    /// <param name="message">Choice prompt message, which is ignored.</param>
    /// <param name="choices">Available choices, which are ignored.</param>
    /// <param name="defaultChoice">Index of the choice to return.</param>
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
