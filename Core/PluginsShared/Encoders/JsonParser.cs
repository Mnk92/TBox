namespace Mnk.TBox.Core.PluginsShared.Encoders
{
	public class JsonParser : CppCodeFormatter
	{
		public JsonParser(): base('{', '}', ',')
		{
		}
	}
}
