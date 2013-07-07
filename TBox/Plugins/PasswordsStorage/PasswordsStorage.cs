using System;
using System.Linq;
using System.Windows;
using Mnk.Library.Common.Log;
using Mnk.Library.WpfControls;
using Mnk.Library.WpfControls.Dialogs.StateSaver;
using Mnk.Library.WpfWinForms;
using Mnk.Library.WpfWinForms.Icons;
using Mnk.TBox.Core.Contracts;
using Mnk.TBox.Locales.Localization.Plugins.PasswordsStorage;
using Mnk.TBox.Plugins.PasswordsStorage.Code;
using Mnk.TBox.Plugins.PasswordsStorage.Code.Settings;
using Mnk.TBox.Plugins.PasswordsStorage.Components;

namespace Mnk.TBox.Plugins.PasswordsStorage
{
    [PluginInfo(typeof(PasswordsStorageLang), 104, PluginGroup.Desktop)]
    public class PasswordsStorage : ConfigurablePlugin<Settings, Config>
    {
        private readonly ILog log = LogManager.GetLogger<PasswordsStorage>();
        private readonly LazyDialog<Dialog> dialog;
        private readonly IPasswordGenerator passwordGenerator;
        private readonly IPasswordsExporter passwordsExporter;

        public PasswordsStorage()
        {
            passwordGenerator = new PasswordGenerator();
            passwordsExporter = new PasswordsExporter();
            Dialogs.Add(dialog = new LazyDialog<Dialog>(CreateDialog));
        }

        private Dialog CreateDialog()
        {
            return new Dialog(passwordGenerator){Icon = Icon.ToImageSource()};
        }

        public override void OnRebuildMenu()
        {
            base.OnRebuildMenu();
            Menu = Config.Profiles
                .Select(p => new UMenuItem
                {
                    Header = p.Key,
                    OnClick = o=>ShowDialog(p,null)
                })
                .Concat(new []
                {
                    new USeparator(), 
                    new UMenuItem
                    {
                        Header = PasswordsStorageLang.NewPassword,
                        OnClick = o=>NewPassword()
                    },
                    new USeparator(),
                    new UMenuItem
                    {
                        Header = PasswordsStorageLang.Export,
                        OnClick = o=>DoExport()
                    },
                    new UMenuItem
                    {
                        Header = PasswordsStorageLang.Import,
                        OnClick = o=>DoImport()
                    },
                })
                .ToArray();
        }

        private void NewPassword()
        {
            SafeDo(()=> passwordGenerator.Generate(Config.PasswordLength, Config.PasswordNonAlphaCharacters));
        }

        private void DoImport()
        {
            SafeDo(() => passwordsExporter.Import(ConfigManager));
        }

        private void DoExport()
        {
            SafeDo(() => passwordsExporter.Export(ConfigManager));
            Context.SaveConfig();
            Context.RebuildMenu();
        }

        private void SafeDo(Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                log.Write(ex, "Unexpected issue");
            }
        }

        private void ShowDialog(Profile p, Window owner)
        {
            dialog.Do(Context.DoSync,
                d => d.ShowDialog(ConfigManager, p, Context, owner), 
                ConfigManager.Config.States);
        }

        protected override Settings CreateSettings()
        {
            var s = base.CreateSettings();
            s.EditHandler += p => ShowDialog(p, Application.Current.MainWindow);
            return s;
        }
    }
}
