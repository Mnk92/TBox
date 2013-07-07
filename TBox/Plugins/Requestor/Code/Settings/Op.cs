using System;
using Mnk.Library.Common.UI.Model;
using Mnk.TBox.Core.PluginsShared.LoadTesting;

namespace Mnk.TBox.Plugins.Requestor.Code.Settings
{
	[Serializable]
	public sealed class Op : CheckableData, IOperation
	{
		public int Threads { get; set; }
		public int Delay { get; set; }
		public int Timeout { get; set; }
		public RequestConfig Request { get; set; }

		public Op()
		{
			Delay = 0;
			Threads = 1;
			Timeout = 120;
			Request = new RequestConfig();
		}

		public override object Clone()
		{
			var p = new Op
			{
				Key = Key,
				IsChecked = IsChecked,
				Threads = Threads,
				Delay = Delay,
				Timeout = Timeout,
				Request = Request.Clone() as RequestConfig
			};
			return p;
		}
	}
}
