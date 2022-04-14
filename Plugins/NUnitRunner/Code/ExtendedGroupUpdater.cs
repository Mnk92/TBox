using Mnk.Library.Common.MT;
using Mnk.ParallelTests.Common;
using Mnk.ParallelTests.Contracts;
using Mnk.TBox.Plugins.NUnitRunner.Components;

namespace Mnk.TBox.Plugins.NUnitRunner.Code
{
    class ExtendedGroupUpdater : GroupUpdater
    {
        public ExtendedGroupUpdater(IUpdater updater, int totalCount) : base(updater, totalCount)
        {
        }

        protected override void ProcessResults(int allCount, ResultMessage[] items, ISynchronizer synchronizer, TestsConfig config)
        {
            base.ProcessResults(allCount, items, synchronizer, config);
            TestsStateSingleton.SetFinished(items.Select(x => new Result(x)).ToArray());
        }
    }
}
