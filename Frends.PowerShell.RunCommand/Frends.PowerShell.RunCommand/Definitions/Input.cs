using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

#pragma warning disable 1591

namespace Frends.PowerShell.RunCommand
{
    public class RunCommandInput
    {
        /// <summary>
        /// The PowerShell command to execute 
        /// </summary>
        public string Command { get; set; }

        /// <summary>
        /// Parameters for the command, provided switch parameters need to have a boolean value
        /// </summary>
        public PowerShellParameter[] Parameters { get; set; }

        /// <summary>
        /// Should the information stream be logged. If false, log will be an empty string.
        /// If set to true, a lot of string data may be logged. Use with caution.
        /// </summary>
        public bool LogInformationStream { get; set; }
    }

    public class PowerShellParameter
    {
        public string Name { get; set; }
        [DisplayFormat(DataFormatString = "Text")]
        public object Value { get; set; }
    }

    public class RunOptions
    {
        [DefaultValue(null)]
        [DisplayFormat(DataFormatString = "Expression")]
        public SessionWrapper Session { get; set; }
    }

    public class PowerShellResult
    {
        public IList<dynamic> Result { get; set; }
        public IList<string> Errors { get; set; }
        public string Log { get; set; }
    }
}
