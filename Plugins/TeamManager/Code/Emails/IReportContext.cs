using System.Collections.Generic;
using Mnk.Library.Common.Communications;
using Mnk.TBox.Plugins.TeamManager.Code.Reports.Contracts;
using Mnk.TBox.Plugins.TeamManager.Code.Settings;

namespace Mnk.TBox.Plugins.TeamManager.Code.Emails
{
    interface IReportContext
    {
        FullReport FullReport { get; }
        IEmailSender Sender { get; }
        IList<ReportPerson> AllPersons { get; }

        string BuildReport(IList<ReportPerson> members);
        IEnumerable<Person> GetPersons(TimeReportType type);
    }
}
