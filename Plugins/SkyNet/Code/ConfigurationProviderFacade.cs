using System.Text.Json;
using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using Mnk.Library.Common.Log;
using Mnk.TBox.Plugins.SkyNet.Code.Interfaces;
using Mnk.TBox.Plugins.SkyNet.Code.Settings;
using Mnk.TBox.Tools.SkyNet.Contracts;

namespace Mnk.TBox.Plugins.SkyNet.Code
{
    class ConfigurationProviderFacade : IConfigurationProviderFacade
    {
        private readonly ILog log = LogManager.GetLogger<ConfigurationProviderFacade>();
        private readonly string agentEndpoint;
        private readonly string serverEndpoint;
        private AgentConfig agentConfig = null;
        private ServerConfig serverConfig = null;

        public ConfigurationProviderFacade(Config config)
        {
            agentEndpoint = config.AgentEndpoint;
            serverEndpoint = config.ServerEndpoint;
        }

        public async Task<AgentConfig> GetAgentConfig()
        {
            if (agentConfig != null) return agentConfig;
            using var proxy = new Proxy(agentEndpoint);
            return agentConfig = await GetConfig<AgentConfig>(proxy);
        }

        public async Task SetAgentConfig(AgentConfig config)
        {
            using var proxy = new Proxy(agentEndpoint);
            await SetConfig(proxy, agentConfig = config);
        }

        public async Task<ServerConfig> GetServerConfig()
        {
            if (serverConfig != null) return serverConfig;
            using var proxy = new Proxy(serverEndpoint);
            return serverConfig = await GetConfig<ServerConfig>(proxy);
        }

        public async Task SetServerConfig(ServerConfig config)
        {
            using var proxy = new Proxy(serverEndpoint);
            await SetConfig(proxy, serverConfig = config);
        }

        sealed class Proxy : IDisposable
        {
            private readonly GrpcChannel channel;
            private readonly IConfigProvider.IConfigProviderClient client;

            public Proxy(string endpoint)
            {
                channel = GrpcChannel.ForAddress(endpoint);
                client = new IConfigProvider.IConfigProviderClient(channel);
            }

            public void Dispose()
            {
                channel.Dispose();
            }

            public async Task<T> GetConfig<T>()
            {
                var config = await client.ReceiveConfigAsync(new Empty());
                return JsonSerializer.Deserialize<T>(config.Config);
            }

            public async Task SetConfig<T>(T config)
            {
                var str = JsonSerializer.Serialize(config);
                await client.UpdateConfigAsync(new ConfigMessage { Config = str });
            }
        }

        private async Task SetConfig<T>(Proxy proxy, T config)
            where T : class
        {
            if (config == null) return;
            try
            {
                await proxy.SetConfig(config);
            }
            catch (Exception ex)
            {
                log.Write(ex, "Can't update configuration");
            }
        }

        private async Task<T> GetConfig<T>(Proxy proxy)
            where T : class
        {
            try
            {
                return await proxy.GetConfig<T>();
            }
            catch (Exception ex)
            {
                log.Write(ex, "Can't get configuration");
            }
            return null;
        }
    }
}
