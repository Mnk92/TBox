using System;
using System.Linq;
using System.Text;
using Mnk.Library.Common;
using Mnk.Library.Common.Tools;

namespace Mnk.TBox.Core.PluginsShared.Encoders
{
	public class HtmlParser
	{
		private const string Begin = "<";
		private const string Slash = "/";
		private const string End = ">";
		private const string CommentBegin = "<!--";
		private const string CommentEnd = "-->";

		public string Parse(string text)
		{
			var sb = new StringBuilder();
			var pos = 0;
			var ident = 0;
			do
			{
				var begin = text.IndexOf(Begin, pos, StringComparison.Ordinal);
				if (begin < 0) break;

				AppendText(text, pos, begin, sb, ident);

				bool isComment;
				var end = FindEnd(text, begin, out isComment);
				if (end < 0) break;

				pos = AppendHtmlElement(text, end, begin, sb, isComment, ref ident);
			} while (true);
			return sb.ToString();
		}

		private static int FindEnd(string text, int begin, out bool isComment)
		{
			isComment = false;
			if (Contain(text, begin, CommentBegin))
			{
				var end = text.IndexOf(CommentEnd, begin, StringComparison.Ordinal);
				if (end < 0) return -1;
				isComment = true;
				return end + CommentEnd.Length - 1;
			}
			return text.IndexOf(End, begin, StringComparison.Ordinal);
		}

		private static bool Contain(string text, int pos, string value)
		{
			return	pos > 0 &&
					pos + value.Length < text.Length &&
					text.Substring(pos, value.Length)
						.EqualsIgnoreCase(value);
		}

		private static int AppendHtmlElement(string text, int end, int begin, StringBuilder sb, bool isComment, ref int ident)
		{
			end++;
			var newIdent = isComment ? ident : CalcIdent(text, begin, end, ident);
			if (newIdent < ident) ident = newIdent;

			sb.AppendIndent(ident)
				.Append(text.Substring(begin, end - begin));

			ident = newIdent;
			return end;
		}

		private static void AppendText(string text, int pos, int begin, StringBuilder sb, int ident)
		{
			var str = text.Substring(pos, begin - pos);
			foreach (var line in str
				.Split(new[]{Environment.NewLine},StringSplitOptions.None)
				.Where(line => !string.IsNullOrWhiteSpace(line)))
			{
				sb.AppendIndent(ident).Append(line.Trim());
			}
		}

		private static int CalcIdent(string text, int begin, int end, int ident)
		{
			var beginEnd = Contain(text, begin + 1, Slash);
			var endEnd = Contain(text, end - 2, Slash);
			if (!beginEnd && !endEnd) return ident + 1;
			if (beginEnd) return Math.Max(0, ident - 1);
			return ident;
		}
	}
}
