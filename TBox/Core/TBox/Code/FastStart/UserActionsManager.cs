using System;
using Mnk.TBox.Locales.Localization.TBox;
using Mnk.TBox.Core.Application.Code.FastStart.Settings;
using Mnk.TBox.Core.Application.Code.Menu;
using Mnk.TBox.Core.Application.Forms;
using Mnk.Library.WpfControls.Components.Units;
using Mnk.Library.WpfControls.Tools;

namespace Mnk.TBox.Core.Application.Code.FastStart
{
	sealed class UserActionsManager : IDisposable
	{
		private readonly CheckableListBoxUnit view;
		private readonly IMenuItemsProvider menuItemsProvider;
		private FastStartConfig originalConfig;
		private readonly UserActionsDialog userActionsDialog = new UserActionsDialog();
		public UserActionsManager(CheckableListBoxUnit view, IMenuItemsProvider menuItemsProvider)
		{
			this.view = view;
			this.menuItemsProvider = menuItemsProvider;
			menuItemsProvider.OnRefresh += Refresh;
			menuItemsProvider.OnRefreshItem += RefreshItem;
		}

		public void Refresh(object sender, EventArgs e)
		{
			view.ConfigureInputText(TBoxLang.UserActions, 
				originalConfig.MenuItemsSequence, 
				validator: x => true);
		}

		internal void RefreshItem(string name)
		{
		}

		public void OnConfigUpdated(FastStartConfig cfg)
		{
			originalConfig = cfg;
		}

		public void ShowDialog(MenuItemsSequence cfg)
		{
			userActionsDialog.ShowDialog(cfg, menuItemsProvider);
		}

		public void Dispose()
		{
			userActionsDialog.Dispose();
		}
	}
}
