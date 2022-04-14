using Mnk.TBox.Tools.SkyNet.Server.Code.Interfaces;
using Mnk.TBox.Tools.SkyNet.Server.Code.Settings;

namespace Mnk.TBox.Tools.SkyNet.Server.Code
{
    class ServerContext : IServerContext
    {
        public ServerContext()
        {
            Config = new Config();
        }

        public Config Config { get; private set; }
    }
}
