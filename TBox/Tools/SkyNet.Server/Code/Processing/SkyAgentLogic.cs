using System;
using Mnk.Library.Common.Log;
using Mnk.TBox.Tools.SkyNet.Common;
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

        public WorkerTask CreateWorkerTask(ServerAgent agent, string agentData, ServerTask task)
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
                agentTask.Id = cache.Get(agent).AddTask(agentTask);
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

        public AgentTask GetTask(WorkerTask workerTask)
        {
            if (workerTask.IsFailed) return null;
            AgentTask task = null;
            Execute(workerTask.Agent, s =>task = s.GetTask(workerTask.Task.Id));
            return task;
        }

        public SkyAgentWork BuildReport(WorkerTask task)
        {
            var report = string.Empty;
            if(!task.IsFailed)
            {
                try
                {
                    report = cache.Get(task.Agent).DeleteTask(task.Task.Id);
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

        public bool IsAlive(ServerAgent agent)
        {
            var result = false;
            Execute(agent, s => { 
                s.PingIsAlive();
                result = true;
            });
            return result;
        }

        public AgentTask GetCurrentTask(ServerAgent agent)
        {
            AgentTask task = null;
            Execute(agent, s => task = s.GetCurrentTask());
            return task;
        }

        public void DeleteTask(ServerAgent agent, string id)
        {
            Execute(agent, s=>s.DeleteTask(id));
        }

        public void TerminateTask(ServerAgent agent, string id)
        {
            Execute(agent, s => s.TerminateTask(id));
        }

        public void CancelTask(ServerAgent agent, string id)
        {
            Execute(agent, s => s.CancelTask(id));
        }

        private void Execute(ServerAgent agent, Action<ISkyNetAgentService> op)
        {
            try
            {
                op(cache.Get(agent));
            }
            catch (Exception ex)
            {
                log.Write(ex, "Unexpected error for agent: " + agent.Endpoint);
            }
        }
    }
}
