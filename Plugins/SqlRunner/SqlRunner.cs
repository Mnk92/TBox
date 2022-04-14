using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Mnk.Library.WpfControls;
using Mnk.Library.WpfControls.Dialogs.StateSaver;
using Mnk.Library.WpfControls.Icons;
using Mnk.Library.WpfControls.Menu;
using Mnk.Library.WpfSyntaxHighlighter;
using Mnk.TBox.Core.Contracts;
using Mnk.TBox.Core.PluginsShared.LoadTesting.Components;
using Mnk.TBox.Locales.Localization.Plugins.SqlRunner;
using Mnk.TBox.Plugins.SqlRunner.Code;
using Mnk.TBox.Plugins.SqlRunner.Code.Settings;
using Mnk.TBox.Plugins.SqlRunner.Components;

namespace Mnk.TBox.Plugins.SqlRunner
{
    [PluginInfo(typeof(SqlRunnerLang), 8, PluginGroup.Database)]
    public sealed class SqlRunner : ConfigurablePlugin<Settings, Config>
    {
        private LoadTester loadTester;
        private readonly LazyDialog<FormLoadTesting> formDdos;
        private readonly LazyDialog<FormBatch> formBatch;
        private readonly Lazy<Executor> executor;

        public SqlRunner()
        {
            Dialogs.Add(formDdos = new LazyDialog<FormLoadTesting>(CreateDdosForm));
            Dialogs.Add(formBatch = new LazyDialog<FormBatch>(CreateBatchForm));
            executor = new Lazy<Executor>(() => new Executor());
        }

        private FormLoadTesting CreateDdosForm()
        {
            var dialog = new FormLoadTesting();
            dialog.Init(Icon.ToImageSource(), Icon, loadTester = new LoadTester());
            loadTester.OnConfigUpdated(Config);
            return dialog;
        }

        private FormBatch CreateBatchForm()
        {
            return new FormBatch { Icon = Icon.ToImageSource() };
        }

        public override void OnRebuildMenu()
        {
            base.OnRebuildMenu();
            if (formDdos.IsValueCreated)
            {
                loadTester.OnConfigUpdated(Config);
            }
            Menu = FillMenu(Config.Profiles).ToArray();
        }

        private UMenuItem[] FillOperationsMenu(Profile p)
        {
            return p.Ops
                .Select(o => new UMenuItem
                {
                    Header = o.Key,
                    OnClick = x => executor.Value.Execute(Application.Current.MainWindow, o, Config.ConnectionString, Config, () => CloseIfNeed(x), Icon.ToImageSource())
                })
                .Concat(
                    new[]
                    {
                        new USeparator(),
                        new UMenuItem{
                            IsEnabled = p.Ops.Count>0,
                            Header = SqlRunnerLang.Ddos,
                            OnClick = x=>RunDdos(p)
                        },
                        new UMenuItem{
                            IsEnabled = p.Ops.Count>0,
                            Header = SqlRunnerLang.Batch,
                            OnClick = x=>RunBatch(p)
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


        private void CloseIfNeed(object o)
        {
            if (o is NonUserRunContext)
            {
                executor.Value.Close();
            }
        }

        private void RunBatch(Profile profile)
        {
            formBatch.Do(Context.DoSync, d => d.ShowDialog(profile, Config), Config.States);
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

        public override void Save(bool autoSaveOnExit)
        {
            base.Save(autoSaveOnExit);
            if (!autoSaveOnExit) return;
            if (formBatch.IsValueCreated)
            {
                formBatch.Value.Save(Config);
            }
            if (executor.IsValueCreated)
            {
                executor.Value.Save(Config);
            }
        }

        protected override Settings CreateSettings()
        {
            var s = base.CreateSettings();
            s.Requestor = new Lazy<FormRequest>(() => new FormRequest());
            return s;
        }
    }
}

