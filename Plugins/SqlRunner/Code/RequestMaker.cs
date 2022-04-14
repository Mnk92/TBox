using System.Threading;
using Mnk.TBox.Core.PluginsShared.LoadTesting;
using Mnk.TBox.Core.PluginsShared.LoadTesting.Statistic;
using Mnk.TBox.Plugins.SqlRunner.Code.Settings;

namespace Mnk.TBox.Plugins.SqlRunner.Code
{
    class RequestMaker : BaseExecutor, IOperationRunner
    {
        private readonly string connectionString;
        private readonly Op operation;
        private readonly int delay;
        private bool mustTerminateThread = false;
        private readonly StatisticsCollector collector;

        public RequestMaker(string connectionString, Op operation, int delay, StatisticsCollector collector)
        {
            this.connectionString = connectionString;
            this.operation = operation;
            this.delay = delay * 1000;
            this.collector = collector;
        }

        private DatabaseInfo GetResult()
        {
            return GetResult(connectionString, operation);
        }

        public void Work()
        {
            while (!mustTerminateThread)
            {
                collector.Push(GetResult());
                if (delay > 0) Thread.Sleep(delay);
            }
        }

        public void Terminate()
        {
            mustTerminateThread = true;
        }
    }
}
