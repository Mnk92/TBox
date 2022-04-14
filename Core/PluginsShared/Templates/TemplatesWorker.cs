using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Mnk.Library.Common.Tools;
using Mnk.Library.Common.UI.Model;

[assembly: InternalsVisibleTo("UnitTests")]
namespace Mnk.TBox.Core.PluginsShared.Templates
{
	public class TemplatesWorker
	{
		private const string Item = "item";
		private KeyValuePair<string, string> template;
		public TemplatesWorker(string itemTemplate)
		{
			if (string.IsNullOrWhiteSpace(itemTemplate) ||
				!itemTemplate.Contains(Item))
				throw new ArgumentException("Item template should contains substring: " + Item);
			template = GetTemplateParams(itemTemplate);
		}

		private static KeyValuePair<string, string> GetTemplateParams(string template)
		{
			var id = template.IndexOf(Item, StringComparison.Ordinal);
			return new KeyValuePair<string, string>(
				template.Substring(0, id),
				template.Substring(id + Item.Length, template.Length - id - Item.Length)
				);
		}

		private static string GetIfExist(IEnumerable<PairData> collection, string s)
		{
			var item = collection
				.FirstOrDefault(x => x.Key.EqualsIgnoreCase(s));
			return item == null ? string.Empty : item.Value;
		}

		public IEnumerable<PairData> GetValues(string sourcePath, IEnumerable<PairData> collection)
		{
			var keysCollector = new KeysCollector(template.Key, template.Value);
			return new List<PairData>(
					keysCollector.GetAllKeys(sourcePath)
					.Select(x => new PairData
					{
						Key = x,
						Value = GetIfExist(collection, x)
					}));
		}

		public IEnumerable<PairData> GetStringValues(string value, ObservableCollection<PairData> collection)
		{
			var keysCollector = new KeysCollector(template.Key, template.Value);
			return new List<PairData>(
					keysCollector.GetKnownKeys(value)
					.Select(x => new PairData
					{
						Key = x,
						Value = GetIfExist(collection, x)
					}));
		}


		public string FillString(string value, IEnumerable<PairData> values)
		{
			var knownValues = new KnownValues(template.Key, template.Value, values);
			var stringFiller = new StringFiller(knownValues.Collection);
			knownValues.Prepare(stringFiller);
			return stringFiller.Fill(value);
		}

		public void Copy(string source, string destination, IEnumerable<PairData> values)
		{
			var knownValues = new KnownValues(template.Key, template.Value, values);
			var stringFiller = new StringFiller(knownValues.Collection);
			var fileSystemProcessor = new FileSystemProcessor(stringFiller);
			knownValues.Prepare(stringFiller);
			fileSystemProcessor.Copy(source, destination);
		}

	}
}
