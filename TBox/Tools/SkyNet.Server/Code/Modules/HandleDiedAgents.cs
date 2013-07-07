using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mnk.TBox.Tools.SkyNet.Common;
using Mnk.TBox.Tools.SkyNet.Common.Modules;
using Mnk.TBox.Tools.SkyNet.Server.Code.Interfaces;

namespace Mnk.TBox.Tools.SkyNet.Server.Code.Modules
{
    class HandleDiedAgents : IModule
    {
        private readonly IServerContext serverContext;
        private readonly ISkyAgentLogic agentLogic;

        public HandleDiedAgents(IServerContext serverContext, ISkyAgentLogic agentLogic)
        {
            this.serverContext = serverContext;
            this.agentLogic = agentLogic;
        }

        public void Process()
        {
            var agents = GetAgents();
            if (!agents.Any())return;
            RemoveDied(agents);
            if (!agents.Any()) return;
            ResetNonIdle(agents);
        }

        private void ResetNonIdle(IEnumerable<ServerAgent> agents)
        {
            var nonIdleAgents = agents.ToArray();
            Parallel.ForEach(nonIdleAgents, agent =>
            {
                var task = agentLogic.GetCurrentTask(agent);
                if (task != null)
                {
                    agentLogic.TerminateTask(agent, task.Id);
                    agentLogic.DeleteTask(agent, task.Id);
                }
                lock (serverContext)
                {
                    agent.State = AgentState.Idle;
                }
            });
        }

        private void RemoveDied(IList<ServerAgent> agents)
        {
            var toRemove = agents
                .AsParallel()
                .Where(a => !agentLogic.IsAlive(a))
                .ToArray();
            if (!toRemove.Any()) return;
            RemoveAgents(toRemove);
            foreach (var agent in toRemove)
            {
                agents.Remove(agent);
            }
        }

        private void RemoveAgents(IEnumerable<ServerAgent> toRemove)
        {
            lock (serverContext)
            {
                foreach (var agent in toRemove)
                {
                    serverContext.Config.Agents.Remove(agent);
                }
            }
        }

        private IList<ServerAgent> GetAgents()
        {
            lock (serverContext)
            {
                return serverContext.Config.Agents.ToArray();
            }
        }

    }
}
