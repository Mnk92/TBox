using System.Linq;
using Mnk.Library.Common.MT;
using Mnk.TBox.Locales.Localization.Plugins.TeamManager;
using Mnk.TBox.Plugins.TeamManager.Code.Settings;

namespace Mnk.TBox.Plugins.TeamManager.Code.Emails.Senders
{
    class FullReportSender : IReportsSender
    {
        public void Send(IReportContext context, IUpdater u)
        {
            var subject = string.Format(TeamManagerLang.FullEmailSubjectTemplate, 
                context.FullReport.From.ToShortDateString(),
                context.FullReport.To.ToShortDateString());
            var emails = context.GetPersons(TimeReportType.Full)
                .Select(x => x.Key)
                .ToArray();
            if (emails.Length <= 0 && !u.UserPressClose) return;
            u.Update(TeamManagerLang.SendingFullReport, 0);
            context.Sender.Send(subject, context.BuildReport(context.AllPersons), true, emails);
        }
    }
}
