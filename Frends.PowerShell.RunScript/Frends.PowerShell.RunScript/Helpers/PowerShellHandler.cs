using System.Management.Automation;

namespace Frends.PowerShell.RunScript.Helpers;

internal static class PowerShellHandler
{
    internal static object GetResultObject(PSObject result)
    {
        if (result?.BaseObject is null or PSCustomObject)
            return result;

        return result.BaseObject;
    }
}
