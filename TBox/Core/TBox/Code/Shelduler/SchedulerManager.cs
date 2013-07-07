using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Threading;
using Mnk.TBox.Core.Contracts;
using Mnk.TBox.Locales.Localization.TBox;
using Mnk.TBox.Core.Application.Code.Menu;
using Mnk.TBox.Core.Application.Code.Shelduler.Settings;
using Mnk.Library.WpfControls.Components.Units;
using Mnk.Library.WpfControls.Tools;

namespace Mnk.TBox.Core.Application.Code.Shelduler
{
	class SchedulerManager : IDisposable
	{
		private readonly CheckableListBoxUnit view;
		private readonly IMenuItemsProvider menuItemsProvider;
		private SchedulerTasks originalConfig;
		private SchedulerTasks config;
		private readonly DispatcherTimer timer = new DispatcherTimer();
		private readonly NonUserRunContext nonUserRunArg = new NonUserRunContext();
		private static readonly ShortDayOfWeek[] ShortDaysOfWeeks =
			Enum.GetValues(typeof(ShortDayOfWeek)).Cast<ShortDayOfWeek>().ToArray();

		public SchedulerManager(CheckableListBoxUnit view, IMenuItemsProvider menuItemsProvider)
		{
			this.view = view;
			this.menuItemsProvider = menuItemsProvider;
			timer.Tick += TimerOnTick;
			menuItemsProvider.OnRefresh += (o,e) => Refresh();
			menuItemsProvider.OnRefreshItem += RefreshItem;
		}

		public void Refresh()
		{
			Dispose();
			view.ConfigureInputMenuItem(TBoxLang.Schelduler, 
				originalConfig.Tasks, 
				menuItemsProvider.GetDialogItems(), 
				validator: x => true);
			if (!config.IsEnabled) return;
			foreach (var item in config.Tasks.CheckedItems.OrderBy(x => x.Key))
			{
				UpdateTask(item);
			}
			ProcessNext(DateTime.Now);
		}

		internal void RefreshItem(string name)
		{
			if (!config.IsEnabled)return;
			foreach (var item in config.Tasks.CheckedItems.OrderBy(x => x.Key).Where(x => x.Key.StartsWith(name, StringComparison.OrdinalIgnoreCase)))
			{
				UpdateTask(item);
			}
		}

		private void UpdateTask(SchedulerTask item)
		{
			item.OnClick = null;
			var menu = menuItemsProvider.Get(item.Key);
			if (menu == null || menu.Items.Count > 0 || menu.OnClick == null) return;
			item.OnClick = menu.OnClick;
		}

		private void ProcessNext(DateTime time)
		{
			RecalcNextTimes(time);
			RunNearestTasks();
		}

		private void RecalcNextTimes(DateTime time)
		{
			foreach (var item in config.Tasks.CheckedItems)
			{
				item.NextRun = FindNearestDate(
									(ShortDayOfWeek)item.DaysOfWeek, 
									new TimeSpan(item.TimeOfTheDay), 
									time);
			}
		}

		private static DateTime? FindNearestDate(ShortDayOfWeek daysOfWeek, TimeSpan time, DateTime currentDate)
		{
			if (daysOfWeek == 0) return null;
			var date = currentDate
				.AddMilliseconds(-currentDate.TimeOfDay.TotalMilliseconds)
				.AddSeconds(time.TotalSeconds);
			var delta = (int) date.DayOfWeek;
			for (var i = 0; i < 7; ++i)
			{
				var id = i + delta;
				if (id >= 7) id -= 7;
				if (daysOfWeek.HasFlag(ShortDaysOfWeeks[id]))
				{
					if(date >= currentDate)break;
				}
				date = date.AddDays(1);
			}
			return date;
		}

		private void RunNearestTasks()
		{
			var tasks = FindNearestTasks(config.Tasks.CheckedItems.Where(x=>x.NextRun!=null).ToArray());
			if(!tasks.Any())return;
			var context = new SchedulerRunContext
				{
					Tasks = tasks.ToArray(),
					StartTime = tasks.First().NextRun.Value
				};
			timer.Tag = context;
			var interval = context.StartTime - DateTime.Now;
			if (interval.Ticks <= 0)
			{
				TimerOnTick(null, null);
			}
			else
			{
				timer.Interval = interval;
				timer.Start();
			}
		}

		private static SchedulerTask[] FindNearestTasks(IList<SchedulerTask> tasks)
		{
			if (!tasks.Any() )
			{
				return new SchedulerTask[0];
			}
			var next = tasks.Min(x => x.NextRun);
			return tasks.Where(x => x.NextRun == next).ToArray();
		}

		private void TimerOnTick(object sender, EventArgs eventArgs)
		{
			timer.Stop();
			if (!config.IsEnabled) return;
			var context = (SchedulerRunContext) timer.Tag;
			foreach (var task in context.Tasks)
			{
				var a = task.OnClick;
				if (a != null) a(nonUserRunArg);
			}
			ProcessNext(context.StartTime.AddSeconds(1));
		}

		public void Dispose()
		{
			timer.Stop();
		}

		public void OnConfigUpdated(SchedulerTasks cfg)
		{
			originalConfig = cfg;
			config = cfg.Clone();
		}
	}
}
