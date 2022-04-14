using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Mnk.Library.Common.MT;
using Mnk.Library.Common.Tools;
using Mnk.TBox.Core.PluginsShared.ReportsGenerator;
using Mnk.Library.ScriptEngine;
using Mnk.TBox.Plugins.TeamManager.Code.Scripts;

namespace Mnk.TBox.Plugins.TeamManager.Code
{
    public class ReportReceiver
    {
        private readonly string readOnlyDataPath;
        public ReportReceiver(string readOnlyDataPath)
        {
            this.readOnlyDataPath = readOnlyDataPath;
        }

        internal FullReport GetTimeReport(DateTime dateFrom, DateTime dateTo, IList<string> persons, IList<SingleFileOperation> operations, IUpdater u)
        {
            var sw = new Stopwatch();
            sw.Start();
            dateFrom = Normalize(dateFrom);
            dateTo = Normalize(dateTo);
            var i = 0;
            var results = new List<LoggedInfo>();
            var runner = new ReportScriptRunner();
            var columns = new Dictionary<string, ColumnInfo>();
            var folder = readOnlyDataPath;
            var culture = Thread.CurrentThread.CurrentCulture;
            Parallel.ForEach(operations, o =>
            {
                Thread.CurrentThread.CurrentCulture = culture;
                Thread.CurrentThread.CurrentUICulture = culture;
                var context = new ReportScriptContext(dateFrom, dateTo, persons, columns)
                {
                    Name = o.Key
                };
                if (u.UserPressClose) return;
                runner.Run(Path.Combine(folder, o.Path), context, o.Parameters);
                lock (results)
                {
                    results.AddRange(context.Report);
                }
                u.Update(o.Key, ++i / (float)operations.Count);
            });
            return new FullReport
                {
                    From = dateFrom,
                    To = dateTo,
                    Emails = persons,
                    Time = (int)sw.ElapsedMilliseconds/1000,
                    Columns = operations.Select(x=>new KeyValuePair<string, ColumnInfo>(x.Key, columns[x.Key])).ToArray(),
                    Items = results
                        .Where(x => Normalize(x.Date) >= dateFrom && Normalize(x.Date) <= dateTo)
                        .OrderBy(x => x.Column, new OrderComparer<string>(operations.Select(x => x.Key).ToArray()))
                        .ToArray()
                };
        }

        private static DateTime Normalize(DateTime date)
        {
            return date.AddHours(-date.Hour).AddMinutes(-date.Minute)
                .AddSeconds(-date.Second).AddMilliseconds(-date.Millisecond);
        }

    }
}
