using System;
using Mnk.TBox.Tools.SkyNet.Common;

namespace Mnk.TBox.Tools.SkyNet.Server.Code.Processing
{
    public class WorkerTask
    {
        public string Config { get { return Task.Config; } }
        public AgentTask Task { get; set; }
        public ServerAgent Agent { get; set; }
        public bool IsFailed { get { return Exception != null; } }
        public Exception Exception { get; set; }
    }
}
