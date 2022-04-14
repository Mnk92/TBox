using Mnk.TBox.Plugins.Market.Contracts;

namespace Mnk.TBox.Plugins.Market.Client
{
    [Serializable]
    public class Config
    {
        public sealed class ClientInfo
        {
            public string EndPoint = "http://localhost:8080/MarketService";
            public int ItemsPerPage = 50;
        }
        public ClientInfo Client = new();

        public sealed class History
        {
            public struct Info
            {
                public string Name;
                public string Author;
                public DateTime Date;
                public bool Installed;
            }
            public List<Info> Items = new();
            public int MaxItemsInHistory = 50;
        }
        public History HistoryConfig = new();

        public sealed class InstalledPluginsConfig
        {
            public List<Plugin> Plugins = new();
        }
        public InstalledPluginsConfig InstalledPlugins = new();
    }
}
