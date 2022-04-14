using System;
using Mnk.Library.Common.UI.ModelsContainers;

namespace Mnk.TBox.Plugins.ProjectMan.Code.Settings
{
	[Serializable]
	public class Config
	{
		public string PathToMsBuild { get; set; }
		public string PathToSvn { get; set; }
		public string SvnUserName { get; set; }
		public CheckableDataCollection<ProjectInfo> Dirs { get; set; }

		public Config()
		{
			PathToMsBuild = @"C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe";
			PathToSvn = @"C:\Program Files\TortoiseSVN\bin\TortoiseProc.exe";
			SvnUserName = "user name";
			Dirs = new CheckableDataCollection<ProjectInfo>
				{
					new ProjectInfo
						{
							Key = "c:\\projects\\sampleProject",
							IsChecked = false
						}
				};
		}
	}
}
