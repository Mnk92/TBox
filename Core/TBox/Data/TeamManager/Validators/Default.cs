using System;
using System.Collections.Generic;
using System.Linq;
using Mnk.TBox.Core.PluginsShared.ReportsGenerator;

namespace TeamManager.Code.Reports
{
    public class SimpleDayStatusStrategy : IDayStatusStrategy
    {
        public string GetState(DateTime day, Dictionary<string, Dictionary<string, double>> values, int workHours, IDayTypeProvider dayTypeProvider)
        {
            var sums = values.Select(item => item.Value.Sum(x => x.Value)).ToArray();
            if (sums.Any(sum => NotEquals(sum, workHours)))
            {
                var first = sums.First();
                if (sums.Any(x => NotEquals(first, x))) return DayTypes.Error;
                return GetState(day, dayTypeProvider);
            }
            return values.Count == 0 ? GetState(day, dayTypeProvider) : DayTypes.Working;
        }

        private static bool NotEquals(double a, double b)
        {
            return Math.Abs(a-b) > 0.000001;
        }

        private static string GetState(DateTime day, IDayTypeProvider dayTypeProvider)
        {
            return dayTypeProvider.IsHoliday(day) ? DayTypes.Holyday : DayTypes.Error;
        }
    }
}
