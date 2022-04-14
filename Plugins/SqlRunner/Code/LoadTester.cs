using Mnk.TBox.Core.PluginsShared.LoadTesting;
using Mnk.TBox.Core.PluginsShared.LoadTesting.Statistic;
using Mnk.TBox.Plugins.SqlRunner.Code.Settings;

namespace Mnk.TBox.Plugins.SqlRunner.Code
{
	class LoadTester : LoadTester<Op>
	{
		private Config config;
		public void OnConfigUpdated(Config cfg)
		{
			config = cfg;
		}

		protected override IOperationRunner CreateOperation(Op operation, StatisticsCollector collector)
		{
			return new RequestMaker(config.ConnectionString, operation, operation.Delay, collector);
		}
	}
}
