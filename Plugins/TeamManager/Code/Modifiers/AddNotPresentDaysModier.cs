using System;
using System.Collections.Generic;
using System.Linq;
using Mnk.Library.Common.Tools;
using Mnk.TBox.Core.PluginsShared.ReportsGenerator;

namespace Mnk.TBox.Plugins.TeamManager.Code.Modifiers
{
    class AddNotPresentDaysModier : IReportModifier
    {
        public void Modify(FullReport report)
        {
            foreach (var email in report.Emails
                .Where(email => !report.Items.Any(x => x.Email.EqualsIgnoreCase(email))))
            {
                report.Items.Add(new LoggedInfo
                {
                    Fake = true,
                    Email = email
                });
            }
            var days = GetDatesRange(report.From, report.To);
            foreach (var i in report.Items.GroupBy(x => x.Email))
            {
                var exist = i.Select(x => x.Date).Distinct().ToArray();
                if (exist.Length == days.Count) continue;
                foreach (var d in days.Where(x => !exist.Contains(x)))
                {
                    report.Items.Add(new LoggedInfo
                    {
                        Date = d,
                        Email = i.Key,
                    });
                }
            }
        }

        private static IList<DateTime> GetDatesRange(DateTime from, DateTime to)
        {
            var date = from;
            var days = new List<DateTime> { from };
            while (date < to)
            {
                days.Add(date = date.AddDays(1));
            }
            return days;
        }
    }
}
