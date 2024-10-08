using System;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Host;
using System.Security;
using Frends.PowerShell.RunCommand;
using NUnit.Framework;

namespace Frends.PowerShell.RunScript.Tests;

public class TaskUserInterfaceTests
{
    /// <summary>
    /// This user interface is a dummy implementation that does not do anything.
    /// Ensure that most methods can be called without exceptions.
    /// </summary>
    [Test]
    public void TestTheWholeThing()
    {
        var taskUserInterface = new TaskUserInterface();
        taskUserInterface.Write("Hello, World!");
        taskUserInterface.Write(ConsoleColor.Black, ConsoleColor.White, "Hello, World!");
        taskUserInterface.WriteLine("Hello, World!");
        taskUserInterface.WriteErrorLine("Hello, World!");
        taskUserInterface.WriteDebugLine("Hello, World!");
        taskUserInterface.WriteProgress(1, new ProgressRecord(1, "Activity", "Status"));
        taskUserInterface.WriteVerboseLine("Hello, World!");
        taskUserInterface.WriteWarningLine("Hello, World!");
        taskUserInterface.ReadLineAsSecureString();

        Assert.AreEqual("", taskUserInterface.ReadLine());
        Assert.AreEqual(1, taskUserInterface.PromptForChoice("Caption", "Message", new Collection<ChoiceDescription>(), 1));

        var answer = taskUserInterface.Prompt("Caption", "Message", new Collection<FieldDescription> { new ("Name") });
        Assert.AreEqual(1, answer.Count);

        Assert.Throws<NotImplementedException>(() => taskUserInterface.PromptForCredential("Caption", "Message", "UserName", "TargetName"));
        Assert.Throws<NotImplementedException>(() => taskUserInterface.PromptForCredential("Caption", "Message", "UserName", "TargetName", PSCredentialTypes.Domain, PSCredentialUIOptions.Default));

    }
}