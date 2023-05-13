using Mnk.Library.Common.Log;
using Mnk.Library.ScriptEngine.Core;
using Mnk.TBox.Core.Contracts;
using System;
using System.IO;

namespace Mnk.TBox.Tools.ConsoleScriptRunner
{
    internal class App
    {
        public static int Run(string[] args) {
            LogManager.Init(new MultiplyLog(new ConsoleLog(), new FileLog(Path.Combine(Folders.UserLogsFolder, "ConsoleScriptRunner.log"))));
            var log = LogManager.GetLogger<Program>();

            if (args.Length <= 0)
            {
                log.Write("You should specify at least 1 parameter: profile name to run");
                return -1;
            }
            try
            {
                var rootPath = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).Parent.FullName;
                Console.WriteLine("Load libraries");
                var loader = new Loader();
                loader.Load(rootPath);
                var worker = new Worker(
                    GetConfigsPath(),
                    rootPath,
                    new CopyPathResolver());
                foreach (var s in args)
                {
                    Console.WriteLine("Execute: " + s);
                    worker.Run(s);
                }
                return 0;

            }
            catch (CompilerExceptions cex)
            {
                log.Write("Compiler error: " + cex);
                return -1;
            }
            catch (Exception ex)
            {
                log.Write(ex, "Internal error");
                return -1;
            }
        }
        private static string GetConfigsPath()
        {
            return !File.Exists(Path.Combine(Folders.UserRootFolder, "Config.config")) ?
                AppDomain.CurrentDomain.BaseDirectory : Folders.UserRootFolder;
        }
    }
}
