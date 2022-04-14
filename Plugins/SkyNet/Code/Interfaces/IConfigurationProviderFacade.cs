using Mnk.TBox.Tools.SkyNet.Contracts;

namespace Mnk.TBox.Plugins.SkyNet.Code.Interfaces
{
    public interface IConfigurationProviderFacade
    {
        Task<AgentConfig> GetAgentConfig();
        Task SetAgentConfig(AgentConfig config);
        Task<ServerConfig> GetServerConfig();
        Task SetServerConfig(ServerConfig config);
    }
}
