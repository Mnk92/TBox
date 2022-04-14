using System;
using Mnk.Library.Common.Tools;
using Mnk.Library.Common.UI.Model;
using Mnk.Library.Common.UI.ModelsContainers;

namespace Mnk.TBox.Plugins.AvailabilityChecker.Code
{
    [Serializable]
    public class Config
    {
        public int CheckInterval { get; set; }
        public bool Started { get; set; }
        public CheckableDataCollection<CheckableData> Items { get; set; }

        public Config()
        {
            CheckInterval = 60;
            Started = false;
            (Items = new CheckableDataCollection<CheckableData>())
                .FillCollection("https://github.com/Mnk92/TBox");
        }
    }
}
