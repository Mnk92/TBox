using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using Mnk.Library.Common.Communications;
using Mnk.Library.Common.Log;
using Mnk.TBox.Core.Contracts;
using Mnk.TBox.Tools.SkyNet.Common;

namespace Mnk.TBox.Tools.SkyNet.Server
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            LogManager.Init(new MultiplyLog(new ConsoleLog(), new FileLog(Path.Combine(Folders.UserLogsFolder, "SkyNet.Server.log")) ));
            var provider = new ConfigProvider<ServerConfig>(Path.Combine(Folders.UserToolsFolder, "SkyNet.Sever.config"));
            using (new InterprocessServer<IConfigProvider<ServerConfig>>(provider, "TBox.SkyNet.Server"))
            {
                using (var container = ServicesRegistrar.Register(provider.Config))
                {
                    using (var service = new SkyNetServerService(provider.Config, container))
                    {
                        if (Environment.UserInteractive)
                        {
                            service.StartService();
                            Console.WriteLine("Press any key to stop program");
                            Console.ReadKey();
                            service.StopService();
                        }
                        else
                        {
                            ServiceBase.Run(new ServiceBase[] { service });
                        }
                    }
                }
            }
        }

        static Program()
        {
            AppDomain.CurrentDomain.AssemblyResolve += LoadFromSameFolder;
        }

        static Assembly LoadFromSameFolder(object sender, ResolveEventArgs args)
        {
            return (from dir in new[] { "Libraries", "Localization" } select Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\" + dir + "\\", new AssemblyName(args.Name).Name + ".dll")) into assemblyPath where File.Exists(assemblyPath) select Assembly.LoadFrom(assemblyPath)).FirstOrDefault();
        }
    }
}
