using Mnk.TBox.Locales.Localization.Plugins.XsltTester;

namespace Mnk.TBox.Plugins.XsltTester.Code.Localization
{
    public class TrExtension : Mnk.Library.WpfControls.Localization.TranslateExtension
    {
        public TrExtension(string key) : base(key, XsltTesterLang.ResourceManager) { }
    }
}
