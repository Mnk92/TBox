using System;
using Mnk.Library.WpfControls;
using Mnk.Library.WpfControls.Dialogs;
using Mnk.Library.WpfControls.Dialogs.StateSaver;
using Mnk.Library.WpfWinForms;
using Mnk.Library.WpfWinForms.Icons;

namespace Mnk.TBox.Core.Contracts
{
    public abstract class SingleDialogConfigurablePlugin<TSettings, TConfig, TDialog> : ConfigurablePlugin<TSettings, TConfig>, IDisposable
        where TSettings : ISettings, new()
        where TConfig : IConfigWithDialogStates, new()
        where TDialog : DialogWindow, new()
    {
        protected readonly LazyDialog<TDialog> Dialog;
        protected SingleDialogConfigurablePlugin(string menuItemName)
        {
            Menu = new[] { new UMenuItem { Header = menuItemName, OnClick = o => ShowDialog() } };
            Dialog = new LazyDialog<TDialog>(CreateDialog);
        }

        protected virtual TDialog CreateDialog()
        {
            var dialog = new TDialog
            {
                DataContext = ConfigManager.Config,
                Icon = Icon.ToImageSource()
            };
            return dialog;
        }

        protected virtual void ShowDialog()
        {
            Dialog.Show(Context.DoSync, ConfigManager.Config.States);
        }

        protected override void OnConfigUpdated()
        {
            base.OnConfigUpdated();
            if (!Dialog.IsValueCreated) return;
            Dialog.Value.DataContext = ConfigManager.Config;
            Dialog.Hide();
        }

        public override void Save(bool autoSaveOnExit)
        {
            base.Save(autoSaveOnExit);
            if (autoSaveOnExit) Dialog.SaveState(ConfigManager.Config.States);
        }

        public override void Dispose()
        {
            Dialog.Dispose();
        }
    }
}
