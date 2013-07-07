using System.Collections.Generic;
using Mnk.Library.Common.MT;
using Mnk.TBox.Plugins.TeamManager.Code.Emails.Senders;

namespace Mnk.TBox.Plugins.TeamManager.Code.Emails
{
    class EmailsSender
    {
        private readonly IReportContext context;
        private readonly IList<IReportsSender> senders;

        public EmailsSender(IReportContext context, params IReportsSender[] senders)
        {
            this.context = context;
            this.senders = senders;
        }

        public void Send(IUpdater u)
        {
            foreach (var sender in senders)
            {
                sender.Send(context, u);
            }
        }
    }
}
