using System;
using Mnk.TBox.Core.Contracts;
using Mnk.TBox.Locales.Localization.Plugins.AvailabilityChecker;
using Mnk.Library.WpfWinForms;
using Mnk.TBox.Plugins.AvailabilityChecker.Code;

namespace Mnk.TBox.Plugins.AvailabilityChecker
{
    [PluginInfo(typeof(AvailabilityCheckerLang), 10, PluginGroup.Web)]
    public sealed class AvailabilityChecker : ConfigurablePlugin<Settings, Config>, IDisposable
    {
        private readonly Lazy<Worker> worker;

        public AvailabilityChecker()
        {
            worker = new Lazy<Worker>(() =>
            {
                var w = new Worker();
                w.Load(Config);
                return w;
            });
        }

        public override void OnRebuildMenu()
        {
            base.OnRebuildMenu();
            var enabled = Config.Items.CheckedValuesCount > 0 || Config.Started;
            Menu = new[] {
                    new UMenuItem {
                            IsEnabled = enabled,
                            Header = Config.Started ? AvailabilityCheckerLang.Stop : AvailabilityCheckerLang.Start,
                            OnClick = o =>
                                {
                                    worker.Value.OnStartStop();
                                    Context.RebuildMenu();
                                }
                        }
                };
            if (worker.IsValueCreated)
            {
                worker.Value.OnUpdateMenu(Menu);
            }
        }

        public override void Load()
        {
            base.Load();
            if (!Config.Started && !worker.IsValueCreated) return;
            worker.Value.Load(Config);
        }

        public override void Save(bool autoSaveOnExit)
        {
            base.Save(autoSaveOnExit);
            if (!Config.Started && !worker.IsValueCreated) return;
            worker.Value.Save(Config, autoSaveOnExit);
        }

        public override void Dispose()
        {
            base.Dispose();
            if (worker.IsValueCreated)
            {
                worker.Value.Dispose();
            }
        }
    }
}
