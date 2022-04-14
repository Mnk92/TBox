using System.Collections.ObjectModel;
using System.Windows;
using Mnk.Library.Common;
using Mnk.Library.Common.Log;
using Mnk.Library.Common.UI.Model;
using Mnk.TBox.Locales.Localization.Plugins.Templates;
using Mnk.TBox.Core.PluginsShared.Templates;
using Mnk.TBox.Plugins.Templates.Code.Settings;
using Mnk.Library.WpfControls.Components.FilesAndFolders;

namespace Mnk.TBox.Plugins.Templates.Components
{
	/// <summary>
	/// Interaction logic for FileDialog.xaml
	/// </summary>
	public partial class FileDialog
	{
		private static readonly ILog Log = LogManager.GetLogger<FileDialog>();
		private TemplatesWorker worker = null;
		private Template template;
		private string sourcePath;
		public FileDialog()
		{
			InitializeComponent();
			TargetPath.PathGetter = new FolderPathGetter(this);
		}

		public void ShowDialog(Template t, string source, string itemTemplate)
		{
			sourcePath = source;
			if(ExceptionsHelper.HandleException(
				() => Prepare(t, itemTemplate),
				()=>"Can't prepare templates", Log))
			{
				Hide();
			}
		}

		private void Prepare(Template t, string itemTemplate)
		{
			template = t;
			Title = TemplatesLang.PluginName + " - [" + t.Key + "]";
			worker = new TemplatesWorker(itemTemplate);
			Options.ItemsSource = 
				template.KnownValues = 
					new ObservableCollection<PairData>(worker.GetValues(sourcePath, t.KnownValues));
			TargetPath.Value = template.Value;
			TargetPathValueChanged(null, null);
			ShowAndActivate();
		}

		private void FillTemplateClick(object sender, RoutedEventArgs e)
		{
			ExceptionsHelper.HandleException(
				FillTemplates, 
				()=>"Can't fill templates",
				Log
				);
		}

		private void FillTemplates()
		{
			worker.Copy(sourcePath, template.Value = TargetPath.Value, template.KnownValues);
			Hide();
		}

		private void TargetPathValueChanged(object sender, RoutedEventArgs e)
		{
			FillButton.IsEnabled = !string.IsNullOrEmpty(TargetPath.Value);
		}

		private void CancelClick(object sender, RoutedEventArgs e)
		{
			Close();
		}
	}
}
