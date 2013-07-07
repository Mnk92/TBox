using System.Windows;
using System.Windows.Controls;
using Mnk.TBox.Plugins.FileWatcher.Code;
using Mnk.TBox.Core.Contracts;

namespace Mnk.TBox.Plugins.FileWatcher
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
