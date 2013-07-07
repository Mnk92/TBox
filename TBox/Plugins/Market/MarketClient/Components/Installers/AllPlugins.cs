using System;
using System.Linq;
using System.Windows;
using Mnk.Library.Common.MT;
using Mnk.Library.WpfControls;
using Mnk.TBox.Plugins.Market.Client.ServiceReference;
using Mnk.Library.WpfControls.Dialogs;
using Mnk.TBox.Plugins.Market.Client.Code;

namespace Mnk.TBox.Plugins.Market.Client.Components.Installers
{
	class AllPlugins : Installer
	{
		public AllPlugins()
		{
			InitializeComponent();
			ActionCaption = "Download";
			Synchronizer.OnReloadData += OnReloadData;
			OnAction += DoOnAction;
			OnNameSelectionChanged += DoOnNameSelectionChanged;
		}

		private void DoOnNameSelectionChanged(object sender, EventArgs e)
		{
			Synchronizer.Do(ReloadTable);
		}

		public void ReloadTable(IMarketService service)
		{
			var plugin = new Plugin();
			if (!string.IsNullOrWhiteSpace(TypeName))
			{
				plugin.Type = TypeName;
			}
			if (!string.IsNullOrWhiteSpace(AuthorName))
			{
				plugin.Author = AuthorName;
			}
			Items = service.Plugin_GetList(plugin, 0, int.MaxValue, true);
		}

		public void OnReloadData(IMarketService service)
		{
			Mt.Do(this, () =>{
					Types = new[] { string.Empty }.Union(Synchronizer.Types).ToArray();
					Authors = new[] { string.Empty }.Union(Synchronizer.Authors).ToArray();
					ReloadTable(service);
				});
		}

		private void DoOnAction(object sender, EventArgs e)
		{
			Synchronizer.Do(service => DialogsCache.ShowProgress((updater) => DoUploading(service, updater), "Download plugins", null));
		}

		private void DoUploading(IMarketService service, IUpdater uploader)
		{
			var plugins = Items;
			var size = (float)plugins.Sum(plugin => plugin.Size);
			var current = 0;
			foreach (var plugin in plugins)
			{
				var caption = string.Format("Downloading plugin: {0}-{1}", plugin.Author, plugin.Name);
				uploader.Update(caption, (size==0) ? 0 : current / size);
				var data = service.Plugin_Download(new DownloadContract
				{
					Author = plugin.Author,
					Name = plugin.Name
				});
				if (data.Length == 0)
				{
					MessageBox.Show(string.Format("Error downloading plugin: '{0}'", plugin.Name),
						string.Empty, MessageBoxButton.OK, MessageBoxImage.Exclamation);
				}
				else
				{
					var tmp = current;
					if (Synchronizer.PluginFiles.Save(plugin, data, (value) => {
					                                                		tmp += value;
																			uploader.Update(caption, tmp / size);
					                                                	}))
					{
						var tmpPlugin = plugin;
						Mt.Do(this, () => Synchronizer.DoOnInstall(tmpPlugin));
					}
					current = tmp;
				}
			}
		}

	}
}
