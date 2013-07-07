using System.Linq;
using Mnk.Library.Common.MT;
using Mnk.Library.Common.Tools;
using Mnk.TBox.Locales.Localization.Plugins.TeamManager;
using Mnk.TBox.Plugins.TeamManager.Code.Settings;

namespace Mnk.TBox.Plugins.TeamManager.Code.Emails.Senders
{
    class TeamReportSender : IReportsSender
    {
        public void Send(IReportContext context, IUpdater u)
        {
            var subject = string.Format(TeamManagerLang.TeamEmailSubjectTemplate, 
                context.FullReport.From.ToShortDateString(),
                context.FullReport.To.ToShortDateString());
            var persons = context.GetPersons(TimeReportType.Team).ToArray();
            var i = 0;
            foreach (var person in persons)
            {
                var members = person.TeamMembers
                    .Select(m => context.AllPersons.FirstOrDefault(x => x.Name.EqualsIgnoreCase(m.Key)))
                    .Where(x => x != null)
                    .OrderBy(x => x.Name)
                    .ToArray();
                if (members.Length == 0 || u.UserPressClose) continue;
                u.Update(TeamManagerLang.SendTo + ": " + person.Key, ++i / (float)(persons.Length + 1));
                context.Sender.Send(subject, context.BuildReport(members), true, new[] { person.Key });
            }
        }
    }
}
