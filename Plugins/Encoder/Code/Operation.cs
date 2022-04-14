using System;

namespace Mnk.TBox.Plugins.Encoder.Code
{
	public class Operation
	{
		public string Header { get; set; }
		public string Format { get; set; }
		public Func<string, string> Work { get; set; }
		public override string ToString()
		{
			return Header;
		}
	}
}
