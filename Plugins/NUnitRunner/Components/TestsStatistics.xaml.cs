using System.Windows.Controls;
using Mnk.ParallelTests.Contracts;
using Mnk.TBox.Plugins.NUnitRunner.Code;

namespace Mnk.TBox.Plugins.NUnitRunner.Components
{
    /// <summary>
    /// Interaction logic for TestsStatistics.xaml
    /// </summary>
    public partial class TestsStatistics : ITestsView
    {
        public TestsStatistics()
        {
            InitializeComponent();
        }

        public void SetItems(TestsResults results)
        {
            ErrorsAndFailures.ItemsSource = results.Metrics.Failed;
            foreach (var item in results.Metrics.NotRun)
            {
                var ti = new TreeViewItem { Header = item.FullName };
                ti.Items.Add(new TreeViewItem { Header = item.Message });
                TestsNotRun.Items.Add(ti);
            }
            TextOutput.Text = string.Join(string.Empty, results.Metrics.All.Select(x => x.Output));
        }

        public void Clear()
        {
            ErrorsAndFailures.ItemsSource = null;
            TestsNotRun.Items.Clear();
        }
    }
}
