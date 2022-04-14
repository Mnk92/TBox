using Mnk.TBox.Locales.Localization.Plugins.TextGenerator;

namespace Mnk.TBox.Plugins.TextGenerator.Code.Localization
{
	public class TrExtension : Mnk.Library.WpfControls.Localization.TranslateExtension
	{
        public TrExtension(string key) : base(key, TextGeneratorLang.ResourceManager) { }
	}
}
