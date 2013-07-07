using System;
using System.Collections.Generic;
using System.Linq;
using Mnk.TBox.Core.PluginsShared.ReportsGenerator;
using Mnk.TBox.Plugins.TeamManager.Code.Scripts;

namespace Mnk.TBox.Plugins.TeamManager.Code
{
    class FullReport
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public IList<LoggedInfo> Items { get; set; }
        public IList<string> Emails { get; set; }
        public int Time { get; set; }
        public IList<KeyValuePair<string, ColumnInfo>> Columns { get; set; }

        public FullReport Clone()
        {
            return new FullReport
                {
                    From = From,
                    To = To,
                    Time = Time,
                    Emails = Emails.ToList(),
                    Columns = Columns.Select(o => new KeyValuePair<string, ColumnInfo>(o.Key, o.Value)).ToArray(),
                    Items = Items.Select(x => x.Clone()).ToList()
                };
        }
    }
}
