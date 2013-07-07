using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Mnk.Library.Common.Models;

namespace Mnk.TBox.Core.Application.Code.Managers
{
	class WarmingUpManager
	{
		private readonly IList<Pair<Type, bool>> items = new List<Pair<Type, bool>>();

		public void CreateAll()
		{
			lock (items)
			{
				foreach (var item in items)
				{
					if (item.Value) continue;
					var type = item.Key;
					var t = new Thread(() => Activator.CreateInstance(type));
					t.SetApartmentState(ApartmentState.STA);
					t.Start();
					item.Value = true;
				}
			}
		}

		public void TryAdd(Type t)
		{
			lock (items)
			{
				if (items.Any(item => item.Key == t))return;
				items.Add(new Pair<Type, bool>(t, false));
			}
		}
	}
}
