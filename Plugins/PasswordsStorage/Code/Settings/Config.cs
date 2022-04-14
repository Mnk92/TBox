using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Mnk.Library.WpfControls.Dialogs.StateSaver;
using Mnk.TBox.Core.Contracts;

namespace Mnk.TBox.Plugins.PasswordsStorage.Code.Settings
{
    [Serializable]
    public class Config : IConfigWithDialogStates
    {
        public IDictionary<string, DialogState> States { get; set; }
        public ObservableCollection<Profile> Profiles { get; set; }
        public string SelectedProfile { get; set; }
        public int PasswordLength { get; set; }
        public int PasswordNonAlphaCharacters { get; set; }

        public Config()
        {
            Profiles = new ObservableCollection<Profile>
            {
                new Profile
                {
                    Key = "Default",
                    LoginInfos = new ObservableCollection<LoginInfo>
                    {
                        new LoginInfo{Key = "Hello", Login = "world", Password = ""}
                    }
                }
            };
            SelectedProfile = Profiles.First().Key;
            States = new Dictionary<string, DialogState>();
            PasswordLength = 8;
            PasswordNonAlphaCharacters = 2;
        }
    }
}
