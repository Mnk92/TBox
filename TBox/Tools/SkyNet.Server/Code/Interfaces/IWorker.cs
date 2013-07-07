using System.Collections.Generic;
using Mnk.TBox.Tools.SkyNet.Common;

namespace Mnk.TBox.Tools.SkyNet.Server.Code.Interfaces
{
    public interface IWorker
    {
        void ProcessTask(ServerTask task, IList<ServerAgent> agents);
    }
}
