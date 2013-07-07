using System.Xml.Linq;

namespace Mnk.TBox.Core.PluginsShared.Encoders
{
	public static class XmlHelper
	{
		public static string Format(string text)
		{
			return XElement.Parse(text).ToString();
		}
	}
}
