using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Runtime.CompilerServices;

[assembly:InternalsVisibleTo("Frends.PowerShell.Tests")]
namespace Frends.PowerShell
{
    /// <summary>
    /// Wraps the powershell session,
    /// </summary>
    public class SessionWrapper : IDisposable
    {
        internal Runspace Runspace;
        internal System.Management.Automation.PowerShell PowerShell;

        public SessionWrapper()
        {
            var host = new TaskPowershellHost();
            Runspace = RunspaceFactory.CreateRunspace(host);
            PowerShell = System.Management.Automation.PowerShell.Create();

            Runspace.Open();
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

        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }
    }

    public static class Runner
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
        /// <returns>Object { PSObject Result }</returns>
        public static PowerShellResult RunScript(RunScriptInput input, [Browsable(false)]RunOptions options)
        {
            return DoAndHandleSession(options?.Session, session =>
            {
                var script = input.Script;
                if (input.ReadFromFile)
                {
                    script = File.ReadAllText(input.ScriptFilePath);
                }

                var powershell = session.PowerShell;
                powershell.AddScript(script);

                return ExecutePowershell(powershell);
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

        /// <summary>
        /// Executes a PowerShell command with parameters, leave parameter value empty for a switch
        /// </summary>
        /// <returns>Object { PSObject Result }</returns>
        public static PowerShellResult RunCommand(RunCommandInput input, [Browsable(false)]RunOptions options)
        {
            return DoAndHandleSession(options?.Session, (session) =>
            {
                var powershell = session.PowerShell;

                var command = new Command(input.Command, isScript: false, useLocalScope: false);
                foreach (var parameter in input.Parameters)
                {
                    var parameterName = parameter.Name.TrimStart('-'); // Remove dash from start
                    if (parameter.Value == null ||
                        (parameter.Value is String && string.IsNullOrWhiteSpace((string)parameter.Value)))
                    {
                        command.Parameters.Add(new CommandParameter(parameterName));
                    }
                    else
                    {
                        command.Parameters.Add(new CommandParameter(parameterName, parameter.Value));
                    }

                }

                powershell.Commands.AddCommand(command);

                return ExecutePowershell(powershell);
            });

        }

        private static PowerShellResult ExecutePowershell(System.Management.Automation.PowerShell powershell)
        {
            var result = powershell.Invoke();

            if (powershell.HadErrors)
            {
                throw new Exception(string.Join("\n", powershell.Streams.Error.Select(e => e.Exception.Message)));
            }

            powershell.Commands.Clear(); // Clear the executed commands so they do not get executed again

            return new PowerShellResult
            {
                Result = result.LastOrDefault(o => o != null)?.BaseObject
            };
        }
    }
}
