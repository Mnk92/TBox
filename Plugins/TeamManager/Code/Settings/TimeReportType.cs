using System;

namespace Mnk.TBox.Plugins.TeamManager.Code.Settings
{
    [Flags]
    public enum TimeReportType
    {
        Personal = 0x1,
        Full = 0x2,
        Team = 0x4,
    }
}
