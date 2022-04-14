using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mnk.Library.Common.Tools;
using Mnk.TBox.Locales.Localization.Plugins.TeamManager;
using Mnk.TBox.Plugins.TeamManager.Code.Reports.Contracts;

namespace Mnk.TBox.Plugins.TeamManager.Code.Reports
{
    class HtmlReport : IReportsPrinter
    {
        private readonly Action<string> setter;
        private readonly string style;

        public HtmlReport(Action<string> setter, string style)
        {
            this.setter = setter;
            this.style = style;
        }

        public void Print(IList<ReportPerson> items, int time, string[] links)
        {
            const string d = "<br/>";
            var sb = new StringBuilder("<html><head><meta http-equiv='content-type' content='text/html; charset=UTF-8'>");
            sb.Append("<style type='text/css'>").Append(style).AppendLine("</style>");
            sb.Append("</head><body>");
            foreach (var p in items)
            {
                sb.AppendLine(d)
                    .AppendLine(string.Format("<div class='person'>{0}</div>", p.Name))
                    .AppendLine(new StringBuilder().PrintHtmlTable(
                        new[]{
                                p.Columns
                            }.
                        Concat(
                            p.Days.Select(x => new[] { x.Name }.Concat(x.Columns).ToArray()).ToArray()
                        ).Concat(
                        new[]{
                                p.Summaries
                            })
                        .ToArray(),
                        new[] { "header" }
                        .Concat(p.Days.Select(x => x.Status))
                        .Concat(new[] { "footer" }).ToArray()
                        ).ToString());
            }
            foreach (var link in links)
            {
                sb.AppendLine(d).AppendLine(string.Format("<a href='{0}'>{0}<a/>", link)).AppendLine(d);
            }
            sb.AppendLine(d).AppendLine(string.Format(TeamManagerLang.ReportFooterTemplate, time))
              .Append("</body></html>");
            setter(sb.ToString());
        }
    }
}
