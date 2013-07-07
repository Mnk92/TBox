using System;
using System.Collections.Generic;
using System.Linq;
using Mnk.TBox.Core.Contracts;
using Mnk.TBox.Plugins.Market.Client.ServiceReference;
using Mnk.TBox.Plugins.Market.Client.Code;

namespace Mnk.TBox.Plugins.Market.Client.Components.Installers
{
	class InstalledPlugins : Installer
	{
		private List<Plugin> plugins;

		public InstalledPlugins()
		{
			InitializeComponent();
			ActionCaption = "Uninstall";
			Synchronizer.OnInstall += OnInstall;
			OnAction += DoOnAction;
			OnNameSelectionChanged += DoOnNameSelectionChanged;
		}

		private void DoOnNameSelectionChanged(object sender, EventArgs e)
		{
			UpdatePluginsList();
		}

		private void UpdatePluginsList()
		{
			var plugins = this.plugins.ToArray();
			if (!string.IsNullOrWhiteSpace(TypeName))
			{
				plugins = plugins.Where(x => x.Type == TypeName).ToArray();
			}
			if (!string.IsNullOrWhiteSpace(AuthorName))
			{
				plugins = plugins.Where(x => x.Author == AuthorName).ToArray();
			}
			Items =plugins;
		}

		private void ReloadData()
		{
			if (plugins.Count > 0)
			{
				Types = new[] { string.Empty }.Union(
					plugins.Select(x => x.Type).Distinct().OrderBy(x => x)).
					ToArray();
				Authors = new[] { string.Empty }.Union(
					plugins.Select(x => x.Author).Distinct().OrderBy(x => x)).
					ToArray();
			}
			else
			{
				Types = Authors = new string[] { };
			}
			UpdatePluginsList();
		}

		private void OnInstall(Plugin plugin)
		{
			plugins.Add(plugin);
			ReloadData();
		}

		private void DoOnAction(object sender, EventArgs e )
		{
			foreach (var plugin in Items)
			{
				Synchronizer.PluginFiles.Delete(plugin);
				plugins.Remove(plugin);
				Synchronizer.DoOnUninstall(plugin);
			}
			ReloadData();
		}

		public void Fill(Config config)
		{
			if (plugins == null)
			{
				plugins = config.InstalledPlugins.Plugins;
				ReloadData();
			}
		}

		public void Read(Config config)
		{
			config.InstalledPlugins.Plugins = plugins;
		}

	}
}
