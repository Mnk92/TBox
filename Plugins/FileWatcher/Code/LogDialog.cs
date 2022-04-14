using System;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Media;
using Mnk.Library.Common.Log;
using Mnk.Library.Common.Tools;
using Mnk.TBox.Locales.Localization.Plugins.FileWatcher;
using Mnk.TBox.Core.PluginsShared.Watcher;
using Mnk.Library.WpfControls.Dialogs;
using Mnk.Library.WpfControls.Tools;
using Mnk.TBox.Plugins.FileWatcher.Components;

namespace Mnk.TBox.Plugins.FileWatcher.Code
{
	class LogDialog : ILogDialog
	{
		private static readonly ILog Log = LogManager.GetLogger<LogDialog>();
		private Regex regExp;
		public MemoBoxLog Dialog { get; private set; }
		public DialogWindow DialogWindow { get { return Dialog; } }
		public int EntriesCount { get { return Dialog.EntriesCount; } }
		public event EventHandler OnClear
		{
			add { Dialog.OnClear += value; }
			remove { Dialog.OnClear -= value; }
		}

		public LogDialog(ImageSource icon)
		{
			Dialog = new MemoBoxLog{Title = FileWatcherLang.PluginName, Icon = icon};
		}

		public void ShowLogs()
		{
			Dialog.ShowLogs();
		}

		public void Clear()
		{
			Dialog.Clear();
		}

		public void Init(string entryRegExp)
		{
			try
			{
				regExp = new Regex(entryRegExp, RegexOptions.Compiled);
			}
			catch (Exception ex)
			{
				Log.Write(ex, "Can't initialize regExp: " + entryRegExp);
			}
		}

		public void Write(string caption, string value)
		{
			if(regExp == null)return;
			var sbValue = new StringBuilder();
			var sbCallStack = new StringBuilder();
			foreach (var line in value.Split('\r', '\n').Select(x=>x.Trim()).Where(x=>!string.IsNullOrEmpty(x)))
			{
				if (regExp.IsMatch(line) && (sbValue.Length>0 || sbCallStack.Length>0))
				{
					Dialog.Write(caption, sbValue.ToString(), sbCallStack.ToString());
					sbValue.Clear();
					sbCallStack.Clear();
				}
				if (sbCallStack.Length > 0 || line.StartsWith("at "))
				{
					sbCallStack.AppendLineIfNeed(line);
				}
				else
				{
					sbValue.AppendLineIfNeed(line);
				}
			}
			Dialog.Write(caption, sbValue.ToString(), sbCallStack.ToString());
		}
	}
}
