using System.Linq;
using Mnk.Library.Common.MT;
using Mnk.Library.Common.Tools;
using Mnk.TBox.Locales.Localization.Plugins.TeamManager;
using Mnk.TBox.Plugins.TeamManager.Code.Settings;

namespace Mnk.TBox.Plugins.TeamManager.Code.Emails.Senders
{
    class PersonalReportSender : IReportsSender
    {
        public void Send(IReportContext context, IUpdater u)
        {
            var subject = string.Format(TeamManagerLang.EmailSubjectTemplate, 
                context.FullReport.From.ToShortDateString(),
                context.FullReport.To.ToShortDateString());
            var emails = context.GetPersons(TimeReportType.Personal)
                .Select(x => x.Key)
                .ToArray();
            var i = 0;
            foreach (var email in emails)
            {
                var person = context.AllPersons.FirstOrDefault(x => x.Name.EqualsIgnoreCase(email));
                if (person == null || u.UserPressClose) continue;
                u.Update(TeamManagerLang.SendTo + ": " + email, ++i / (float)(emails.Length + 1));
                context.Sender.Send(subject, context.BuildReport(new[] { person }), true, new[] { email });
            }
        }
    }
}
