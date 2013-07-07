using Mnk.Library.WpfSyntaxHighlighter;
using Mnk.TBox.Core.Contracts;
using Mnk.TBox.Locales.Localization.Plugins.XsltTester;
using Mnk.TBox.Plugins.XsltTester.Code.Settings;

namespace Mnk.TBox.Plugins.XsltTester
{
    [PluginInfo(typeof(XsltTesterLang), typeof(Properties.Resources), PluginGroup.Development)]
    public sealed class XsltTester : SingleDialogPlugin<Config, Dialog>
    {
        public XsltTester() : base(XsltTesterLang.Test)
        {
        }

        public override void Init(IPluginContext context)
        {
            base.Init(context);
            context.AddTypeToWarmingUp(typeof(SyntaxHighlighter));
        }
    }
}
