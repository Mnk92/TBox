using Mnk.TBox.Locales.Localization.TBox;
using Mnk.Library.WpfControls.Localization;

namespace Mnk.TBox.Core.Application.Code.Localization
{
	public class TrExtension : TranslateExtension
	{
        public TrExtension(string key): base(key, TBoxLang.ResourceManager){}
	}
}
