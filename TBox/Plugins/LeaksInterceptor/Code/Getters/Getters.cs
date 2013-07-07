using System;
using System.Diagnostics;

namespace Mnk.TBox.Plugins.LeaksInterceptor.Code.Getters
{
	class BasePriority : IGetter
	{
		public float Get(Process p) { return p.BasePriority; }
	}
	class HandleCount : IGetter
	{
		public float Get(Process p) { return p.HandleCount; }
	}
	class ModulesCount : IGetter
	{
		public float Get(Process p) { return p.Modules.Count; }
	}
	class NonpagedSystemMemorySize : IGetter
	{
		public float Get(Process p) { return p.NonpagedSystemMemorySize64 / 1024f; }
	}
	class PagedMemorySize : IGetter
	{
		public float Get(Process p) { return p.PagedMemorySize64 / 1024f; }
	}
	class PagedSystemMemorySize : IGetter
	{
		public float Get(Process p) { return p.PagedSystemMemorySize64 / 1024f; }
	}
	class PeakVirtualMemorySize : IGetter
	{
		public float Get(Process p) { return p.PeakVirtualMemorySize64 / 1024f; }
	}
	class PeakWorkingSet : IGetter
	{
		public float Get(Process p) { return p.PeakWorkingSet64 / 1024f; }
	}
	class PrivateMemorySize : IGetter
	{
		public float Get(Process p) { return p.PrivateMemorySize64 / 1024f; }
	}
	class PrivilegedProcessorTime : DifferenceCalculatror
	{
		protected override double GetValue(Process p) { return p.PrivilegedProcessorTime.TotalMilliseconds; }
	}
	class Responding : IGetter
	{
		public float Get(Process p) { return p.Responding ? 1 : 0; }
	}
	class Threads : IGetter
	{
		public float Get(Process p) { return p.Threads.Count; }
	}
	class ProcessorTime : DifferenceCalculatror
	{
		protected override double GetValue(Process p){return p.TotalProcessorTime.TotalMilliseconds;}
	}
	class UserProcessorTime : DifferenceCalculatror
	{
		protected override double GetValue(Process p) { return p.UserProcessorTime.TotalMilliseconds; }
	}
	class VirtualMemorySize : IGetter
	{
		public float Get(Process p) { return p.VirtualMemorySize64 / 1024f; }
	}
	class WorkingSet : IGetter
	{
		public float Get(Process p) { return p.WorkingSet64 / 1024f; }
	}
	abstract class DifferenceCalculatror : IGetter
	{
		private double oldCpuUsage = -1;
		protected abstract double GetValue(Process p);
		public float Get(Process p)
		{
			var actual = GetValue(p);
			if (oldCpuUsage < 0)
			{
				oldCpuUsage = actual;
				return 0;
			}
			var value = (actual - oldCpuUsage) ;
			oldCpuUsage = actual;
			return Math.Min(100, (float)value / Environment.ProcessorCount / 10);
		}
	}
}
