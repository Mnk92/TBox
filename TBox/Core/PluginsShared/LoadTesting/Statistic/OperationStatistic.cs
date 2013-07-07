using System.Collections.Generic;

namespace Mnk.TBox.Core.PluginsShared.LoadTesting.Statistic
{
	public sealed class OperationStatistic
	{
		public string Key { get; set; }
		public IDictionary<string, int> Statutes { get; private set; }
		public long Time { get; set; }
		public long MinTime { get; set; }
		public long MaxTime { get; set; }
		public long Count { get; set; }

		public OperationStatistic()
		{
			Key = string.Empty;
			Statutes = new Dictionary<string, int>();
			Time = 0;
			MinTime = int.MaxValue;
			MaxTime = int.MinValue;
			Count = 0;
		}

		public OperationStatistic Clone()
		{
			return new OperationStatistic{
					Key = Key,
					Count = Count,
					Statutes = Statutes,
					MaxTime = MaxTime,
					MinTime = MinTime,
					Time = Time
				};
		}
	}
}
