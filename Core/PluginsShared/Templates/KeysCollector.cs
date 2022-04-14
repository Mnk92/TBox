using System;
using System.Collections.Generic;
using System.IO;
using Mnk.Library.Common.Log;
using Mnk.Library.Common.Tools;

namespace Mnk.TBox.Core.PluginsShared.Templates
{
	class KeysCollector
	{
		private static readonly ILog Log = LogManager.GetLogger<KeysCollector>();
		private readonly string begin;
		private readonly string end;

		public KeysCollector(string begin, string end)
		{
			this.begin = begin;
			this.end = end;
		}

		public IEnumerable<string> GetAllKeys(string source)
		{
			return GetAllKeys(source, new HashSet<string>());
		}

		private IEnumerable<string> GetAllKeys(string source, ISet<string> set)
		{
			var info = new DirectoryInfo(source);
			set.UnionWith(GetKnownKeys(info.Name));
			foreach (var dir in info.SafeEnumerateDirectories(Log))
			{
				set.UnionWith(GetKnownKeys(dir.Name));
				set.UnionWith(GetAllKeys(dir.FullName, set));
			}
			foreach (var file in info.SafeEnumerateFiles(Log))
			{
				set.UnionWith(GetKnownKeys(file.Name));
				set.UnionWith(GetKnownKeys(File.ReadAllText(file.FullName)));
			}
			return set;
		}

		public IEnumerable<string> GetKnownKeys(string text)
		{
			if(string.IsNullOrEmpty(text))yield break;
			var first = 0;
			while (true)
			{
				first = text.IndexOf(begin, first, StringComparison.Ordinal);
				if (first == -1) yield break;
				first += begin.Length;
				var last = text.IndexOf(end, first, StringComparison.Ordinal);
				if (first == -1) yield break;
				yield return text.Substring(first, last-first);
				first = last + end.Length;
			}
		}

	}
}
