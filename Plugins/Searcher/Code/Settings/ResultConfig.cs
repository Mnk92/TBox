using System;

namespace Mnk.TBox.Plugins.Searcher.Code.Settings
{
	[Serializable]
	public sealed class ResultConfig
	{
		public bool AutoLoadFile { get; set; }
		public string OpenWith { get; set; }

		public ResultConfig()
		{
			AutoLoadFile = true;
			OpenWith = "notepad.exe";
		}
	}
}
