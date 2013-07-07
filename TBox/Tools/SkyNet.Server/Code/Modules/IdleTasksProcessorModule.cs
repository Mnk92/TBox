using System;
using System.Collections.Generic;
using System.Linq;
using Mnk.TBox.Tools.SkyNet.Common;
using Mnk.TBox.Tools.SkyNet.Common.Modules;
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

        public void Process()
        {
            var task = GetNextTask();
            if(task == null)return;
            var agents = GetAgents();
            if(!agents.Any())return;
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

        private ServerTask GetNextTask()
        {
            lock (serverContext)
            {
                return serverContext.Config.Tasks.FirstOrDefault(x => x.State == TaskState.Idle);
            }
        }

        private IList<ServerAgent> GetAgents()
        {
            lock (serverContext)
            {
                return serverContext.Config.Agents
                    .Where(x => x.State == AgentState.Idle)
                    .ToArray();
            }
        }

        private void PrepareToStart(ServerTask task, IEnumerable<ServerAgent> agents)
        {
            cache.Clear();
            lock (serverContext)
            {
                task.State = TaskState.InProgress;
                foreach (var agent in agents)
                {
                    agent.State = AgentState.InProgress;
                }
            }
        }

        private void PrepareToEnd(ServerTask task, IEnumerable<ServerAgent> agents)
        {
            lock (serverContext)
            {
                task.State = TaskState.Done;
                foreach (var agent in agents)
                {
                    agent.State = AgentState.Idle;
                }
            }
        }

    }
}
