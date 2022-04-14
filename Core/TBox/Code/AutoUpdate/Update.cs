using System;
using Mnk.Library.Common.AutoUpdate;

namespace Mnk.TBox.Core.Application.Code.AutoUpdate
{
    [Serializable]
    public class Update
    {
        public UpdateInterval Interval { get; set; }
        public DateTime Last { get; set; }
        public bool ShowChanglog { get; set; }
        public long LastChanglogPosition { get; set; }

        public Update()
        {
            Interval = UpdateInterval.Startup;
            ShowChanglog = true;
            LastChanglogPosition = 0;
        }
    }
}
