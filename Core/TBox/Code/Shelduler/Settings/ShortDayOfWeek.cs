using System;

namespace Mnk.TBox.Core.Application.Code.Shelduler.Settings
{
	[Flags]
	[Serializable]
	public enum ShortDayOfWeek
	{
		Su = 0x1,
		Mo = 0x2,
		Tu = 0x4,
		We = 0x8,
		Th = 0x10,
		Fr = 0x20,
		Sa = 0x40
	}
}
