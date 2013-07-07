using System.Collections.ObjectModel;
using System.Windows;
using Mnk.Library.Common;
using Mnk.Library.Common.Log;
using Mnk.Library.Common.UI.Model;
using Mnk.TBox.Locales.Localization.Plugins.Templates;
using Mnk.TBox.Core.PluginsShared.Templates;
using Mnk.TBox.Plugins.Templates.Code.Settings;
using Path = System.IO.Path;

namespace Mnk.TBox.Plugins.Templates.Components
{
	/// <summary>
	/// Interaction logic for StringDialog.xaml
	/// </summary>
	public partial class StringDialog 
	{
		private static readonly ILog Log = LogManager.GetLogger<StringDialog>();
		private Template template;
		private TemplatesWorker worker;
		public StringDialog()
		{
			InitializeComponent();
		}

		public void ShowDialog(Template t, string itemTemplate)
		{
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
			Title = TemplatesLang.PluginName + " - [" + Path.GetFileName(t.Key) + "]";
			worker = new TemplatesWorker(itemTemplate);
			Options.ItemsSource =
				template.KnownValues = 
					new ObservableCollection<PairData>(worker.GetStringValues(t.Value, t.KnownValues));
			ShowAndActivate();
		}

		private void FillTemplateClick(object sender, RoutedEventArgs e)
		{
			ExceptionsHelper.HandleException(
				FillTemplate, 
				()=>"Can't fill template",
				Log
				);
		}

		private void FillTemplate()
		{
			Clipboard.SetText(worker.FillString(template.Value, template.KnownValues));
			Hide();
		}

		private void CancelClick(object sender, RoutedEventArgs e)
		{
			Close();
		}
	}
}
