using System;
using System.Collections.Generic;
using Mnk.TBox.Core.PluginsShared.ReportsGenerator;

namespace Mnk.TBox.Plugins.TeamManager.Code.Scripts
{
    sealed class ReportScriptContext : IReportScriptContext
    {
        private readonly IDictionary<string, ColumnInfo> columns;
        public ReportScriptContext(DateTime dateFrom, DateTime dateTo, IList<string> persons, IDictionary<string,ColumnInfo> columns )
        {
            this.columns = columns;
            DateFrom = dateFrom;
            DateTo = dateTo;
            Persons = persons;
            Report = new List<LoggedInfo>();
        }

        public string Name { get; set; }
        public DateTime DateFrom { get; private set; }
        public DateTime DateTo { get; private set; }
        public IList<string> Persons { get; private set; }
        public IList<LoggedInfo> Report { get; private set; }
        public void AddResult(LoggedInfo info)
        {
            info.HasTime = columns[Name].HasTime;
            Report.Add(info);
        }

        public void Configure(bool hasTime)
        {
            lock (columns)
            {
                columns[Name] = new ColumnInfo { HasTime = hasTime };
            }
        }
    }
}
