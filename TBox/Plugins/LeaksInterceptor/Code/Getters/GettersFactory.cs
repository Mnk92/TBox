using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Mnk.TBox.Plugins.LeaksInterceptor.Code.Getters
{
	class GettersFactory
	{
		private readonly IDictionary<string, Type> items = new Dictionary<string, Type>();
		public GettersFactory()
		{
			var target = typeof (IGetter);
			foreach (var type in Assembly.GetAssembly(typeof(GettersFactory)).GetTypes()
									.Where(x => !x.IsAbstract && x.GetInterface(target.Name)!=null ))
			{
				items[type.Name] = type;
			}
		}

		public int Count{get { return items.Count; }}

		public IEnumerable<string> GetNames()
		{
			return items.Keys;
		}

		public IGetter Get(string name)
		{
			return (IGetter)Activator.CreateInstance(items[name]);
		}
	}
}
