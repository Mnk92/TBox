using System;

namespace Mnk.TBox.Core.PluginsShared.ReportsGenerator
{
    public interface IDayTypeProvider
    {
        bool IsHoliday(DateTime date);
    }
}
