﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Runtime.CompilerServices;
using System.Text;

#pragma warning disable 1591

[assembly: InternalsVisibleTo("Frends.PowerShell.RunScript.Tests")]
namespace Frends.PowerShell.RunScript
{

    public static class PowerShell
    {
        /// <summary>
        /// Creates a PowerShell session which can be shared between scripts and commands.
        /// </summary>
        /// <returns>Object</returns>
        internal static SessionWrapper CreateSession()
        {
            // Leave this internal for now, turn public to support shared sessions between task executions
            return new SessionWrapper();
        }

        /// <summary>
        /// Executes a PowerShell script from a file or the script parameter
        /// </summary>
        /// <returns>Object { Result: List&lt;dynamic&gt;, Errors: List&lt;string&gt;, Log: string}</returns>
        public static PowerShellResult RunScript(RunScriptInput input, [Browsable(false)]RunOptions options)
        {
            return DoAndHandleSession(options?.Session, session =>
            {
                var script = input.Script;
                if (input.ReadFromFile)
                {
                    script = File.ReadAllText(input.ScriptFilePath);
                }

                var tempScript = Path.Combine(Path.GetTempPath(), $"{Path.GetRandomFileName()}.ps1");
                try
                {
                    File.WriteAllText(tempScript, script, Encoding.UTF8);

                    return ExecuteCommand(tempScript, input.Parameters, input.LogInformationStream, session.PowerShell);
                }
                finally
                {
                    File.Delete(tempScript);
                }
            });
        }

        private static PowerShellResult DoAndHandleSession(SessionWrapper sessionFromOutside, Func<SessionWrapper, PowerShellResult> action)
        {
            SessionWrapper internalSession = null;
            // use the external session if provided or create and dispose an internal session
            var session = sessionFromOutside ?? (internalSession = CreateSession());

            try
            {
                return action.Invoke(session);
            }
            finally
            {
                internalSession?.Dispose();
            }
        }

        private static PowerShellResult ExecuteCommand(string inputCommand, PowerShellParameter[] powerShellParameters, bool logInformationStream,
            System.Management.Automation.PowerShell powershell)
        {
            var command = new Command(inputCommand, isScript: false, useLocalScope: false);

            foreach (var parameter in powerShellParameters ?? new PowerShellParameter[] { })
            {
                var parameterName = parameter.Name.Trim('-', ' '); // Remove dash from start

                // Switch parameters will have to specify value as true:
                command.Parameters.Add(new CommandParameter(parameterName, parameter.Value));
            }

            powershell.Commands.AddCommand(command);

            return ExecutePowershell(powershell, logInformationStream);
        }

        private static IList<string> GetErrorMessages(PSDataCollection<ErrorRecord> errors)
        {
            return errors.Select(err => $"{err.ScriptStackTrace}: {err.Exception.Message}").ToList();
        }

        private static PowerShellResult ExecutePowershell(System.Management.Automation.PowerShell powershell, bool logInformationStream)
        {
            try
            {
                var execution = powershell.Invoke();
                var result = new PowerShellResult
                {
                    // Powershell return values are usually wrapped inside of a powershell object, unwrap it or if it does not have a baseObject, return the actual object
                    Result = execution?.Select(GetResultObject).ToList(),
                    Errors = GetErrorMessages(powershell.Streams.Error),
                    Log = logInformationStream == false ? "" : string.Join("\n", powershell.Streams.Information.Select(info => info.MessageData.ToString()))
                };

                return result;
            }
            catch (Exception e)
            {
                throw new Exception($"Encountered terminating error while executing powershell: \n{e}\nErrors:\n{string.Join("\n", GetErrorMessages(powershell.Streams.Error))}");
            }
            finally
            {
                powershell.Commands.Clear(); // Clear the executed commands from the session so they do not get executed again
                powershell.Streams.ClearStreams();
            }
        }

        private static object GetResultObject(PSObject result)
        {
            if (result?.BaseObject == null || result.BaseObject is PSCustomObject)
            {
                return result;
            }

            return result.BaseObject;
        }
    }
}
