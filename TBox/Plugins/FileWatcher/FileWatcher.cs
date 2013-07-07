using System;
using System.Windows;
using Mnk.TBox.Core.Contracts;
using Mnk.TBox.Locales.Localization.Plugins.FileWatcher;
using Mnk.TBox.Core.PluginsShared.Watcher;
using Mnk.Library.WpfWinForms;
using Mnk.Library.WpfWinForms.Icons;
using Mnk.TBox.Plugins.FileWatcher.Code;

namespace Mnk.TBox.Plugins.FileWatcher
{
    [PluginInfo(typeof(FileWatcherLang), 55, PluginGroup.Desktop)]
    public sealed class FileWatcher : ConfigurablePlugin<Settings, Config>, IDisposable
    {
        private LogDialog dialog;
        private Lazy<Worker<LogDialog>> worker;
        private static readonly string[] StartStop = { FileWatcherLang.Start, FileWatcherLang.Stop };
        public FileWatcher()
        {
            Menu = new[]{
                new UMenuItem{Header = StartStop[0], OnClick = o=>OnStartStop()},
                new UMenuItem{Header = FileWatcherLang.ShowLogs, OnClick = o=>OnShowLogs()},
                new UMenuItem{Header = FileWatcherLang.FillFromClipboard, OnClick = o=>OnFillFromClipboard()},
                new UMenuItem{Header = FileWatcherLang.Clear, OnClick = o=>OnClear()}
            };
        }

        private void OnFillFromClipboard()
        {
            worker.Value.ClearLogs();
            InitLogDialog();
            worker.Value.Write(FileWatcherLang.Clipboard, Clipboard.GetText());
            worker.Value.ShowLogsList();
        }

        public override void Init(IPluginContext context)
        {
            base.Init(context);
            worker = new Lazy<Worker<LogDialog>>(
                () => new Worker<LogDialog>(dialog = new LogDialog(Icon.ToImageSource()), new DataParser(), FileWatcherLang.PluginName, Config, Context.PathResolver));
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
                InitLogDialog();
                worker.Value.Start(
                    Config.RescanLogsInterval,
                    Config.ToolTipsTimeOut,
                    Config.Watches.Files.CheckedItems,
                    Config.ToolTipsEnabled
                    );
                worker.Value.SetMenuItems(Menu);
            }
            Config.Started = !Config.Started;
            Context.RebuildMenu();
        }

        private void InitLogDialog()
        {
            worker.Value.Log.Dialog.Title = FileWatcherLang.PluginName;
            worker.Value.Log.Dialog.MaxEntries = Config.MaxEntriesInLog;
            dialog.Init(Config.EntryRegularExpression);
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
            worker.Value.Log.Dialog.Close();
            worker.Value.Dispose();
        }
    }
}
