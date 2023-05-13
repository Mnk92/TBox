using System;
using System.IO;
using System.Reflection;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Mnk.TBox.Core.Application
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        public App()
        {
            // Fix to load dependencies correctly
            AppDomain.CurrentDomain.AssemblyResolve += (s, a) =>
            {
                return (from dir in new[] { "Libraries", "Localization" }
                        select Path.GetFullPath(
                            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, dir, new AssemblyName(a.Name).Name + ".dll"))
                    into assemblyPath
                        where File.Exists(assemblyPath)
                        select Assembly.LoadFrom(assemblyPath)).FirstOrDefault();
            };
            ShutdownMode = ShutdownMode.OnMainWindowClose;
            AppDomain.CurrentDomain.UnhandledException += Core.CurrentDomainUnhandledException;
            DispatcherUnhandledException += Core.CurrentDispatcherUnhandledException;
            Dispatcher.UnhandledException += Core.DispatcherOnUnhandledException;
            TaskScheduler.UnobservedTaskException += Core.TaskSchedulerOnUnobservedTaskException;

            Core.Init(this);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            Core.OnExit();
        }
    }
}
