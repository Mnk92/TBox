using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Mnk.Library.Common.Tools;
using Mnk.Library.WpfControls.Dialogs.Menu;
using Mnk.Library.WpfControls.Icons;
using Mnk.Library.WpfControls.Menu;

namespace Mnk.TBox.Core.Application.Code.Menu
{
    class MenuItemsProvider : IMenuItemsProvider
    {
        public event EventHandler OnRefresh;
        public event Action<string> OnRefreshItem;

        private readonly IDictionary<string, UMenuItem> items = new Dictionary<string, UMenuItem>();
        private readonly IDictionary<string, UMenuItem> root = new Dictionary<string, UMenuItem>();
        private IList<MenuDialogItem> menuDialogItems;

        public void Refresh(IList<UMenuItem> menuItems)
        {
            items.Clear();
            root.Clear();
            AddItems(string.Empty, menuItems);
        }

        public void Create(IList<UMenuItem> menuItems)
        {
            Refresh(menuItems);
            menuDialogItems = CreateMenuDialogItems(menuItems, null);

            OnRefresh(this, null);
        }

        public void Refresh(string name, IList<UMenuItem> menuItems)
        {
            RemoveExistSubitems(name);
            AddItems(name, menuItems);

            UpdateMenuDialogItems(name, menuItems);

            OnRefreshItem(name);
        }

        public UMenuItem Get(string path)
        {
            return items.ContainsKey(path) ? items[path] : null;
        }

        public UMenuItem GetRoot(string path)
        {
            return root.ContainsKey(path) ? root[path] : null;
        }

        public IEnumerable<UMenuItem> GetItems()
        {
            return items.Values;
        }

        public IList<MenuDialogItem> GetDialogItems()
        {
            return menuDialogItems;
        }

        private void RemoveExistSubitems(string name)
        {
            var target = name + Environment.NewLine;
            foreach (var key in items.Where(x => x.Key.StartsWith(target)).Select(x => x.Key).ToArray())
            {
                items.Remove(key);
            }
        }

        private void AddItems(string rootPath, IList<UMenuItem> menuItems)
        {
            if (!string.IsNullOrEmpty(rootPath))
            {
                rootPath += Environment.NewLine;
            }
            else
            {
                foreach (var item in menuItems)
                {
                    root[item.Header] = item;
                }
            }
            foreach (var item in menuItems)
            {
                if (item.OnClick != null)
                {
                    items[rootPath + item.Header] = item;
                }
                else if (item.Items.Any())
                {
                    AddItems(rootPath + item.Header, item.Items);
                }
            }
        }

        private static IList<MenuDialogItem> CreateMenuDialogItems(IEnumerable<UMenuItem> items, MenuDialogItem parent)
        {
            return new ObservableCollection<MenuDialogItem>(
                items
                .Where(x => !(x is USeparator))
                .Select(x =>
                {
                    var item = new MenuDialogItem(x.Header, parent);
                    if (x.Icon != null)
                    {
                        item.Icon = x.Icon.ToImageSource();
                    }
                    if (x.Items.Count > 0)
                    {
                        var children = CreateMenuDialogItems(x.Items, item);
                        if (children.Count == 0) return null;
                        item.Children.AddRange(children);
                    }
                    return item;
                })
                .Where(x => x != null)
                );
        }

        private void UpdateMenuDialogItems(string name, IEnumerable<UMenuItem> menuItems)
        {
            if (menuDialogItems == null) return;
            var item = menuDialogItems.FirstOrDefault(x => x.Name.EqualsIgnoreCase(name));
            if (item == null) return;
            item.Children.Clear();
            item.Children.AddRange(CreateMenuDialogItems(menuItems, item));
        }
    }
}
