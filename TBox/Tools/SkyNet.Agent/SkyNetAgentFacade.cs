using System;
using System.Net;
using System.ServiceModel;
using Mnk.Library.Common.Communications;
using Mnk.Library.Common.Log;
using Mnk.TBox.Tools.SkyNet.Agent.Code;
using Mnk.TBox.Tools.SkyNet.Common;

namespace Mnk.TBox.Tools.SkyNet.Agent
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, IncludeExceptionDetailInFaults = false)]
    class SkyNetAgentFacade : ISkyNetAgentService
    {
        private readonly ISkyNetAgentLogic agentLogic;
        private readonly IHttpContextHelper contextHelper;
        private readonly ILog log = LogManager.GetLogger<SkyNetAgentFacade>();
        private readonly object locker = new object();


        public SkyNetAgentFacade(ISkyNetAgentLogic agentLogic, IHttpContextHelper contextHelper)
        {
            this.agentLogic = agentLogic;
            this.contextHelper = contextHelper;
        }

        public string AddTask(AgentTask task)
        {
            var result = string.Empty;
            Execute(() => result = agentLogic.AddTask(task));
            return result;
        }

        public AgentTask GetTask(string id)
        {
            AgentTask task = null;
            Execute(() => task = agentLogic.GetTask(id));
            return task;
        }

        public AgentTask GetCurrentTask()
        {
            AgentTask task = null;
            Execute(() => task = agentLogic.GetCurrentTask());
            return task;
        }

        public void CancelTask(string id)
        {
            Execute(() => agentLogic.CancelTask(id));
        }

        public void TerminateTask(string id)
        {
            Execute(() => agentLogic.TerminateTask(id));
        }

        public string DeleteTask(string id)
        {
            var result = string.Empty;
            Execute(() => result = agentLogic.DeleteTask(id));
            return result;
        }

        public void PingIsAlive()
        {
            agentLogic.PingIsAlive();
        }

        private void Execute(Action op)
        {
            try
            {
                lock (locker)
                {
                    op();
                }
            }
            catch (Exception ex)
            {
                ProcessError(ex);
            }
        }

        private void ProcessError(Exception ex)
        {
            log.Write(ex, "Unexpected exception");
            contextHelper.SetStatusCode(HttpStatusCode.InternalServerError);
        }

    }
}
