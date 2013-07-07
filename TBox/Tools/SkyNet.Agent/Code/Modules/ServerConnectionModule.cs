using System;
using Mnk.Library.Common.Communications;
using Mnk.Library.Common.Log;
using Mnk.TBox.Tools.SkyNet.Common;
using Mnk.TBox.Tools.SkyNet.Common.Modules;

namespace Mnk.TBox.Tools.SkyNet.Agent.Code.Modules
{
    class ServerConnectionModule : IModule, IDisposable
    {
        private readonly ILog log = LogManager.GetLogger<ServerConnectionModule>();
        private readonly AgentConfig config;

        public ServerConnectionModule(AgentConfig config)
        {
            this.config = config;
        }

        public void Process()
        {
            try
            {
                using (var cl = new NetworkClient<ISkyNetServerAgentsService>(new Uri(config.ServerEndpoint)))
                {
                    cl.Instance.ConnectAgent(new ServerAgent
                    {
                        TotalCores = config.TotalCores,
                        Endpoint = ServerEndpoint
                    });
                }
            }
            catch (Exception ex)
            {
                log.Write(ex, "Can't connect to server");
            }

        }

        public string ServerEndpoint
        {
            get { return NetworkServer<ISkyNetServerAgentsService>.BuildEndpoint(config.Port); }
        }

        public void Dispose()
        {
            /*
            try
            {
                using (var cl = new NetworkClient<ISkyNetServerAgentsService>(new Uri(config.ServerEndpoint)))
                {
                    cl.Instance.DisconnectAgent(ServerEndpoint);
                }
            }
            catch (Exception ex)
            {
                log.Write(ex, "Can't disconnect from server");
            }
            */
        }
    }
}
