using System;
using Mnk.TBox.Core.Application.Code.Shelduler.Settings;

namespace Mnk.TBox.Core.Application.Code.Shelduler
{
	class SchedulerRunContext
	{
		public DateTime StartTime { get; set; }
		public SchedulerTask[] Tasks { get; set; }
	}
}
