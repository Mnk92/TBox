using Mnk.Library.Common.MT;
using Mnk.TBox.Tools.SkyNet.Contracts;

namespace Mnk.TBox.Tools.SkyNet.Common
{
    public interface ISkyContext : IUpdater
    {
        void Reset(AgentTask task);
        void Cancel();
    }
}
