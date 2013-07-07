using System;
using System.Drawing;
using Mnk.TBox.Core.Contracts;

namespace Mnk.TBox.Core.Application.Code.Objects
{
    class PluginContext : IPluginContext
    {
        private readonly Action onMenuChanged;
        private readonly PluginsContextShared pluginsContextShared;
        private readonly Action onSave;

        public IDataProvider DataProvider { get; private set; }
        public IPathResolver PathResolver { get; private set; }
        public PluginContext(IDataProvider dataProvider, IPathResolver pathResolver, Action onMenuChanged, PluginsContextShared pluginsContextShared, Action onSave)
        {
            DataProvider = dataProvider;
            PathResolver = pathResolver;
            this.onMenuChanged = onMenuChanged;
            this.pluginsContextShared = pluginsContextShared;
            this.onSave = onSave;
        }

        public void DoSync(Action action)
        {
            pluginsContextShared.DoSync(action);
        }

        public void AddTypeToWarmingUp(Type type)
        {
            pluginsContextShared.AddTypeToWarmingUp(type);
        }

        public void RebuildMenu()
        {
            onMenuChanged();
        }

        public void SaveConfig()
        {
            onSave();
        }

        public Icon GetIcon(string path, int id)
        {
            return pluginsContextShared.GetIcon(path, id);
        }

        public Icon GetSystemIcon(int id)
        {
            return pluginsContextShared.GetSystemIcon(id);
        }

    }
}
