using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Mnk.Library.Common.Tools;
using Mnk.Library.WpfControls;
using Mnk.TBox.Core.Contracts;
using Mnk.TBox.Core.PluginsShared.ScriptEngine;
using Mnk.Library.WpfControls.Code;
using Mnk.TBox.Plugins.Automator.Code.Settings;

namespace Mnk.TBox.Plugins.Automator
{
	/// <summary>
	/// Interaction logic for Settings.xaml
	/// </summary>
	public sealed partial class Settings : ISettings, IDisposable
	{
		private readonly LazyDialog<OperationDialog> operationsDialog = 
			new LazyDialog<OperationDialog>(()=>new OperationDialog());
		public Settings()
		{
			InitializeComponent();
		}

		public void Dispose()
		{
			operationsDialog.Dispose();
		}

		public UserControl Control { get { return this; } }
		public Func<IList<string>> FilePathsGetter { get; set; }

		private void BtnEditClick(object sender, RoutedEventArgs e)
		{
			var selectedKey = ((TextBlock)((DockPanel)((Button)sender).Parent).Children[1]).Text;
			var profile = (Profile)Profile.SelectedValue;
			var id = profile.Operations.GetExistIndexByKeyIgnoreCase(selectedKey);
			operationsDialog.Value.ShowDialog(FilePathsGetter(), profile.Operations[id]);
		}
	}
}
