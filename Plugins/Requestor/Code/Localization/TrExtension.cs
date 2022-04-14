using Mnk.TBox.Locales.Localization.Plugins.Requestor;

namespace Mnk.TBox.Plugins.Requester.Code.Localization
{
	public class TrExtension : Mnk.Library.WpfControls.Localization.TranslateExtension
	{
        public TrExtension(string key) : base(key, RequestorLang.ResourceManager) { }
	}
}
