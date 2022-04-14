using Mnk.Library.Common.UI.Model;

namespace Mnk.TBox.Plugins.PasswordsStorage.Code.Settings
{
    public class LoginInfo : Data
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public string Comment { get; set; }

        public override object Clone()
        {
            return new LoginInfo
            {
                Key = Key,
                Comment = Comment,
                Login = Login,
                Password = Password
            };
        }
    }
}
