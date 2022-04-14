using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace Mnk.TBox.Plugins.FileWatcher.Components
{
	/// <summary>
	/// Interaction logic for MemoBoxLog.xaml
	/// </summary>
	public partial class MemoBoxLog
	{
		private int globalCount = 0;
		public int MaxEntries { get; set; }
		public int EntriesCount { get { return outputData.Count; } }
		private readonly ObservableCollection<CaptionedEntity> outputData = new ObservableCollection<CaptionedEntity>();
		public event EventHandler OnClear;

		public MemoBoxLog()
		{
			InitializeComponent();
			UpdateStatus();
			output.SizeChanged += OutputSizeChanged;
		}

		void OutputSizeChanged(object sender, SizeChangedEventArgs e)
		{
			UpdateStatus();
		}

		public void Write(string caption, string value, string extra)
		{
			outputData.Add(new CaptionedEntity
			{
				Title = "[" + (++globalCount) + "] " + caption,
				Value = value,
				Extra = extra
			});
			while (EntriesCount > MaxEntries)
			{
				outputData.RemoveAt(0);
			}
			UpdateStatus();
		}

		protected override void OnShow()
		{
			base.OnShow();
			output.ItemsSource = outputData;
			UpdateStatus();
		}

		protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
		{
			base.OnClosing(e);
			output.ItemsSource = null;
		}

		private void UpdateStatus()
		{
			sbiEntriesCount.Content = EntriesCount;
			if (output.Items.Count > 0)
			{
				output.ScrollIntoView(output.Items[output.Items.Count - 1]);
			}
		}

		private void ButtonClearClick(object sender, RoutedEventArgs e)
		{
			Clear();
		}

		private void ButtonCloseClick(object sender, RoutedEventArgs e)
		{
			Close();
		}

		public void ShowLogs()
		{
			ShowAndActivate();
			UpdateStatus();
		}

		public void Clear()
		{
			outputData.Clear();
			UpdateStatus();
			if (OnClear != null) OnClear(this, null);
		}
	}
}
