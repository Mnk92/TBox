﻿using Mnk.TBox.Tools.SkyNet.Contracts;

namespace Mnk.TBox.Plugins.SkyNet.Code.Interfaces
{
    public class SkyNetStatus
    {
        public AgentTask Task { get; set; }
        public IList<ServerAgent> Agents { get; set; }
        public IList<ServerTask> Tasks { get; set; }

    }
}
