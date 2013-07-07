using System;
using Mnk.Library.Common.Tools;
using Mnk.Library.Common.UI.ModelsContainers;

namespace Mnk.TBox.Core.Application.Code.Shelduler.Settings
{
	[Serializable]
	public class SchedulerTasks
	{
		public bool IsEnabled { get; set; }

		public CheckableDataCollection<SchedulerTask> Tasks { get; set; }

		public SchedulerTasks()
		{
			IsEnabled = false;
			Tasks = new CheckableDataCollection<SchedulerTask>();
		}

		public SchedulerTasks Clone()
		{
			return new SchedulerTasks
				{
					IsEnabled = IsEnabled,
					Tasks = Tasks.Clone()
				};
		}
	}
}
