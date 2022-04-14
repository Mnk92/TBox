using System.Windows;
using Mnk.TBox.Core.Contracts;
using Mnk.TBox.Plugins.Notes.Code.Settings;

namespace Mnk.TBox.Plugins.Notes.Components
{
    /// <summary>
    /// Interaction logic for dialog.xaml
    /// </summary>
    public partial class Dialog
    {
        private IPluginContext pluginContext;
        public Dialog()
        {
            InitializeComponent();
        }

        public void ShowDialog(Profile p, IPluginContext context, Window owner)
        {
            Title = p.Key;
            pluginContext = context;
            DataContext = p;
            Owner = owner;
            if (owner != null)
            {
                SafeShowDialog();
            }
            else ShowAndActivate();
        }

        private void CloseClick(object sender, RoutedEventArgs e)
        {
            pluginContext.SaveConfig();
            Close();
        }
    }
}
