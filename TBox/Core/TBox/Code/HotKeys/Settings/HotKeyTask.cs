using System;
using Mnk.Library.Common.UI.Model;
using Mnk.Library.WpfWinForms.GlobalHotkeys;

namespace Mnk.TBox.Core.Application.Code.HotKeys.Settings
{
	[Serializable]
	public class HotkeyTask : CheckableData
	{
		public GlobalHotkey HotKey { get; set; }

		public override object Clone()
		{
			return new HotkeyTask
				{
					Key = Key,
					IsChecked = IsChecked,
					HotKey = (HotKey == null) ? 
								null : new GlobalHotkey(HotKey.Modifier, HotKey.Key)
				};
		}

		public override string ToString()
		{
			return Key.Replace(Environment.NewLine, "/");
		}
	}
}
