using System;
using System.Text;
using Mnk.Library.Common;

namespace Mnk.TBox.Plugins.Encoder.Code
{
	static class FqlParser
	{
		private const char Begin = '(';
		private const char End = ')';
		private static readonly char[] Dividers = { Begin, End };

		public static string ParseSimple(string text)
		{
			var result = new StringBuilder();
			var ident = 0;
			var pos = 0;
			while (true)
			{
				var lastPos = pos;
				pos = text.IndexOfAny(Dividers, pos);
				if (pos == -1)
				{
					result.AppendIndent(ident);
					result.Append(text.Substring(lastPos, text.Length - lastPos));
					break;
				}
				result.AppendIndent(ident);
				result.Append(text.Substring(lastPos, pos - lastPos)).Append(text[pos]);
				ident = Math.Max(0, ident + ((text[pos] == Begin) ? +1 : -1));
				++pos;
			}
			return result.ToString();
		}

		public static string ParseWithSubItems(string text)
		{
			var result = new StringBuilder();
			var ident = 0;
			var pos = 0;
			var isBegin = false;
			while (true)
			{
				var lastPos = pos;
				pos = text.IndexOfAny(Dividers, pos);
				if (pos == -1)
				{
					result.AppendIndent(ident);
					result.Append(text.Substring(lastPos, text.Length - lastPos));
					break;
				}
				var lastBegin = isBegin;
				isBegin = (text[pos] == Begin);
				if (isBegin && lastBegin || !isBegin && !lastBegin)
				{
					result.AppendIndent(ident);
				}
				result.Append(text.Substring(lastPos, pos - lastPos)).Append(text[pos]);
				ident = Math.Max(0, ident + (isBegin ? +1 : -1));
				++pos;
			}
			return result.ToString();
		}
	}
}
