using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using LightInject;
using Mnk.Library.Common.Tools;
using Mnk.TBox.Tools.SkyNet.Contracts;
using Mnk.TBox.Tools.SkyNet.Server.Code.Interfaces;

namespace Mnk.TBox.Tools.SkyNet.Server.Code
{
    class SkyNetServerTasksService : ISkyNetServerTasksService.ISkyNetServerTasksServiceBase
    {
        private readonly IServerContext serverContext;
        private readonly ISkyAgentLogic skyAgentLogic;

        public SkyNetServerTasksService(IServiceContainer container)
        {
            this.serverContext = container.GetInstance<IServerContext>();
            this.skyAgentLogic = container.GetInstance<ISkyAgentLogic>();
        }

        public override Task<IdMessage> AddServerTask(ServerTask request, ServerCallContext context)
        {
            request.Id = Guid.NewGuid().ToString();
            if (string.IsNullOrEmpty(request.ScriptParameters) || string.IsNullOrEmpty(request.Script))
            {
                throw new RpcException(new Status(StatusCode.FailedPrecondition, "Invalid parameters"));
            }
            serverContext.Config.Tasks.Add(new ServerTask
            {
                Id = request.Id,
                State = ServerTask.Types.TaskState.Idle,
                ScriptParameters = request.ScriptParameters,
                Script = request.Script,
                Owner = request.Owner,
                ZipPackageId = request.ZipPackageId,
                CreatedTime = Timestamp.FromDateTime(DateTime.UtcNow),
                IsDone = false,
                Progress = 0,
                Report = string.Empty
            });
            return Task.FromResult(new IdMessage { Id = request.Id });
        }

        public override Task<ServerTasksMessage> GetServerTasks(Empty request, ServerCallContext context)
        {
            var result = serverContext.Config.Tasks
                .Select(x => new ServerTask
                {
                    Id = x.Id,
                    State = x.State,
                    Progress = x.Progress,
                    IsDone = x.IsDone,
                    Owner = x.Owner,
                    CreatedTime = x.CreatedTime
                })
                .ToArray();
            return Task.FromResult(new ServerTasksMessage { ServerTasks = { result } });
        }

        public override async Task<Empty> CancelServerTask(IdMessage request, ServerCallContext context)
        {
            await Parallel.ForEachAsync(serverContext.Config.Agents,
                async (agent, _) => await skyAgentLogic.CancelTask(agent, request.Id));
            return new Empty();
        }

        public override async Task<Empty> TerminateServerTask(IdMessage request, ServerCallContext context)
        {
            await Parallel.ForEachAsync(serverContext.Config.Agents,
                async (agent, _) => await skyAgentLogic.TerminateTask(agent, request.Id));
            return new Empty();
        }

        public override Task<ServerTask> GetServerTask(IdMessage request, ServerCallContext context)
        {
            var task = serverContext.Config.Tasks.FirstOrDefault(x => x.Id.EqualsIgnoreCase(request.Id));
            if (task == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Task not found"));
            }
            return Task.FromResult(task);
        }

        public override async Task<ReportMessage> DeleteServerTask(IdMessage request, ServerCallContext context)
        {
            var task = await GetServerTask(request, context);
            if (task == null) return new ReportMessage();
            if (task.State == ServerTask.Types.TaskState.InProgress)
            {
                throw new RpcException(new Status(StatusCode.Internal, "Task is already in progress"));
            }
            serverContext.Config.Tasks.Remove(task);
            return new ReportMessage { Report = task.Report };
        }
    }
}
