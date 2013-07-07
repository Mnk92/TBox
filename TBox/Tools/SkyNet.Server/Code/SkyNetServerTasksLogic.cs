using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Mnk.Library.Common.Communications;
using Mnk.Library.Common.Tools;
using Mnk.TBox.Tools.SkyNet.Common;
using Mnk.TBox.Tools.SkyNet.Server.Code.Interfaces;

namespace Mnk.TBox.Tools.SkyNet.Server.Code
{
    class SkyNetServerTasksLogic : ISkyNetServerTasksLogic
    {
        private readonly IHttpContextHelper contextHelper;
        private readonly IServerContext serverContext;
        private readonly ISkyAgentLogic skyAgentLogic;

        public SkyNetServerTasksLogic(IHttpContextHelper contextHelper, IServerContext serverContext, ISkyAgentLogic skyAgentLogic)
        {
            this.contextHelper = contextHelper;
            this.serverContext = serverContext;
            this.skyAgentLogic = skyAgentLogic;
        }

        public string AddTask(ServerTask task)
        {
            task.Id = Guid.NewGuid().ToString();
            if (task.ScriptParameters==null || string.IsNullOrEmpty(task.Script))
            {
                contextHelper.SetStatusCode(HttpStatusCode.ExpectationFailed);
                return string.Empty;
            }
            serverContext.Config.Tasks.Add(new ServerTask
            {
                Id = task.Id,
                State = TaskState.Idle,
                ScriptParameters = task.ScriptParameters,
                Script = task.Script,
                Owner = task.Owner,
                ZipPackageId = task.ZipPackageId,
                CreatedTime = DateTime.UtcNow,
                IsDone = false,
                Progress = 0,
                Report = string.Empty
            });
            return task.Id;
        }

        public IList<ServerTask> GetTasks()
        {
            return serverContext.Config.Tasks
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
        }

        public void CancelTask(string id)
        {
            Parallel.ForEach(serverContext.Config.Agents, 
                agent => skyAgentLogic.CancelTask(agent, id));
        }

        public void TerminateTask(string id)
        {
            Parallel.ForEach(serverContext.Config.Agents,
                agent => skyAgentLogic.TerminateTask(agent, id));
        }

        public ServerTask GetTask(string id)
        {
            var task = serverContext.Config.Tasks.FirstOrDefault(x => x.Id.EqualsIgnoreCase(id));
            if (task == null)
            {
                contextHelper.SetStatusCode(HttpStatusCode.NotFound);
                return null;
            }
            return task;
        }

        public string DeleteTask(string id)
        {
            var task = GetTask(id);
            if (task == null) return string.Empty;
            if (task.State == TaskState.InProgress)
            {
                contextHelper.SetStatusCode(HttpStatusCode.Conflict);
                return string.Empty;
            }
            serverContext.Config.Tasks.Remove(task);
            return task.Report;
        }
    }
}
