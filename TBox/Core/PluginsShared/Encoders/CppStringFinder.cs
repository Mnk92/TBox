using System.Collections.Generic;

namespace Mnk.TBox.Core.PluginsShared.Encoders
{
    public static class CppStringFinder
    {
		class FindContext
		{
			public int Pos;
			public bool SingleQuote;
			public bool DoubleQuote;

			public FindContext(int start)
			{
				Pos = start;
				SingleQuote = false;
				DoubleQuote = false;
			}
		}
		public static int FindFirstNotString(this string text, char target, int start)
		{
			var ctx = new FindContext(start);
			for (; ctx.Pos < text.Length; ++ctx.Pos)
			{
				char ch;
				if (!SkipQuotesAndComments(text, out ch, ctx)) continue;
				if (ch == target) return ctx.Pos;
			}
			return -1;
		}


		public static int FindFirstNotString(this string text, char target, int start, KeyValuePair<char, char> extraCharsToSkip)
		{
			var ctx = new FindContext(start);
			var extraChars = -1;
			for (; ctx.Pos < text.Length; ++ctx.Pos)
			{
				char ch;
				if (!SkipQuotesAndComments(text, out ch, ctx)) continue;
				if (ch == extraCharsToSkip.Key)
				{
					++extraChars;
				}
				else if (extraChars != -1 && ch == extraCharsToSkip.Value)
				{
					--extraChars;
				}
				else if (extraChars == -1 && ch == target) return ctx.Pos;
			}
			return -1;
		}

		private static bool SkipQuotesAndComments(string text, out char ch, FindContext ctx)
		{
			ch = text[ctx.Pos];
			if ((ctx.SingleQuote || ctx.DoubleQuote) && ch == '\\')
			{
				if (ctx.Pos + 2 < text.Length)
				{
					ctx.Pos += 2;
					return SkipQuotesAndComments(text, out ch, ctx);
				}
				ctx.Pos++;
				return false;
			}
			if (ch == '\'' && !ctx.DoubleQuote)
			{
				ctx.SingleQuote = !ctx.SingleQuote;
			}
			else if (ch == '\"' && !ctx.SingleQuote)
			{
				ctx.DoubleQuote = !ctx.DoubleQuote;
			}
			else if (!ctx.SingleQuote && !ctx.DoubleQuote)
			{
				if (ch == '/')
				{
					if (ctx.Pos + 1 < text.Length)
					{
						++ctx.Pos;
						ch = text[ctx.Pos];
						if (ch == '/')
						{
							return GoToSymbol("\n", text, ref ch, ctx);
						}
						if (ch == '*')
						{
							return GoToSymbol("*/", text, ref ch, ctx);
						}
					}
					return false;
				}
				return true;
			}
			return false;
		}
	    private static bool GoToSymbol(string target, string text, ref char ch, FindContext ctx)
	    {
			var id = text.IndexOf(target, ctx.Pos+1, System.StringComparison.Ordinal);
		    if (id != -1)
		    {
				ctx.Pos = id + target.Length;
				return (ctx.Pos < text.Length) &&
			           SkipQuotesAndComments(text, out ch, ctx);
		    }
			ctx.Pos = text.Length;
		    return false;
	    }
    }
}
