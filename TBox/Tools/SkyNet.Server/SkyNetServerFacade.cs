using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.ServiceModel;
using Mnk.Library.Common.Communications;
using Mnk.Library.Common.Log;
using Mnk.TBox.Tools.SkyNet.Common;
using Mnk.TBox.Tools.SkyNet.Server.Code.Interfaces;

namespace Mnk.TBox.Tools.SkyNet.Server
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, IncludeExceptionDetailInFaults = true)]
    public class SkyNetServerFacade : ISkyNetCommon
    {
        private readonly ISkyNetServerAgentsLogic serverAgentsLogic;
        private readonly ISkyNetServerTasksLogic serverTasksLogic;
        private readonly ISkyNetFileServiceLogic fileServiceLogic;
        private readonly IHttpContextHelper contextHelper;
        private readonly IServerContext serverContext;
        private readonly ILog log = LogManager.GetLogger<SkyNetServerFacade>();


        public SkyNetServerFacade(ISkyNetServerAgentsLogic serverAgentsLogic, ISkyNetServerTasksLogic serverTasksLogic, ISkyNetFileServiceLogic fileServiceLogic, IHttpContextHelper contextHelper, IServerContext serverContext)
        {
            this.serverAgentsLogic = serverAgentsLogic;
            this.serverTasksLogic = serverTasksLogic;
            this.fileServiceLogic = fileServiceLogic;
            this.contextHelper = contextHelper;
            this.serverContext = serverContext;
        }

        public IList<ServerAgent> GetAgents()
        {
            IList<ServerAgent> result = null;
            Execute(() => result = serverAgentsLogic.GetAgents());
            return result ?? new List<ServerAgent>();
        }

        public void ConnectAgent(ServerAgent agent)
        {
            Execute(() => serverAgentsLogic.ConnectAgent(agent));
        }

        public void DisconnectAgent(string endpoint)
        {
            Execute(() => serverAgentsLogic.DisconnectAgent(endpoint));
        }

        public string AddTask(ServerTask task)
        {
            var result = string.Empty;
            Execute(() => result = serverTasksLogic.AddTask(task));
            return result;
        }

        public IList<ServerTask> GetTasks()
        {
            IList<ServerTask> result = null;
            Execute(() => result = serverTasksLogic.GetTasks());
            return result??new List<ServerTask>();
        }

        public void CancelTask(string id)
        {
            Execute(() => serverTasksLogic.CancelTask(id));
        }

        public void TerminateTask(string id)
        {
            Execute(() => serverTasksLogic.TerminateTask(id));
        }

        public ServerTask GetTask(string id)
        {
            ServerTask result = null;
            Execute(() => result = serverTasksLogic.GetTask(id));
            return result;
        }

        public string DeleteTask(string id)
        {
            var result = string.Empty;
            Execute(() => result = serverTasksLogic.DeleteTask(id));
            return result;
        }

        public void PingIsAlive()
        {
            serverAgentsLogic.PingIsAlive();
        }

        public string Upload(Stream stream)
        {
            var result = string.Empty;
            ExecuteNoLock(() => result = fileServiceLogic.Upload(stream));
            return result;
        }

        public Stream Download(string id)
        {
            Stream s = null;
            ExecuteNoLock(() => s = fileServiceLogic.Download(id));
            return s ?? new MemoryStream();
        }

        public void Delete(string id)
        {
            ExecuteNoLock(() => fileServiceLogic.Delete(id));
        }

        private void Execute(Action op)
        {
            lock (serverContext)
            {
                ExecuteNoLock(op);
            }
        }

        private void ExecuteNoLock(Action op)
        {
            try
            {
                op();
            }
            catch (Exception ex)
            {
                ProcessError(ex);
            }
        }

        private void ProcessError(Exception ex)
        {
            log.Write(ex, "Unexpected exception");
            contextHelper.SetStatusCode(ex is FileNotFoundException ?
                HttpStatusCode.NotFound : HttpStatusCode.InternalServerError);
        }

    }
}
