using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Mnk.Library.Common.UI.Model;
using Mnk.Library.WpfControls.Dialogs.StateSaver;
using Mnk.TBox.Core.Contracts;

namespace Mnk.TBox.Plugins.Notes.Code.Settings
{
    [Serializable]
    public class Config : IConfigWithDialogStates
    {
        public IDictionary<string, DialogState> States { get; set; }
        public ObservableCollection<Profile> Profiles { get; set; }
        public string SelectedProfile { get; set; }

        public Config()
        {
            Profiles = new ObservableCollection<Profile>
            {
                new Profile
                {
                    Key = "Default",
                    Notes = new ObservableCollection<Data<string>>
                    {
                        new Data<string>{Key = "Hello", Value = "world"}
                    }
                }
            };
            States = new Dictionary<string, DialogState>();
        }

    }
}
