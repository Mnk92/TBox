using Mnk.TBox.Tools.SkyNet.Contracts;
using Mnk.TBox.Tools.SkyNet.Server.Code.Processing;

namespace Mnk.TBox.Tools.SkyNet.Server.Code.Interfaces
{
    public interface ISkyAgentLogic
    {
        Task<WorkerTask> CreateWorkerTask(ServerAgent agent, string agentData, ServerTask task);
        Task<AgentTask> GetTask(WorkerTask task);
        Task<SkyAgentWork> BuildReport(WorkerTask task);
        Task<bool> IsAlive(ServerAgent arg);
        Task<AgentTask> GetCurrentTask(ServerAgent agent);
        Task DeleteTask(ServerAgent agent, string id);
        Task TerminateTask(ServerAgent agent, string id);
        Task CancelTask(ServerAgent agent, string id);
    }
}
