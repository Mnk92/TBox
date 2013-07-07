using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Mnk.Library.Common;
using Mnk.Library.Common.Log;
using Mnk.Library.Common.MT;
using Mnk.Library.WpfControls;
using Mnk.Library.WpfControls.Dialogs;
using Mnk.Library.WpfControls.Tools;
using Mnk.TBox.Plugins.LocalizationTool.Code;
using Mnk.TBox.Plugins.LocalizationTool.Code.Settings;

namespace Mnk.TBox.Plugins.LocalizationTool.Components
{
    /// <summary>
    /// Interaction logic for Translate.xaml
    /// </summary>
    public partial class Translate
    {
        private readonly ILog log = LogManager.GetLogger<Translate>();
        public Translate()
        {
            InitializeComponent();
        }

        public void ShowDialog(Config cfg)
        {
            if (!IsVisible)
            {
                DataContext = cfg;
                Tabs.SelectedIndex = 0;
                Source.SetFocus();
            }
            ShowAndActivate();
            SelectionChanged(this, null);
        }

        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            SelectionChanged(sender, null);
            if (!btnTranslate.IsEnabled) return;
            var config = (Config) DataContext;
            var translateFrom = config.Languages[config.SelectedLanguageFrom].Key;
            var languages = config.Languages.Select(x => x.Key).ToArray();
            var inputFormat = GetFormat(config, config.SelectedInputFormat);
            var outputFormat = GetFormat(config, config.SelectedOutputFormat);
            var template = config.Templates[config.SelectedTemplate].Value;
            var value = Source.Text;
            DialogsCache.ShowProgress(
                u => TranslateSource(translateFrom, languages, inputFormat, outputFormat, template, value, u), 
                Title, this, icon:Icon);
        }
        
        private static string GetFormat(Config config, int id)
        {
            return config.Formats[id].Key.Replace("\\t", "\t");
        }

        private void TranslateSource(string translateFrom, string[]languages, string inputFormat, string outputFormat, string template, string source, IUpdater u)
        {
            var worker = new Worker();
            var translation = string.Empty;
            ExceptionsHelper.HandleException(
                () => translation = worker.Translate(translateFrom, languages, inputFormat, outputFormat, template, source, u), 
                ex=>log.Write(ex, "Internal error"));
            Mt.Do(this, ()=>Translation.Text = translation);
        }

        private void CloseClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var config = (Config)DataContext;
            btnTranslate.IsEnabled =
                IsValid(config.SelectedInputFormat, config.Formats.Count) &&
                IsValid(config.SelectedOutputFormat, config.Formats.Count) &&
                IsValid(config.SelectedLanguageFrom, config.Languages.Count) &&
                IsValid(config.SelectedTemplate, config.Templates.Count);
        }

        private static bool IsValid(int value, int max)
        {
            return value >= 0 && value < max;
        }
    }
}
