using System;
using Mnk.Library.Common.UI.ModelsContainers;
using Mnk.TBox.Plugins.DirectoryProcessor.Code;

namespace Mnk.TBox.Plugins.DirectoryProcessor
{
	[Serializable]
	public class Config
	{
		public CheckableDataCollection<DirInfo> Directories { get; set; }

		public Config()
		{
			Directories = new CheckableDataCollection<DirInfo>
				{
					new DirInfo
						{
							IsChecked=false,
							Key = "c:\\data",
							Executable = "explorer.exe"
						}
				};
		}
	}
}
