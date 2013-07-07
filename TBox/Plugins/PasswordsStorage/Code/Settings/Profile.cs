using System;
using System.Collections.ObjectModel;
using Mnk.Library.Common.Tools;
using Mnk.Library.Common.UI.Model;

namespace Mnk.TBox.Plugins.PasswordsStorage.Code.Settings
{
    [Serializable]
    public class Profile : Data
    {
        public ObservableCollection<LoginInfo> LoginInfos { get; set; }

        public Profile()
        {
            LoginInfos = new ObservableCollection<LoginInfo>();
        }

        public override object Clone()
        {
            return new Profile
            {
                Key = Key,
                LoginInfos = LoginInfos.Clone()
            };
        }
    }
}
