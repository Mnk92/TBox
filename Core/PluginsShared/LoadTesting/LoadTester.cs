using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Mnk.TBox.Core.PluginsShared.LoadTesting.Statistic;

namespace Mnk.TBox.Core.PluginsShared.LoadTesting
{
	public abstract class LoadTester<T> : ILoadTester
		where T : IOperation
	{
		private readonly object locker = new object();
		private IList<IOperationRunner> requests;
		private Thread thread;

		public bool IsWorks 
		{
			get
			{
				return thread!=null && thread.IsAlive;
			}
		}

		public void Start(IEnumerable<IOperation> operations, Analyzer analyzer)
		{
			lock (locker)
			{
				var items = operations.Cast<T>().ToArray();
				var threadsCount = items.Sum(item => item.Threads);
				if (threadsCount == 0)
				{
					return;
				}
				var collectors = new List<StatisticsCollector>();
				var threads = CreateRequests(threadsCount, items, collectors);
				thread = new Thread(() => Work(threads, analyzer, collectors));
				thread.Start();
			}
		}

		private static void Work(Thread[] threads, Analyzer analyzer, IList<StatisticsCollector> collectors)
		{
			do
			{
				Thread.Sleep(1000);
				Analize(analyzer, collectors);
			} while (CheckAnyThreadRun(threads));
			Analize(analyzer, collectors);
		}

		private static void Analize(Analyzer analyzer, IEnumerable<StatisticsCollector> collectors)
		{
			analyzer.Calc(collectors.ToDictionary(x=>x.Key, x=>x.Pop()));
		}

		private static bool CheckAnyThreadRun(IEnumerable<Thread> threads)
		{
			return threads.Any(t => t != null && t.ThreadState != ThreadState.Stopped);
		}

		private Thread[] CreateRequests(int threadsCount, IEnumerable<T> operations, IList<StatisticsCollector> collectors)
		{
			requests = new IOperationRunner[threadsCount];
			var threads = new Thread[threadsCount];
			var reqNo = 0;
			foreach (var item in operations)
			{
				var collector = new StatisticsCollector(item.Key);
				collectors.Add(collector);
				for (var i = 0; i < item.Threads; i++)
				{
					var r = requests[reqNo] = CreateOperation(item, collector);
					threads[reqNo] = new Thread(r.Work);
					threads[reqNo].Start();
					++reqNo;
				}
			}
			return threads;
		}

		protected abstract IOperationRunner CreateOperation(T operation, StatisticsCollector collector);

		private void Terminate()
		{
			foreach (var r in requests.Where(r => r != null))
			{
				r.Terminate();
			}
		}

		public void Stop()
		{
			lock (locker)
			{
				if ( thread == null || !thread.IsAlive) return;
				Terminate();
				while (thread.IsAlive) Thread.Sleep(100);
			}
		}
	}
}
