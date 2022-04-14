using Mnk.TBox.Locales.Localization.Plugins.AppConfigManager;

namespace Mnk.TBox.Plugins.AppConfigManager.Code.Localization
{
	public class TrExtension : Mnk.Library.WpfControls.Localization.TranslateExtension
	{
        public TrExtension(string key) : base(key, AppConfigManagerLang.ResourceManager) { }
	}
}
