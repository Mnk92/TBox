using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mnk.TBox.Plugins.LocalizationTool.Code.Parsers
{
	class TemplatesParser : ITemplatesParser
	{
		private const string BeginTemplate = "$({0}: ";
		private const string EndTemplate = ")$";
		private const string ElementId = "lang";
		private const string DefaultId = "default";
		public string Save(string template, KeyValueParser parser, IDictionary<string, string> source, IDictionary<string, IDictionary<string, string>> translations)
		{
			var result = new StringBuilder();
			foreach (var line in template.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
			{
				var search = Search(ElementId, line);
				if (!search.IsSuccess)
				{
					result.AppendLine(line);
					continue;
				}
				IDictionary<string, string> target;
				if (string.Equals(DefaultId, search.Value))
				{
					target = source;
				}
				else
				{
					target = translations.ContainsKey(search.Value) ? 
						translations[search.Value] : 
						new Dictionary<string, string>();
				}
				result.AppendLine(FillValues(parser, search.Ident, target));
			}
			return result.ToString();
		}

		private static SearchResult Search(string param, string text)
		{
			var search = string.Format(BeginTemplate, param);
			var start = text.IndexOf(search, StringComparison.InvariantCultureIgnoreCase);
			if (start != -1)
			{
				var ident = text.Substring(0, start);
				start += search.Length;
				var end = text.IndexOf(EndTemplate, start, StringComparison.InvariantCultureIgnoreCase);
				if (end != -1)
				{
					var value = text.Substring(start, end - start);
					return new SearchResult(value, search + value + EndTemplate, ident);
				}
			}
			return new SearchResult();
		}

		private static string FillValues(KeyValueParser parser, string ident, IDictionary<string, string> values)
		{
			return string.Join(Environment.NewLine, values
				.Select(value => ident + parser.Save(value.Key, value.Value)));
		}
	}
}
