using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Threading;
using Mnk.Library.Common.Log;
using Mnk.Library.Common.MT;
using Mnk.TBox.Core.Contracts;

namespace Mnk.TBox.Plugins.ServicesCommander.Code
{
    class Runner
    {
        private readonly IPluginContext context;
        private static readonly ILog Log = LogManager.GetLogger<Runner>();

        public Runner(IPluginContext context)
        {
            this.context = context;
        }

        public bool IsRunning(ServiceInfo info)
        {
            try
            {
                using (var s = new ServiceController(ResolveName(info.Key)))
                {
                    return IsRunning(s);
                }
            }
            catch (Exception ex)
            {
                Log.Write(ex, "Can't get service status: " + info.Key);
            }
            return false;
        }

        private static bool IsRunning(ServiceController s)
        {
            return s.Status == ServiceControllerStatus.Running || s.Status == ServiceControllerStatus.StartPending;
        }

        public void ToggleService(ServiceInfo info)
        {
            var operation = string.Empty;
            try
            {
                var name = ResolveName(info.Key);
                using (var s = new ServiceController(name))
                {
                    if (IsRunning(s))
                    {
                        operation = "stop";
                        StopService(name);
                    }
                    else
                    {
                        operation = "start";
                        StartService(name);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Write(ex, "Can't " + operation + " service: " + info.Key);
            }
            context.RebuildMenu();
        }

        public void RestartService(ServiceInfo info)
        {
            try
            {
                ReStartService(ResolveName(info.Key));
            }
            catch (Exception ex)
            {
                Log.Write(ex, "Can't restart service: " + info.Key);
            }
            context.RebuildMenu();
        }

        public void StopAll(Profile profile, IUpdater u)
        {
            Do(StopService, profile, u);
        }

        public void StartAll(Profile profile, IUpdater u)
        {
            Do(StartService, profile, u);
        }

        private void Do(Action<string> op, Profile profile, IUpdater u)
        {
            var items = profile.Services.CheckedItems.ToArray();
            var i = 0;
            foreach (var s in items)
            {
                var name = ResolveName(s.Key);
                op(name);
                u.Update(name, ++i / (float)items.Length);
            }
            context.RebuildMenu();
        }


        private void StopService(string name)
        {
            DoWork("net stop " + name + " /y");
        }

        private void StartService(string name)
        {
            DoWork("net start " + name + " /y");
        }

        private void ReStartService(string name)
        {
            DoWork(string.Format("net stop {0} /y & net start {0} /y", name));
        }

        private void DoWork(string operation)
        {
            try
            {
                var pi = new ProcessStartInfo
                {
                    Arguments = "/c " + operation,
                    FileName = "cmd",
                    UseShellExecute = true,
                    CreateNoWindow = true,
                    Verb = "runas"
                };
                using (var p = Process.Start(pi))
                {
                    p.WaitForExit();
                }
                Thread.Sleep(500);
            }
            catch (Exception ex)
            {
                Log.Write(ex, "Can't run " + operation);
            }
        }

        private static string ResolveName(string name)
        {
            var s = ServiceController.GetServices().FirstOrDefault(x => string.Equals(x.DisplayName, name));
            return s != null ? s.ServiceName : string.Empty;
        }
    }
}
