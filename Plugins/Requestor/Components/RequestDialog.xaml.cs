using System.Windows;
using Mnk.Library.Common.Network;

namespace Mnk.TBox.Plugins.Requester.Components
{
	/// <summary>
	/// Interaction logic for RequestDialog.xaml
	/// </summary>
	public partial class RequestDialog
	{
		public RequestDialog()
		{
			InitializeComponent();
		}

		public void ShowDialog(string title, ResponseInfo info, Window owner)
		{
			Title = title;
			Info.Value = info;
			Owner = owner;
			ShowAndActivate();
		}

		private void ButtonCloseClick(object sender, RoutedEventArgs e)
		{
			Close();
		}
	}
}
