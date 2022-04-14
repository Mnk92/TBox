using System;
using Mnk.TBox.Core.Application.Code.HotKeys.Settings;

namespace Mnk.TBox.Core.Application.Code.HotKeys
{
	class HotkeyAction : HotkeyTask
	{
		public Action Action { get; set; }
	}
}
