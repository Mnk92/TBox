using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Mnk.Library.Common;
using Mnk.Library.Common.Log;
using Mnk.Library.Common.MT;
using Mnk.Library.ScriptEngine;
using Mnk.Library.WpfControls;
using Mnk.Library.WpfControls.Dialogs;
using Mnk.TBox.Core.Contracts;
using Mnk.TBox.Locales.Localization.Plugins.SkyNet;
using Mnk.TBox.Plugins.SkyNet.Code.Interfaces;
using Mnk.TBox.Tools.SkyNet.Common;

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

        public TaskDialog(ITaskExecutor taskExecutor, IServicesFacade servicesFacade, IPluginContext context, IScriptsHelper scriptsHelper)
        {
            this.taskExecutor = taskExecutor;
            this.servicesFacade = servicesFacade;
            this.scriptsHelper = scriptsHelper;
            InitializeComponent();
            timer = new DispatcherTimer{Interval = new TimeSpan(0,0,0,5)};
            timer.Tick += (o,e)=>Refresh();
        }

        private void Refresh()
        {
            try
            {
                ExistTasks.ItemsSource = servicesFacade.GetServiceTasks();
                ConnectedAgents.ItemsSource = servicesFacade.GetServiceAgents();
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
                Title = string.Format("{0} - [ {1} ]", SkyNetLang.PluginName, operation.Key);
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
            var operation = (SingleFileOperation) DataContext;
            DialogsCache.ShowProgress(u=>DoStart(u,operation), Title, this, topmost:false);
        }

        private void DoStart(IUpdater updater, SingleFileOperation operation)
        {
            try
            {
                var info = taskExecutor.Execute(operation);
                do
                {
                    var task = servicesFacade.GetTask(info.Id);
                    if (task.State == TaskState.Done) break;
                    updater.Update(task.Progress / 100.0f);
                    Thread.Sleep(5000);
                } while (!updater.UserPressClose);
                if (updater.UserPressClose)
                {
                    servicesFacade.Terminate(info.Id);
                }
                var report = servicesFacade.DeleteTask(info.Id);
                if (!string.IsNullOrEmpty(info.ZipPackageId)) servicesFacade.DeleteFile(info.ZipPackageId);
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
            return (ServerTask) ((Button) sender).DataContext;
        }

        private void ConfigureScriptClick(object sender, RoutedEventArgs e)
        {
            scriptsHelper.ShowParameters((SingleFileOperation)DataContext);
        }

    }
}
