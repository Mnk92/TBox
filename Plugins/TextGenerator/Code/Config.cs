using System;

namespace Mnk.TBox.Plugins.TextGenerator.Code
{
	[Serializable]
	public class Config
	{
		public string Fill { get; set; }
		public int TextLength { get; set; }

		public Config()
		{
			Fill = " ";
			TextLength = 64;
		}
	}
}
