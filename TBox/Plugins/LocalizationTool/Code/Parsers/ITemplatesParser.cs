using System.Collections.Generic;

namespace Mnk.TBox.Plugins.LocalizationTool.Code.Parsers
{
	interface ITemplatesParser
	{
		string Save(string template, KeyValueParser parser, IDictionary<string, string> source, IDictionary<string, IDictionary<string, string>> values);
	}
}
