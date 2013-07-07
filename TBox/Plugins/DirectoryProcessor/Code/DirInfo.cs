using System;
using Mnk.Library.Common.UI.Model;

namespace Mnk.TBox.Plugins.DirectoryProcessor.Code
{
	[Serializable]
	public class DirInfo : CheckableData
	{
		public string Executable { get; set; }
		public string ExtendedArguments { get; set; }
		public int Deep { get; set; }

		public DirInfo()
		{
			Deep = 0;
		}

		public override object Clone()
		{
			return new DirInfo
			{
				Key = Key,
				IsChecked = IsChecked,
				Executable = Executable,
				Deep = Deep,
				ExtendedArguments = ExtendedArguments,
			};
		}
	}
}
