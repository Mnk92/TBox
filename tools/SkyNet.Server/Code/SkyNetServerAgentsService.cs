using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using LightInject;
using Mnk.Library.Common.Tools;
using Mnk.TBox.Tools.SkyNet.Contracts;
using Mnk.TBox.Tools.SkyNet.Server.Code.Interfaces;

namespace Mnk.TBox.Tools.SkyNet.Server.Code
{
    class SkyNetServerAgentsService : ISkyNetServerAgentsService.ISkyNetServerAgentsServiceBase
    {
        private readonly IServerContext serverContext;

        public SkyNetServerAgentsService(IServiceContainer container)
        {
            serverContext = container.GetInstance<IServerContext>();
        }

        public override Task<ServerAgentsMessage> GetAgents(Empty request, ServerCallContext context)
        {
            return Task.FromResult(new ServerAgentsMessage { ServerAgents = { serverContext.Config.Agents } });
        }

        public override Task<Empty> ConnectAgent(ServerAgent request, ServerCallContext context)
        {
            if (string.IsNullOrEmpty(request.Endpoint) || request.TotalCores <= 0)
            {
                throw new RpcException(new Status(StatusCode.FailedPrecondition, "Invalid parameters"));
            }
            var exist = serverContext.Config.Agents.FirstOrDefault(x => x.Endpoint.EqualsIgnoreCase(request.Endpoint));
            if (exist != null)
            {
                if (exist.TotalCores == request.TotalCores)
                {
                    return Task.FromResult(new Empty());
                }
                exist.TotalCores = request.TotalCores;
            }
            else
            {
                serverContext.Config.Agents.Add(new ServerAgent
                {
                    Endpoint = request.Endpoint,
                    TotalCores = request.TotalCores,
                    State = ServerAgent.Types.AgentState.Idle
                });
            }
            return Task.FromResult(new Empty());
        }

        public override Task<Empty> DisconnectAgent(EndpointMessage request, ServerCallContext context)
        {
            var agent = serverContext.Config.Agents.FirstOrDefault(x => x.Endpoint.EqualsIgnoreCase(request.Endpoint));
            if (agent == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Agent not found"));
            }
            serverContext.Config.Agents.Remove(agent);
            return Task.FromResult(new Empty());
        }

        public override Task<Empty> PingIsServerAgentsAlive(Empty request, ServerCallContext context)
        {
            return Task.FromResult(new Empty());
        }
    }
}
