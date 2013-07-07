using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Mnk.Library.CodePlex;
using Mnk.Library.Common.Log;
using Mnk.Library.Common.Tools;
using Mnk.Library.WpfControls;
using Mnk.TBox.Core.Contracts;
using LightInject;
using Mnk.TBox.Locales.Localization.TBox;
using Mnk.TBox.Core.Application.Code.AutoUpdate;
using Mnk.TBox.Core.Application.Code.FastStart;
using Mnk.TBox.Core.Application.Code.Managers;
using Mnk.TBox.Core.Application.Code.Menu;
using Mnk.TBox.Core.Application.Code.Objects;
using Mnk.TBox.Core.Application.Forms;
using Mnk.Library.WpfControls.Components.ButtonsView;
using Mnk.Library.WpfWinForms;
using Button = System.Windows.Controls.Button;

namespace Mnk.TBox.Core.Application.Code
{
	class UiConfigurator : IDisposable
	{
		private static readonly ILog InfoLog = LogManager.GetInfoLogger<UiConfigurator>();
		private readonly PluginsSettings pluginsSettings;
		private readonly IMenuItemsProvider menuItemsProvider;
		private readonly ControlsMan controlsMan;
		private readonly MenuMan menuMan;
		private NotifyIconMan notifyIconMan;
		private bool menuRebuildHolded = true;
		private Config cfg;
		public FastStartShower FastStartShower { get; set; }


		public UiConfigurator(
			GroupedList view, 
			ContentControl pluginsBack,
			Button btnBack,
			IServiceFactory container,
			IEnumerable<UMenuItem> menuItems,
			IEnumerable<EnginePluginInfo> existWindows,
			Action<EnginePluginInfo> onPluginSettingsChanged
			)
		{
			pluginsSettings = container.GetInstance<PluginsSettings>();
			menuItemsProvider = container.GetInstance<IMenuItemsProvider>();
			controlsMan = new ControlsMan(view, pluginsBack, btnBack, onPluginSettingsChanged);
			var main = ((MainWindow) System.Windows.Application.Current.MainWindow);
			FastStartShower = new FastStartShower(
				() =>
				{
					main.ShowAndActivate();
					main.GoBack();
				},
				() =>
				{
					main.ShowAndActivate();
					controlsMan.GoTo(TBoxLang.FastStart);
				});

			if (existWindows != null)
			{
				foreach (var w in existWindows)
				{
					controlsMan.Add(w);
				}
			}
			menuMan = new MenuMan(menuItems.ToArray(), container.GetInstance<MenuCallsVisitor>());
			pluginsSettings.EnableHotKeys += (o,e)=>Configure();
		}

		public void Init(Window owner, Config config, IAutoUpdater appUpdater, IEnumerable<EnginePluginInfo> toAdd)
		{
			Mt.Do(Sync, () =>
			{
				cfg = config;
				pluginsSettings.Init(appUpdater);
				pluginsSettings.Collection.Clear();
				pluginsSettings.Collection.AddRange(toAdd.OrderBy(x=>x.Name));
				pluginsSettings.Load(config);
			});
		}

		public void ShowTrayIcon(Window owner)
		{
			Mt.Do(Sync, () =>
			{
				notifyIconMan = new NotifyIconMan(owner, Properties.Resources.Icon, o => FastStartShower.Show());
				notifyIconMan.NotifyIcon.MouseClick += NotifyIconOnMouseClick;
			});
			DoHoldOperation(() =>{});
		}

		private void NotifyIconOnMouseClick(MouseButton mouseButton)
		{
			if (mouseButton == MouseButton.Left && cfg.ShowSettingsByTraySingleClick)
			{
				FastStartShower.Show();
			}
		}

		public void Configure()
		{
			if (notifyIconMan == null || menuRebuildHolded) return;
			var time = Environment.TickCount;
			Mt.Do(Sync, () =>
				{
					var main = ((MainWindow)System.Windows.Application.Current.MainWindow);
					menuItemsProvider.Refresh(menuMan.MenuItems.ToArray());
					menuMan.SetMenuItems(TBoxLang.UserActions,
						main.RecentItemsCollector.CollectUserActions(cfg.FastStartConfig.MenuItemsSequence.CheckedItems));
					menuItemsProvider.Create(menuMan.MenuItems.ToArray());
					notifyIconMan.SetMenuItems(menuMan.MenuItems, cfg.UseMenuWithIcons);
					notifyIconMan.NotifyIcon.HoverText = string.Format(TBoxLang.ToolTipTemplate, menuMan.Count);
					controlsMan.Refresh();
			});
			InfoLog.Write("Refresh menu and plugins settings time: {0}", Environment.TickCount - time);
		}

		public void ConfigurePlugin(IPlugin plugin, string name)
		{
			var cplg = plugin as IConfigurablePlugin;
			if (notifyIconMan == null || menuRebuildHolded) return;
			Mt.Do(Sync, () => {
				UMenuItem[] pluginMenu = null;
				if (cplg != null)
				{
					cplg.OnRebuildMenu();
					pluginMenu = cplg.Menu.ToArray();
					menuMan.SetMenuItems(name, pluginMenu);
				}
				else
				{
					pluginMenu = menuMan.GetMenuItems(name).ToArray();
				}
				menuItemsProvider.Refresh(name, pluginMenu);
				notifyIconMan.UpdateSubMenuItems(name, pluginMenu);
				notifyIconMan.NotifyIcon.HoverText = string.Format(TBoxLang.ToolTipTemplate, menuMan.Count);
			});
		}

		public void DoHoldOperation(Action a)
		{
			try
			{
				menuRebuildHolded = true;
				a();
			}
			finally
			{
				menuRebuildHolded = false;
				Configure();
			}
		}

		public void SetPluginCheck(string key, bool value)
		{
			pluginsSettings.Collection.SetCheck(key, value);
		}

		public void Load(Config config)
		{
			cfg = config;
			Mt.Do(Sync, ()=>pluginsSettings.Load(config));
		}

		public void Save(Config config)
		{
			Configure();
			Mt.Do(Sync, controlsMan.Refresh);
			pluginsSettings.Save(config);
		}

		public EnginePluginInfo InitPlugin(IPlugin plugin, string key)
		{
			var info = pluginsSettings.Collection.First(x => key.EqualsIgnoreCase(x.Key));
			info.Plugin = plugin;
			plugin.Icon = info.Icon;
			return info;
		}

		public void Remove(string key)
		{
			var info = pluginsSettings.Collection.First(x => key.EqualsIgnoreCase(x.Key));
			menuMan.Remove(info.Name);
			controlsMan.Remove(key);
		}

		public IEnumerable<string> GetPluginsStates(IList<string> toAdd, IList<string> toRemove, IList<string> disabledBefore, PluginsMan plugMan )
		{
			foreach (var t in pluginsSettings.Collection)
			{
				var name = t.Key;
				var enabled = t.IsChecked;
				if (enabled == disabledBefore.Contains(name))
				{
					(enabled ? toAdd : toRemove).Add(name);
				}
				else if (enabled)
				{
					var plugin = plugMan.Get(name);
					menuMan.Change(t.Name, t.Icon, plugin.Menu);
				}
				if (!enabled)
				{
					yield return name;
				}
			}
		}

		public void Dispose()
		{
			if (notifyIconMan!=null) notifyIconMan.Dispose();
			pluginsSettings.Dispose();
		}

		public Control Sync { get { return pluginsSettings; } }

		public void InitUi(List<EnginePluginInfo> ret)
		{
			foreach (var p in ret.OrderBy(x=>x.Name.ToString(CultureInfo.InvariantCulture)))
			{
				if (p.Settings != null) controlsMan.Add(p);
				if (p.Menu != null) menuMan.Add(p.Name, p.Icon, p.Menu);
			}
		}
	}
}
