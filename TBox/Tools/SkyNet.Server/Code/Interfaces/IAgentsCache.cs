using System;
using Mnk.TBox.Tools.SkyNet.Common;

namespace Mnk.TBox.Tools.SkyNet.Server.Code.Interfaces
{
    public interface IAgentsCache: IDisposable
    {
        ISkyNetAgentService Get(ServerAgent agent);
        void Clear();
    }
}
