using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Mnk.Library.Common.Log;
using Mnk.TBox.Core.Contracts;
using Mnk.TBox.Tools.SkyNet.Common;
using Mnk.TBox.Tools.SkyNet.Contracts;
using Mnk.TBox.Tools.SkyNet.Server.Code;

namespace Mnk.TBox.Tools.SkyNet.Server
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            LogManager.Init(new MultiplyLog(new ConsoleLog(), new FileLog(Path.Combine(Folders.UserLogsFolder, "SkyNet.Server.log"))));
            var builder = WebApplication.CreateBuilder(args);
            var settings = new ConfigSettings<ServerConfig>
            {
                Path = Path.Combine(Folders.UserToolsFolder, "SkyNet.Server.config"),
                Config = new ServerConfig
                {
                    MaximumTaskExecutionTime = 60,
                }
            };
            builder.Services.AddScoped(_ => settings);
            builder.Services.AddScoped(x => ServicesRegistrar.Register(x.GetService<ConfigProvider<ServerConfig>>()));
            builder.Services.AddGrpc();
            var app = builder.Build();
            settings.Config.ServerEndpoint = app.Configuration.GetSection("Kestrel")?.GetSection("Endpoints")
                ?.GetSection("Https")?.GetSection("Url")?.Value;
            app.MapGrpcService<ConfigProvider<ServerConfig>>();
            app.MapGrpcService<SkyNetFileService>();
            app.MapGrpcService<SkyNetServerAgentsService>();
            app.MapGrpcService<SkyNetServerTasksService>();
            app.MapGrpcService<SkyNetFileService>();
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

