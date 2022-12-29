﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace Frends.PowerShell.RunCommand.Definitions;

/// <summary>
/// RunOptions generated by task.
/// </summary>
public class RunOptions
{
    /// <summary>
    /// Session expression.
    /// </summary>
    /// <example>{System.Management.Automation.PowerShell} and {System.Management.Automation.Runspaces.LocalRunspace} values.</example>
    [DefaultValue(null)]
    [DisplayFormat(DataFormatString = "Expression")]
    public SessionWrapper Session { get; set; }
}