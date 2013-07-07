using System.Collections.Generic;
using System.Xml.Linq;
using Mnk.TBox.Core.PluginsShared.Automator;
using Mnk.Library.ScriptEngine;
using Mnk.TBox.Core.PluginsShared.Tools;

namespace Solution.Scripts
{
    public class AddTracing : IScript
    {
        [FileDictionary("d:/project/Web.config", "d:/sample.log")]
        public IDictionary<string, string> Files { get; set; }

        public void Run(IScriptContext s)
        {
            var i = 0;
            foreach (var file in Files)
            {
                var source = s.PathResolver.Resolve(file.Key);
                var target = s.PathResolver.Resolve(file.Value);
                s.Updater.Update(source, i++ / (float)Files.Count);
                var doc = XDocument.Load(source, LoadOptions.PreserveWhitespace);
                AddTracingToConfig(doc.Root, target);
                doc.Save(source);
            }
        }

        private static void AddTracingToConfig(XElement root, string targetPath)
        {
            root.AddNodeIfNotExist("system.diagnostics/trace")
                .SetAttributeIfNeed("autoflush", "true");
            root.AddNodeIfNotExist("system.diagnostics/sources/source")
                .SetAttributeIfNeed("name", "System.Net.Sockets")
                .SetAttributeIfNeed("maxdatasize", "10241024");
            root.AddNodeIfNotExist("system.diagnostics/sources/source/listeners/add")
                .SetAttributeIfNeed("name", "TraceFile");
            root.AddNodeIfNotExist("system.diagnostics/sharedListeners/add")
                .SetAttributeIfNeed("name", "TraceFile")
                .SetAttributeIfNeed("type", "System.Diagnostics.TextWriterTraceListener")
                .SetAttributeIfNeed("initializeData", targetPath);
            root.AddNodeIfNotExist("system.diagnostics/switches/add")
                .SetAttributeIfNeed("name", "System.Net.Sockets")
                .SetAttributeIfNeed("value", "Verbose");
        }
    }
}
