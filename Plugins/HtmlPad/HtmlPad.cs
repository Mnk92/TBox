using Mnk.TBox.Core.Contracts;
using Mnk.TBox.Locales.Localization.Plugins.HtmlPad;
using Mnk.Library.WpfSyntaxHighlighter;
using Mnk.TBox.Plugins.HtmlPad.Code.Settings;

namespace Mnk.TBox.Plugins.HtmlPad
{
    [PluginInfo(typeof(HtmlPadLang), 220, PluginGroup.Web)]
    public sealed class HtmlPad : SingleDialogPlugin<Config, Dialog>
    {
        public HtmlPad()
            : base(HtmlPadLang.Edit)
        {
        }

        public override void Init(IPluginContext context)
        {
            base.Init(context);
            context.AddTypeToWarmingUp(typeof(SyntaxHighlighter));
        }
    }
}
