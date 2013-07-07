using Mnk.TBox.Locales.Localization.Plugins.HtmlPad;

namespace Mnk.TBox.Plugins.HtmlPad.Code.Localization
{
    public class TrExtension : Mnk.Library.WpfControls.Localization.TranslateExtension
    {
        public TrExtension(string key) : base(key, HtmlPadLang.ResourceManager) { }
    }
}
