using System;
using System.Windows.Controls;

namespace Mnk.TBox.Core.Contracts
{
    public class SimpleConfigurablePlugin<TConfig> : SimplePlugin, IConfigurablePlugin
        where TConfig : new()
    {
        protected TConfig Config => ConfigManager.Config;
        protected ConfigManager<TConfig> ConfigManager { get; set; }

        protected SimpleConfigurablePlugin()
        {
            ConfigManager = new ConfigManager<TConfig> { Config = new TConfig() };
        }

        public Type ConfigType => typeof(TConfig);

        public object ConfigObject
        {
            get => ConfigManager.Config;
            set => ConfigManager.Config = (TConfig)value;
        }

        protected virtual void OnConfigUpdated()
        {
            OnRebuildMenu();
        }

        public virtual void Load()
        {
            OnConfigUpdated();
        }

        public virtual void Save(bool autoSaveOnExit)
        {
            if (autoSaveOnExit) return;
            OnConfigUpdated();
        }

        public virtual void OnRebuildMenu()
        {
        }

        public virtual Func<Control> SettingsGetter => null;
    }
}
