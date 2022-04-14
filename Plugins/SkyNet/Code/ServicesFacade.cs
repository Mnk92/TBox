using System.IO;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using Mnk.TBox.Plugins.SkyNet.Code.Interfaces;
using Mnk.TBox.Tools.SkyNet.Contracts;

namespace Mnk.TBox.Plugins.SkyNet.Code
{
    class ServicesFacade : IServicesFacade
    {
        private readonly IConfigurationProviderFacade configurationProviderFacade;

        public ServicesFacade(IConfigurationProviderFacade configurationProviderFacade)
        {
            this.configurationProviderFacade = configurationProviderFacade;
        }

        public async Task<IList<ServerTask>> GetServiceTasks()
        {
            using var channel = GrpcChannel.ForAddress(Config.ServerEndpoint);
            var client = new ISkyNetServerTasksService.ISkyNetServerTasksServiceClient(channel);
            return (await client.GetServerTasksAsync(new Empty())).ServerTasks;
        }

        public async Task<IList<ServerAgent>> GetServiceAgents()
        {
            using var channel = GrpcChannel.ForAddress(Config.ServerEndpoint);
            var client = new ISkyNetServerAgentsService.ISkyNetServerAgentsServiceClient(channel);
            return (await client.GetAgentsAsync(new Empty())).ServerAgents;
        }

        public async Task<AgentTask> GetAgentCurrentTask()
        {
            using var channel = GrpcChannel.ForAddress(Config.AgentEndpoint);
            var agentClient = new ISkyNetAgentService.ISkyNetAgentServiceClient(channel);
            return await agentClient.GetCurrentAgentTaskAsync(new Empty());
        }

        public async Task<SkyNetStatus> GetStatus()
        {
            return new SkyNetStatus
            {
                Task = await GetAgentCurrentTask(),
                Agents = await GetServiceAgents(),
                Tasks = await GetServiceTasks()
            };
        }


        public async Task<string> UploadFile(string path)
        {
            using var channel = GrpcChannel.ForAddress(Config.ServerEndpoint);
            var client = new ISkyNetFileService.ISkyNetFileServiceClient(channel);
            await using var f = File.OpenRead(path);
            using var result = client.Upload();
            await result.RequestStream.WriteAsync(new StreamMessage { Data = ByteString.FromStream(f) });
            return (await result.ResponseAsync).Id;
        }

        public async Task<string> StartTask(ServerTask task)
        {
            using var channel = GrpcChannel.ForAddress(Config.ServerEndpoint);
            var client = new ISkyNetServerTasksService.ISkyNetServerTasksServiceClient(channel);
            return (await client.AddServerTaskAsync(task)).Id;
        }

        public async Task Cancel(string id)
        {
            using var channel = GrpcChannel.ForAddress(Config.ServerEndpoint);
            var client = new ISkyNetServerTasksService.ISkyNetServerTasksServiceClient(channel);
            await client.CancelServerTaskAsync(new IdMessage { Id = id });
        }

        public async Task Terminate(string id)
        {
            using var channel = GrpcChannel.ForAddress(Config.ServerEndpoint);
            var client = new ISkyNetServerTasksService.ISkyNetServerTasksServiceClient(channel);
            await client.TerminateServerTaskAsync(new IdMessage { Id = id });
        }

        public async Task<string> DeleteTask(string id)
        {
            using var channel = GrpcChannel.ForAddress(Config.ServerEndpoint);
            var client = new ISkyNetServerTasksService.ISkyNetServerTasksServiceClient(channel);
            return (await client.DeleteServerTaskAsync(new IdMessage { Id = id })).Report;
        }

        public async Task<ServerTask> GetTask(string id)
        {
            using var channel = GrpcChannel.ForAddress(Config.ServerEndpoint);
            var client = new ISkyNetServerTasksService.ISkyNetServerTasksServiceClient(channel);
            return await client.GetServerTaskAsync(new IdMessage { Id = id });
        }

        public async Task DeleteFile(string zipPackageId)
        {
            using var channel = GrpcChannel.ForAddress(Config.ServerEndpoint);
            var client = new ISkyNetFileService.ISkyNetFileServiceClient(channel);
            await client.DeleteAsync(new IdMessage { Id = zipPackageId });
        }

        private AgentConfig Config => new(configurationProviderFacade.GetAgentConfig().Result);
    }
}
