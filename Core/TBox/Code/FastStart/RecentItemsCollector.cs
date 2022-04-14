using System;
using System.Collections.Generic;
using System.Linq;
using Mnk.Library.Common.Models;
using Mnk.Library.Common.Tools;
using Mnk.TBox.Core.Contracts;
using Mnk.TBox.Locales.Localization.TBox;
using Mnk.TBox.Core.Application.Code.FastStart.Settings;
using Mnk.TBox.Core.Application.Code.Menu;
using Mnk.Library.WpfControls.Components.ButtonsView;
using Mnk.Library.WpfControls.Menu;
using Mnk.Library.WpfControls.Icons;

namespace Mnk.TBox.Core.Application.Code.FastStart
{
    class RecentItemsCollector : IMenuRunHandler
    {
        private readonly IConfigManager<Config> cm;
        private readonly IMenuItemsProvider menuItemsProvider;

        public RecentItemsCollector(IConfigManager<Config> cm, IMenuItemsProvider menuItemsProvider)
        {
            this.cm = cm;
            this.menuItemsProvider = menuItemsProvider;
            menuItemsProvider.OnRefresh += (o, e) => RefreshMenuItems();
            menuItemsProvider.OnRefreshItem += name => RefreshMenuItems();
            RefreshMenuItems();
        }

        private IList<MenuItemStatistic> MenuItems => cm.Config.FastStartConfig.MenuItems;

        public void Handle(UMenuItem item, string[] path)
        {
            //skip system menu items
            if (path.Length < 2) return;
            lock (MenuItems)
            {
                var realPath = string.Join(Environment.NewLine, path);
                foreach (var menuItem in MenuItems.Where(x => string.Equals(x.Path, realPath)))
                {
                    menuItem.Count++;
                    return;
                }
                MenuItems.Add(
                    new MenuItemStatistic
                    {
                        IsValid = true,
                        Icon = item.Icon ?? menuItemsProvider.GetRoot(path[0]).Icon,
                        OnClick = item.OnClick,
                        Path = realPath,
                        Count = 1
                    }
                    );
            }
        }

        public void GetStatistic(int count, IList<IButtonInfo> items, Action action)
        {
            lock (MenuItems)
            {
                var id = 0;
                foreach (var i in MenuItems.Where(x => x.IsValid).OrderBy(x => -x.Count).Take(count))
                {
                    var item = i;
                    items.Add(new ButtonInfo
                    {
                        Name = i.Path.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).LastOrDefault(),
                        Icon = i.Icon.ToImageSource(),
                        GroupName = TBoxLang.RecentActions,
                        Handler = (o, e) =>
                        {
                            action();
                            item.OnClick(e);
                        },
                        Order = ++id
                    });
                }
            }
        }

        public void CollectUserActions(IEnumerable<MenuItemsSequence> checkedItems, IList<IButtonInfo> items, Action action)
        {
            foreach (var i in CollectUserActions(checkedItems))
            {
                var item = i;
                items.Add(new ButtonInfo
                {
                    Name = item.Header,
                    GroupName = TBoxLang.UserActions,
                    Icon = item.Icon.ToImageSource(),
                    Handler = (o, e) =>
                    {
                        action();
                        item.OnClick(new NonUserRunContext());
                    }
                });
            }
        }

        public IEnumerable<UMenuItem> CollectUserActions(IEnumerable<MenuItemsSequence> checkedItems)
        {
            foreach (var item in checkedItems)
            {
                var selected = item.MenuItems.CheckedItems.ToArray();
                if (!selected.Any()) continue;
                var founded = selected.Select(o =>
                    new Pair<UMenuItem, UMenuItem>(
                        menuItemsProvider.Get(o.Key),
                        menuItemsProvider.GetRoot(o.Key.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault())
                        )).ToArray();
                if (founded.Any(x => x.Key == null || x.Value == null)) continue;
                var last = founded.LastOrDefault(x => x.Key.Icon != null || x.Value.Icon != null);
                if (last == null) continue;
                yield return new UMenuItem
                {
                    Header = item.Key,
                    Icon = (last.Key.Icon ?? last.Value.Icon),
                    OnClick = o => founded.ForEach(x => x.Key.OnClick(new NonUserRunContext()))
                };
            }
        }

        private void RefreshMenuItems()
        {
            foreach (var item in MenuItems)
            {
                var menu = menuItemsProvider.Get(item.Path);
                if (menu == null)
                {
                    item.IsValid = false;
                    continue;
                }
                var root = item.Path.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).First();
                item.IsValid = true;
                item.Icon = menu.Icon ?? menuItemsProvider.GetRoot(root).Icon;
                item.OnClick = menu.OnClick;
            }
        }

    }
}
