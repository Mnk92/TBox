using Mnk.TBox.Core.Contracts;
using Mnk.TBox.Locales.Localization.Plugins.SourcesUniter;
using Mnk.TBox.Plugins.SourcesUniter.Code;

namespace Mnk.TBox.Plugins.SourcesUniter
{
    [PluginInfo(typeof(SourcesUniterLang), 68, PluginGroup.Other)]
    public sealed class SourcesUniter : SingleDialogPlugin<Config, Dialog>
    {
        public SourcesUniter() : base(SourcesUniterLang.Unite)
        {
        }
    }
}
