using Mnk.TBox.Core.PluginsShared.Watcher;
using Mnk.Library.WpfControls.Code.Log;

namespace Mnk.TBox.Plugins.FileWatcher.Code
{
	class DataParser : IDataParser
	{
		public void Parse(string key, string name, string text, ICaptionedLog log)
		{
			log.Write(string.Format("{0}: '{1}'", key, name), text);
		}
	}
}
