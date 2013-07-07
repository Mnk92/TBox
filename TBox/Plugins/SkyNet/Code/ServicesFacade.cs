using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Mnk.Library.Common.Communications;
using Mnk.TBox.Plugins.SkyNet.Code.Interfaces;
using Mnk.TBox.Tools.SkyNet.Common;

namespace Mnk.TBox.Plugins.SkyNet.Code
{
    class ServicesFacade : IServicesFacade
    {
        private readonly IConfigsFacade configsFacade;

        public ServicesFacade(IConfigsFacade configsFacade)
        {
            this.configsFacade = configsFacade;
        }

        public IList<ServerTask> GetServiceTasks()
        {
            using (var serverClient = CreateServerTasksClient())
            {
                return serverClient.Instance.GetTasks();
            }
        }

        public IList<ServerAgent> GetServiceAgents()
        {
            using (var serverClient = CreateServerAgentsClient())
            {
                return serverClient.Instance.GetAgents();
            }
        }

        public AgentTask GetAgentCurrentTask()
        {
            using (var agentClient = CreateAgentServerClient())
            {
                return agentClient.Instance.GetCurrentTask();
            }
        }

        public SkyNetStatus GetStatus()
        {
            var status = new SkyNetStatus();
            Parallel.Invoke(
                () => status.Task = GetAgentCurrentTask(),
                () => status.Agents = GetServiceAgents(),
                () => status.Tasks = GetServiceTasks()
                );
            return status;
        }


        public string UploadFile(string path)
        {
            using (var cl = CreateFileServerClient())
            {
                using (var f = File.OpenRead(path))
                {
                    return cl.Instance.Upload(f);
                }
            }
        }

        public string StartTask(ServerTask task)
        {
            using (var cl = CreateServerTasksClient())
            {
                return cl.Instance.AddTask(task);
            }
        }

        public void Cancel(string id)
        {
            using (var cl = CreateServerTasksClient())
            {
                cl.Instance.CancelTask(id);
            }
        }

        public void Terminate(string id)
        {
            using (var cl = CreateServerTasksClient())
            {
                cl.Instance.TerminateTask(id);
            }
        }

        public string DeleteTask(string id)
        {
            using (var cl = CreateServerTasksClient())
            {
                return cl.Instance.DeleteTask(id);
            }
        }

        public ServerTask GetTask(string id)
        {
            using (var cl = CreateServerTasksClient())
            {
                return cl.Instance.GetTask(id);
            }
        }

        public void DeleteFile(string zipPackageId)
        {
            using (var cl = CreateFileServerClient())
            {
                cl.Instance.Delete(zipPackageId);
            }
        }

        private NetworkClient<ISkyNetServerAgentsService> CreateServerAgentsClient()
        {
            return new NetworkClient<ISkyNetServerAgentsService>(ServerUri);
        }

        private NetworkClient<ISkyNetServerTasksService> CreateServerTasksClient()
        {
            return new NetworkClient<ISkyNetServerTasksService>(ServerUri);
        }

        private NetworkClient<ISkyNetFileService> CreateFileServerClient()
        {
            return new NetworkClient<ISkyNetFileService>(ServerUri);
        }

        private NetworkClient<ISkyNetAgentService> CreateAgentServerClient()
        {
            return new NetworkClient<ISkyNetAgentService>(Environment.MachineName, configsFacade.AgentConfig.Port);
        }

        private Uri ServerUri
        {
            get { return new Uri(configsFacade.AgentConfig.ServerEndpoint); }
        }

    }
}
