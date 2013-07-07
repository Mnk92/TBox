using System;
using System.Collections.Generic;

namespace Mnk.TBox.Plugins.LocalizationTool.Code.Parsers
{
	class KeyValueParser
	{
		private const string KeyId = "$(key)";
		private const string ValueId = "$(value)";
		private readonly string first;
		private readonly string divider;
		private readonly string last;

		public KeyValueParser(string template)
		{
			template = template.Replace("\\t", "\t");
			var keyId = template.IndexOf(KeyId, StringComparison.InvariantCultureIgnoreCase);
			if (keyId == -1)
				throw new ArgumentException("Invalid key template in parameter template: " + template);
			var valueId = template.IndexOf(ValueId, StringComparison.InvariantCultureIgnoreCase);
			if (valueId == -1)
				throw new ArgumentException("Invalid value template in parameter template: " + template);
			if(keyId > valueId)
				throw new ArgumentException("Key should be before value");
			first = template.Substring(0, keyId);
			divider = template.Substring(keyId + KeyId.Length, valueId - keyId - KeyId.Length);
			last = template.Substring(valueId + ValueId.Length, template.Length - valueId - ValueId.Length);
			if (string.IsNullOrEmpty(divider))
				throw new ArgumentException("Divider can't be empty");
		}

		public KeyValuePair<string, string> Load(string str)
		{
			var firstId = str.IndexOf(first, StringComparison.InvariantCultureIgnoreCase);
			if (firstId != -1)
			{
				var dividerId = str.IndexOf(divider, firstId + first.Length, StringComparison.InvariantCultureIgnoreCase);
				if (dividerId != -1)
				{
					var lastId = string.IsNullOrEmpty(last) ? 
						str.Length :
						str.LastIndexOf(last, StringComparison.InvariantCultureIgnoreCase);
					if (lastId != -1)
					{
						var key = str.Substring(firstId + first.Length, dividerId - firstId - first.Length);
						var value = str.Substring(dividerId + divider.Length, lastId - dividerId - divider.Length);
						return new KeyValuePair<string, string>(key, value);
					}
				}
			}
			throw new ArgumentException("Invalid line format: " + str);
		}

		public string Save(string key, string value)
		{
			return first + key + divider + value + last;
		}
	}
}
