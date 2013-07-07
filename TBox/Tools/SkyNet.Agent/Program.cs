using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using Mnk.Library.Common.Communications;
using Mnk.Library.Common.Log;
using Mnk.TBox.Core.Contracts;
using Mnk.TBox.Tools.SkyNet.Common;

namespace Mnk.TBox.Tools.SkyNet.Agent
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            LogManager.Init(new MultiplyLog(new IBaseLog[] { new ConsoleLog(), new FileLog(Path.Combine(Folders.UserLogsFolder, "SkyNet.Agent.log")) }));
            var provider = new ConfigProvider<AgentConfig>(Path.Combine(Folders.UserToolsFolder, "SkyNet.Agent.config"));
            using (new InterprocessServer<IConfigProvider<AgentConfig>>(provider, "TBox.SkyNet.Agent"))
            {
                using (var container = ServicesRegistrar.Register(provider.Config))
                {
                    using (var service = new SkyNetAgentService(provider.Config, container))
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
