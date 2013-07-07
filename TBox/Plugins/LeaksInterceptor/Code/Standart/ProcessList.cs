using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Mnk.Library.Common.Tools;

namespace Mnk.TBox.Plugins.LeaksInterceptor.Code.Standart
{
	class ProcessList
	{
		public ObservableCollection<ProcessInfo> Processes { get; private set; }

		public ProcessList()
		{
			Processes = new ObservableCollection<ProcessInfo>();
		}

		public void RefreshProcesses()
		{
			var processes = Process.GetProcesses();
			foreach (var p in Processes.Where(x => processes.All(y => y.Id != x.Pid)).ToArray())
			{
				Processes.Remove(p);
			}
			foreach (var p in processes.Where(x => Processes.All(y => y.Pid != x.Id)))
			{
				AddProcess(p);
			}
		}

		private void AddProcess(Process p)
		{
			Processes.Insert(new ProcessInfo { Pid = p.Id, Name = p.ProcessName }, x => x.Name);
		}

		public ProcessInfo Get(int index)
		{
			return Processes[index];
		}
	}
}
