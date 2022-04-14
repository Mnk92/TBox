using Mnk.TBox.Locales.Localization.PluginsShared;

namespace Mnk.TBox.Core.PluginsShared.Code.Localization
{
	public class TrExtension : Mnk.Library.WpfControls.Localization.TranslateExtension
	{
        public TrExtension(string key) : base(key, PluginsSharedLang.ResourceManager) { }
	}
}
