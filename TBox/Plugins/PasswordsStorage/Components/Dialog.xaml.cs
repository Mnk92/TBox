using System;
using System.Collections.ObjectModel;
using System.Web.Security;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Mnk.Library.Common.Log;
using Mnk.Library.WpfControls.Components.Captioned;
using Mnk.Library.WpfControls.Tools;
using Mnk.TBox.Core.Contracts;
using Mnk.TBox.Plugins.PasswordsStorage.Code;
using Mnk.TBox.Plugins.PasswordsStorage.Code.Settings;

namespace Mnk.TBox.Plugins.PasswordsStorage.Components
{
    /// <summary>
    /// Interaction logic for Dialog.xaml
    /// </summary>
    public partial class Dialog
    {
        private readonly IPasswordGenerator passwordGenerator;
        private readonly ILog log = LogManager.GetLogger<Dialog>();
        private IPluginContext pluginContext;
        private IConfigManager<Config> cm;
        public Dialog(IPasswordGenerator passwordGenerator)
        {
            this.passwordGenerator = passwordGenerator;
            InitializeComponent();
        }

        public void ShowDialog(IConfigManager<Config> cm, Profile p, IPluginContext context, Window owner)
        {
            if (!IsVisible)
            {
                this.cm = cm;
                Title = p.Key;
                DataContext = null;
                DataContext = p;
                Owner = owner;
                pluginContext = context;
                IsReadOnly.IsChecked = true;
            }
            ShowAndActivate();
        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {
            pluginContext.SaveConfig();
            Close();
        }

        private void CellClick(object sender, RoutedEventArgs routedEventArgs)
        {
            var textbox = sender as TextBox;
            var text = textbox != null ? textbox.Text : ((CaptionedPasswordBox) sender).Value.DecryptPassword();
            if (!string.IsNullOrEmpty(text))
            {
                Clipboard.SetText(text);
            }
        }

        private void CopyAll(object sender, RoutedEventArgs e)
        {
            var info = GetLoginInfo(sender);
            Clipboard.SetText(
                info.Key + Environment.NewLine +
                (string.IsNullOrEmpty(info.Comment) ? string.Empty : (info.Comment + Environment.NewLine)) +
                info.Login + Environment.NewLine +
                info.Password.DecryptPassword()
                );
        }

        private static LoginInfo GetLoginInfo(object sender)
        {
            return (LoginInfo) ((Button) sender).DataContext;
        }

        private void NewPassword(object sender, RoutedEventArgs e)
        {
            try
            {
                var info = GetLoginInfo(sender);
                info.Password = passwordGenerator
                    .Generate(cm.Config.PasswordLength, cm.Config.PasswordNonAlphaCharacters);
                Passwords.Items.Items.Refresh();
            }
            catch (Exception ex)
            {
                log.Write(ex, "Unexpected issue");
            }
        }
    }
}
