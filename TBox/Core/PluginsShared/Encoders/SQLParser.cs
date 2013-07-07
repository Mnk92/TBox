using System;
using System.Collections.Generic;
using System.Text;
using Mnk.Library.Common.Models;

namespace Mnk.TBox.Core.PluginsShared.Encoders
{
	public class SqlParser
	{
		private static readonly string[] Keywords =
			new[]
				{
					"SELECT "," FROM ",
					" INNER JOIN "," LEFT JOIN "," RIGHT JOIN ",
					" LEFT OUTER JOIN "," RIGHT OUTER JOIN ",
					" WHERE ",
					" ORDER BY "," GROUP BY "
				};

		private const char ParamsDivider = ';';
		private const char ParamNameBegin = '@';

		private static string Prepare(string text)
		{
			var sb = new StringBuilder(text);
			for (var i = 0; i < sb.Length; ++i)
			{
				if (Char.IsWhiteSpace(sb[i]))
					sb[i] = ' ';
			}
			return sb.ToString();
		}

		private static Pair<int, string> FindKeyword(string text, int start)
		{
			var keyword = string.Empty;
			var length = -1;
			foreach (var t in Keywords)
			{
				var id = text.IndexOf(t, start, StringComparison.InvariantCultureIgnoreCase);
				if (id < 0) continue;
				if (length >= 0 && id >= length) continue;
				length = id;
				keyword = t;
			}
			return new Pair<int, string>(length, keyword);
		}

        private static IEnumerable<Pair<string, string>> DecodeParams(string text, bool removeTypeInfo)
		{
			var ret = new List<Pair<string, string>>();
			var start = 0;
			while (start < text.Length)
			{
				var id = text.IndexOf(ParamNameBegin, start);
				if (id < 0) break;
				var nextDivider = text.FindFirstNotString(',', start);
				if (nextDivider < 0) nextDivider = text.Length;
				var paramLine = text.Substring(start, nextDivider - start);
				var paramNameEnd = paramLine.IndexOf('=');
				if (paramNameEnd > 0)
				{
					ret.Add(new Pair<string, string>(
						paramLine.Substring(0, paramNameEnd).Trim(),
                        PrepareParam(paramLine.Substring(paramNameEnd + 1).Trim(), removeTypeInfo)
						));
					start = nextDivider + 1;
				}
				else break;
			}
			ret.Reverse();
			return ret;
		}

	    private static string PrepareParam(string value, bool removeTypeInfo)
	    {
	        if (removeTypeInfo)
	        {
                var id = value.LastIndexOf('[');
                if (id != -1 && value.LastIndexOf(']') > id)
                {
                    var type = value.Substring(id, value.Length - id);
                    value = value.Substring(0, id).Trim();
                    if (type.Contains("Type: Guid (0)"))
                    {
                        return "'" + value + "'";
                    }
                }
	        }
            return value;
	    }

	    public string Parse(string text, bool removeTypeInfo = true)
		{
			if (string.IsNullOrWhiteSpace(text)) return text;
			text = Prepare(text);
			var sb = new StringBuilder();
			var start = 0;
			while(true)
			{
				var pair = FindKeyword(text, start);
				if(pair.Key < 0 )break;
				var substring = text.Substring(start, pair.Key - start);
				if (substring.IndexOf(ParamsDivider) != -1) break;
				sb.Append(substring)
					.Append(Environment.NewLine)
					.Append(pair.Value);
				start = pair.Key + pair.Value.Length;
			}
			var line = text.Substring(start);
			var paramsStart = line.IndexOf(ParamsDivider);
			if(paramsStart == -1)
			{
				sb.Append(line);
			}
			else
			{
				sb.Append(line.Substring(0, paramsStart));
				foreach (var p in DecodeParams(line.Substring(paramsStart+1), removeTypeInfo))
				{
					sb.Replace(p.Key, p.Value);
				}
			}
			return sb.ToString();
		}
	}
}
