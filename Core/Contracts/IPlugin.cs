using System.Drawing;
using Mnk.Library.WpfControls.Menu;

namespace Mnk.TBox.Core.Contracts
{
    public interface IPlugin
    {
        void Init(IPluginContext context);
        UMenuItem[] Menu { get; }
        Icon Icon { get; set; }
        PluginGroup PluginGroup { get; set; }
    }
}
