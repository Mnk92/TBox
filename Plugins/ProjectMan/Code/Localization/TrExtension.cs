using System.Resources;
using Mnk.TBox.Locales.Localization.Plugins.ProjectMan;

namespace Mnk.TBox.Plugins.ProjectMan.Code.Localization
{
	public class TrExtension : Mnk.Library.WpfControls.Localization.TranslateExtension
	{
        public TrExtension(string key) : base(key, ProjectManLang.ResourceManager) { }
	}
}
