using System.Windows;
using System.Windows.Controls;
using Mnk.TBox.Core.Contracts;

namespace Mnk.TBox.Plugins.SqlWatcher
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

		private void CheckBoxChecked(object sender, RoutedEventArgs e)
		{
			Dirs.OnCheckChangedEvent(sender, e);
		}

		public UserControl Control { get { return this; } }
	}
}
