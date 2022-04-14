using Grpc.Net.Client;
using Mnk.TBox.Tools.SkyNet.Contracts;
using Mnk.TBox.Tools.SkyNet.Server.Code.Interfaces;

namespace Mnk.TBox.Tools.SkyNet.Server.Code
{
    class AgentsCache : IAgentsCache
    {
        class Connection
        {
            public GrpcChannel Channel { get; init; }
            public ISkyNetAgentService.ISkyNetAgentServiceClient Client { get; init; }
        }

        private readonly IDictionary<string, Connection> items =
            new Dictionary<string, Connection>();

        public ISkyNetAgentService.ISkyNetAgentServiceClient Get(ServerAgent agent)
        {
            lock (items)
            {
                if (items.TryGetValue(agent.Endpoint, out var connection))
                {
                    return connection.Client;
                }

                var channel = GrpcChannel.ForAddress(agent.Endpoint);
                connection = new Connection
                {
                    Channel = channel,
                    Client = new ISkyNetAgentService.ISkyNetAgentServiceClient(channel)
                };
                items.Add(agent.Endpoint, connection);
                return connection.Client;
            }
        }

        public void Clear()
        {
            lock (items)
            {
                foreach (var s in items)
                {
                    s.Value.Channel.Dispose();
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
