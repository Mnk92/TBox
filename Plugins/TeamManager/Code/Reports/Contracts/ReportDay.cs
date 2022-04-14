using System.Collections.Generic;

namespace Mnk.TBox.Plugins.TeamManager.Code.Reports.Contracts
{
    class ReportDay
    {
        public string Name { get; set; }
        public string Status { get; set; }
        public IList<string> Columns { get; set; }
    }
}
