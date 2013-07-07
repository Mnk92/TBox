using System.Text;

namespace Mnk.TBox.Core.PluginsShared.Encoders
{
	public class CppCodeFormatter : BracketsFormatter
	{
		private readonly char lineDivider;
		public CppCodeFormatter(char begin = '{', char end = '}', char lineDivider = ';') : base(begin, end)
		{
			this.lineDivider = lineDivider;
		}

		protected override void AppendText(StringBuilder sb, string text)
		{
			text = text.Trim();
			if (string.IsNullOrWhiteSpace(text)) return;
			var pos = 0;
			while(true)
			{
				var last = text.FindFirstNotString(lineDivider, pos);
				if (last != -1)
				{
					var value = text.Substring(pos, last - pos + 1).Trim();
					if (value.Length == 1 && IsDivider(value[0]))
					{
						sb.Append(lineDivider);
					}
					else
					{
						base.AppendText(sb, value);
					}
					pos = last + 1;
					continue;
				}
				base.AppendText(sb, text.Substring(pos, text.Length - pos));
				break;
			}
		}

		private bool IsDivider(char ch)
		{
			return ch == lineDivider || ch == ',';
		}
	}
}
