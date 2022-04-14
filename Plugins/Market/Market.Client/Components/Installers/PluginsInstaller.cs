using Mnk.TBox.Plugins.Market.Contracts;

namespace Mnk.TBox.Plugins.Market.Client.Components.Installers
{
    public class PluginsInstaller
    {
        public Plugin[] Items { get; set; }

        public string[] Names
        {
            get { return Items.Select(x => x.Name).ToArray(); }
        }
    }
}
