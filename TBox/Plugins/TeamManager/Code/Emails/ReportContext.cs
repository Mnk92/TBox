using System;
using System.Collections.Generic;
using System.Linq;
using Mnk.Library.Common.Communications;
using Mnk.Library.WpfControls.Tools;
using Mnk.TBox.Core.PluginsShared.ReportsGenerator;
using Mnk.TBox.Plugins.TeamManager.Code.Reports.Contracts;
using Mnk.TBox.Plugins.TeamManager.Code.Settings;

namespace Mnk.TBox.Plugins.TeamManager.Code.Emails
{
    class ReportContext : IReportContext
    {
        private readonly Profile profile;
        private readonly Func<IList<ReportPerson>, string> reportBuilder;

        public FullReport FullReport { get; private set; }
        public IEmailSender Sender { get; private set; }
        public IList<ReportPerson> AllPersons { get; private set; }

        public ReportContext(FullReport fullReport, IList<ReportPerson> allPersons, Profile profile, Func<IList<ReportPerson>, string> reportBuilder)
        {
            AllPersons = allPersons;
            this.profile = profile;
            this.reportBuilder = reportBuilder;
            FullReport = fullReport;
            Sender = GetEmailSender(profile);
        }

        public string BuildReport(IList<ReportPerson> members)
        {
            return reportBuilder(members);
        }

        public IEnumerable<Person> GetPersons(TimeReportType type)
        {
            return profile.Persons.CheckedItems
                .Where(x => ((TimeReportType)x.ReportType).HasFlag(type));
        }

        private static IEmailSender GetEmailSender(Profile profile)
        {
            var password = profile.Email.Password.DecryptPassword();
            if (profile.Email.IsSmtp)
            {
                return new SmptEmailSender(profile.Email.ServerUrl, profile.Email.Port,
                    profile.Email.Login, password);
            }
            return new ExchangeEmailSender(new Uri(new Uri(profile.Email.ServerUrl), "/ews/Exchange.asmx").ToString(),
                profile.Email.Login, password);
        }

    }
}
