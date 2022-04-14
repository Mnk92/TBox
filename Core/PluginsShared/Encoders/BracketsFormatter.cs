using System.Collections.Generic;
using System.Text;
using Mnk.Library.Common;

namespace Mnk.TBox.Core.PluginsShared.Encoders
{
	public class BracketsFormatter
	{
		private readonly char begin;
		private readonly char end;
		public BracketsFormatter(char begin = '{', char end = '}')
		{
			this.begin = begin;
			this.end = end;
		}

		public string Format(string text)
		{
			var sb = new StringBuilder();
			var pos = 0;
			while (true)
			{
				var start = text.FindFirstNotString(begin, pos);
				if (start != -1)
				{
					var last = text.FindFirstNotString(end, start + 1, new KeyValuePair<char, char>(begin, end));
					if (last != -1)
					{
						var block = text.Substring(start + 1, last - start - 1);
						AppendText(sb, text.Substring(pos, start - pos));
						sb.AppendIndent(0).Append(begin);
						sb.AppendIndentedText(Format(block), 1);
						sb.AppendIndent(0).Append(end);
						pos = last + 1;
						continue;
					}
				}
				AppendText(sb, text.Substring(pos, text.Length - pos));
				break;
			}
			return sb.ToString();
		}

		protected virtual void AppendText(StringBuilder sb, string text)
		{
			text = text.Trim();
			if(string.IsNullOrWhiteSpace(text))return;
			sb.AppendIndent(0).Append(text);
		}
	}
}
