using System;
using System.Windows;
using System.Windows.Controls;
using Mnk.TBox.Core.Contracts;
using Mnk.TBox.Plugins.Notes.Code.Settings;

namespace Mnk.TBox.Plugins.Notes
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : ISettings
    {
        public Settings()
        {
            InitializeComponent();
        }
        
        public UserControl Control
        {
            get { return this; }
        }

        public event Action<Profile> EditHandler;

        protected virtual void OnEditHandler(Profile obj)
        {
            var handler = EditHandler;
            if (handler != null) handler(obj);
        }

        private void EditProfileClick(object sender, RoutedEventArgs e)
        {
            OnEditHandler((Profile)((Button)sender).DataContext);
        }
    }
}
