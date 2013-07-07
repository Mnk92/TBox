using System;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using Mnk.TBox.Core.PluginsShared.Automator;
using Mnk.TBox.Core.PluginsShared.Tools;
using Mnk.Library.ScriptEngine;


namespace Mnk.TBox.Core.Application.Data.Automater.Scripts
{
    public class ExecuteCommands : IScript
    {
        [Directory(ShouldExist = false)]
        public string WorkingDirectory { get; set; }
        [StringList]
        public string[] Commands { get; set; }

        public void Run(IScriptContext s)
        {
            if (string.IsNullOrEmpty(WorkingDirectory))
            {
                WorkingDirectory = Environment.CurrentDirectory;
            }
            foreach (var command in Commands)
            {
                using (var p = CreateProcess(command, WorkingDirectory))
                {
                    p.WaitForExit();
                }
            }
        }

        private static Process CreateProcess(string command, string workingDirectory)
        {
            return Process.Start(new ProcessStartInfo
            {
                FileName = "cmd",
                Arguments = "/c " + command,
                WorkingDirectory = workingDirectory,
                CreateNoWindow = false,
                UseShellExecute = false,
            });
        }
    }
}
