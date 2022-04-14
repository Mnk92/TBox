using System;
using System.Linq;
using System.Xml.Linq;
using Mnk.Library.Common.Tools;

namespace Mnk.TBox.Core.PluginsShared.Tools
{
    public static class XmlExtensions
    {
        public static XElement AddNodeIfNotExist(this XElement owner, string path)
        {
            return path.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries)
                .Aggregate(owner,
                    (current, name) =>
                        current.Elements()
                        .SingleOrDefault(x => x.Name.LocalName.EqualsIgnoreCase(name)) ??
                        CreateElement(current, name));
        }

        public static XElement CreateElement(this XElement owner, string name)
        {
            var node = new XElement(name);
            owner.Add(node);
            return node;
        }

        public static XElement SetAttributeIfNeed(this XElement owner, string key, string value)
        {
            var exist = owner.Attribute(key);
            if (exist != null && string.Equals(exist.Value, value)) return owner;
            owner.SetAttributeValue(key, value);
            return owner;
        }
    }
}
