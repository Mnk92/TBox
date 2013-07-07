using System.Collections.Generic;

namespace Mnk.TBox.Plugins.TeamManager.Code.Reports.Contracts
{
    interface IReportsPrinter
    {
        void Print(IList<ReportPerson> items, int time, string[] links);
    }
}
