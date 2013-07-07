using System;
using Mnk.Library.Common.Communications;
using Mnk.Library.Common.Log;
using Mnk.TBox.Plugins.SkyNet.Code.Interfaces;
using Mnk.TBox.Tools.SkyNet.Common;

namespace Mnk.TBox.Plugins.SkyNet.Code
{
    class ConfigsFacade : IConfigsFacade
    {
        private readonly ILog log = LogManager.GetLogger<ConfigsFacade>();
        private AgentConfig agentConfig = null;
        private ServerConfig serverConfig = null;

        public AgentConfig AgentConfig
        {
            get
            {
                if (agentConfig != null) return agentConfig;
                using (var cl = CreateAgentConfigProvider())
                {
                    return GetConfig(cl.Instance);
                }
            }
            set
            {
                using (var cl = CreateAgentConfigProvider())
                {
                    SetConfig(cl.Instance, agentConfig = value);
                }
            }
        }

        public ServerConfig ServerConfig
        {
            get
            {
                if (serverConfig != null) return serverConfig;
                using (var cl = CreateServerConfigProvider())
                {
                    return GetConfig(cl.Instance);
                }
            }
            set
            {
                using (var cl = CreateServerConfigProvider())
                {
                    SetConfig(cl.Instance, serverConfig=value);
                }
            }
        }

        private static InterprocessClient<IConfigProvider<AgentConfig>> CreateAgentConfigProvider()
        {
            return new InterprocessClient<IConfigProvider<AgentConfig>>(Constants.AgentServiceName);
        }

        private static InterprocessClient<IConfigProvider<ServerConfig>> CreateServerConfigProvider()
        {
            return new InterprocessClient<IConfigProvider<ServerConfig>>(Constants.ServerServiceName);
        }

        private void SetConfig<T>(IConfigProvider<T> cl, object config)
            where T : class
        {
            if (config == null) return;
            try
            {
                cl.UpdateConfig((T)config);
            }
            catch (Exception ex)
            {
                log.Write(ex, "Can't update configuration");
            }
        }

        private T GetConfig<T>(IConfigProvider<T> cl)
            where T : class
        {
            try
            {
                return cl.ReceiveConfig();
            }
            catch (Exception ex)
            {
                log.Write(ex, "Can't get configuration");
            }
            return null;
        }

    }
}
