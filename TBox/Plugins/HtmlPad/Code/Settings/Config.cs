using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Mnk.Library.WpfControls.Dialogs.StateSaver;
using Mnk.TBox.Core.Contracts;

namespace Mnk.TBox.Plugins.HtmlPad.Code.Settings
{
    [Serializable]
    public class Config : IConfigWithDialogStates
    {
        public IDictionary<string, DialogState> States { get; set; }
        public ObservableCollection<Profile> Profiles { get; set; }
        public string SelectedProfile { get; set; }

        public Config()
        {
            States = new Dictionary<string, DialogState>();
            Profiles = new ObservableCollection<Profile>
            {
                new Profile
                {
                    Key = "Default",
                    Text = 
@"<html>
<body>
<h1>Hello world</h1>
</body>
</html>"
                }
            };
            SelectedProfile = "Default";
        }
    }
}
