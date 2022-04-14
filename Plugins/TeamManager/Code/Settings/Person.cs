using System.Collections.ObjectModel;
using Mnk.Library.Common.Tools;
using Mnk.Library.Common.UI.Model;

namespace Mnk.TBox.Plugins.TeamManager.Code.Settings
{
    public class Person : CheckableData
    {
        public int ReportType { get; set; }
        public ObservableCollection<Data> TeamMembers { get; set; }

        public Person()
        {
            ReportType = (int)TimeReportType.Personal;
            TeamMembers = new ObservableCollection<Data>();
        }

        public override object Clone()
        {
            return new Person
            {
                Key = Key,
                IsChecked = IsChecked,
                ReportType = ReportType,
                TeamMembers = TeamMembers.Clone()
            };
        }
    }
}
