using Mnk.TBox.Tools.SkyNet.Common.Modules;
using Mnk.TBox.Tools.SkyNet.Contracts;
using Mnk.TBox.Tools.SkyNet.Server.Code.Interfaces;

namespace Mnk.TBox.Tools.SkyNet.Server.Code.Modules
{
    class IdleTasksProcessorModule : IModule
    {
        private readonly IAgentsCache cache;
        private readonly IServerContext serverContext;
        private readonly IWorker worker;

        public IdleTasksProcessorModule(IAgentsCache cache, IServerContext serverContext, IWorker worker)
        {
            this.cache = cache;
            this.serverContext = serverContext;
            this.worker = worker;
        }

        public Task Process()
        {
            var task = GetNextTask();
            if (task != null)
            {
                var agents = GetAgents();
                if (agents.Any())
                {
                    PrepareToStart(task, agents);
                    try
                    {
                        worker.ProcessTask(task, agents);
                    }
                    catch (Exception ex)
                    {
                        task.Report = "Server failed" + Environment.NewLine + ex;
                    }
                    finally
                    {
                        PrepareToEnd(task, agents);
                    }
                }
            }
            return Task.CompletedTask;
        }

        private ServerTask GetNextTask()
        {
            lock (serverContext)
            {
                return serverContext.Config.Tasks.FirstOrDefault(x => x.State == ServerTask.Types.TaskState.Idle);
            }
        }

        private IList<ServerAgent> GetAgents()
        {
            lock (serverContext)
            {
                return serverContext.Config.Agents
                    .Where(x => x.State == ServerAgent.Types.AgentState.Idle)
                    .ToArray();
            }
        }

        private void PrepareToStart(ServerTask task, IEnumerable<ServerAgent> agents)
        {
            cache.Clear();
            lock (serverContext)
            {
                task.State = ServerTask.Types.TaskState.InProgress;
                foreach (var agent in agents)
                {
                    agent.State = ServerAgent.Types.AgentState.InProgress;
                }
            }
        }

        private void PrepareToEnd(ServerTask task, IEnumerable<ServerAgent> agents)
        {
            lock (serverContext)
            {
                task.State = ServerTask.Types.TaskState.Done;
                foreach (var agent in agents)
                {
                    agent.State = ServerAgent.Types.AgentState.Idle;
                }
            }
        }

    }
}
