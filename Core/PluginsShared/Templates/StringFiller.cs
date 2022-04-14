using System;
using System.Collections.Generic;
using System.Linq;

namespace Mnk.TBox.Core.PluginsShared.Templates
{
	public sealed class StringFiller : IStringFiller
	{
		private const int MaxChanges = 0xffff;
		private readonly IDictionary<string, string> knownValues;
		public StringFiller(IDictionary<string, string> knownValues)
		{
			this.knownValues = knownValues;
		}

		public bool CanFill(string template)
		{
			if (string.IsNullOrWhiteSpace(template)) return false;
			return knownValues
				.Any(value => template.IndexOf(value.Key, StringComparison.CurrentCultureIgnoreCase) >= 0);
		}

		public string Fill(string template)
		{
			var i = 0;
			while (true)
			{
				if(!DoFill(ref template))break;
				if (++i > MaxChanges)
					throw new ArgumentException("To much replaces for template: " + template);
			}
			return template;
		}

		private bool DoFill(ref string template)
		{
			var changed = false;
			foreach (var value in knownValues)
			{
				if (template.IndexOf(value.Key, StringComparison.CurrentCultureIgnoreCase) < 0) 
					continue;
				changed = true;
				template = template.Replace(value.Key, value.Value);
			}
			return changed;
		}
	}
}
