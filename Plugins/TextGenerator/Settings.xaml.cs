using System.Windows.Controls;
using Mnk.TBox.Core.Contracts;

namespace Mnk.TBox.Plugins.TextGenerator
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public sealed partial class Settings : ISettings
    {
        public Settings()
        {
            InitializeComponent();
        }

        public UserControl Control => this;
    }
}
