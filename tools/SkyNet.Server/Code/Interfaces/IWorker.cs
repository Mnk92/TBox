using Mnk.TBox.Tools.SkyNet.Contracts;

namespace Mnk.TBox.Tools.SkyNet.Server.Code.Interfaces
{
    public interface IWorker
    {
        Task ProcessTask(ServerTask task, IList<ServerAgent> agents);
    }
}
