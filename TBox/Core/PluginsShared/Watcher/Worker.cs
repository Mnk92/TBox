using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Input;
using Mnk.Library.Common.MT;
using Mnk.Library.Common.Tools;
using Mnk.Library.WpfWinForms.Icons;
using Mnk.TBox.Core.Contracts;
using Mnk.TBox.Locales.Localization.PluginsShared;
using Mnk.Library.WpfControls.Code.Log;
using Mnk.Library.WpfControls.Dialogs.StateSaver;
using Mnk.Library.WpfWinForms;

namespace Mnk.TBox.Core.PluginsShared.Watcher
{
    public sealed class Worker<TLogDialog> : IDisposable
        where TLogDialog : ILogDialog
    {
        private const string DialogId = "results";
        private readonly TrayIcon trayIcon = new TrayIcon();
        private readonly string workerId;
        private readonly IPathResolver pathResolver;
        private readonly WatchEngine watcher;
        private readonly SafeTimer timer;
        private readonly IconWithText iconWithText;
        private readonly LogCache logCache;
        private TrayLog trayLog = null;
        public TLogDialog Log { get; private set; }

        public Worker(TLogDialog loggedList, IDataParser dataParser, string workerId, IConfigWithDialogStates config, IPathResolver pathResolver, Color background = default(Color))
        {
            iconWithText = new IconWithText(background: background);
            Log = loggedList;
            Log.OnClear += (o, e) => loggedList_OnDataChanged(null, null);
            logCache = new LogCache(Log, loggedList_OnDataChanged);
            this.workerId = workerId;
            this.pathResolver = pathResolver;
            watcher = new WatchEngine(workerId, dataParser);
            trayIcon.MouseClick += b => { if (b == MouseButton.Left)ShowLogsList(); };
            timer = new SafeTimer(Work);
            Log.DialogWindow.SetState(config.States.Get(DialogId));
            loggedList_OnDataChanged(null, null);
        }

        public void SetMenuItems(IList<UMenuItem> items)
        {
            trayIcon.SetMenuItems(items, false);
        }

        private void loggedList_OnDataChanged(string caption, string text)
        {
            iconWithText.Create(Log.EntriesCount.ToString(CultureInfo.InvariantCulture));
            trayIcon.Icon = iconWithText.Icon;
            trayIcon.HoverText = string.Format(CultureInfo.InvariantCulture, PluginsSharedLang.EntriesCountTemplate, workerId, Log.EntriesCount);
            var log = trayLog;
            if (log != null && text != null)
            {
                log.Write(caption, text);
            }
        }

        private void Work()
        {
            watcher.CheckFiles();
        }

        public void Start(int rescanTime, int toolTipsTimeout, IEnumerable<DirInfo> directories, bool toolTipsEnabled)
        {
            Stop();
            if (toolTipsEnabled)
            {
                trayLog = new TrayLog(trayIcon, toolTipsTimeout);
            }
            watcher.Start(directories.Select(x=>new DirInfo
            {
                Direction = x.Direction,
                IsChecked = x.IsChecked,
                Mask = x.Mask,
                Key = x.Key,
                Path = pathResolver.Resolve(x.Path)
            }), new MultiCaptionedLog(new ICaptionedLog[] { logCache }));
            trayIcon.IsVisible = true;
            logCache.Start();
            timer.Start(rescanTime);
        }

        public void Stop()
        {
            trayLog = null;
            logCache.Stop();
            timer.Stop();
            trayIcon.IsVisible = false;
            watcher.Stop();
        }

        public void ShowLogsList()
        {
            Log.ShowLogs();
            logCache.Refresh();
        }

        public void Save(IConfigWithDialogStates config)
        {
            config.States[DialogId] = Log.DialogWindow.GetState();
        }

        public void Write(string caption, string text)
        {
            logCache.Write(caption, text);
        }

        public void Dispose()
        {
            Stop();
            timer.Dispose();
            trayIcon.Dispose();
            iconWithText.Dispose();
        }

        public void ClearLogs()
        {
            Log.Clear();
        }
    }
}
