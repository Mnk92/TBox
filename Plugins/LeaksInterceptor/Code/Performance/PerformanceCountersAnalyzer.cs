using System.Diagnostics;
using System.Text;
using Mnk.Library.Common.Log;
using Mnk.Library.WpfControls.Components.Drawings.Graphics;
using Mnk.Library.WpfControls.Dialogs.PerformanceCounters;
using Mnk.TBox.Plugins.LeaksInterceptor.Code.Standard;

namespace Mnk.TBox.Plugins.LeaksInterceptor.Code.Performance
{
    sealed class PerformanceCountersAnalyzer : IAnalyzer, IDisposable
    {
        private readonly IDictionary<string, PerformanceInfo> counters;
        private static readonly ILog Log = LogManager.GetLogger<ProcessAnalyzer>();

        public PerformanceCountersAnalyzer(IList<SelectedEntity> countersEntities)
        {
            counters = new Dictionary<string, PerformanceInfo>();
            var catNames = countersEntities.Select(x => x.Category).Distinct().ToArray();
            var cats = PerformanceCounterCategory.GetCategories().Where(x => catNames.Contains(x.CategoryName)).ToArray();
            foreach (var e in countersEntities)
            {
                var c = cats
                    .First(x => string.Equals(x.CategoryName, e.Category))
                    .GetCounters(e.Instance)
                    .First(x => string.Equals(x.CounterName, e.Name));
                counters[e.ToString()] = new PerformanceInfo(c);
            }
        }

        public bool Analyze()
        {
            foreach (var info in counters.ToArray())
            {
                try
                {
                    info.Value.Graphic.Add(info.Value.Getter.NextValue());
                }
                catch (Exception ex)
                {
                    Log.Write(ex, "Can't get counter information: " + info.Value);
                    return false;
                }
            }
            return true;
        }

        public string CopyResults()
        {
            var list = counters;
            if (list == null) return string.Empty;
            var sb = new StringBuilder();
            foreach (var val in list)
            {
                sb.Append(val.Key).Append('\t');
                foreach (var x in val.Value.Graphic.Values)
                {
                    sb.Append(x).Append('\t');
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }

        public IGraphic GetGraphic(string name)
        {
            return counters.ContainsKey(name) ? counters[name].Graphic : null;
        }

        public IEnumerable<string> GetNames()
        {
            return counters.Keys;
        }

        public void Dispose()
        {
            foreach (var info in counters)
            {
                info.Value.Getter.Dispose();
            }
        }
    }
}
