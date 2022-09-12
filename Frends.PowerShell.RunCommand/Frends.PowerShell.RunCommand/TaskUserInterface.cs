using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Management.Automation;
using System.Management.Automation.Host;
using System.Security;
namespace Frends.PowerShell.RunCommand;

internal class TaskUserInterface : PSHostUserInterface
{
    public override string ReadLine()
    {
        return ""; // We cannot provide user input
    }

    public override SecureString ReadLineAsSecureString()
    {
        return new SecureString();
    }

    public override void Write(string value)
    {
        Trace.Write(value);
    }

    public override void Write(ConsoleColor foregroundColor, ConsoleColor backgroundColor, string value)
    {
        Write(value);
    }

    public override void WriteLine(string value)
    {
        Trace.WriteLine(value);//_messages.AppendLine(value);
    }

    public override void WriteErrorLine(string value)
    {
        Trace.WriteLine(value);
    }

    public override void WriteDebugLine(string message)
    {
        Trace.WriteLine(message);
    }

    public override void WriteProgress(long sourceId, ProgressRecord record)
    {
        // We don't handle progress
    }

    public override void WriteVerboseLine(string message)
    {
        Trace.WriteLine(message);
    }

    public override void WriteWarningLine(string message)
    {
        Trace.WriteLine(message);
    }

    public override Dictionary<string, PSObject> Prompt(string caption, string message, Collection<FieldDescription> descriptions)
    {
        return descriptions?.ToDictionary(desc => desc.Name, desc => new PSObject("")); // Return empty string as answer
    }

    public override PSCredential PromptForCredential(string caption, string message, string userName, string targetName)
    {
        throw new NotImplementedException("We cannot prompt for user credentials");
    }

    public override PSCredential PromptForCredential(string caption, string message, string userName, string targetName,
        PSCredentialTypes allowedCredentialTypes, PSCredentialUIOptions options)
    {
        throw new NotImplementedException("We cannot prompt for user credentials");
    }

    public override int PromptForChoice(string caption, string message, Collection<ChoiceDescription> choices, int defaultChoice)
    {
        //always go by default
        return defaultChoice;
    }

    private readonly PSHostRawUserInterface _rawUserInterface = null; // Not implemented

    public override PSHostRawUserInterface RawUI
    {
        get { return _rawUserInterface; }
    }
}