using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mnk.Library.Common.Models;
using Mnk.Library.WpfControls.Code.Log;
using Mnk.TBox.Core.Contracts;

namespace Mnk.TBox.Core.PluginsShared.Watcher
{
    sealed class WatchEngine
    {
        private ICaptionedLog logger;
        private IList<Pair<DirInfo, Watch>> watchers = new List<Pair<DirInfo, Watch>>();
        private readonly object locker = new object();
        private readonly string workerId;
        private readonly IDataParser dataParser;

        public WatchEngine(string workerId, IDataParser dataParser)
        {
            this.workerId = workerId;
            this.dataParser = dataParser;
        }

        public void Start(IEnumerable<DirInfo> infoList, ICaptionedLog log)
        {
            lock (locker)
            {
                DoStop();
                logger = log;
                foreach (var info in infoList)
                {
                    watchers.Add(new Pair<DirInfo, Watch>(info, null));
                }
            }
        }

        public void Stop()
        {
            lock (locker)
            {
                DoStop();
            }
        }

        private void DoStop()
        {
            foreach (var watcher in watchers.Where(x => x.Value != null))
            {
                watcher.Value.Dispose();
            }
            watchers = new List<Pair<DirInfo, Watch>>();
        }

        public void CheckFiles()
        {
            lock (locker)
            {
                foreach (var watcher in watchers)
                {
                    if (Directory.Exists(watcher.Key.Path))
                    {
                        if (watcher.Value == null)
                        {
                            watcher.Value = new Watch((DirInfo)watcher.Key.Clone(), logger, dataParser, workerId);
                        }
                    }
                    else
                    {
                        if (watcher.Value != null)
                        {
                            watcher.Value.Dispose();
                            watcher.Value = null;
                        }
                    }
                    if (watcher.Value != null) watcher.Value.CheckFiles();
                }
            }
        }
    }
}
