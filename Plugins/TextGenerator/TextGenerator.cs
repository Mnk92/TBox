using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using Mnk.TBox.Core.Contracts;
using Mnk.TBox.Locales.Localization.Plugins.TextGenerator;
using Mnk.TBox.Plugins.TextGenerator.Code;
using Mnk.Library.WpfControls.Dialogs;
using Mnk.Library.WpfControls.Menu;

namespace Mnk.TBox.Plugins.TextGenerator
{
    [PluginInfo(typeof(TextGeneratorLang), 96, PluginGroup.Development)]
    public sealed class TextGenerator : ConfigurablePlugin<Settings, Config>
    {
        public TextGenerator()
        {
            Menu = new[]{
                new UMenuItem{Header = TextGeneratorLang.ReplaceCharsSequensesByTabs, OnClick = _=>PrepareTable()},
                new UMenuItem{Header = TextGeneratorLang.GenerateTextFormClipboard, OnClick = _=>GenerateText(Clipboard.GetText())},
                new UMenuItem{Header = TextGeneratorLang.GenerateText, OnClick = _=>GenerateText(string.Empty)},
                new UMenuItem{Header = TextGeneratorLang.GenerateGuid, OnClick = _=>GenerateGuid()},
                new UMenuItem{Header = TextGeneratorLang.CalcClipboardTextLength, OnClick = _=>CalcClipboardTextLength()},
                new UMenuItem{Header = TextGeneratorLang.SortLines, OnClick = _=>SortLines()},
            };
        }

        private static void SortLines()
        {
            var lines = Clipboard.GetText().Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            Clipboard.SetText(string.Join(Environment.NewLine, lines.OrderBy(x => x)));
        }

        private static void CalcClipboardTextLength()
        {
            MessageBox.Show(TextGeneratorLang.LengthOfTheClipboardText + " = " + Clipboard.GetText().Length, TextGeneratorLang.PluginName,
                            MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private static void GenerateGuid()
        {
            Clipboard.SetText(Guid.NewGuid().ToString());
        }

        private void GenerateText(string text)
        {
            var value = DialogsCache.ShowInputBox(TextGeneratorLang.PleaseSpecifyTextLength, TextGeneratorLang.GenerateText, Config.TextLength.ToString(CultureInfo.InvariantCulture), (x) => int.TryParse(x, out _), Application.Current.MainWindow);
            if (!value.Key) return;

            Config.TextLength = int.Parse(value.Value);
            var sb = new StringBuilder(Config.TextLength);
            if (string.IsNullOrEmpty(text))
                text = TextGeneratorLang.SampleDefaultText + ". ";
            while (sb.Length < Config.TextLength) sb.Append(text);
            Clipboard.SetText(sb.ToString().Substring(0, Config.TextLength));
        }

        private void PrepareTable()
        {
            var text = Clipboard.GetText();
            if (string.IsNullOrWhiteSpace(text)) return;
            var lines = text.Split(
                new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            var result = new StringBuilder();
            foreach (var words in
                lines.Select(line => line.Split(new[] { Config.Fill }, StringSplitOptions.RemoveEmptyEntries)))
            {
                foreach (var word in words)
                {
                    result.Append(word).Append('\t');
                }
                result.Append(Environment.NewLine);
            }
            Clipboard.SetText(result.ToString());
        }
    }
}
