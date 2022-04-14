using System.Linq;
using System.Collections.Generic;
using Mnk.TBox.Core.PluginsShared.Automator;
using Mnk.TBox.Core.PluginsShared.Tools;
using Mnk.Library.ScriptEngine;

namespace Solution.Scripts
{
    public class EnableFeatures : IScript
    {
        [FileList]
        public string[] ConfigFiles { get; set; }

        [StringDictionary("Feature.Sample", "true")]
        public IDictionary<string, string> Features { get; set; }

        [String("appSettings")]
        public string XmlSelector { get; set; }

        [String("key")]
        public string KeyKey { get; set; }

        [String("value")]
        public string ValueKey { get; set; }

        [String("add")]
        public string SectionName { get; set; }

        public void Run(IScriptContext s)
        {
            new FeatureToggler(XmlSelector, KeyKey, ValueKey, SectionName)
                .Execute(
                    ConfigFiles.Select(x => s.PathResolver.Resolve(x)).ToArray(),
                    Features.Select(x => new KeyValuePair<string, string>(x.Key, x.Value)).ToArray());
        }
    }
}