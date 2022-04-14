using System;
using Mnk.Library.CodePlex;
using Mnk.Library.Common.AutoUpdate;
using Mnk.Library.Common.Log;
using Mnk.TBox.Core.Contracts;

namespace Mnk.TBox.Core.Application.Code.AutoUpdate
{
    public class ApplicationUpdater : IAutoUpdater
    {
        private static readonly ILog Log = LogManager.GetLogger<ApplicationUpdater>();
        private readonly IConfigManager<Config> cm;
        private readonly IApplicationUpdater applicationUpdater;

        public ApplicationUpdater(IConfigManager<Config> cm, IApplicationUpdater applicationUpdater)
        {
            this.cm = cm;
            this.applicationUpdater = applicationUpdater;
        }

        public bool? TryUpdate(bool manual = false)
        {
            if (!manual && cm.Config.Update.Interval == UpdateInterval.Never) return false;
            try
            {
                var needUpdate = applicationUpdater.NeedUpdate();
                if (needUpdate == null) return null;
                if (needUpdate == true &&
                    (manual || Merger.CheckDate(cm.Config.Update.Last, cm.Config.Update.Interval)))
                {
                    applicationUpdater.Update();
                }
                else return false;
            }
            catch (Exception ex)
            {
                Log.Write(ex, "Can't update application.");
                return false;
            }
            cm.Config.Update.Last = DateTime.Now;
            System.Windows.Application.Current.Dispatcher.Invoke(new Action(DoExit));
            return true;
        }

        private static void DoExit()
        {
            var w = System.Windows.Application.Current.MainWindow as MainWindow;
            if (w != null)
            {
                w.MenuClose();
            }
            System.Windows.Application.Current.Shutdown();
        }
    }
}
