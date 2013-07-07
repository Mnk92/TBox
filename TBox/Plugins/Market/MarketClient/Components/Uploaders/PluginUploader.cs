using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using Mnk.Library.Common.Models;
using Mnk.Library.Common.MT;
using Mnk.Library.Common.SaveLoad;
using Mnk.TBox.Plugins.Market.Client.ServiceReference;
using Mnk.Library.WpfControls.Dialogs;
using Mnk.TBox.Plugins.Market.Client.Code;

namespace Mnk.TBox.Plugins.Market.Client.Components.Uploaders
{
    public class PluginUploader
    {
        private readonly Func<Plugin> creator;
        public PluginUploader(Func<Plugin> creator)
        {
            this.creator = creator;
        }

        private PluginUploadContract CreateContract(Plugin item, IList<string> paths, bool allowNoExistFile)
        {
            var ret = new PluginUploadContract { Item = item };
            foreach (var path in paths.Where(path => string.IsNullOrWhiteSpace(path) || !File.Exists(path)))
            {
                if (!allowNoExistFile)
                {
                    throw new Exception($"File '{path}' not exist!");
                }
                return ret;
            }

            ret.Length = ExtFile.LoadDirectoryFiles(paths, out var pairs, out var stream);
            ret.Descriptions = pairs.Select(x => new PairOfstringint { Key = x.Key, Value = x.Value }).ToArray();
            var streamWithProgress = new StreamWithProgress(stream);
            streamWithProgress.ProgressChanged += ProgressChanged;
            ret.FileByteStream = streamWithProgress;

            return ret;
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
            var ret = new UploadContract();
            DoWithProgress(
                $"Uploading plugin: '{name}'",
                () => Synchronizer.Do(service =>
                                     ret = service.Plugin_Upload(CreateContract(creator(), paths, false))));
            if (!ret.Success)
            {
                ShowError($"Can't upload: '{name}'. {(ret.Exist ? "Already exist!" : "Internal error!")}");
            }
        }

        public void Upgrade(string name, string[] paths)
        {
            var ret = new UploadContract();
            DoWithProgress(
                $"Upgrading plugin: '{name}'",
                            () => Synchronizer.Do(service =>
                                                 ret = service.Plugin_Upgrade(CreateContract(creator(), paths, true))));
            if (!ret.Success)
            {
                ShowError($"Can't upgrade: '{name}'. {(!ret.Exist ? "Not exist!" : "Internal error!")}");
            }
        }

        public void Delete(string name)
        {
            if (MessageBox.Show($"Are you want delete '{name}'?",
                                name, MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes) return;
            var ret = false;
            DoWithProgress(
                $"Delete file: '{name}'",
                () => Synchronizer.Do(service =>
                                      ret = service.Plugin_Delete(creator())));
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

        public void GetList(IMarketService service, string author)
        {
            Items = service.Plugin_GetList(new Plugin { Author = author }, 0, int.MaxValue, null);
        }
    }
}
