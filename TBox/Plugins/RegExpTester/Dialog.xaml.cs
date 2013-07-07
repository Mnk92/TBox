using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using Mnk.Library.Common;
using Mnk.Library.WpfControls.Tools;
using Mnk.TBox.Plugins.RegExpTester.Code.Settings;

namespace Mnk.TBox.Plugins.RegExpTester
{
    /// <summary>
    /// Interaction logic for Dialog.xaml
    /// </summary>
    public partial class Dialog
    {
        private readonly Stopwatch sw = new Stopwatch();
        public Dialog()
        {
            InitializeComponent();
        }

        protected override void OnShow()
        {
            base.OnShow();
            RegExp.SetFocus();
        }

        private void DataChanged(object sender, RoutedEventArgs e)
        {
            OnTextChanged(this, e);
        }

        private void OnTextChanged(object sender, EventArgs e)
        {
            if (TestManual.IsChecked == true) return;
            DoTest();
        }

        private void DoTest()
        {
            sbError.Visibility = sbErrorCaption.Visibility = Visibility.Hidden;
            Results.Items.Clear();
            var profile = Data.DataContext as Profile;
            if (profile == null) return;
            var options = (RegexOptions)profile.Options;
            if (!CanUseRegExp()) return;
            sw.Restart();
            ExceptionsHelper.HandleException(
                () => PrintResult(new Regex(RegExp.Text, options), Text.Text),
                ex =>
                {
                    sbError.Content = ex.Message;
                    sbError.Visibility = sbErrorCaption.Visibility = Visibility.Visible;
                }
                );
        }

        private bool CanUseRegExp()
        {
            return
                !string.IsNullOrWhiteSpace(RegExp.Text) &&
                !string.IsNullOrWhiteSpace(Text.Text);
        }

        private void PrintResult(Regex regex, string text)
        {
            var matches = regex.Matches(text);
            sbTime.Content = string.Format("{0:0.###}", sw.ElapsedMilliseconds / 1000.0);
            sbCount.Content = matches.Count;

            var groups = regex.GetGroupNames();
            Results.Visibility = Visibility.Hidden;
            for (var i = 0; i < matches.Count; ++i)
            {
                var m = matches[i];
                var item = new TreeViewItem { Header = string.Format("[{0}]", m.Value) };
                for (var j = 1; j < m.Groups.Count; ++j)
                {
                    var g = m.Groups[j];
                    item.Items.Add(string.Format("{0}: [{1}]", groups[j], g.Value));
                }
                Results.Items.Add(item);
            }
            Results.Visibility = Visibility.Visible;
        }

        private void ButtonTestClick(object sender, RoutedEventArgs e)
        {
            DoTest();
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
