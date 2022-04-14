using System.Collections;
using System.Windows;
using Mnk.Library.Common.MT;
using Mnk.Library.WpfControls.Components.Drawings.Graphics;

namespace Mnk.TBox.Plugins.LeaksInterceptor.Code
{
    sealed class SystemAnalyzer : IDisposable
    {
        private readonly SafeTimer dataTimer;
        private Action onGetInfoFailed;
        private IList<IAnalyzer> analyzers;

        public bool Work => dataTimer.Enabled;

        public SystemAnalyzer()
        {
            dataTimer = new SafeTimer(DataTimerOnElapsed);
        }

        private void DataTimerOnElapsed()
        {
            if (analyzers.ToArray().Any(a => !a.Analyze()))
            {
                onGetInfoFailed();
            }
        }

        public void Start(int interval, Action onGetFailed, IList<IAnalyzer> analyzersInstances)
        {
            analyzers = analyzersInstances;
            onGetInfoFailed = onGetFailed;
            dataTimer.Start(interval);
        }

        public void Stop()
        {
            dataTimer.Stop();
            foreach (var d in analyzers.ToArray().OfType<IDisposable>())
            {
                d.Dispose();
            }
        }

        public void CopyResultsToClipboard()
        {
            Clipboard.SetText(string.Join(Environment.NewLine, analyzers.Select(x => x.CopyResults())));
        }

        public IGraphic GetGraphic(string name)
        {
            return analyzers.ToArray()
                .Select(a => a.GetGraphic(name))
                .FirstOrDefault(g => g != null);
        }

        public void Dispose()
        {
            dataTimer.Dispose();
        }

        public IEnumerable GetNames()
        {
            return analyzers.SelectMany(x => x.GetNames());
        }
    }
}
