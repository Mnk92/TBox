using System.Resources;
using Mnk.TBox.Locales.Localization.Plugins.SqlRunner;

namespace Mnk.TBox.Plugins.SqlRunner.Code.Localization
{
	public class TrExtension : Mnk.Library.WpfControls.Localization.TranslateExtension
	{
        public TrExtension(string key) : base(key, SqlRunnerLang.ResourceManager) { }
	}
}
