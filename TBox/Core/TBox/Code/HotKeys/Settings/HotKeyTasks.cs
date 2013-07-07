using System;
using Mnk.Library.Common.Tools;
using Mnk.Library.Common.UI.ModelsContainers;

namespace Mnk.TBox.Core.Application.Code.HotKeys.Settings
{
	[Serializable]
	public class HotkeyTasks
	{
		public bool IsEnabled { get; set; }
		public CheckableDataCollection<HotkeyTask> Tasks { get; set; }

		public HotkeyTasks()
		{
			IsEnabled = false;
			Tasks = new CheckableDataCollection<HotkeyTask>();
		}

		public HotkeyTasks Clone()
		{
			return new HotkeyTasks
				{
					IsEnabled = IsEnabled,
					Tasks = Tasks.Clone()
				};
		}
	}
}
