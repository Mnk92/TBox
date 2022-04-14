using System;
using Mnk.Library.Common.UI.ModelsContainers;

namespace Mnk.TBox.Core.PluginsShared.Watcher
{
	[Serializable]
	public sealed class WatchSettings
	{
		public CheckableDataCollection<DirInfo> Files { get; set; }
		public WatchSettings()
		{
			Files = new CheckableDataCollection<DirInfo>();
		}
	}
}
