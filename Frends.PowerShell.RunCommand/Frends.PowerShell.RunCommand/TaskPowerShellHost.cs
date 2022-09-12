using System.Globalization;
using System.Management.Automation.Host;
namespace Frends.PowerShell.RunCommand;

/// <summary>
/// This is a sample implementation of the PSHost abstract class for 
/// console applications. Not all members are implemented. Those that 
/// are not implemented throw a NotImplementedException exception or 
/// return nothing.
/// </summary>
internal class TaskPowershellHost : PSHost
{
    private readonly TaskUserInterface _userInterface = new();

    /// <summary>
    /// The culture information of the thread that created this object.
    /// </summary>
    private readonly CultureInfo _originalCultureInfo = Thread.CurrentThread.CurrentCulture;

    /// <summary>
    /// The UI culture information of the thread that created this object.
    /// </summary>
    private readonly CultureInfo _originalUiCultureInfo = Thread.CurrentThread.CurrentUICulture;

    /// <summary>
    /// The identifier of this PSHost implementation.
    /// </summary>
    private readonly Guid _myId = Guid.NewGuid();

    /// <summary>
    /// Initializes a new instance of the MyHost class. Keep
    /// a reference to the host application object so that it 
    /// can be informed of when to exit.
    /// </summary>
    public TaskPowershellHost()
    {
    }

    /// <summary>
    /// Return the culture information to use. This implementation 
    /// returns a snapshot of the culture information of the thread 
    /// that created this object.
    /// </summary>
    public override CultureInfo CurrentCulture
    {
        get { return _originalCultureInfo; }
    }

    /// <summary>
    /// Return the UI culture information to use. This implementation 
    /// returns a snapshot of the UI culture information of the thread 
    /// that created this object.
    /// </summary>
    public override CultureInfo CurrentUICulture
    {
        get { return _originalUiCultureInfo; }
    }

    /// <summary>
    /// This implementation always returns the GUID allocated at 
    /// instantiation time.
    /// </summary>
    public override Guid InstanceId
    {
        get { return _myId; }
    }

    /// <summary>
    /// Return a string that contains the name of the host implementation. 
    /// Keep in mind that this string may be used by script writers to
    /// identify when your host is being used.
    /// </summary>
    public override string Name
    {
        get { return "SimpleTaskPowershellHost"; }
    }


    /// <summary>
    /// This sample does not implement a PSHostUserInterface component so
    /// this property simply returns null.
    /// </summary>
    public override PSHostUserInterface UI
    {
        get { return _userInterface; }
    }

    /// <summary>
    /// Return the version object for this application. Typically this
    /// should match the version resource in the application.
    /// </summary>
    public override Version Version
    {
        get { return new Version(1, 0, 0, 0); }
    }

    public override void SetShouldExit(int exitCode)
    {
    }

    /// <summary>
    /// Not implemented by this example class. The call fails with
    /// a NotImplementedException exception.
    /// </summary>
    public override void EnterNestedPrompt()
    {
        throw new NotImplementedException("The method or operation is not implemented.");
    }

    /// <summary>
    /// Not implemented by this example class. The call fails
    /// with a NotImplementedException exception.
    /// </summary>
    public override void ExitNestedPrompt()
    {
        throw new NotImplementedException("The method or operation is not implemented.");
    }

    /// <summary>
    /// This API is called before an external application process is 
    /// started. Typically it is used to save state so the parent can 
    /// restore state that has been modified by a child process (after 
    /// the child exits). In this example, this functionality is not  
    /// needed so the method returns nothing.
    /// </summary>
    public override void NotifyBeginApplication()
    {
        return;
    }

    /// <summary>
    /// This API is called after an external application process finishes.
    /// Typically it is used to restore state that a child process may
    /// have altered. In this example, this functionality is not  
    /// needed so the method returns nothing.
    /// </summary>
    public override void NotifyEndApplication()
    {
        return;
    }
}