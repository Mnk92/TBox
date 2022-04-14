using Mnk.TBox.Tools.SkyNet.Contracts;

namespace Mnk.TBox.Tools.SkyNet.Server.Code.Interfaces
{
    public interface IAgentsCache : IDisposable
    {
        ISkyNetAgentService.ISkyNetAgentServiceClient Get(ServerAgent agent);
        void Clear();
    }
}
