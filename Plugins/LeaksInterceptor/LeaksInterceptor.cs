using Mnk.TBox.Core.Contracts;
using Mnk.TBox.Locales.Localization.Plugins.LeaksInterceptor;
using Mnk.TBox.Plugins.LeaksInterceptor.Components;

namespace Mnk.TBox.Plugins.LeaksInterceptor
{
    [PluginInfo(typeof(LeaksInterceptorLang), 165, PluginGroup.Desktop)]
    public sealed class LeaksInterceptor : SingleDialogConfigurablePlugin<Settings, Config, Dialog>, IDisposable
    {
        public LeaksInterceptor() : base(LeaksInterceptorLang.Analysis)
        {
        }

        protected override Dialog CreateDialog()
        {
            var dialog = base.CreateDialog();
            dialog.Init(Icon);
            return dialog;
        }
    }
}
