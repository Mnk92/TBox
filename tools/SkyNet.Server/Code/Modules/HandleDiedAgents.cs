using Mnk.TBox.Tools.SkyNet.Common.Modules;
using Mnk.TBox.Tools.SkyNet.Contracts;
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

        public async Task Process()
        {
            var agents = GetAgents();
            if (!agents.Any()) return;
            await RemoveDied(agents);
            if (!agents.Any()) return;
            await ResetNonIdle(agents);
        }

        private async Task ResetNonIdle(IEnumerable<ServerAgent> agents)
        {
            var nonIdleAgents = agents.ToArray();
            await Parallel.ForEachAsync(nonIdleAgents, async (agent, _) =>
            {
                var task = await agentLogic.GetCurrentTask(agent);
                if (task != null)
                {
                    await agentLogic.TerminateTask(agent, task.Id);
                    await agentLogic.DeleteTask(agent, task.Id);
                }
                lock (serverContext)
                {
                    agent.State = ServerAgent.Types.AgentState.Idle;
                }
            });
        }

        private async Task RemoveDied(IList<ServerAgent> agents)
        {
            var toRemove = (await Task.WhenAll(
                agents.Select(x => agentLogic.IsAlive(x).ContinueWith(t => new { IsAlive = t.Result, Value = x }))))
                .Where(x => !x.IsAlive)
                .Select(x => x.Value)
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
