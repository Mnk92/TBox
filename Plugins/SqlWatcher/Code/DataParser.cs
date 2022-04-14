using System;
using System.Linq;
using Mnk.Library.Common;
using Mnk.TBox.Core.PluginsShared.Encoders;
using Mnk.TBox.Core.PluginsShared.Watcher;
using Mnk.Library.WpfControls.Code.Log;

namespace Mnk.TBox.Plugins.SqlWatcher.Code
{
	class DataParser : IDataParser
	{
	    public bool RemoveTypeInfo { get; set; }
	    private readonly SqlParser parser = new SqlParser();

	    public void Parse(string key, string name, string text, ICaptionedLog log)
		{
			foreach (var line in text.Split('\r', '\n').Where(x => !string.IsNullOrWhiteSpace(x)))
			{
				log.Write(string.Format("{0}: '{1}'", key, name), Parse(line));
			}
		}

		public string Parse(string line)
		{
			var text = string.Empty;
			ExceptionsHelper.HandleException(
				() => text = parser.Parse(line, RemoveTypeInfo),
				ex => text = ex.Message + Environment.NewLine + line);
			return text;
		}
	}
}
