using Mnk.TBox.Tools.SkyNet.Contracts;

namespace Mnk.TBox.Plugins.SkyNet.Code.Interfaces
{
    public interface IServicesFacade
    {
        Task<IList<ServerTask>> GetServiceTasks();
        Task<IList<ServerAgent>> GetServiceAgents();
        Task<AgentTask> GetAgentCurrentTask();
        Task<SkyNetStatus> GetStatus();
        Task<string> UploadFile(string path);
        Task<string> StartTask(ServerTask task);
        Task Cancel(string id);
        Task Terminate(string id);
        Task<string> DeleteTask(string id);
        Task<ServerTask> GetTask(string id);
        Task DeleteFile(string zipPackageId);
    }
}
