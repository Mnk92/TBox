using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Mnk.Library.Common.Tools;
using Mnk.TBox.Locales.Localization.Plugins.TeamManager;
using Mnk.TBox.Core.PluginsShared.ReportsGenerator;
using Mnk.TBox.Plugins.TeamManager.Code.Reports.Contracts;
using Mnk.TBox.Plugins.TeamManager.Code.Scripts;

namespace Mnk.TBox.Plugins.TeamManager.Code.Reports
{
    class ReportsBuilder
    {
        private readonly int workHours;
        private readonly IDayStatusStrategy dayStatusStrategy;
        private readonly IDayTypeProvider dayTypeProvider;
        public ReportsBuilder(int workHours, IDayStatusStrategy dayStatusStrategy, IDayTypeProvider dayTypeProvider)
        {
            this.workHours = workHours;
            this.dayStatusStrategy = dayStatusStrategy;
            this.dayTypeProvider = dayTypeProvider;
        }

        public IList<ReportPerson> BuildLoggedTimeReport(FullReport report)
        {
            return report.Items
                .OrderBy(x => x.Email)
                .GroupBy(x => x.Email)
                .Select(t => FillPerson(t, report))
                .ToList();
        }

        private ReportPerson FillPerson(IGrouping<string, LoggedInfo> times, FullReport report)
        {
            var items = GetItems(times);
            var columns = GetColumns(report, times.ToArray());
            var days = items
                .Select(x => FillDay(x.Key, x.ToArray(), columns, report))
                .ToList();
            return new ReportPerson
                {
                    Name = times.Key,
                    Columns = new[] { TeamManagerLang.Date }
                                .Concat(columns.SelectMany(x =>BuildHeaders(x.Key, report)))
                                .ToArray(),
                    Days = days,
                    Summaries = new[] { TeamManagerLang.Days + ":" + days.Count }
                            .Concat(columns.SelectMany(x =>BuildFooters(x.Key, report, times)))
                            .ToArray()};
        }

        private static IEnumerable<string> BuildHeaders(string key, FullReport report)
        {
            return HasTime(key, report)
                       ? new[]
                           {
                               key + " " + TeamManagerLang.Spent,
                               key + " " + TeamManagerLang.Assignable
                           }
                       : new[] {key + " " + TeamManagerLang.Assignable};
        }

        private static IEnumerable<string> BuildFooters(string key, FullReport report, IEnumerable<LoggedInfo> times)
        {
            return HasTime(key, report) ?
                                    new[] { TeamManagerLang.Spent + ":" + 
                                        times.Where(o => string.Equals(key, o.Column))
                                        .Sum(o => o.Spent), 
                                        string.Empty } :
                                    new[] { string.Empty };
        } 

        private ReportDay FillDay(string date, LoggedInfo[] items, IDictionary<string, double> columns, FullReport report)
        {
            return new ReportDay
                {
                    Name = date,
                    Columns = FillDayColumns(items, columns, report),
                    Status = dayStatusStrategy.GetState(
                        items.First().Date,
                        columns.Where(x=>HasTime(x.Key, report))
                                .ToDictionary(x => x.Key,
                                             x => items.Where(o =>o.Column.EqualsIgnoreCase(x.Key))
                                                       .GroupBy(o => o.Task)
                                                       .ToDictionary(o => o.Key, o => o.Sum(z => z.Spent))), 
                         workHours,
                         dayTypeProvider
                         )
                };
        }

        private static IList<string> FillDayColumns(LoggedInfo[] items, IEnumerable<KeyValuePair<string, double>> columns, FullReport report)
        {
            var results = new List<string>();
            foreach (var column in columns)
            {
                var c = column;
                var colItems = items.Where(x => string.Equals(x.Column, c.Key)).ToArray();
                if (HasTime(column.Key, report))
                {
                    var sum = colItems.Sum(x => x.Spent);
                    results.Add(sum <= 0 ? string.Empty : sum.ToString(CultureInfo.InvariantCulture));
                }
                results.Add(string.Join(";", colItems.OrderBy(x => x.Task).GroupBy(x => x.Task).Select(x =>
                {
                    var key = x.Key;
                    var count = x.Count();
                    var sum = x.Where(y => string.Equals(y.Task, x.Key)).Sum(y => y.Spent);
                    if (count < 2) return AppendSumIfNeed(sum, key);
                    return AppendSumIfNeed(sum, string.Format("{0}(x{1})", key, count));
                })));
            }
            return results.ToArray();
        }

        private static string AppendSumIfNeed(double sum, string str)
        {
            return sum > 0 ? string.Format("[{0}]{1}", sum, str) : str;
        }


        private static Dictionary<string, double> GetColumns(FullReport report, IList<LoggedInfo> t)
        {
            return t
                .Where(x => !string.IsNullOrEmpty(x.Column))
                .Concat(GetMissingColumns(report, t).Select(y=>new LoggedInfo{Column = y.Key, HasTime = y.Value.HasTime}))
                .OrderBy(x=>x.Column, new OrderComparer<string>(report.Columns.Select(o=>o.Key).ToArray()))
                .GroupBy(x => x.Column)
                .ToDictionary(x => x.Key, x => x.Sum(y => y.Spent));
        }

        private static IEnumerable<KeyValuePair<string, ColumnInfo>> GetMissingColumns(FullReport report, IEnumerable<LoggedInfo> t)
        {
            return report.Columns
                .Where(o => t.All(y => !y.Column.EqualsIgnoreCase(o.Key)))
                .Select(x=>x);
        }

        private static IEnumerable<IGrouping<string, LoggedInfo>> GetItems(IEnumerable<LoggedInfo> t)
        {
            return t
                .Where(x => !x.Fake)
                .OrderBy(x => x.Date)
                .GroupBy(x => string.Format("{0} ({1})", x.Date.ToShortDateString(), x.Date.DayOfWeek.ToString().Substring(0, 2)))
                .ToArray();
        }

        private static bool HasTime(string key, FullReport report)
        {
            return report.Columns.First(x=>x.Key.EqualsIgnoreCase(key)).Value.HasTime;
        }

    }
}

