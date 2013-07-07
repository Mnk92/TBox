using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Mnk.Library.Common.Log;
using Mnk.Library.WpfControls.Components.Drawings.Graphics;
using Mnk.TBox.Plugins.LeaksInterceptor.Code.Getters;

namespace Mnk.TBox.Plugins.LeaksInterceptor.Code.Standart
{
	class ProcessAnalizer : IAnalizer
	{
		private readonly IDictionary<string, CounterInfo> counters;
		private static readonly ILog Log = LogManager.GetLogger<ProcessAnalizer>();
		private readonly GettersFactory factory;
		private readonly int targetPid;

		public ProcessAnalizer(int targetPid, IEnumerable<string> counterNames, GettersFactory factory)
		{
			this.factory = factory;
			this.targetPid = targetPid;
			counters = new Dictionary<string, CounterInfo>();
			foreach (var name in counterNames)
			{
				counters[name] = new CounterInfo(factory.Get(name));
			}
		}

		public bool Analize()
		{
			var p = Process.GetProcesses().FirstOrDefault(clsProcess => clsProcess.Id == targetPid);
			if (p == null)
			{
				return false;
			}
			var list = counters;
			if (list == null) return true;
			foreach (var info in list)
			{
				try
				{
					info.Value.Graphic.Add(info.Value.Getter.Get(p));
				}
				catch (Win32Exception ex)
				{
					Log.Write(ex, "Can't get process information. Maybe you need to have admin rights.");
					return false;
				}
			}
			return true;
		}

		public string CopyResults()
		{
			var list = counters;
			if (list == null) return string.Empty;
			var sb = new StringBuilder();
			foreach (var val in list)
			{
				sb.Append(val.Key).Append('\t');
				foreach (var x in val.Value.Graphic.Values)
				{
					sb.Append(x).Append('\t');
				}
				sb.AppendLine();
			}
			return sb.ToString();
		}

		public IGraphic GetGraphic(string name)
		{
			return counters.ContainsKey(name) ? counters[name].Graphic : null;
		}

		public IEnumerable<string> GetNames()
		{
			return counters.Keys;
		}
	}
}
