using System;
using System.Drawing;

namespace Mnk.TBox.Core.Contracts
{
    public interface IPluginContext
    {
        IDataProvider DataProvider { get; }
        IPathResolver PathResolver { get; }
        void RebuildMenu();
        void SaveConfig();
        Icon GetIcon(string path, int id);
        Icon GetSystemIcon(int id);
        void DoSync(Action action);
        void AddTypeToWarmingUp(Type type);
    }
}
