using System.Globalization;
using System.Management.Automation.Host;

namespace Frends.PowerShell.RunCommand;

/// <summary>
/// Provides the host implementation used by the PowerShell task.
/// </summary>
internal class TaskPowershellHost : PSHost
{
    private readonly TaskUserInterface userInterface = new();
    private readonly CultureInfo originalCultureInfo = Thread.CurrentThread.CurrentCulture;
    private readonly CultureInfo originalUiCultureInfo = Thread.CurrentThread.CurrentUICulture;
    private readonly Guid myId = Guid.NewGuid();

    /// <summary>
    /// Initializes a new instance of the <see cref="TaskPowershellHost"/> class.
    /// </summary>
    public TaskPowershellHost()
    {
    }

    /// <summary>
    /// Gets the culture information captured when this host was created.
    /// </summary>
    /// <example>en-US</example>
    public override CultureInfo CurrentCulture
    {
        get { return originalCultureInfo; }
    }

    /// <summary>
    /// Gets the UI culture information captured when this host was created.
    /// </summary>
    /// <example>en-US</example>
    public override CultureInfo CurrentUICulture
    {
        get { return originalUiCultureInfo; }
    }

    /// <summary>
    /// Gets the identifier allocated when this host was created.
    /// </summary>
    /// <example>3f2504e0-4f89-11d3-9a0c-0305e82c3301</example>
    public override Guid InstanceId
    {
        get { return myId; }
    }

    /// <summary>
    /// Gets the name of this host implementation.
    /// </summary>
    /// <example>SimpleTaskPowershellHost</example>
    public override string Name
    {
        get { return "SimpleTaskPowershellHost"; }
    }

    /// <summary>
    /// Gets the user interface for this host.
    /// </summary>
    /// <example>TaskUserInterface</example>
    public override PSHostUserInterface UI
    {
        get { return userInterface; }
    }

    /// <summary>
    /// Gets the version of this host implementation.
    /// </summary>
    /// <example>1.0.0.0</example>
    public override Version Version
    {
        get { return new Version(1, 0, 0, 0); }
    }

    /// <summary>
    /// Ignores the exit request because the host is not interactive.
    /// </summary>
    /// <param name="exitCode">The exit code requested by the script.</param>
    public override void SetShouldExit(int exitCode)
    {
    }

    /// <summary>
    /// Throws because nested prompts are not supported.
    /// </summary>
    /// <example>Not supported</example>
    public override void EnterNestedPrompt()
    {
        throw new NotImplementedException("The method or operation is not implemented.");
    }

    /// <summary>
    /// Throws because nested prompts are not supported.
    /// </summary>
    /// <example>Not supported</example>
    public override void ExitNestedPrompt()
    {
        throw new NotImplementedException("The method or operation is not implemented.");
    }

    /// <summary>
    /// Does nothing before an external application process is started.
    /// </summary>
    /// <example>No action</example>
    public override void NotifyBeginApplication()
    {
        return;
    }

    /// <summary>
    /// Does nothing after an external application process finishes.
    /// </summary>
    /// <example>No action</example>
    public override void NotifyEndApplication()
    {
        return;
    }
}
