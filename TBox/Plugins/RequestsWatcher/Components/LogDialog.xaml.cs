using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using Mnk.Library.Common.Network;
using ServiceStack.Text;
using Mnk.TBox.Core.PluginsShared.Watcher;
using Mnk.Library.WpfControls.Components.Drawings.DirectionsTable;
using Mnk.Library.WpfControls.Dialogs;

namespace Mnk.TBox.Plugins.RequestsWatcher.Components
{
	/// <summary>
	/// Interaction logic for LogDialog.xaml
	/// </summary>
	public partial class LogDialog : ILogDialog
	{
		private readonly DirectionsTable table = new DirectionsTable();
		private readonly DispatcherTimer timer = new DispatcherTimer();
		private bool needRedraw = false;
		public DialogWindow DialogWindow { get { return this; } }
		public int EntriesCount
		{
			get { return table.EntriesCount; }
		}

		public int MaxEntriesInLog
		{
			get { return table.MaxCount; }
			set { table.MaxCount = value; }
		}

		public LogDialog(ImageSource icon)
		{
			InitializeComponent();
			scrollView.Content = table;
			table.Selected += TableOnSelected;
			timer.Interval =new TimeSpan(0,0,1);
			timer.Tick += TimerOnTick;
			
			Icon = icon;
		}

		private void TimerOnTick(object sender, EventArgs eventArgs)
		{
			if (!needRedraw) return;
			table.Redraw();
			needRedraw = false;
		}

		private void TableOnSelected(object o, IDirectionable directionable)
		{
			var item = directionable as Code.Request;
			Send.Value = item == null ? null : CreateInfo(item.Send);
			Receive.Value = item == null ? null : CreateInfo(item.Receive);
		}

		private static ResponseInfo CreateInfo(string text)
		{
			var id = text.IndexOf(Environment.NewLine + Environment.NewLine, StringComparison.Ordinal);
			if (id == -1)
			{
				return new ResponseInfo{Headers = text};
			}
			return new ResponseInfo
				{
					Headers = text.Substring(0, id),
					Body = text.Substring(id+2*Environment.NewLine.Length)
				};
		}

		public event EventHandler OnClear
		{
			add { table.Cleared += value; }
			remove { table.Cleared -= value; }
		}

		public void ShowLogs()
		{
			needRedraw = false;
			table.Redraw();
			timer.Start();
			ShowAndActivate();
		}

		public void Clear()
		{
			table.Clear();
			table.Redraw();
		}

		public void Write(string caption, string value)
		{
			table.Add(JsonSerializer.DeserializeFromString<Code.Request>(value) );
			needRedraw = true;
		}

		private void ButtonClearClick(object sender, RoutedEventArgs e)
		{
			Clear();
		}

		private void ButtonCloseClick(object sender, RoutedEventArgs e)
		{
			Close();
		}

		protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
		{
			base.OnClosing(e);
			timer.Stop();
		}
	}
}
