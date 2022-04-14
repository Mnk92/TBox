using Mnk.Library.Common.MT;
using Mnk.Library.WpfControls;
using Mnk.Library.WpfControls.Dialogs;
using Mnk.TBox.Plugins.Market.Client.Code;
using Mnk.TBox.Plugins.Market.Contracts;

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

        public void ReloadTable(MarketService.MarketServiceClient client)
        {
            Items = client.GetPluginsList(new PluginSearch
            {
                Type = TypeName,
                Author = AuthorName,
                Offset = 0,
                Count = int.MaxValue,
                OnlyPlugins = true
            }).Items.ToArray();
        }

        public void OnReloadData(MarketService.MarketServiceClient client)
        {
            Mt.Do(this, () =>
            {
                Types = new[] { string.Empty }.Union(Synchronizer.Types).ToArray();
                Authors = new[] { string.Empty }.Union(Synchronizer.Authors).ToArray();
                ReloadTable(client);
            });
        }

        private void DoOnAction(object sender, EventArgs e)
        {
            Synchronizer.Do(service => DialogsCache.ShowProgress((updater) => DoUploading(service, updater), "Download plugins", null));
        }

        private void DoUploading(MarketService.MarketServiceClient client, IUpdater uploader)
        {
            var plugins = Items;
            var size = plugins.Sum(plugin => (float)plugin.Size);
            var current = 0;
            foreach (var plugin in plugins)
            {
                var caption = $"Downloading plugin: {plugin.Author}-{plugin.Name}";
                uploader.Update(caption, (size == 0) ? 0 : current / size);
                using var data = client.DownloadPlugin(new DownloadPluginMessage { PluginId = plugin.PluginId });
                var tmp = current;
                if (Synchronizer.PluginFiles.Save(plugin, data.ResponseStream, (value) =>
                {
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
