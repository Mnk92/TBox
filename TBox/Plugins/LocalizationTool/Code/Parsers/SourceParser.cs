using System;
using System.Collections.Generic;
using System.Linq;

namespace Mnk.TBox.Plugins.LocalizationTool.Code.Parsers
{
	class SourceParser : ISourceParser
	{
		public IDictionary<string, string> Parse(KeyValueParser parser, string value)
		{
			var dict = new Dictionary<string, string>();
			foreach (var line in value.Split(Environment.NewLine.ToCharArray()).Where(x => !string.IsNullOrWhiteSpace(x)))
			{
				var pair = parser.Load(line);
				if (dict.ContainsKey(pair.Key))
				{
					throw new ArgumentException("You already have such item to translate with key: " + pair.Key);
				}
				dict.Add(pair.Key, pair.Value);
			}
			return dict;
		}

		public string Save(KeyValueParser parser, IDictionary<string, string> values)
		{
			return string.Join(Environment.NewLine,
				values.Select(x => parser.Save(x.Key, x.Value)));
		}
	}
}
