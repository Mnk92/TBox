using Mnk.TBox.Locales.Localization.Plugins.SkyNet;
using Mnk.Library.WpfControls.Localization;

namespace Mnk.TBox.Plugins.SkyNet.Code.Localization
{
	public class TrExtension : TranslateExtension
	{
        public TrExtension(string key) : base(key, SkyNetLang.ResourceManager) { }
	}
}
