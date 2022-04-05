using System;
using System.Collections.Generic;
using System.Linq;
using Mnk.TBox.Locales.Localization.TBox;
using Mnk.TBox.Core.Application.Code.HotKeys.Settings;
using Mnk.TBox.Core.Application.Code.Menu;
using Mnk.Library.WpfControls.Components.Units;
using Mnk.Library.WpfControls.Tools;
using Mnk.Library.WpfWinForms;
using Mnk.Library.WpfWinForms.GlobalHotkeys;

namespace Mnk.TBox.Core.Application.Code.HotKeys
{
    sealed class HotkeysManager : IDisposable
    {
        private GlobalHotkeysManager globalHotkeysManager = null;
        private readonly CheckableListBoxUnit view;
        private readonly IMenuItemsProvider menuItemsProvider;
        private HotkeyTasks config;
        private HotkeyTasks originalConfig;
        public HotkeysManager(CheckableListBoxUnit view, IMenuItemsProvider menuItemsProvider)
        {
            this.view = view;
            this.menuItemsProvider = menuItemsProvider;
            menuItemsProvider.OnRefresh += (o, e) => Refresh();
            menuItemsProvider.OnRefreshItem += RefreshItem;
        }

        public void Refresh()
        {
            Dispose();
            ClearHotKeys(menuItemsProvider.GetItems());
            view.ConfigureInputMenuItem(TBoxLang.HotKeys,
                originalConfig.Tasks,
                menuItemsProvider.GetDialogItems(),
                validator: x => true);
            if (!config.IsEnabled) return;
            globalHotkeysManager = new GlobalHotkeysManager();
            foreach (var item in config.Tasks.CheckedItems.OrderBy(x => x.Key))
            {
                UpdateMenu(item);
            }
        }

        internal void RefreshItem(string name)
        {
            ClearHotKeys(menuItemsProvider.GetItems());
            if (!config.IsEnabled)
            {
                return;
            }
            foreach (var item in config.Tasks.CheckedItems.OrderBy(x => x.Key).Where(x => x.Key.StartsWith(name, StringComparison.OrdinalIgnoreCase)))
            {
                UpdateMenu(item);
            }
        }

        private static void ClearHotKeys(IEnumerable<UMenuItem> menuItems)
        {
            foreach (var item in menuItems)
            {
                item.HotKey = null;
                ClearHotKeys(item.Items);
            }
        }

        private void UpdateMenu(HotkeyTask item)
        {
            var menu = menuItemsProvider.Get(item.Key);
            if (menu == null || menu.Items.Count > 0 || item.HotKey == null) return;
            menu.HotKey = item.HotKey.ToString();
            globalHotkeysManager.RegisterHotkey(item.HotKey, () => menu.OnClick(null));
        }

        public void OnConfigUpdated(HotkeyTasks cfg)
        {
            originalConfig = cfg;
            config = cfg.Clone();
        }

        public void Dispose()
        {
            if (globalHotkeysManager == null) return;
            globalHotkeysManager.Dispose();
            globalHotkeysManager = null;
        }
    }
}
