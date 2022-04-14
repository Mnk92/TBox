using System;
using System.Drawing;
using System.Windows;
using Mnk.TBox.Core.Contracts;
using Mnk.TBox.Locales.Localization.Plugins.RequestsWatcher;
using Mnk.TBox.Core.PluginsShared.Watcher;
using Mnk.TBox.Plugins.RequestsWatcher.Code;
using Mnk.TBox.Plugins.RequestsWatcher.Code.Parser;
using Mnk.TBox.Plugins.RequestsWatcher.Components;
using Mnk.Library.WpfControls.Menu;
using Mnk.Library.WpfControls.Icons;

namespace Mnk.TBox.Plugins.RequestsWatcher
{
    [PluginInfo(typeof(RequestsWatcherLang), 14, PluginGroup.Web)]
    public sealed class RequestsWatcher : ConfigurablePlugin<Settings, Config>
    {
        private Lazy<Worker<LogDialog>> worker;
        private readonly DataParser parser = new DataParser();
        private static readonly string[] StartStop = { RequestsWatcherLang.Start, RequestsWatcherLang.Stop };
        public RequestsWatcher()
        {
            Menu = new[]{
                new UMenuItem{Header = StartStop[0], OnClick = o=>OnStartStop()},
                new UMenuItem{Header = RequestsWatcherLang.ShowLogs, OnClick = o=>OnShowLogs()},
                new UMenuItem{Header = RequestsWatcherLang.FillFromClipboard, OnClick = o=>OnFillFromClipboard()},
                new UMenuItem{Header = RequestsWatcherLang.Clear, OnClick = o=>OnClear()}
            };
        }

        public override void Init(IPluginContext context)
        {
            base.Init(context);
            worker = new Lazy<Worker<LogDialog>>(
                () => new Worker<LogDialog>(new LogDialog(Icon.ToImageSource()), parser, RequestsWatcherLang.PluginName, Config, Context.PathResolver, Color.Green));
        }

        private void OnFillFromClipboard()
        {
            InitWorker();
            worker.Value.ClearLogs();
            foreach (var line in Clipboard.GetText().Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
            {
                string result;
                if (parser.TryParse(RequestsWatcherLang.Clipboard, line, out result))
                {
                    worker.Value.Write(RequestsWatcherLang.Clipboard, result);
                }

            }
            worker.Value.ShowLogsList();

        }

        private void OnClear()
        {
            worker.Value.ClearLogs();
        }

        private void OnShowLogs()
        {
            worker.Value.ShowLogsList();
        }

        private void OnStartStop()
        {
            if (Config.Started)
            {
                Menu[0].Header = StartStop[0];
                if (worker.IsValueCreated)
                {
                    worker.Value.Stop();
                }
            }
            else
            {
                Menu[0].Header = StartStop[1];
                InitWorker();
                worker.Value.Start(
                    Config.RescanLogsInterval,
                    Config.ToolTipsTimeOut,
                    Config.Watches.Files.CheckedItems,
                    Config.ToolTipsEnabled);
                worker.Value.SetMenuItems(Menu);
            }
            Config.Started = !Config.Started;
            Context.RebuildMenu();
        }

        private void InitWorker()
        {
            worker.Value.Log.Title = RequestsWatcherLang.PluginName;
            worker.Value.Log.MaxEntriesInLog = Config.MaxEntriesInLog;
            parser.Reset();
        }

        public override void Load()
        {
            base.Load();
            Config.Started = !Config.Started;
            OnStartStop();
        }

        public override void Save(bool autoSaveOnExit)
        {
            base.Save(autoSaveOnExit);
            if (autoSaveOnExit)
            {
                if (worker.IsValueCreated) worker.Value.Save(Config);
                return;
            }
            Config.Started = !Config.Started;
            OnStartStop();
        }

        public override void Dispose()
        {
            base.Dispose();
            if (!worker.IsValueCreated) return;
            worker.Value.Log.Dispose();
            worker.Value.Dispose();
        }
    }
}
