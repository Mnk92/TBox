using Mnk.TBox.Tools.SkyNet.Common;

namespace Mnk.TBox.Plugins.SkyNet.Code.Interfaces
{
    public interface IConfigsFacade
    {
        AgentConfig AgentConfig { get; set; }
        ServerConfig ServerConfig { get; set; }
    }
}
