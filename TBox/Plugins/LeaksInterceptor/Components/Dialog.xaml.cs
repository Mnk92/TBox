using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Mnk.Library.Common.Log;
using Mnk.Library.Common.UI.Model;
using Mnk.Library.WpfControls;
using Mnk.Library.WpfWinForms.Icons;
using Mnk.TBox.Locales.Localization.Plugins.LeaksInterceptor;
using Mnk.Library.WpfWinForms;
using Mnk.TBox.Plugins.LeaksInterceptor.Code;
using Mnk.TBox.Plugins.LeaksInterceptor.Code.Getters;
using Mnk.TBox.Plugins.LeaksInterceptor.Code.Perfomance;
using Mnk.TBox.Plugins.LeaksInterceptor.Code.Standart;

namespace Mnk.TBox.Plugins.LeaksInterceptor.Components
{
	/// <summary>
	/// Interaction logic for Dialog.xaml
	/// </summary>
	sealed partial class Dialog
	{
		private static readonly ILog Log = LogManager.GetLogger<Dialog>();
		private readonly DispatcherTimer viewTimer;
		private readonly string caption;
		private readonly ProcessList processList = new ProcessList();
		private readonly SystemAnalizer systemAnalizer = new SystemAnalizer();
		private readonly TrayIcon trayIcon = new TrayIcon();
		private readonly GettersFactory factory = new GettersFactory();
		public Dialog()
		{
			InitializeComponent();
			caption = Title;
			lvProcess.ItemsSource = processList.Processes;
			viewTimer = new DispatcherTimer();
			viewTimer.Tick += OnViewTimer;
			CountersPanel.View = Counters;
			CountersPanel.CheckChangedByPanel += CountersOnOnCheckChanged;
			trayIcon.MouseClick += TrayIconOnMouseClick;
			PerfomanceCounters.OnDataChanged += (o,e) => EnableStartButton();
		}

		public void Init(Icon icon)
		{
			trayIcon.Icon = icon;
		}

		private void TrayIconOnMouseClick(MouseButton mouseButton)
		{
			ShowAndActivate();
		}

		public override void Dispose()
		{
			systemAnalizer.Dispose();
			trayIcon.Dispose();
			base.Dispose();
		}

		private void OnViewTimer(object sender, EventArgs eventArgs)
		{
			switch (Tabs.SelectedIndex)
			{
				case 0:
					processList.RefreshProcesses();
					break;
				case 2:
					if (viewTimer.IsEnabled && systemAnalizer.Work) RedrawGraphic();
					break;
			}
		}

		private void RedrawGraphic()
		{
			lock (Graph)
			{
				Graph.Redraw();
			}
		}

		protected override void OnShow()
		{
			base.OnShow();
			var counters = Config.Counters;
			if (factory.Count != counters.Count)
			{
				foreach (var name in factory.GetNames())
				{
					counters.Add(new CheckableData { Key = name });
				}
			}
			Counters.ItemsSource = counters;

			viewTimer.Interval = new TimeSpan(0, 0, 0, 0, Config.RefreshViewInterval);
			viewTimer.Start();
			processList.RefreshProcesses();
			SelectLastSelectedProcess();
		}

		private void SelectLastSelectedProcess()
		{
			var last = Config.LastProcessName;
			if (string.IsNullOrEmpty(last)) return;
			for (var i = 0; i < lvProcess.Items.Count; ++i)
			{
				if (!string.Equals(last, processList.Get(i).Name)) continue;
				lvProcess.SelectedIndex = i;
				return;
			}
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			viewTimer.Stop();
			base.OnClosing(e);
		}

		private int GetSelectedPid()
		{
			var selected = -1;
			if (lvProcess.SelectedIndex >= 0)
			{
				selected = processList.Get(lvProcess.SelectedIndex).Pid;
			}
			return selected;
		}

		private string GetSelectedName()
		{
			return lvProcess.SelectedIndex >= 0 ? 
				processList.Get(lvProcess.SelectedIndex).Name : string.Empty;
		}

		private void LvProcessSelectedIndexChanged(object sender, RoutedEventArgs e)
		{
			var pid = GetSelectedPid();
			if (pid >= 0)
			{
				var name = processList.Get(lvProcess.SelectedIndex).Name;
				Title = string.Format("{0}: {1} - \"{2}\"", caption, pid, name);
			}
			else
			{
				Title = caption;
				if (systemAnalizer.Work)
				{
					ButtonStartStopClick(sender,e);
				}
			}
			EnableStartButton();
		}

		private void EnableStartButton()
		{
			if(systemAnalizer.Work)return;
			btnStartStop.IsEnabled = IsProcessSelected() || IsCountersExists();
		}

		private bool IsCountersExists()
		{
			return Config.PerfomanceCounters.Count > 0;
		}

		private bool IsProcessSelected()
		{
			return (lvProcess.SelectedIndex != -1 && Config.Counters.CheckedItems.Any());
		}

		private void ButtonCopyClick(object sender, RoutedEventArgs e)
		{
			systemAnalizer.CopyResultsToClipboard();
		}

		private void EnableControls(bool enabled)
		{
			lvProcess.IsEnabled = enabled;
			Counters.IsEnabled = enabled;
			CountersPanel.IsEnabled = enabled;
			PerfomanceCounters.IsEnabled = enabled;
		}

		private void ButtonStartStopClick(object sender, RoutedEventArgs e)
		{
			btnStartStop.IsEnabled = false;
			if (systemAnalizer.Work)
			{
				Stop();
			}
			else
			{
				Start();
			}
			btnStartStop.IsEnabled = true;
		}

		private void Start()
		{
			try
			{
				btnStartStop.IsEnabled = false;
				try
				{
					CreateAnalizers();
				}
				catch (Exception ex)
				{
					Log.Write(ex, "Can't initialize counters");
					return;
				}
				Config.LastProcessName = GetSelectedName();
				EnableControls(false);
				btnStartStop.Content = LeaksInterceptorLang.Stop;
				cbGraphics.IsEnabled = true;
				btnCopy.IsEnabled = false;
				trayIcon.HoverText = Title;
				trayIcon.IsVisible = true;
				cbGraphics.ItemsSource = systemAnalizer.GetNames();
				RedrawGraphic();
				OnSelectedGraphicChanged(null, null);
				Graph.StartTime = DateTime.Now;
				Graph.EndTime = DateTime.MinValue;
			}
			finally 
			{
				EnableStartButton();
			}
		}

		private void CreateAnalizers()
		{
			var names = Config.Counters.CheckedItems.Select(x => x.Key).ToArray();
			var analizers = new List<IAnalizer>();
			var pid = GetSelectedPid();
			if(pid>=0 && names.Any())
				analizers.Add(new ProcessAnalizer(GetSelectedPid(), names, factory));
			if(Config.PerfomanceCounters.Count>0)
				analizers.Add(new PerfomanceCountersAnalizer(Config.PerfomanceCounters));
			systemAnalizer.Start(Config.RefreshDataInterval,
			                     () => Mt.Do(this, () =>
				                     {
					                     processList.RefreshProcesses();
					                     Stop();
				                     }),
			                     analizers
				);
		}

		private void Stop()
		{
			EnableControls(true);
			btnStartStop.Content = LeaksInterceptorLang.Start;
			systemAnalizer.Stop();
			trayIcon.IsVisible = false;
			btnCopy.IsEnabled = true;
			EnableStartButton();
			Graph.EndTime = DateTime.Now;
			RedrawGraphic();
		}

		private void OnSelectedGraphicChanged(object sender, SelectionChangedEventArgs e)
		{
			lock (Graph)
			{
				Graph.Clear();
				if (cbGraphics.SelectedIndex != -1)
				{
					var name = cbGraphics.SelectedItem.ToString();
					Graph.AddGrapic(systemAnalizer.GetGraphic(name));
				}
			}
			RedrawGraphic();
		}

		private Config Config
		{
			get { return (Config)DataContext; }
		}

		private void CountersOnOnCheckChanged(object sender, RoutedEventArgs e)
		{
			EnableStartButton();
		}

		private void CancelClick(object sender, RoutedEventArgs e)
		{
			Close();
		}
	}
}
