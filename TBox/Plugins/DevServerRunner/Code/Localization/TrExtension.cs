using Mnk.TBox.Locales.Localization.Plugins.DevServerRunner;

namespace Mnk.TBox.Plugins.DevServerRunner.Code.Localization
{
	public class TrExtension : Mnk.Library.WpfControls.Localization.TranslateExtension
	{
        public TrExtension(string key) : base(key, DevServerRunnerLang.ResourceManager) { }
	}
}
