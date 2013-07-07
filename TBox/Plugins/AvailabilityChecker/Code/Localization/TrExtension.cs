using Mnk.TBox.Locales.Localization.Plugins.AvailabilityChecker;

namespace Mnk.TBox.Plugins.AvailabilityChecker.Code.Localization
{
	public class TrExtension : Mnk.Library.WpfControls.Localization.TranslateExtension
	{
        public TrExtension(string key) : base(key, AvailabilityCheckerLang.ResourceManager) { }
	}
}
