using System;
using Mnk.Library.WpfControls.Code.Log;
using Mnk.Library.WpfControls.Dialogs;

namespace Mnk.TBox.Core.PluginsShared.Watcher
{
	public interface ILogDialog : ICaptionedLog
	{
		int EntriesCount { get; }
		event EventHandler OnClear;
		void ShowLogs();
		void Clear();
		DialogWindow DialogWindow { get; }
	}
}
