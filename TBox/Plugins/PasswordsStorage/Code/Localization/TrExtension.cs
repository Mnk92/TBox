using Mnk.TBox.Locales.Localization.Plugins.PasswordsStorage;

namespace Mnk.TBox.Plugins.PasswordsStorage.Code.Localization
{
    public class TrExtension : Mnk.Library.WpfControls.Localization.TranslateExtension
    {
        public TrExtension(string key) : base(key, PasswordsStorageLang.ResourceManager) { }
    }
}
