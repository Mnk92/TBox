using System;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Threading;
using System.Windows;
using Mnk.Library.Common.Log;
using Mnk.Library.Common.Tools;
using Mnk.Library.WpfControls.Components;

namespace Mnk.TBox.Core.PluginsShared.Controls
{
    /// <summary>
    /// Interaction logic for ServiceControl.xaml
    /// </summary>
    public partial class ServiceControl
    {
        private readonly ILog log = LogManager.GetLogger<ServiceControl>();

        public ServiceControl()
        {
            IsRunning = false;
            InitializeComponent();
        }

        public static readonly DependencyProperty ServiceNameProperty =
                DpHelper.Create<ServiceControl, string>("ServiceName", (s, v) => s.ServiceName = v);
        public string ServiceName
        {
            get => (string)GetValue(ServiceNameProperty);
            set
            {
                SetValue(ServiceNameProperty, value);
                RefreshState();
            }
        }

        public static readonly DependencyProperty ServicePathProperty =
                    DpHelper.Create<ServiceControl, string>("ServicePath", (s, v) => s.ServicePath = v);

        public string ServicePath
        {
            get => (string)GetValue(ServicePathProperty);
            set => SetValue(ServicePathProperty, value);
        }

        public static readonly DependencyProperty IsRunningProperty =
                DpHelper.Create<ServiceControl, bool>("IsRunning");
        public bool IsRunning
        {
            get => (bool)GetValue(IsRunningProperty);
            private set => SetValue(IsRunningProperty, value);
        }

        private void RefreshState()
        {
            var state = IsServiceRunning(ServiceName);
            if (state.HasValue)
            {
                btnInstall.IsEnabled = false;
                btnStop.IsEnabled = state.Value;
                btnUninstall.IsEnabled = btnStart.IsEnabled = !btnStop.IsEnabled;
                IsRunning = state.Value;
            }
            else
            {
                IsRunning = btnStart.IsEnabled = btnStop.IsEnabled = btnUninstall.IsEnabled = false;
                btnInstall.IsEnabled = true;
            }
        }

        private bool? IsServiceRunning(string serviceName)
        {
            try
            {
                var services = ServiceController.GetServices();
                var service = services.FirstOrDefault(x => x.ServiceName.EqualsIgnoreCase(serviceName));
                if (service == null) return null;
                return service.Status == ServiceControllerStatus.Running ||
                        service.Status == ServiceControllerStatus.StartPending;
            }
            catch (Exception ex)
            {
                log.Write(ex, "Can't get service status: " + serviceName);
            }
            return null;
        }

        private void InstallClick(object sender, RoutedEventArgs e)
        {
            DoWork("sc", $"create {ServiceName} binPath= \"{ServicePath}\" start= auto");
            StartService(ServiceName);
            RefreshState();
        }

        private void UninstallClick(object sender, RoutedEventArgs e)
        {
            StopService(ServiceName);
            DoWork("sc", $"delete {ServiceName}");
            RefreshState();
        }

        private void StartClick(object sender, RoutedEventArgs e)
        {
            StartService(ServiceName);
            RefreshState();
        }

        private void StopClick(object sender, RoutedEventArgs e)
        {
            StopService(ServiceName);
            RefreshState();
        }

        private void RefreshClick(object sender, RoutedEventArgs e)
        {
            RefreshState();
        }

        private void StopService(string name)
        {
            DoWork("net", "stop " + name + " /y");
        }

        private void StartService(string name)
        {
            DoWork("net", "start " + name + " /y");
        }

        private void DoWork(string executable, string operation)
        {
            try
            {
                var pi = new ProcessStartInfo
                {
                    Arguments = $"/c {executable} {operation}",
                    FileName = "cmd.exe",
                    UseShellExecute = true,
                    CreateNoWindow = false,
                    Verb = "runas"
                };
                using (var p = Process.Start(pi))
                {
                    p.WaitForExit();
                    if (p.ExitCode < 0)
                    {
                        log.Write("Invalid exit code: " + p.ExitCode);
                    }
                }
                Thread.Sleep(500);
            }
            catch (Exception ex)
            {
                log.Write(ex, "Can't run " + operation);
            }
        }

    }
}
