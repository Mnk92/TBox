using System.Collections.Generic;
using Mnk.TBox.Core.PluginsShared.LoadTesting.Statistic;

namespace Mnk.TBox.Core.PluginsShared.LoadTesting
{
	public interface ILoadTester
	{
		void Start(IEnumerable<IOperation> operations, Analyzer analyzer);
		void Stop();
		bool IsWorks { get; }
	}
}
