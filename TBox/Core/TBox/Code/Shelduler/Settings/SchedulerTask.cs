using System;
using Mnk.Library.Common.UI.Model;

namespace Mnk.TBox.Core.Application.Code.Shelduler.Settings
{
	public class SchedulerTask : CheckableData
	{
		public long TimeOfTheDay { get; set; }
		public int DaysOfWeek { get; set; }

		internal Action<object> OnClick { get; set; }
		internal DateTime? NextRun { get; set; }

		public SchedulerTask()
		{
			DaysOfWeek = (int)ShortDayOfWeek.Mo;
			var time = DateTime.Now.TimeOfDay;
			TimeOfTheDay = new TimeSpan(time.Hours, time.Minutes, 0).Ticks;
		}

		public override object Clone()
		{
			return new SchedulerTask
				{
					Key = Key, 
					IsChecked = IsChecked, 
					DaysOfWeek = DaysOfWeek, 
					TimeOfTheDay = TimeOfTheDay,
				};
		}

		public override string ToString()
		{
			return Key.Replace(Environment.NewLine, "/");
		}
	}
}
