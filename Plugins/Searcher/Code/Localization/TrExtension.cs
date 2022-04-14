using Mnk.TBox.Locales.Localization.Plugins.Searcher;

namespace Mnk.TBox.Plugins.Searcher.Code.Localization
{
	public class TrExtension : Mnk.Library.WpfControls.Localization.TranslateExtension
	{
        public TrExtension(string key) : base(key, SearcherLang.ResourceManager) { }
	}
}
