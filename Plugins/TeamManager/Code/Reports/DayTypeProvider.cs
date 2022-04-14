using System;
using System.Collections.Generic;
using System.Linq;
using Mnk.Library.Common.Tools;
using Mnk.TBox.Core.PluginsShared.ReportsGenerator;
using Mnk.TBox.Plugins.TeamManager.Code.Settings;

namespace Mnk.TBox.Plugins.TeamManager.Code.Reports
{
    class DayTypeProvider : IDayTypeProvider
    {
        private readonly IList<SpecialDay> specialDays;

        public DayTypeProvider(IList<SpecialDay> specialDays)
        {
            this.specialDays = specialDays;
        }

        public bool IsHoliday(DateTime date)
        {
            var str = date.ToShortDateString();
            var special = specialDays.FirstOrDefault(x => x.Key.EqualsIgnoreCase(str));
            if (special != null) return special.IsHolyday;
            return date.DayOfWeek == DayOfWeek.Sunday || date.DayOfWeek == DayOfWeek.Saturday;
        }
    }
}
