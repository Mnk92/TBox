using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Mnk.Library.Common.Tools;
using Mnk.TBox.Plugins.AppConfigManager.Code;

namespace Mnk.TBox.Plugins.AppConfigManager.Components
{
	/// <summary>
	/// Interaction logic for BuildDialog.xaml
	/// </summary>
	public partial class BuildDialog
	{
		private Collection<Option> options;
		private const string ItemTemplate = "{item}";
		public BuildDialog()
		{
			InitializeComponent();
		}

		public bool? ShowDialog(Window owner, Collection<Option> optionsCollection, IEnumerable knownValues)
		{
			Owner = owner;
			DefaultValue.ItemsSource = knownValues;
			options = optionsCollection;
			DefaultValueValueChanged(null, null);
			return base.ShowDialog();
		}

		private void DefaultValueValueChanged(object sender, RoutedEventArgs e)
		{
			var needDisable = string.IsNullOrWhiteSpace(DefaultValue.Value) ||
				IsTemplateInvalid(ItemSourceTemplate.Value) ||
				IsTemplateInvalid(ItemResultTemplate.Value) ||
				string.IsNullOrWhiteSpace(Text.Text);
			ButtonMerge.IsEnabled = !needDisable;
		}

		private static bool IsTemplateInvalid(string value)
		{
			return string.IsNullOrWhiteSpace(value) ||
				   value.IndexOf(ItemTemplate, StringComparison.Ordinal) == -1;
		}

		private void TextTextChanged(object sender, TextChangedEventArgs e)
		{
			DefaultValueValueChanged(sender, e);
		}

		private void ButtonMergeClick(object sender, RoutedEventArgs e)
		{
			foreach (var o in ParseText())
			{
				var exist = options.GetExistByKeyIgnoreCase(o.Key);
				if (exist != null)
				{
					exist.Value = o.Value;
				}
				else
				{
					options.Add(o);
				}
			}
			DialogResult = true;
			Close();
		}

		private void ButtonCloseClick(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private void PasteFromClipboardClick(object sender, RoutedEventArgs e)
		{
			Text.Text = Clipboard.GetText();
		}

		private IEnumerable<Option> ParseText()
		{
			var value = DefaultValue.Value;
			var template = new StringTemplate(ItemSourceTemplate.Value, ItemTemplate);
			var resultTemplate = ItemResultTemplate.Value;
			foreach (var item in Text.Text.Split(new[] { ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
			{
				string key;
				if (template.TryParse(item, out key))
				{
					yield return new Option { Key = resultTemplate.Replace(ItemTemplate, key), Value = value };
				}
			}
		}

	}
}
