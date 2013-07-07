using Mnk.TBox.Core.PluginsShared.LoadTesting;
using Mnk.TBox.Core.PluginsShared.LoadTesting.Statistic;
using Mnk.TBox.Plugins.Requestor.Code.Settings;

namespace Mnk.TBox.Plugins.Requestor.Code
{
	class LoadTester : LoadTester<Op>
	{
		protected override IOperationRunner CreateOperation(Op operation, StatisticsCollector collector)
		{
			return new RequestMaker(operation, operation.Delay, collector);
		}
	}
}
