using System;
using System.Web.Security;
using System.Windows;
using Mnk.Library.WpfControls.Tools;

namespace Mnk.TBox.Plugins.PasswordsStorage.Code
{
    class PasswordGenerator : IPasswordGenerator
    {
        public string Generate(int passwordLength, int passwordNonAlphaCharacters)
        {
            var password = Membership.GeneratePassword(passwordLength,
                passwordNonAlphaCharacters);
            Clipboard.SetText(password);
            return password.EncryptPassword();
        }
    }
}
