using System;
using System.Windows;

namespace Mnk.TBox.Plugins.Market.Client.Components.Uploaders
{
	/// <summary>
	/// Interaction logic for DependencyChooser.xaml
	/// </summary>
	public partial class DependencyChooserDialog
	{
		public DependencyChooserDialog()
		{
			InitializeComponent();
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			e.Cancel = true;
			Hide();
		}

		public bool? ShowDialog(Func<string, bool> validator)
		{
			Owner = Application.Current.MainWindow;
			Chooser.SetFilter(validator);
			return base.ShowDialog();
		}

	}
}
