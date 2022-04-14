using System;
using System.Windows.Media;
using Mnk.TBox.Locales.Localization.Plugins.SqlWatcher;
using Mnk.TBox.Core.PluginsShared.Watcher;
using Mnk.Library.WpfControls.Dialogs;
using Mnk.Library.WpfSyntaxHighlighter;

namespace Mnk.TBox.Plugins.SqlWatcher.Code
{
	internal class LogDialog : ILogDialog
	{
		public MemoBox Dialog { get; private set; }
		public DialogWindow DialogWindow { get { return Dialog; } }

		public int EntriesCount
		{
			get { return Dialog.EntriesCount; }
		}

		public LogDialog(ImageSource icon)
		{
			Dialog = new MemoBox
			{
				Title = SqlWatcherLang.PluginName, 
				IsReadOnly = true, 
				Icon = icon
			};
		}

		public event EventHandler OnClear
		{
			add { Dialog.OnClear += value; }
			remove { Dialog.OnClear -= value; }
		}

		public void ShowLogs()
		{
			Dialog.ShowAndActivate();
		}

		public void Clear()
		{
			Dialog.Clear();
		}

		public void Write(string caption, string value)
		{
			Dialog.Write(caption, value);
		}
	}
}
