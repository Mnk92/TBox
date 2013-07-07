using Mnk.Library.Common.MT;
using Mnk.Library.ParallelNUnit;
using Mnk.Library.ParallelNUnit.Contracts;
using Mnk.Library.ParallelNUnit.Core;

namespace Mnk.TBox.Plugins.NUnitRunner.Code
{
    class ExtendedGroupUpdater : GroupUpdater
    {
        public ExtendedGroupUpdater(IUpdater updater, int totalCount) : base(updater, totalCount)
        {
        }

        protected override void ProcessResults(int allCount, Result[] items, ISynchronizer synchronizer, ITestsConfig config)
        {
            base.ProcessResults(allCount, items, synchronizer, config);
            TestsStateSingleton.SetFinished(items);
        }
    }
}
