using System.Collections.Generic;

namespace Mnk.TBox.Plugins.LocalizationTool.Code.Parsers
{
	interface ISourceParser
	{
		IDictionary<string, string> Parse(KeyValueParser parser, string value);
		string Save(KeyValueParser parser, IDictionary<string, string> values);
	}
}
