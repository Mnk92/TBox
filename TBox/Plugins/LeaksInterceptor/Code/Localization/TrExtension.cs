using Mnk.TBox.Locales.Localization.Plugins.LeaksInterceptor;

namespace Mnk.TBox.Plugins.LeaksInterceptor.Code.Localization
{
	public class TrExtension : Mnk.Library.WpfControls.Localization.TranslateExtension
	{
        public TrExtension(string key) : base(key, LeaksInterceptorLang.ResourceManager) { }
	}
}
