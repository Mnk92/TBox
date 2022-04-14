using System.Resources;
using Mnk.TBox.Locales.Localization.Plugins.DirectoryProcessor;

namespace Mnk.TBox.Plugins.DirectoryProcessor.Code.Localization
{
	public class TrExtension : Mnk.Library.WpfControls.Localization.TranslateExtension
	{
        public TrExtension(string key) : base(key, DirectoryProcessorLang.ResourceManager) { }
	}
}
