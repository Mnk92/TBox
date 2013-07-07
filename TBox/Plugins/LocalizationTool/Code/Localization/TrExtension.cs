using System.Resources;
using Mnk.TBox.Locales.Localization.Plugins.LocalizationTool;

namespace Mnk.TBox.Plugins.LocalizationTool.Code.Localization
{
	public class TrExtension : Mnk.Library.WpfControls.Localization.TranslateExtension
	{
        public TrExtension(string key) : base(key, LocalizationToolLang.ResourceManager) { }
	}
}
