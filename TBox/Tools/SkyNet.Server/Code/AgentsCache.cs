using System;
using System.Collections.Generic;
using Mnk.Library.Common.Communications;
using Mnk.TBox.Tools.SkyNet.Common;
using Mnk.TBox.Tools.SkyNet.Server.Code.Interfaces;

namespace Mnk.TBox.Tools.SkyNet.Server.Code
{
    class AgentsCache : IAgentsCache
    {
        private readonly IDictionary<ServerAgent,NetworkClient<ISkyNetAgentService>> items = 
            new Dictionary<ServerAgent, NetworkClient<ISkyNetAgentService>>();

        public ISkyNetAgentService Get(ServerAgent agent)
        {
            lock (items)
            {
                NetworkClient<ISkyNetAgentService> service;
                if (items.TryGetValue(agent, out service))
                {
                    return service.Instance;
                }
                items.Add(agent, service = new NetworkClient<ISkyNetAgentService>(new Uri(agent.Endpoint)));
                return service.Instance;
            }
        }

        public void Clear()
        {
            lock (items)
            {
                foreach (var s in items)
                {
                    s.Value.Dispose();
                }
                items.Clear();
            }
        }

        public void Dispose()
        {
            Clear();
        }

    }
}
