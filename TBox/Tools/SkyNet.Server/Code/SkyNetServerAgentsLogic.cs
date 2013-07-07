using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Mnk.Library.Common.Communications;
using Mnk.Library.Common.Tools;
using Mnk.TBox.Tools.SkyNet.Common;
using Mnk.TBox.Tools.SkyNet.Server.Code.Interfaces;

namespace Mnk.TBox.Tools.SkyNet.Server.Code
{
    class SkyNetServerAgentsLogic : ISkyNetServerAgentsLogic
    {
        private readonly IHttpContextHelper contextHelper;
        private readonly IServerContext serverContext;

        public SkyNetServerAgentsLogic(IHttpContextHelper contextHelper, IServerContext serverContext)
        {
            this.contextHelper = contextHelper;
            this.serverContext = serverContext;
        }

        public IList<ServerAgent> GetAgents()
        {
            return serverContext.Config.Agents;
        }

        public void ConnectAgent(ServerAgent agent)
        {
            if (string.IsNullOrEmpty(agent.Endpoint) || agent.TotalCores <= 0)
            {
                contextHelper.SetStatusCode(HttpStatusCode.ExpectationFailed);
                return;
            }
            var exist = serverContext.Config.Agents.FirstOrDefault(x => x.Endpoint.EqualsIgnoreCase(agent.Endpoint));
            if (exist != null)
            {
                if(exist.TotalCores == agent.TotalCores)return;
                exist.TotalCores = agent.TotalCores;
            }
            else
            {
                serverContext.Config.Agents.Add(new ServerAgent
                {
                    Endpoint = agent.Endpoint,
                    TotalCores = agent.TotalCores,
                    State = AgentState.Idle
                });
            }
        }

        public void DisconnectAgent(string endpoint)
        {
            var agent = serverContext.Config.Agents.FirstOrDefault(x => x.Endpoint.EqualsIgnoreCase(endpoint));
            if (agent == null)
            {
                contextHelper.SetStatusCode(HttpStatusCode.NotFound);
                return;
            }
            serverContext.Config.Agents.Remove(agent);
        }

        public void PingIsAlive()
        {
        }
    }
}
