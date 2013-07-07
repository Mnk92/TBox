using System;
using System.Windows;
using System.Windows.Controls;
using Mnk.Library.Common.Tools;
using Mnk.TBox.Core.Contracts;
using Mnk.TBox.Plugins.SqlRunner.Code.Settings;
using Mnk.TBox.Plugins.SqlRunner.Components;

namespace Mnk.TBox.Plugins.SqlRunner
{
	/// <summary>
	/// Interaction logic for Settings.xaml
	/// </summary>
	public partial class Settings : ISettings
	{
		public Lazy<FormRequest> Requestor { get; set; }
		public Settings()
		{
			InitializeComponent();
		}
		
		private void BtnRequestClick(object sender, RoutedEventArgs e)
		{
			var selectedKey = ((Button) sender).Tag.ToString();
			var profile = (Profile)Profile.SelectedValue;
			var id = profile.Ops.GetExistIndexByKeyIgnoreCase(selectedKey);
			Requestor.Value.ShowDialog(profile.Ops[id], ((Config)DataContext).ConnectionString);
		}
		
		public UserControl Control {get { return this; }}
	}
}

