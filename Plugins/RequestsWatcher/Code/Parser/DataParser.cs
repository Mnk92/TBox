using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Mnk.Library.Common.Log;
using Mnk.TBox.Core.PluginsShared.Watcher;
using Mnk.Library.WpfControls.Code.Log;

namespace Mnk.TBox.Plugins.RequestsWatcher.Code.Parser
{
    class DataParser : IDataParser
    {
        private static readonly ILog Log = LogManager.GetLogger<DataParser>();
        private readonly IDictionary<string, IList<RequestEntry>> items =
            new Dictionary<string, IList<RequestEntry>>();

        public void Reset()
        {
            items.Clear();
        }

        public void Parse(string key, string name, string text, ICaptionedLog log)
        {
            foreach (var line in text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
            {
                string result;
                if (TryParse(key, line, out result))
                {
                    log.Write(string.Format("{0}: '{1}'", key, name), result);
                }
            }
        }

        public bool TryParse(string key, string value, out string result)
        {
            try
            {
                IList<RequestEntry> requests;
                if (!items.TryGetValue(key, out requests))
                {
                    items.Add(key, requests = new List<RequestEntry>());
                }
                foreach (var r in requests.Where(r => r.Process(value)))
                {
                    Request req;
                    if (r.TryGetResult(out req))
                    {
                        req.From = PrepareAddress(key);
                        req.To = PrepareAddress(req.To);
                        result = JsonSerializer.Serialize(req);
                        requests.Remove(r);
                        return true;
                    }
                    result = string.Empty;
                    return false;
                }
                var idle = requests.FirstOrDefault(x => x.Idle);
                if (idle == null)
                {
                    requests.Add(idle = new RequestEntry());
                }
                idle.Process(value);
            }
            catch (Exception ex)
            {
                Log.Write(ex, "Exception on parse line: '{0}'", value);
            }
            result = string.Empty;
            return false;
        }

        private static string PrepareAddress(string address)
        {
            return address.Replace("127.0.0.1:", "").Replace(Environment.MachineName + ":", "");
        }
    }
}
