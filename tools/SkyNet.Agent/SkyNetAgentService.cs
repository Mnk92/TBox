using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using LightInject;
using Mnk.Library.Common.Log;
using Mnk.Library.Common.Tools;
using Mnk.TBox.Tools.SkyNet.Agent.Code;
using Mnk.TBox.Tools.SkyNet.Contracts;

namespace Mnk.TBox.Tools.SkyNet.Agent
{
    class SkyNetAgentService : Contracts.ISkyNetAgentService.ISkyNetAgentServiceBase
    {
        private AgentTask currentTask = null;
        private readonly IWorker worker;
        private readonly ILog log = LogManager.GetLogger<SkyNetAgentService>();

        public SkyNetAgentService(IWorker worker)
        {
            this.worker = worker;
        }

        public override Task<IdMessage> AddAgentTask(AgentTask request, ServerCallContext context)
        {
            if (currentTask != null)
            {
                throw new ArgumentException("Task already in progress.");
            }
            currentTask = request;
            currentTask.Progress = 0;
            currentTask.IsDone = false;
            currentTask.Report = string.Empty;
            worker.Start(currentTask);
            currentTask.Id = Guid.NewGuid().ToString();
            return Task.FromResult(new IdMessage { Id = currentTask.Id });
        }

        public override Task<AgentTask> GetAgentTask(IdMessage request, ServerCallContext context)
        {
            if (currentTask == null || !currentTask.Id.EqualsIgnoreCase(request.Id))
            {
                throw new ArgumentException("Could not find task.");
            }
            return Task.FromResult(currentTask);
        }

        public override Task<AgentTask> GetCurrentAgentTask(Empty request, ServerCallContext context)
        {
            if (currentTask == null) return Task.FromResult<AgentTask>(null);
            return Task.FromResult(new AgentTask
            {
                Id = currentTask.Id,
                Progress = currentTask.Progress,
                IsDone = worker.IsDone || currentTask.IsCanceled,
            });
        }

        public override Task<Empty> CancelTask(IdMessage request, ServerCallContext context)
        {
            if (currentTask == null || !currentTask.Id.EqualsIgnoreCase(request.Id))
            {
                throw new ArgumentException("Could not find task.");
            }
            currentTask.IsCanceled = true;
            worker.Cancel();
            return Task.FromResult(new Empty());
        }

        public override Task<Empty> TerminateAgentTask(IdMessage request, ServerCallContext context)
        {
            if (currentTask == null || !currentTask.Id.EqualsIgnoreCase(request.Id))
            {
                throw new ArgumentException("Could not find task.");
            }
            currentTask.IsCanceled = true;
            worker.Terminate();
            return Task.FromResult(new Empty());
        }

        public override Task<ReportMessage> DeleteAgentTask(IdMessage request, ServerCallContext context)
        {
            if (currentTask == null || !currentTask.Id.EqualsIgnoreCase(request.Id))
            {
                throw new ArgumentException("Could not find task.");
            }
            if (!(worker.IsDone || currentTask.IsCanceled))
            {
                throw new ArgumentException("Task in progress.");
            }
            var report = currentTask.Report;
            currentTask = null;
            return Task.FromResult(new ReportMessage { Report = report });
        }

        public override Task<Empty> PingIsAgentAlive(Empty request, ServerCallContext context)
        {
            return Task.FromResult(new Empty());
        }
    }
}
