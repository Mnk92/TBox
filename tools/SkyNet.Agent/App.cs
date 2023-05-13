using Microsoft.AspNetCore.Builder;
using Mnk.Library.Common.Log;
using Mnk.TBox.Core.Contracts;
using Mnk.TBox.Tools.SkyNet.Common;
using Mnk.TBox.Tools.SkyNet.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Mnk.TBox.Tools.SkyNet.Agent
{
    internal class App
    {
        public static void Run(string[] args)
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
    }
}
