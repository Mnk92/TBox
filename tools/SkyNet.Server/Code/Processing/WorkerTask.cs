using Mnk.TBox.Tools.SkyNet.Contracts;

namespace Mnk.TBox.Tools.SkyNet.Server.Code.Processing
{
    public class WorkerTask
    {
        public string Config => Task.Config;
        public AgentTask Task { get; init; }
        public ServerAgent Agent { get; init; }
        public bool IsFailed => Exception != null;
        public Exception Exception { get; set; }
    }
}
