using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Mnk.Library.Common.Log;
using Mnk.Library.Common.Tools;

namespace Mnk.TBox.Core.PluginsShared.Tools
{
	public class FeatureToggler
	{
		private static readonly ILog Log = LogManager.GetLogger<FeatureToggler>();
		private readonly string valueKey;
		private readonly string keyKey;
		private readonly string sectionName;
		private readonly string xmlSelector;

		public FeatureToggler(string xmlSelector = "appSettings", string keyKey = "key", string valueKey="value", string sectionName = "add")
		{
			this.valueKey = valueKey;
			this.sectionName = sectionName;
			this.keyKey = keyKey;
			this.xmlSelector = xmlSelector;
		}

		public void Execute(IEnumerable<string> files, KeyValuePair<string, string>[] options )
		{
			if (files == null)return;
			foreach (var file in files)
			{
				try
				{
					ProcessFile(file, options);
				}
				catch (Exception ex)
				{
					Log.Write(ex, "Error processing file: " + file );
				}
			}
		}

		private void ProcessFile(string path, IEnumerable<KeyValuePair<string, string>> options)
		{
			var doc = XDocument.Load(path, LoadOptions.PreserveWhitespace);
            var owner = doc.Root.AddNodeIfNotExist(xmlSelector);
			if (options.Count(o => CreateNode(owner, sectionName, o.Key, o.Value)) > 0)
			{
				doc.Save(path);
			}
		}

		private bool CreateNode(XElement owner, string path, string key, string value)
		{
			var node = owner.Elements()
				.Where(x => x.Name.LocalName.EqualsIgnoreCase(path))
				.LastOrDefault(x =>
				                 x.Attributes()
									 .Where(a => a.Name.LocalName.EqualsIgnoreCase(keyKey))
					                 .Any(a => a.Value.EqualsIgnoreCase(key))
				);
			if(node == null)
			{
                node = owner.CreateElement(path);
				node.SetAttributeValue(keyKey, key);
				node.SetAttributeValue(valueKey, value);
				return true;
			}
			var exist = node.Attribute(valueKey);
			if (exist!=null && string.Equals(exist.Value, value)) return false;
			node.SetAttributeValue(valueKey, value);
			return true;
		}
	}
}
