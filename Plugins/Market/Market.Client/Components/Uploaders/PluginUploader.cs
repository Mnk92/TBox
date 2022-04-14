using System.IO;
using System.Windows;
using Google.Protobuf;
using Grpc.Core;
using Mnk.Library.Common.MT;
using Mnk.Library.Common.SaveLoad;
using Mnk.Library.WpfControls.Dialogs;
using Mnk.TBox.Plugins.Market.Client.Code;
using Mnk.TBox.Plugins.Market.Contracts;

namespace Mnk.TBox.Plugins.Market.Client.Components.Uploaders
{
    public class PluginUploader
    {
        private readonly Func<Plugin> creator;
        public PluginUploader(Func<Plugin> creator)
        {
            this.creator = creator;
        }

        private void CheckPaths(IList<string> paths, bool allowNoExistFile)

        {
            foreach (var path in paths.Where(path => string.IsNullOrWhiteSpace(path) || !File.Exists(path)))
            {
                if (!allowNoExistFile)
                {
                    throw new Exception($"File '{path}' not exist!");
                }
            }
        }

        private BoolResult CreateContract(AsyncClientStreamingCall<UploadPluginStream, BoolResult> call, Plugin plugin, IList<string> paths)
        {
            var length = ExtFile.LoadDirectoryFiles(paths, out var pairs, out var stream);
            using var streamWithProgress = new StreamWithProgress(stream);
            streamWithProgress.ProgressChanged += ProgressChanged;
            var ret = new UploadPluginStream
            {
                Plugin = plugin,
                Length = length,
                Descriptions = { pairs.ToDictionary(x => x.Key, x => x.Value) },
                Data = ByteString.FromStream(streamWithProgress)
            };
            call.RequestStream.WriteAsync(ret).Wait();
            call.ResponseAsync.Wait();
            return call.ResponseAsync.Result;
        }

        private void ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            updater.Update(string.Empty, e.Length == 0 ? 0 : e.BytesRead / (float)e.Length);
        }

        private static void ShowError(string text)
        {
            MessageBox.Show(text, "Plugins error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }

        private IUpdater updater;
        private void DoWithProgress(string caption, Action action)
        {
            DialogsCache.ShowProgress(u =>
            {
                this.updater = u;
                try
                {
                    action();
                }
                finally
                {
                    Synchronizer.RefreshTables(u);
                }
            }, caption, null);
        }

        public void Upload(string name, string[] paths)
        {
            var ret = new BoolResult();
            DoWithProgress(
                $"Uploading plugin: '{name}'",
                () => Synchronizer.Do(service =>
                {
                    CheckPaths(paths, false);
                    using var call = service.UploadPlugin();
                    ret = CreateContract(call, creator(), paths);
                }));
            if (!ret.Success)
            {
                ShowError($"Can't upload: '{name}'. Already exist!");
            }
        }

        public void Upgrade(string name, string[] paths)
        {
            var ret = new BoolResult();
            DoWithProgress(
                $"Upgrading plugin: '{name}'",
                            () => Synchronizer.Do(service =>
                            {
                                CheckPaths(paths, false);
                                using var call = service.UpgradePlugin();
                                ret = CreateContract(call, creator(), paths);
                            }));
            if (!ret.Success)
            {
                ShowError($"Can't upgrade: '{name}'. Not exist!");
            }
        }

        public void Delete(string name)
        {
            if (MessageBox.Show($"Are you want delete '{name}'?",
                                name, MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes) return;
            var ret = false;
            DoWithProgress(
                $"Delete file: '{name}'",
                () => Synchronizer.Do(client =>
                                      ret = client.DeletePlugin(creator()).Success));
            if (!ret)
            {
                ShowError($"Can't delete: '{name}'.");
            }
        }

        public void Refresh()
        {
            DoWithProgress("Refreshing..", () => { });
        }

        public Plugin[] Items { get; private set; }

        public void GetList(MarketService.MarketServiceClient client, string author)
        {
            Items = client.GetPluginsList(new PluginSearch
            {
                Author = author,
                Offset = 0,
                Count = int.MaxValue,
                OnlyPlugins = false
            }).Items.ToArray();
        }
    }
}
