using System.Collections.Generic;
using System.Collections.ObjectModel;
using Mnk.TBox.Core.Contracts;
using Mnk.Library.WpfControls.Dialogs.StateSaver;

namespace Mnk.TBox.Plugins.TeamManager.Code.Settings
{
    public class Config : IConfigWithDialogStates
    {
        public IDictionary<string, DialogState> States { get; set; }
        public string SelectedProfile { get; set; }
        public ObservableCollection<Profile> Profiles { get; set; }
        public ObservableCollection<SpecialDay> SpecialDays { get; set; }

        public Config()
        {
            SelectedProfile = string.Empty;
            Profiles = new ObservableCollection<Profile>();
            States = new Dictionary<string, DialogState>();
            SpecialDays = new ObservableCollection<SpecialDay>();
        }
    }
}
