using System;
using System.Collections.Generic;
using System.Linq;
using Mnk.Library.Common.MT;
using Mnk.TBox.Core.Contracts;
using Mnk.TBox.Locales.Localization.Plugins.ServicesCommander;
using Mnk.TBox.Plugins.ServicesCommander.Code;
using Mnk.Library.WpfControls.Dialogs;
using Mnk.Library.WpfWinForms;
using Mnk.Library.WpfWinForms.Icons;

namespace Mnk.TBox.Plugins.ServicesCommander
{
    [PluginInfo(typeof(ServicesCommanderLang), 71, PluginGroup.Desktop)]
    public class ServicesCommander : ConfigurablePlugin<Settings, Config>
    {
        private Runner runner;
        public override void OnRebuildMenu()
        {
            base.OnRebuildMenu();
            Menu = FillMenu(
                    Config.Profiles.Where(x => x.Services.CheckedItems.Any()).ToArray()
                )
                .Concat(new[]{new UMenuItem
					{
						Header = ServicesCommanderLang.Refresh,
						OnClick = o=>Context.RebuildMenu()
					} })
                .ToArray();
        }

        private UMenuItem[] FillOperationsMenu(Profile p)
        {
            return p.Services.CheckedItems
                .SelectMany(s => new[]{
                    new UMenuItem
                    {
                        Header = s.Key + " - " + (runner.IsRunning(s) ? ServicesCommanderLang.Stop : ServicesCommanderLang.Start),
                        OnClick = o =>DoWithProgress(s.Key + " - " + (runner.IsRunning(s) ? ServicesCommanderLang.Stop : ServicesCommanderLang.Start), u=>runner.ToggleService(s)) 
                    },
                    new UMenuItem
                    {
                        Header = s.Key + " - " + ServicesCommanderLang.Restart,
                        OnClick = o =>DoWithProgress(s.Key + " - " + ServicesCommanderLang.Restart, u=>runner.RestartService(s))
                    }
                })
                .Concat(new[]
                {
                    new USeparator(), 
                    new UMenuItem
                    {
                        Header = ServicesCommanderLang.StartAll,
                        OnClick = o=>StartAll(p)
                    },
                    new UMenuItem
                    {
                        Header = ServicesCommanderLang.StopAll,
                        OnClick = o=>StopAll(p)
                    }
                })
                .ToArray();
        }

        private IEnumerable<UMenuItem> FillMenu(IList<Profile> ops)
        {
            if (ops.Count == 1)
            {
                var p = ops.First();
                return new[]
                {
                    new UMenuItem{Header = p.Key, IsEnabled = false}, 
                }
                .Concat(FillOperationsMenu(p));
            }

            return ops.Select(p => new UMenuItem
            {
                Header = p.Key,
                Items = FillOperationsMenu(p)
            });
        }

        private void StopAll(Profile p)
        {
            DoWithProgress(ServicesCommanderLang.StopAll, u => runner.StopAll(p, u));
        }

        private void StartAll(Profile p)
        {
            DoWithProgress(ServicesCommanderLang.StartAll, u => runner.StartAll(p, u));
        }

        private void DoWithProgress(string title, Action<IUpdater> op)
        {
            DialogsCache.ShowProgress(op, title, null, icon: Icon.ToImageSource(), showInTaskBar: true);
        }

        public override void Init(IPluginContext context)
        {
            base.Init(context);
            runner = new Runner(context);
        }
    }
}
