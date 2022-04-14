using Mnk.TBox.Locales.Localization.Plugins.FileWatcher;

namespace Mnk.TBox.Plugins.FileWatcher.Code.Localization
{
	public class TrExtension : Mnk.Library.WpfControls.Localization.TranslateExtension
	{
        public TrExtension(string key) : base(key, FileWatcherLang.ResourceManager) { }
	}
}
