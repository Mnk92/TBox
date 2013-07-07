using Mnk.TBox.Tools.SkyNet.Common;
using Mnk.TBox.Tools.SkyNet.Server.Code.Processing;

namespace Mnk.TBox.Tools.SkyNet.Server.Code.Interfaces
{
    public interface ISkyAgentLogic
    {
        WorkerTask CreateWorkerTask(ServerAgent agent, string agentData, ServerTask task);
        AgentTask GetTask(WorkerTask task);
        SkyAgentWork BuildReport(WorkerTask task);
        bool IsAlive(ServerAgent arg);
        AgentTask GetCurrentTask(ServerAgent agent);
        void DeleteTask(ServerAgent agent, string id);
        void TerminateTask(ServerAgent agent, string id);
        void CancelTask(ServerAgent agent, string id);
    }
}
