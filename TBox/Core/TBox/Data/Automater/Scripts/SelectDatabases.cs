using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Mnk.Library.Common.Tools;
using Mnk.TBox.Core.PluginsShared.Automator;
using Mnk.Library.ScriptEngine;

namespace Solution.Scripts
{
    public class SelectDatabases : IScript
    {
        [StringDictionary(
            "DB1", "Data Source=(local);Initial Catalog=DB1;",
            "DB2", "Data Source=(local);Initial Catalog=DB2;"
            )]
        public IDictionary<string, string> ConnectionStrings { get; set; }

        [String("add")]
        public string SectionKey { get; set; }

        [String("name")]
        public string KeyAttribute { get; set; }

        [String("connectionString")]
        public string ValueAttribute { get; set; }

        [String("connectionStrings")]
        public string XmlPath { get; set; }

        [FileList()]
        public string[] WebConfigsPathes { get; set; }

        public void Run(IScriptContext context)
        {
            var xmlPathEls = XmlPath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            var id = 0;
            foreach (var path in WebConfigsPathes.Select(context.PathResolver.Resolve))
            {
                context.Updater.Update(path, id / (float)WebConfigsPathes.Length);
                try
                {
                    var doc = XDocument.Load(path, LoadOptions.PreserveWhitespace);
                    ProcessFile(doc, xmlPathEls);
                    doc.Save(path);
                }
                catch (Exception ex)
                {
                    throw new ArgumentException("Can't process fileL" + path, ex);
                }
            }
        }

        private void ProcessFile(XDocument doc, IEnumerable<string> xmlPathEls)
        {
            var xml = doc.Root;
            if (xml == null) return;
            foreach (var el in xmlPathEls)
            {
                xml = xml.Elements().FirstOrDefault(x => x.Name.LocalName.EqualsIgnoreCase(el));
                if (xml == null) return;
            }
            foreach (var exist in xml.Elements()
                .Where(x => x.Name.LocalName.EqualsIgnoreCase(SectionKey)).ToArray())
            {
                exist.Remove();
            }
            foreach (var s in ConnectionStrings)
            {
                var node = new XElement(SectionKey);
                node.SetAttributeValue(KeyAttribute, s.Key);
                node.SetAttributeValue(ValueAttribute, s.Value);
                xml.Add(node);
            }
        }
    }
}