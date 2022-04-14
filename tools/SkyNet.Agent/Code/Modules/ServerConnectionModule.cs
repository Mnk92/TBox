using Grpc.Net.Client;
using Mnk.Library.Common.Log;
using Mnk.TBox.Tools.SkyNet.Common;
using Mnk.TBox.Tools.SkyNet.Common.Modules;
using Mnk.TBox.Tools.SkyNet.Contracts;

namespace Mnk.TBox.Tools.SkyNet.Agent.Code.Modules
{
    class ServerConnectionModule : IModule, IDisposable
    {
        private readonly ILog log = LogManager.GetLogger<ServerConnectionModule>();
        private readonly ConfigProvider<AgentConfig> configProvider;

        public ServerConnectionModule(ConfigProvider<AgentConfig> configProvider)
        {
            this.configProvider = configProvider;
        }

        public async Task Process()
        {
            try
            {
                using var channel = GrpcChannel.ForAddress(configProvider.Config.ServerEndpoint);
                var client = new ISkyNetServerAgentsService.ISkyNetServerAgentsServiceClient(channel);
                await client.ConnectAgentAsync(new ServerAgent
                {
                    TotalCores = configProvider.Config.TotalCores,
                    Endpoint = configProvider.Config.AgentEndpoint,
                    State = ServerAgent.Types.AgentState.Idle
                });
            }
            catch (Exception ex)
            {
                log.Write(ex, "Can't connect to server");
            }
        }

        public void Dispose()
        {
            try
            {
                using var channel = GrpcChannel.ForAddress(configProvider.Config.ServerEndpoint);
                var client = new ISkyNetServerAgentsService.ISkyNetServerAgentsServiceClient(channel);
                client.DisconnectAgent(new EndpointMessage { Endpoint = configProvider.Config.AgentEndpoint });
            }
            catch (Exception ex)
            {
                log.Write(ex, "Can't disconnect from server");
            }
        }
    }
}
