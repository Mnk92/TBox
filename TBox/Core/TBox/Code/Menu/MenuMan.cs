using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Media;
using Mnk.Library.Common.Tools;
using Mnk.Library.WpfWinForms;

namespace Mnk.TBox.Core.Application.Code.Menu
{
	sealed class MenuMan 
	{
		private readonly MenuCallsVisitor visitor;
		private readonly int delta;
		private readonly List<UMenuItem> menuItems = new List<UMenuItem>(); 
		public MenuMan(UMenuItem[] items, MenuCallsVisitor visitor)
		{
			this.visitor = visitor;
			menuItems.AddRange(items);
			visitor.ApplyMenuItems(items);
			delta = menuItems.Count;
		}

		public IList<UMenuItem> MenuItems
		{
			get { return menuItems; }
		}

		public int Count
		{
			get { return menuItems.Count - delta; }
		}

		public void Add(string name, Icon icon, UMenuItem[] items)
		{
			if (items == null) return;
			var root = new UMenuItem { Header = name, Icon = icon};
			menuItems.Insert(root, x => x.Header, 0, menuItems.Count - delta);
			foreach (var item in items)
			{
				root.Items.Add(item);
			}
			visitor.ApplyMenuItems(root);
		}

		private UMenuItem Find(string name)
		{
			for (var i = 0; i < menuItems.Count - delta; i++)
			{
				var item = menuItems[i];
				if (item == null) continue;
				if (name.EqualsIgnoreCase(item.Header))
				{
					return item;
				}
			}
			return null;
		}

		public void Remove(string name)
		{
			var item = Find(name);
			visitor.RemoveMenuItem(item);
			menuItems.Remove(item);
		}

		public void Change(string name, Icon icon, UMenuItem[] items)
		{
			var root = Find(name);
			if (root == null)
			{
				Add(name, icon, items);
				return;
			}
			root.Items.Clear();
			visitor.RemoveMenuItem(root);
			foreach (var item in items)
			{
				root.Items.Add(item);
			}
			visitor.ApplyMenuItems(root);
		}

		public IEnumerable<UMenuItem> GetMenuItems(string name)
		{
			return menuItems.First(x => string.Equals(x.Header, name)).Items;
		}

		public void SetMenuItems(string name, IEnumerable<UMenuItem> items)
		{
			menuItems.First(x => string.Equals(x.Header, name)).Items = new List<UMenuItem>(items);
		}
	}
}
