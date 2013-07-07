using Mnk.TBox.Locales.Localization.Plugins.RegExpTester;

namespace Mnk.TBox.Plugins.RegExpTester.Code.Localization
{
    public class TrExtension : Library.WpfControls.Localization.TranslateExtension
    {
        public TrExtension(string key) : base(key, RegExpTesterLang.ResourceManager) { }
    }
}
