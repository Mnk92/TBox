using Mnk.TBox.Core.Contracts;
using Mnk.TBox.Locales.Localization.Plugins.RegExpTester;
using Mnk.Library.WpfSyntaxHighlighter;
using Mnk.TBox.Plugins.RegExpTester.Code.Settings;

namespace Mnk.TBox.Plugins.RegExpTester
{
    [PluginInfo(typeof(RegExpTesterLang), typeof(Properties.Resources), PluginGroup.Development)]
    public sealed class RegExpTester : SingleDialogPlugin<Config, Dialog>
    {
        public RegExpTester()
            : base(RegExpTesterLang.Test)
        {
        }

        public override void Init(IPluginContext context)
        {
            base.Init(context);
            context.AddTypeToWarmingUp(typeof(SyntaxHighlighter));
        }
    }
}
