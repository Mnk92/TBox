using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Mnk.Library.WpfControls;
using Mnk.TBox.Plugins.Requestor.Code;
using Mnk.TBox.Plugins.Requestor.Code.Settings;
using Mnk.Library.WpfControls.Tools;

namespace Mnk.TBox.Plugins.Requestor.Components
{
	/// <summary>
	/// </summary>
	public partial class FormRequest 
	{
		public IList<string> KnownHeaderNames { get; set; }
		public IList<string> KnownHeaderValues { get; set; }
		public IList<string> KnownUrls { get; set; }
		private readonly BaseExecutor executor = new BaseExecutor();

		public FormRequest(ImageSource icon)
		{
			KnownHeaderNames = new ObservableCollection<string>();
			KnownHeaderValues = new ObservableCollection<string>();
			KnownUrls = new ObservableCollection<string>();
			InitializeComponent();
			cbUrl.KeyDown += CbUrlKeyDown;
		    Icon = icon;
		}

		public void ShowDialog(Op config)
		{
			Tabs.SelectedIndex = 0;
			Response.Value = null;
			Owner = Application.Current.MainWindow;
			DataContext = config;
			Headers.ConfigureInputTextList(Title, config.Request.Headers, KnownHeaderNames);
			cbUrl.SetFocus();
			SafeShowDialog();
		}

		private void CbUrlKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Return)
			{
				BtnTestClick(sender, e);
			}
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			if (!cbUrl.IsEnabled)
			{
				e.Cancel = true;
			}
			else
			{
				base.OnClosing(e);
			}
		}

		private void BtnTestClick(object sender, RoutedEventArgs e)
		{
			executor.Execute(this, (Op)DataContext, 
				r => Mt.Do(this, 
					() =>{
							Tabs.SelectedIndex = 1;
							Response.Value = r;
					}), Icon);
		}

		private void CancelClick(object sender, RoutedEventArgs e)
		{
			Close();
		}
	}
}
