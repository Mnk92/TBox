using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Mnk.Library.WpfControls.Icons;
using Mnk.TBox.Core.PluginsShared.LoadTesting.Statistic;
using Mnk.TBox.Locales.Localization.PluginsShared;

namespace Mnk.TBox.Core.PluginsShared.LoadTesting.Components
{
    /// <summary>
    /// Interaction logic for FormLoadTesting.xaml
    /// </summary>
    sealed partial class FormLoadTesting
    {
        private ILoadTester loadTester;
        private Analyzer analyzer;
        private readonly TrayIcon trayIcon = new TrayIcon();
        private readonly DispatcherTimer viewTimer;

        public FormLoadTesting()
        {
            InitializeComponent();
            SetGraphic(null);
            Panel.View = Operations;
            Panel.CheckChangedByPanel += CheckBoxChecked;
            trayIcon.MouseClick += TrayIconOnMouseClick;
            viewTimer = new DispatcherTimer();
            viewTimer.Tick += OnViewTimer;
            viewTimer.Interval = new TimeSpan(0, 0, 0, 1);
        }

        public override void Dispose()
        {
            loadTester.Stop();
            trayIcon.Dispose();
            base.Dispose();
        }

        private void SetGraphic(GraphicsInfo graphicsInfo)
        {
            Graph.Clear();
            if (graphicsInfo == null) return;
            Graph.AddGrapic(graphicsInfo.MaxTime);
            Graph.AddGrapic(graphicsInfo.AverageTime);
            Graph.AddGrapic(graphicsInfo.MinTime);
        }

        private void OnViewTimer(object sender, EventArgs eventArgs)
        {
            if (!viewTimer.IsEnabled || !loadTester.IsWorks) return;
            switch (Tabs.SelectedIndex)
            {
                case 1:
                    RedrawGraphic();
                    break;
                case 2:
                    OnSelectedStatisticChanged(null, null);
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

        private void TrayIconOnMouseClick(MouseButton obj)
        {
            ShowAndActivate();
        }

        public void Init(ImageSource icon, Icon nativeIcon, ILoadTester loadTester)
        {
            Icon = icon;
            trayIcon.Icon = nativeIcon;
            this.loadTester = loadTester;
        }

        private void FillComboBoxes(ComboBox cb)
        {
            cb.Items.Clear();
            foreach (var key in analyzer.Keys)
            {
                cb.Items.Add(key);
            }
            cb.SelectedIndex = 0;
        }

        private static void AppendResult(StringBuilder sb, string property, double value)
        {
            sb.AppendFormat("{0}: \t {1:0.##}", property, value).AppendLine();
        }

        private static void AppendResult(StringBuilder sb, object property, object value)
        {
            sb.AppendFormat("{0}: \t {1}", property, value).AppendLine();
        }

        public void ShowDialog(IProfile profile)
        {
            if (IsVisible || loadTester.IsWorks) return;
            Title = PluginsSharedLang.Ddos + " - [" + profile.Key + "]";
            analyzer = null;
            Statistic.Items.Clear();
            Graphics.Items.Clear();
            SetGraphic(null);
            Results.Text = string.Empty;
            DataContext = profile;
            CheckBoxChecked(Operations, null);
            ShowAndActivate();
        }

        private void CheckBoxChecked(object sender, RoutedEventArgs e)
        {
            Operations.OnCheckChangedEvent(sender, e);
            btnStartStop.IsEnabled = ((IProfile)DataContext).GetOperations().Any();
            btnCopy.IsEnabled = (analyzer != null) && btnStartStop.IsEnabled;
        }

        private void BtnStartClick(object sender, RoutedEventArgs e)
        {
            btnStartStop.IsEnabled = false;
            if (loadTester.IsWorks)
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
            EnableControls(false);
            Results.Text = string.Empty;
            var p = ((IProfile)DataContext);
            btnStartStop.Content = PluginsSharedLang.Stop;
            trayIcon.HoverText = Title;
            trayIcon.IsVisible = true;
            var ops = p.GetOperations().ToArray();
            analyzer = new Analyzer(ops.Select(x => x.Key));
            SetGraphic(analyzer.GetGraphic(string.Empty));
            FillComboBoxes(Statistic);
            FillComboBoxes(Graphics);
            loadTester.Start(ops, analyzer);
            Graph.StartTime = DateTime.Now;
            Graph.EndTime = DateTime.MinValue;
        }

        private void EnableControls(bool state)
        {
            Operations.IsEnabled = Panel.IsEnabled = btnCopy.IsEnabled = state;
        }

        public void Stop()
        {
            Graph.EndTime = DateTime.Now;
            loadTester.Stop();
            btnStartStop.Content = PluginsSharedLang.Start;
            trayIcon.IsVisible = false;
            EnableControls(true);
        }

        private void OnSelectedStatisticChanged(object sender, SelectionChangedEventArgs e)
        {
            ShowStatistic((Statistic.SelectedValue ?? string.Empty).ToString());
        }

        private void ShowStatistic(string key)
        {
            if (analyzer == null) return;
            var s = analyzer.GetStatistic(key);
            var sb = new StringBuilder();
            AppendResult(sb, PluginsSharedLang.Count, s.Count);
            AppendResult(sb, PluginsSharedLang.Time, s.Time / 1000.0);
            AppendResult(sb, PluginsSharedLang.AverageTime, (s.Count == 0) ? 0 : (s.Time / (float)s.Count / 1000.0));
            AppendResult(sb, PluginsSharedLang.MinTime, s.MinTime / 1000.0);
            AppendResult(sb, PluginsSharedLang.MaxTime, s.MaxTime / 1000.0);
            sb.AppendLine();
            foreach (var status in s.Statutes)
            {
                AppendResult(sb, status.Key,
                             string.Format(CultureInfo.InvariantCulture, "{0}  ( {1:0.##} )%", status.Value, 100 * status.Value / (float)s.Count));
            }
            Results.Text = sb.ToString();
        }

        private void OnSelectedGraphicChanged(object sender, SelectionChangedEventArgs e)
        {
            DrawGraphic((Graphics.SelectedValue ?? string.Empty).ToString());
        }

        private void DrawGraphic(string key)
        {
            if (analyzer == null) return;
            SetGraphic(analyzer.GetGraphic(key));
            RedrawGraphic();
        }

        private void BtnCopyClick(object sender, RoutedEventArgs e)
        {
            if (analyzer == null) return;
            var sb = new StringBuilder();
            foreach (var key in new[] { "" }.Concat(analyzer.Keys))
            {
                AppendValues(sb, key, "Count", x => x.Count);
                AppendValues(sb, key, "Min", x => x.MinTime);
                AppendValues(sb, key, "Avg", x => x.Time / (float)x.Count);
                AppendValues(sb, key, "Max", x => x.MaxTime);
                sb.AppendLine();
            }
            Clipboard.SetText(sb.ToString());
        }

        private void AppendValues(StringBuilder sb, string key, string prefix, Func<OperationStatistic, double> getter)
        {
            const char divider = '\t';
            sb.Append(key).Append(divider).Append(prefix).Append(divider);
            foreach (var s in analyzer.GetValues(key))
            {
                if (s.Count <= 0)
                {
                    sb.Append(string.Empty);
                }
                else
                {
                    sb.Append(getter(s));
                }
                sb.Append(divider);
            }
            sb.AppendLine();
        }

        protected override void OnShow()
        {
            base.OnShow();
            viewTimer.Start();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            viewTimer.Stop();
            base.OnClosing(e);
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
