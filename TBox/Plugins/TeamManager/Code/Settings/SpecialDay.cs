using System;
using Mnk.Library.Common.UI.Model;

namespace Mnk.TBox.Plugins.TeamManager.Code.Settings
{
    public class SpecialDay : Data
    {
        public bool IsHolyday { get; set; }

        public SpecialDay()
        {
            IsHolyday = true;
        }

        public override object Clone()
        {
            return new SpecialDay
                {
                    Key = Key,
                    IsHolyday = IsHolyday
                };
        }

        public override string ToString()
        {
            return DateTime.Parse(Key).ToString("yyyyMMdd");
        }
    }
}
