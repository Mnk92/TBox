using Mnk.Library.Common.MT;

namespace Mnk.TBox.Plugins.TeamManager.Code.Emails.Senders
{
    interface IReportsSender
    {
        void Send(IReportContext context, IUpdater u);
    }
}
