using System;
using System.Drawing;

namespace Mnk.TBox.Core.Application.Code.FastStart.Settings
{
	[Serializable]
	public class MenuItemStatistic
	{
		public string Path { get; set; }
		public int Count { get; set; }
		internal Icon Icon { get; set; }
		internal Action<object> OnClick { get; set; }
		internal bool IsValid { get; set; }

		public MenuItemStatistic Clone()
		{
			return new MenuItemStatistic
				{
					Count = Count,
					Icon = Icon,
					OnClick = OnClick,
					Path = Path,
					IsValid = IsValid
				};
		}
	}
}
