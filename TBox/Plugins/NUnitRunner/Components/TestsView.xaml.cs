using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Mnk.Library.Common.Tools;
using Mnk.Library.ParallelNUnit.Contracts;
using Mnk.Library.ParallelNUnit.Core;
using Mnk.Library.WpfControls;
using Mnk.Library.WpfControls.Components.CheckableTreeView;
using Mnk.TBox.Locales.Localization.Plugins.NUnitRunner;
using Mnk.TBox.Plugins.NUnitRunner.Code;

namespace Mnk.TBox.Plugins.NUnitRunner.Components
{
    /// <summary>
    /// Interaction logic for UnitTestsView.xaml
    /// </summary>
    public partial class TestsView : ITestsView
    {
        private ITestsMetricsCalculator tmc;
        public TestsView()
        {
            InitializeComponent();
        }

        public void Refresh(int time)
        {
            if (results.IsEmpty) return;
            Total.Content = tmc.Total;
            Passed.Content = tmc.Passed;
            Failed.Content = tmc.Failures;
            Errors.Content = tmc.Errors;
            Inconclusive.Content = tmc.Inconclusive;
            Invalid.Content = tmc.Invalid;
            Ignored.Content = tmc.Ignored;
            Skipped.Content = tmc.Skipped;
            Time.Content = time.FormatTimeInSec();
            results.Refresh();
        }

        private void SelectedTestChanged(object sender, RoutedPropertyChangedEventArgs<object> routedPropertyChangedEventArgs)
        {
            var selected = results.SelectedValue as Result;
            if (selected == null || results.IsEmpty)
            {
                TestTime.Text = NUnitRunnerLang.TestTime;
                Description.Text = string.Empty;
                return;
            }
            TestTime.Text = string.Format(CultureInfo.InvariantCulture, "{0}: {1}", NUnitRunnerLang.TestTime, selected.Time);
            Description.Text = (
                selected.Description + Environment.NewLine +
                selected.Output + Environment.NewLine +
                selected.Message + Environment.NewLine +
                selected.StackTrace).Trim();
        }

        public void SetItems(TestsResults r)
        {
            TestsStateSingleton.Clear();
            if (results.IsEmpty)
            {
                results.SetItems(r.Items);
            }
            Mt.Do(this, () => SelectedTestChanged(null, null));
            tmc = r.Metrics;
        }

        public void Clear()
        {
            results.SetItems(null);
            SelectedTestChanged(null, null);
        }

        public IList<Result> GetTests()
        {
            var items = results.GetChecked().Cast<Result>().ToArray();
            TestsStateSingleton.SetItems(items);
            return items;
        }

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            CheckableTreeView.OnMouseLeftButtonDown(sender, e);
        }
    }
}
