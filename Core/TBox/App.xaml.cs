using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Mnk.Library.Common;
using Mnk.Library.Common.Log;
using Mnk.Library.WpfControls;
using Mnk.Library.WpfControls.Localization;
using Mnk.Library.WpfControls.Menu;
using Mnk.TBox.Core.Application.Code;

namespace Mnk.TBox.Core.Application
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private readonly ILog log = LogManager.GetLogger<App>();
        private static bool handled = false;

        public App()
        {
            Translator.Culture = new CultureInfo("en");

            ShutdownMode = ShutdownMode.OnMainWindowClose;
            FormsStyles.Enable();
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainUnhandledException;
            DispatcherUnhandledException += CurrentDispatcherUnhandledException;
            Dispatcher.UnhandledException += DispatcherOnUnhandledException;
            TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;

            OneInstance.App.Init(this);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            if (ServicesRegistrar.Container != null)
            {
                ServicesRegistrar.Container.Dispose();
            }
        }

        private void TaskSchedulerOnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            LogException(e.Exception);
        }

        private void DispatcherOnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            LogException(e.Exception);
        }

        private void CurrentDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            LogException(e.Exception);
        }

        void CurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            LogException(e.ExceptionObject);
        }

        void LogException(object ex)
        {
            if (handled) return;
            handled = true;
            const string message =
                "Sorry, unhandled exception occurred. Application will be terminated.\nPlease contact with author to fix this issue.\nYou can try restart application to continue working...";
            if (ex is Exception exception)
            {
                log.Write(exception, message);
            }
            else log.Write(message);
            ExceptionsHelper.HandleException(DoExit, x => { });
            Shutdown(-1);
        }

        private static void DoExit()
        {
            if (Current.MainWindow is MainWindow w)
            {
                w.MenuClose(true);
            }
        }
    }
}
