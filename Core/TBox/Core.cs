using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Threading;
using Mnk.Library.Common;
using Mnk.Library.Common.Log;
using Mnk.Library.WpfControls;
using Mnk.Library.WpfControls.Localization;
using Mnk.TBox.Core.Application.Code;

namespace Mnk.TBox.Core.Application
{
    internal class Core
    {
        private static readonly ILog log = LogManager.GetLogger<App>();
        private static System.Windows.Application rootApplication;
        private static bool handled = false;

        public static void Init(System.Windows.Application root)
        {
            rootApplication = root;
            Translator.Culture = new CultureInfo("en");

            FormsStyles.Enable();

            OneInstance.App.Init(root);
        }

        public static void OnExit()
        {
            if (ServicesRegistrar.Container != null)
            {
                ServicesRegistrar.Container.Dispose();
            }
        }

        public static void TaskSchedulerOnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            LogException(e.Exception);
        }

        public static void DispatcherOnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            LogException(e.Exception);
        }

        public static void CurrentDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            LogException(e.Exception);
        }

        public static void CurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            LogException(e.ExceptionObject);
        }

        private static void LogException(object ex)
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
            ExceptionsHelper.HandleException(Core.DoExit, x => { });
            rootApplication.Shutdown(-1);
        }

        private static void DoExit()
        {
            //if(Current.MainWindow is MainWindow w)
            //w.MenuClose(true);
        }
    }
}
