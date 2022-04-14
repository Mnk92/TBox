using Google.Protobuf.WellKnownTypes;
using Mnk.Library.Common.Log;
using Mnk.TBox.Tools.SkyNet.Contracts;
using Mnk.TBox.Tools.SkyNet.Server.Code.Interfaces;

namespace Mnk.TBox.Tools.SkyNet.Server.Code.Processing
{
    class SkyAgentLogic : ISkyAgentLogic
    {
        private readonly IAgentsCache cache;
        private readonly ILog log = LogManager.GetLogger<SkyAgentLogic>();

        public SkyAgentLogic(IAgentsCache cache)
        {
            this.cache = cache;
        }

        public async Task<WorkerTask> CreateWorkerTask(ServerAgent agent, string agentData, ServerTask task)
        {
            var agentTask = new AgentTask
            {
                ScriptParameters = task.ScriptParameters,
                Config = agentData,
                Script = task.Script,
                ZipPackageId = task.ZipPackageId
            };
            try
            {
                agentTask.Id = (await cache.Get(agent).AddAgentTaskAsync(agentTask)).Id;
                return new WorkerTask
                {
                    Agent = agent,
                    Task = agentTask
                };
            }
            catch (Exception ex)
            {
                log.Write(ex, "Can't create agent task");
                return new WorkerTask
                {
                    Exception = ex
                };
            }
        }

        public async Task<AgentTask> GetTask(WorkerTask workerTask)
        {
            if (workerTask.IsFailed) return null;
            return await cache.Get(workerTask.Agent).GetAgentTaskAsync(new IdMessage { Id = workerTask.Task.Id });
        }

        public async Task<SkyAgentWork> BuildReport(WorkerTask task)
        {
            var report = string.Empty;
            if (!task.IsFailed)
            {
                try
                {
                    report = (await cache.Get(task.Agent).DeleteAgentTaskAsync(new IdMessage { Id = task.Task.Id })).Report;
                }
                catch (Exception ex)
                {
                    log.Write(ex, "Can't get agent task state");
                    task.Exception = ex;
                }
            }
            if (task.IsFailed)
            {
                report = task.Exception.ToString();
            }
            return new SkyAgentWork
            {
                IsFailed = task.IsFailed,
                Config = task.Config,
                Agent = task.Agent,
                Report = report
            };
        }

        public async Task<bool> IsAlive(ServerAgent agent)
        {
            try
            {
                await cache.Get(agent).PingIsAgentAliveAsync(new Empty());
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<AgentTask> GetCurrentTask(ServerAgent agent)
        {
            return await cache.Get(agent).GetCurrentAgentTaskAsync(new Empty());
        }

        public async Task DeleteTask(ServerAgent agent, string id)
        {
            await cache.Get(agent).DeleteAgentTaskAsync(new IdMessage { Id = id });
        }

        public async Task TerminateTask(ServerAgent agent, string id)
        {
            await cache.Get(agent).TerminateAgentTaskAsync(new IdMessage { Id = id });
        }

        public async Task CancelTask(ServerAgent agent, string id)
        {
            await cache.Get(agent).CancelTaskAsync(new IdMessage { Id = id });
        }
    }
}
