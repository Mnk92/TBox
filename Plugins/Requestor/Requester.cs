using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Mnk.Library.Common.Tools;
using Mnk.Library.WpfControls;
using Mnk.Library.WpfControls.Dialogs.StateSaver;
using Mnk.Library.WpfControls.Icons;
using Mnk.Library.WpfControls.Menu;
using Mnk.Library.WpfSyntaxHighlighter;
using Mnk.TBox.Core.Contracts;
using Mnk.TBox.Core.PluginsShared.LoadTesting.Components;
using Mnk.TBox.Locales.Localization.Plugins.Requestor;
using Mnk.TBox.Plugins.Requester.Code;
using Mnk.TBox.Plugins.Requester.Code.Settings;
using Mnk.TBox.Plugins.Requester.Components;

namespace Mnk.TBox.Plugins.Requester
{
    [PluginInfo(typeof(RequestorLang), 13, PluginGroup.Web)]
    public sealed class Requester : ConfigurablePlugin<Settings, Config>
    {
        private readonly LazyDialog<FormLoadTesting> formDdos;
        private readonly Lazy<Executor> executor;

        public Requester()
        {
            Dialogs.Add(formDdos = new LazyDialog<FormLoadTesting>(CreateForm));
            executor = new Lazy<Executor>(() => new Executor());
        }

        private FormLoadTesting CreateForm()
        {
            var dialog = new FormLoadTesting();
            dialog.Init(Icon.ToImageSource(), Icon, new LoadTester());
            return dialog;
        }

        public override void OnRebuildMenu()
        {
            base.OnRebuildMenu();
            Menu = FillMenu(Config.Profiles).ToArray();
        }

        private UMenuItem[] FillOperationsMenu(Profile p)
        {
            return p.Ops
                .Select(x => new UMenuItem
                {
                    Header = x.Key,
                    OnClick = o => executor.Value.Execute(Application.Current.MainWindow, x, Config, null, Icon.ToImageSource())
                })
                .Concat(
                    new[]
                    {
                        new USeparator(),
                        new UMenuItem{
                            IsEnabled = p.Ops.Count>0,
                            Header = RequestorLang.Ddos,
                            OnClick = o=>RunDdos(p)
                        },
                    }
                ).ToArray();
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

        private void RunDdos(Profile profile)
        {
            formDdos.Do(Context.DoSync, d => d.ShowDialog(profile), Config.States);
        }

        public override void Dispose()
        {
            base.Dispose();
            if (executor.IsValueCreated) executor.Value.Dispose();
        }

        public override void Init(IPluginContext context)
        {
            base.Init(context);
            context.AddTypeToWarmingUp(typeof(SyntaxHighlighter));
        }

        protected override Settings CreateSettings()
        {
            var s = base.CreateSettings();
            s.Requestor = new Lazy<FormRequest>(() => FillHelpInfo(new FormRequest(Icon.ToImageSource())));
            return s;
        }

        private FormRequest FillHelpInfo(FormRequest requestor)
        {
            requestor.KnownHeaderValues.Clear();
            requestor.KnownHeaderValues.Clear();
            requestor.KnownUrls.Clear();
            foreach (var item in Config.Profiles.SelectMany(p => p.Ops))
            {
                requestor.KnownUrls.AddIfNotExist(item.Request.Url);
                foreach (var h in item.Request.Headers)
                {
                    requestor.KnownHeaderNames.AddIfNotExist(h.Key);
                    requestor.KnownHeaderValues.AddIfNotExist(h.Value);
                }
            }
            return requestor;
        }

        public override void Save(bool autoSaveOnExit)
        {
            base.Save(autoSaveOnExit);
            if (!autoSaveOnExit) return;
            if (executor.IsValueCreated) executor.Value.Save(Config);
        }

    }
}
