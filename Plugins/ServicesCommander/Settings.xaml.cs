using System.Linq;
using System.ServiceProcess;
using System.Windows;
using System.Windows.Controls;
using Mnk.TBox.Core.Contracts;
using Mnk.Library.WpfControls.Code.Dialogs;

namespace Mnk.TBox.Plugins.ServicesCommander
{
	/// <summary>
	/// Interaction logic for Settings.xaml
	/// </summary>
	public partial class Settings : ISettings
	{
		public Settings()
		{
			InitializeComponent();
			Services.OnConfigured += ServicesOnOnConfigured;
		}

		private void ServicesOnOnConfigured(BaseDialog baseDialog)
		{
			((InputSelect)baseDialog).ItemsSource = 
				ServiceController.GetServices()
				.Select(x => x.DisplayName)
				.OrderBy(x=>x)
				.ToArray();
		}

		public UserControl Control
		{
			get { return this; }
		}
	}
}
