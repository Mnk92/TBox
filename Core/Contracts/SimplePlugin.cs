using System.Drawing;
using Mnk.Library.WpfControls.Menu;

namespace Mnk.TBox.Core.Contracts
{
    public abstract class SimplePlugin : IPlugin
    {
        public IPluginContext Context { get; private set; }
        public UMenuItem[] Menu { get; protected set; }
        public Icon Icon { get; set; }
        public PluginGroup PluginGroup { get; set; }

        public virtual void Init(IPluginContext context)
        {
            Context = context;
        }
    }
}
