using Mnk.Library.WpfControls.Code.Log;

namespace Mnk.TBox.Core.PluginsShared.Watcher
{
	public interface IDataParser
	{
		void Parse(string key, string name, string text, ICaptionedLog log);
	}
}
