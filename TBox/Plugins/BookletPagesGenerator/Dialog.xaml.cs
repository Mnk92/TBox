using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using Mnk.Library.Common;
using Mnk.Library.Common.Log;
using Mnk.TBox.Locales.Localization.Plugins.BookletPagesGenerator;
using Mnk.Library.WpfControls.Tools;
using Mnk.TBox.Plugins.BookletPagesGenerator.Code;

namespace Mnk.TBox.Plugins.BookletPagesGenerator
{
	/// <summary>
	/// Interaction logic for Dialog.xaml
	/// </summary>
	public partial class Dialog
	{
		private static readonly ILog Log = LogManager.GetLogger<Dialog>();
		private readonly PagePrinter printer = new PagePrinter(2);
		private Result printedPages = new Result();

		public Dialog()
		{
			InitializeComponent();
			lbResult.SelectionChanged += Results_OnSelected;
		}

		protected override void OnShow()
		{
			nudTotalPages_ValueChanged(null, null);
			nudPagesToPrintCount_ValueChanged(null, null);
			nudPagesOffset_ValueChanged(null, null);
			OnGenerateClick(null, null);
			nudTotalPages.SetFocus();
		}

		private void OnPrevClick(object sender, RoutedEventArgs e)
		{
			lbResult.SelectedIndex = Math.Max(0, lbResult.SelectedIndex - 1);
		}

		private void OnNextClick(object sender, RoutedEventArgs e)
		{
			lbResult.SelectedIndex = Math.Min(lbResult.Items.Count - 1, lbResult.SelectedIndex + 1);
		}

		private void UpdateControls()
		{
			btnGenerate.IsEnabled = nudTotalPages.Value > 0 && 
				nudPagesToPrintCount.Value > 0 &&
				nudTotalPages.Value >= nudPagesToPrintCount.Value;
			OnGenerateClick(null, null);
		}

		private void Results_OnSelected(object sender, RoutedEventArgs e)
		{
			btnPrev.IsEnabled = lbResult.SelectedIndex > 0;
			btnNext.IsEnabled = lbResult.SelectedIndex < lbResult.Items.Count - 1;
			if (lbResult.SelectedIndex >= 0)
			{
				tbPages.Value = lbResult.SelectedItem.ToString();
			}
			if (lbResult.Items.Count > 0)
			{
				((Config) DataContext).PrintPageId = lbResult.SelectedIndex;
				var id = 2*(lbResult.SelectedIndex/2);
				var arrFirst = printedPages.Numbers[id];
				var arrBack = printedPages.Numbers[id + 1];
				lRange.Content = string.Format("{0}-{1}",
				                               Math.Min(arrFirst.Min(), arrBack.Min()),
				                               Math.Max(arrFirst.Max(), arrBack.Max()));
				lUpOrDown.Content = ((lbResult.SelectedIndex%2) == 0) ?  BookletPagesGeneratorLang.Up : BookletPagesGeneratorLang.Down;
			}
			else
			{
				tbPages.Value = string.Empty;
			}
			Clipboard.SetText(tbPages.Value);
		}

		private void nudPagesOffset_ValueChanged(object sender, RoutedEventArgs e)
		{
			nudPagesToPrintCount.Value = Math.Max(nudPagesToPrintCount.Minimum,
				Math.Min(nudPagesToPrintCount.Value, nudTotalPages.Value - nudPagesOffset.Value));
			UpdateControls();
		}

		private void nudTotalPages_ValueChanged(object sender, RoutedEventArgs e)
		{
			nudPagesToPrintCount.Maximum = nudTotalPages.Value;
			nudPagesOffset.Maximum = nudTotalPages.Value;
			UpdateControls();
		}

		private void nudPagesToPrintCount_ValueChanged(object sender, RoutedEventArgs e)
		{
			nudPagesOffset.Maximum = nudTotalPages.Value;
			UpdateControls();
		}
		
		private void OnGenerateClick(object sender, RoutedEventArgs e)
		{
			if(!IsVisible)return;
			ExceptionsHelper.HandleException(
				GeneratePages,
				() => "Generate internal error!",
				Log);
		}

		private void GeneratePages()
		{
			Clipboard.Clear();
			lbResult.ItemsSource = null;
			if (!btnGenerate.IsEnabled) return;

			printedPages = printer.Calc(
				nudPagesOffset.Value,
				nudTotalPages.Value,
				nudPagesToPrintCount.Value);
			btnPrev.IsEnabled = false;
			btnNext.IsEnabled = printedPages.IsValid();
			tbPages.Value = string.Empty;
			lUpOrDown.Content = lRange.Content = string.Empty;
			lPagesCount.Content = ((nudTotalPages.Value - nudPagesOffset.Value)/4).ToString(CultureInfo.InvariantCulture);
			lSteps.Content = (printedPages.IsValid())
				                 ? (printedPages.Pages.Length/2).ToString(CultureInfo.InvariantCulture)
				                 : 0.ToString(CultureInfo.InvariantCulture);

			var config = (Config) DataContext;
			if (printedPages.IsValid())
			{
				lbResult.ItemsSource = printedPages.Pages;
				lbResult.SelectedIndex = config.PrintPageId;
			}
			else
			{
				config.PrintPageId = 0;
			}
			GC.Collect();
		}

		private void CancelClick(object sender, RoutedEventArgs e)
		{
			Close();
		}
	}
}
