using Mnk.Library.Common.MT;

namespace Mnk.TBox.Tools.SkyNet.Common
{
    public interface ISkyContext : IUpdater
    {
        void Reset(AgentTask task);
        void Cancel();
    }
}
