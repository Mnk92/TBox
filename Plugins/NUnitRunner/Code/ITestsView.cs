using Mnk.ParallelTests.Contracts;

namespace Mnk.TBox.Plugins.NUnitRunner.Code
{
    public interface ITestsView
    {
        void SetItems(TestsResults results);
        void Clear();
    }
}
