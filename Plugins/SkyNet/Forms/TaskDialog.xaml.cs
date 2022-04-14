using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Mnk.Library.Common;
using Mnk.Library.Common.Log;
using Mnk.Library.Common.MT;
using Mnk.Library.ScriptEngine;
using Mnk.Library.WpfControls;
using Mnk.Library.WpfControls.Dialogs;
using Mnk.TBox.Locales.Localization.Plugins.SkyNet;
using Mnk.TBox.Plugins.SkyNet.Code.Interfaces;
using Mnk.TBox.Tools.SkyNet.Contracts;

namespace Mnk.TBox.Plugins.SkyNet.Forms
{
    /// <summary>
    /// Interaction logic for TaskDialog.xaml
    /// </summary>
    public partial class TaskDialog
    {
        private readonly ITaskExecutor taskExecutor;
        private readonly IServicesFacade servicesFacade;
        private readonly IScriptsHelper scriptsHelper;
        private readonly DispatcherTimer timer;
        private readonly ILog log = LogManager.GetLogger<TaskDialog>();

        public TaskDialog(ITaskExecutor taskExecutor, IServicesFacade servicesFacade, IScriptsHelper scriptsHelper)
        {
            this.taskExecutor = taskExecutor;
            this.servicesFacade = servicesFacade;
            this.scriptsHelper = scriptsHelper;
            InitializeComponent();
            timer = new DispatcherTimer { Interval = new TimeSpan(0, 0, 0, 5) };
            timer.Tick += (_, _) => Refresh().Wait();
        }

        private async Task Refresh()
        {
            try
            {
                ExistTasks.ItemsSource = await servicesFacade.GetServiceTasks();
                ConnectedAgents.ItemsSource = await servicesFacade.GetServiceAgents();
            }
            catch (Exception ex)
            {
                log.Write(ex, "Can't retrieve tasks state. Please, verify is server service started and you have correctly configured firewall.");
                Close();
            }
        }

        public void ShowDialog(SingleFileOperation operation)
        {
            if (!IsVisible)
            {
                Title = $"{SkyNetLang.PluginName} - [ {operation.Key} ]";
                Report.Text = string.Empty;
                timer.Start();
                DataContext = operation;
            }
            ShowAndActivate();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            timer.Stop();
            base.OnClosing(e);
        }

        private void CloseClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void StartClick(object sender, RoutedEventArgs e)
        {
            Report.Text = string.Empty;
            timer.Stop();
            var operation = (SingleFileOperation)DataContext;
            DialogsCache.ShowProgress(u => DoStart(u, operation).Wait(), Title, this, topmost: false);
        }

        private async Task DoStart(IUpdater updater, SingleFileOperation operation)
        {
            try
            {
                var info = await taskExecutor.Execute(operation);
                do
                {
                    var task = await servicesFacade.GetTask(info.Id);
                    if (task.State == ServerTask.Types.TaskState.Done) break;
                    updater.Update(task.Progress / 100.0f);
                    Thread.Sleep(5000);
                } while (!updater.UserPressClose);
                if (updater.UserPressClose)
                {
                    await servicesFacade.Terminate(info.Id);
                }
                var report = await servicesFacade.DeleteTask(info.Id);
                if (!string.IsNullOrEmpty(info.ZipPackageId)) await servicesFacade.DeleteFile(info.ZipPackageId);
                Mt.Do(this, () => Report.Text = report);
            }
            catch (Exception ex)
            {
                log.Write(ex, "Error executing task");
            }
            finally
            {
                Mt.Do(this, timer.Start);
            }
        }

        private void CancelTask(object sender, RoutedEventArgs e)
        {
            var task = GetServerTask(sender);
            ExceptionsHelper.HandleException(
                () => servicesFacade.Cancel(task.Id),
                () => "Can't cancel task", log
                );
        }

        private void TerminateTask(object sender, RoutedEventArgs e)
        {
            var task = GetServerTask(sender);
            ExceptionsHelper.HandleException(
                () => servicesFacade.Terminate(task.Id),
                () => "Can't terminate task", log
                );
        }

        private void DeleteTask(object sender, RoutedEventArgs e)
        {
            var task = GetServerTask(sender);
            ExceptionsHelper.HandleException(
                () => servicesFacade.DeleteTask(task.Id),
                () => "Can't delete task", log
                );
        }

        private static ServerTask GetServerTask(object sender)
        {
            return (ServerTask)((Button)sender).DataContext;
        }

        private void ConfigureScriptClick(object sender, RoutedEventArgs e)
        {
            scriptsHelper.ShowParameters((SingleFileOperation)DataContext);
        }

    }
}
