using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mnk.Library.Common.Tools;
using Mnk.TBox.Locales.Localization.Plugins.TeamManager;
using Mnk.TBox.Plugins.TeamManager.Code.Reports.Contracts;

namespace Mnk.TBox.Plugins.TeamManager.Code.Reports
{
    class PlainTextReport : IReportsPrinter
    {
        private readonly Action<string> setter;
        public PlainTextReport(Action<string> setter)
        {
            this.setter = setter;
        }

        public void Print(IList<ReportPerson> items, int time, string[] links)
        {
            var sb = new StringBuilder();
            foreach (var p in items)
            {
                sb.AppendLine(p.Name)
                  .AppendLine(new StringBuilder().PrintTable(
                        new []{
                                p.Columns
                            }.
                        Concat(
                            p.Days.Select(x=>new[]{x.Name}.Concat(x.Columns).ToArray()).ToArray()
                        ).Concat(
                        new []{
                                p.Summaries
                            })
                        .ToArray()
                        ).ToString());
            }
            foreach (var link in links)
            {
                sb.AppendLine(link);
            }
            sb.AppendLine(string.Format(TeamManagerLang.ReportFooterTemplate, time));
            setter(sb.ToString());
        }
    }
}
