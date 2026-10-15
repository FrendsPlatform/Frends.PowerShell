using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Management.Automation;
using System.Management.Automation.Host;
using System.Security;
namespace Frends.PowerShell.RunCommand;

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
    public override void Write(string value)
    {
        Trace.Write(value);
    }

    /// <summary>
    /// Writes the value to the trace output, ignoring the colors.
    /// </summary>
    public override void Write(ConsoleColor foregroundColor, ConsoleColor backgroundColor, string value)
    {
        Write(value);
    }

    /// <summary>
    /// Writes the line to the trace output.
    /// </summary>
    public override void WriteLine(string value)
    {
        Trace.WriteLine(value);//_messages.AppendLine(value);
    }

    /// <summary>
    /// Writes the error line to the trace output.
    /// </summary>
    public override void WriteErrorLine(string value)
    {
        Trace.WriteLine(value);
    }

    /// <summary>
    /// Writes the debug line to the trace output.
    /// </summary>
    public override void WriteDebugLine(string message)
    {
        Trace.WriteLine(message);
    }

    /// <summary>
    /// Ignores progress records.
    /// </summary>
    public override void WriteProgress(long sourceId, ProgressRecord record)
    {
        // We don't handle progress
    }

    /// <summary>
    /// Writes the verbose line to the trace output.
    /// </summary>
    public override void WriteVerboseLine(string message)
    {
        Trace.WriteLine(message);
    }

    /// <summary>
    /// Writes the warning line to the trace output.
    /// </summary>
    public override void WriteWarningLine(string message)
    {
        Trace.WriteLine(message);
    }

    /// <summary>
    /// Answers every field with an empty string.
    /// </summary>
    public override Dictionary<string, PSObject> Prompt(string caption, string message, Collection<FieldDescription> descriptions)
    {
        return descriptions?.ToDictionary(desc => desc.Name, desc => new PSObject("")); // Return empty string as answer
    }

    /// <summary>
    /// Throws because credentials cannot be prompted.
    /// </summary>
    public override PSCredential PromptForCredential(string caption, string message, string userName, string targetName)
    {
        throw new NotImplementedException("We cannot prompt for user credentials");
    }

    /// <summary>
    /// Throws because credentials cannot be prompted.
    /// </summary>
    public override PSCredential PromptForCredential(string caption, string message, string userName, string targetName,
        PSCredentialTypes allowedCredentialTypes, PSCredentialUIOptions options)
    {
        throw new NotImplementedException("We cannot prompt for user credentials");
    }

    /// <summary>
    /// Always selects the default choice.
    /// </summary>
    public override int PromptForChoice(string caption, string message, Collection<ChoiceDescription> choices, int defaultChoice)
    {
        //always go by default
        return defaultChoice;
    }

    private readonly PSHostRawUserInterface _rawUserInterface = null; // Not implemented

    /// <summary>
    /// Gets the raw user interface, which is not implemented.
    /// </summary>
    /// <example>null</example>
    public override PSHostRawUserInterface RawUI
    {
        get { return _rawUserInterface; }
    }
}