namespace Frends.PowerShell.RunScript.Definitions;

/// <summary>
/// Execution policy used by a custom PowerShell process.
/// </summary>
public enum PowerShellExecutionPolicy
{
    /// <summary>
    /// Do not set a process execution policy. PowerShell resolves the effective policy from its environment.
    /// </summary>
    SystemDefault,

    /// <summary>
    /// Permit individual commands but do not permit scripts.
    /// </summary>
    Restricted,

    /// <summary>
    /// Require all scripts and configuration files to be signed by a trusted publisher.
    /// </summary>
    AllSigned,

    /// <summary>
    /// Require scripts downloaded from the internet to be signed by a trusted publisher.
    /// </summary>
    RemoteSigned,

    /// <summary>
    /// Permit unsigned scripts and warn before running scripts downloaded from the internet.
    /// </summary>
    Unrestricted,

    /// <summary>
    /// Do not block scripts or display warnings.
    /// </summary>
    Bypass,
}
