using System;
using System.Collections.Generic;
using System.Windows.Controls;
using Mnk.Library.WpfControls;
using Mnk.Library.WpfControls.Dialogs;
using Mnk.Library.WpfControls.Dialogs.StateSaver;

namespace Mnk.TBox.Core.Contracts
{
    public abstract class ConfigurablePlugin<TSettings, TConfig> : SimpleConfigurablePlugin<TConfig>, IConfigurablePlugin, IDisposable
        where TSettings : ISettings, new()
        where TConfig : new()
    {
        protected readonly List<ILazyDialog<DialogWindow>> Dialogs = new List<ILazyDialog<DialogWindow>>();
        private Lazy<TSettings> Settings { get; }

        protected ConfigurablePlugin()
        {
            Settings = new Lazy<TSettings>(CreateSettings);
        }

        protected virtual TSettings CreateSettings()
        {
            var s = CreateSettingsInstance();
            s.Control.DataContext = ConfigManager.Config;
            return s;
        }

        protected virtual TSettings CreateSettingsInstance()
        {
            return new TSettings();
        }

        public override void Load()
        {
            foreach (var dialog in Dialogs)
            {
                dialog.Hide();
            }
            if (Settings.IsValueCreated)
                Settings.Value.Control.DataContext = ConfigManager.Config;
            base.Load();
        }

        public override Func<Control> SettingsGetter
        {
            get
            {
                return () => Settings.Value.Control;
            }
        }

        public override void Save(bool autoSaveOnExit)
        {
            base.Save(autoSaveOnExit);
            if (autoSaveOnExit)
            {
                var statableConfig = ConfigManager.Config as IConfigWithDialogStates;
                if (statableConfig == null) return;
                foreach (var dialog in Dialogs)
                {
                    dialog.SaveState(statableConfig.States);
                }
            }
        }

        public virtual void Dispose()
        {
            foreach (var dialog in Dialogs)
            {
                dialog.Dispose();
            }
        }
    }
}
