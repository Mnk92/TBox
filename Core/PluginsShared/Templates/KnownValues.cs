using System;
using System.Collections.Generic;
using System.Linq;
using Mnk.Library.Common.UI.Model;

namespace Mnk.TBox.Core.PluginsShared.Templates
{
	class KnownValues : IKnownValues
	{
		private const int MaxChanges = 0xffff;
		public IDictionary<string, string> Collection { get; private set; }
		public KnownValues(string begin, string end, IEnumerable<PairData> collection )
		{
			Collection = new Dictionary<string, string>(
				collection.ToDictionary(
				x=>begin + x.Key + end,
				x=>x.Value
				));
		}

		public void Prepare(IStringFiller filler)
		{
			var i = 0;
			while (DoPrepare(filler))
			{
				if (++i > MaxChanges)
					throw new ArgumentException("To much replaces for KnownValues!");
			}
		}

		private bool DoPrepare(IStringFiller filler)
		{
			foreach (var item in Collection
				.Where(item => filler.CanFill(item.Value)))
			{
				Collection[item.Key] = filler.Fill(item.Value);
				return true;
			}
			return false;
		}
	}
}
