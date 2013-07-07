using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace Mnk.TBox.Tools.SkyNet.Common.Modules
{
    public class ModulesRunner : IModulesRunner
    {
        private readonly IList<IModule> modules;
        private readonly Timer timer;
        private bool started = false;

        public ModulesRunner(IEnumerable<IModule> modules)
        {
            this.modules = modules.ToArray();
            timer = new Timer
            {
                Interval = 5000, 
                AutoReset = false
            };
            timer.Elapsed += (o,e)=>TimerEvent();
        }

        private void TimerEvent()
        {
            foreach (var module in modules)
            {
                if (!started) return;
                module.Process();
            }
            if(started)timer.Start();
        }

        public void Start()
        {
            started = true;
            timer.Start();
        }

        public void Stop()
        {
            started = false;
            timer.Stop();
        }

        public void Dispose()
        {
            Stop();
            timer.Dispose();
        }
    }
}
