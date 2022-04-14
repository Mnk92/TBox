using System.Resources;
using Mnk.TBox.Locales.Localization.Plugins.Encoder;

namespace Mnk.TBox.Plugins.Encoder.Code.Localization
{
	public class TrExtension : Mnk.Library.WpfControls.Localization.TranslateExtension
	{
        public TrExtension(string key) : base(key, EncoderLang.ResourceManager) { }
	}
}
