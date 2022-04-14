using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Mnk.Library.Common.Log;
using Mnk.TBox.Core.Contracts;
using Mnk.TBox.Tools.SkyNet.Common;
using Mnk.TBox.Tools.SkyNet.Contracts;

namespace Mnk.TBox.Tools.SkyNet.Agent
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            LogManager.Init(new MultiplyLog(new IBaseLog[] { new ConsoleLog(), new FileLog(Path.Combine(Folders.UserLogsFolder, "SkyNet.Agent.log")) }));
            var builder = WebApplication.CreateBuilder(args);
            var settings = new ConfigSettings<AgentConfig>
            {
                Path = Path.Combine(Folders.UserToolsFolder, "SkyNet.Agent.config"),
                Config = new AgentConfig
                {
                    Name = Environment.MachineName,
                    TotalCores = Environment.ProcessorCount
                }
            };
            builder.Services.AddScoped(x => settings);
            builder.Services.AddScoped(x => ServicesRegistrar.Register(x.GetService<ConfigProvider<AgentConfig>>()));
            builder.Services.AddGrpc();
            var app = builder.Build();
            settings.Config.AgentEndpoint = app.Configuration.GetSection("Kestrel")?.GetSection("Endpoints")
                ?.GetSection("Https")?.GetSection("Url")?.Value;
            app.MapGrpcService<ConfigProvider<AgentConfig>>();
            app.MapGrpcService<SkyNetAgentService>();
            app.Run();
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
