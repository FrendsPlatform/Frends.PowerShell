using System.Diagnostics;
using System.Management.Automation.Runspaces;
namespace Frends.PowerShell.RunCommand;

/// <summary>
/// Wraps the powershell session
/// </summary>
public class SessionWrapper : IDisposable
{
    internal Runspace Runspace;
    internal System.Management.Automation.PowerShell PowerShell;

    internal SessionWrapper()
    {
        var host = new TaskPowershellHost();
        Runspace = RunspaceFactory.CreateRunspace(host);
        Runspace.Open();
        PowerShell = System.Management.Automation.PowerShell.Create();

        PowerShell.Runspace = Runspace;
    }

    private void ReleaseUnmanagedResources()
    {
        try
        {
            PowerShell?.Dispose();
        }
        catch (Exception e)
        {
            Trace.WriteLine($"Encountered error while disposing powershell session: {e}");
        }
        PowerShell = null;

        try
        {
            Runspace?.Dispose();
        }
        catch (Exception e)
        {
            Trace.WriteLine($"Encountered error while disposing powershell runspace: {e}");
        }
        Runspace = null;
    }

    /// <summary>
    /// Dispose.
    /// </summary>
    public void Dispose()
    {
        ReleaseUnmanagedResources();
        GC.SuppressFinalize(this);
    }
}
