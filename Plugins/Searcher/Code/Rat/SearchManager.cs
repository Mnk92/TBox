using System;
using System.IO;
using System.Windows.Media;
using Mnk.Library.Common.MT;
using Mnk.Library.WpfControls;
using Mnk.Library.WpfControls.Dialogs;
using Mnk.Library.WpfControls.Dialogs.StateSaver;
using Mnk.Rat;
using Mnk.TBox.Core.Contracts;
using Mnk.TBox.Locales.Localization.Plugins.Searcher;
using Mnk.TBox.Plugins.Searcher.Code.Settings;
using Mnk.TBox.Plugins.Searcher.Components;

namespace Mnk.TBox.Plugins.Searcher.Code.Rat
{
    sealed class SearchManager : IDisposable
    {
        private readonly IAvailabilityChecker availabilityChecker;
        private readonly ISearchEngine searchEngine;
        private Action<Action> synchronizer;
        private readonly IPathResolver pathResolver;
        public ImageSource Icon { get; set; }
        private string folderPath;
        private readonly LazyDialog<SearchDialog> searchDialog;
        private Config mainConfig;

        public SearchManager(IAvailabilityChecker availabilityChecker, ISearchEngine searchEngine, IPathResolver pathResolver)
        {
            this.availabilityChecker = availabilityChecker;
            this.searchEngine = searchEngine;
            this.pathResolver = pathResolver;
            searchDialog = new LazyDialog<SearchDialog>(() =>
            {
                var dialog = new SearchDialog();
                dialog.Load(mainConfig);
                return dialog;
            });
        }

        public void InitFolders(string path)
        {
            folderPath = Path.Combine(path, "Indexes");
        }

        private void Init(IUpdater updater)
        {
            var success = false;
            try
            {
                success = searchEngine.LoadSearchInfo(folderPath, updater);
                GC.Collect();
                GC.WaitForFullGCComplete();
                synchronizer(() => ShowDialog(success));
            }
            finally
            {
                availabilityChecker.EndInitSearch(success);
            }
        }

        public void ShowSearchDialog()
        {
            if (availabilityChecker.IndexesLoaded)
            {
                ShowDialog(true);
            }
            else
            {
                BeginOperation();
                DialogsCache.ShowProgress(Init, SearcherLang.LoadIndexes, null, icon: Icon, showInTaskBar: true);
            }
        }

        private bool DoMakeIndexes(IUpdater updater)
        {
            var success = false;
            try
            {
                success = searchEngine.MakeIndex(folderPath, updater);
            }
            finally
            {
                availabilityChecker.EndMakeIndexes(success);
            }
            GC.Collect();
            GC.WaitForFullGCComplete();
            return true;
        }

        public void RebuildIndexes()
        {
            BeginOperation();
            DialogsCache.ShowProgress(u => DoMakeIndexes(u), SearcherLang.CreateIndexes, null, showInTaskBar: true, icon: Icon);
        }

        private void ShowDialog(bool success)
        {
            if (success)
            {
                searchDialog.LoadState(mainConfig.States);
                searchDialog.Value.ShowDialog(searchEngine, pathResolver);
            }
            else searchDialog.Hide();
        }

        public void UnloadIndexes()
        {
            try
            {
                BeginOperation();
                searchEngine.Unload();
                GC.Collect();
                GC.WaitForFullGCComplete();
            }
            finally
            {
                availabilityChecker.EndUnload();
            }
        }

        public void Fill(Config config, Action<Action> synchronizer)
        {
            this.synchronizer = synchronizer;
            mainConfig = config;
            if (searchDialog.IsValueCreated)
            {
                searchDialog.Value.Load(config);
            }
            UnloadIndexes();
        }

        public void Save(Config config, bool autosaveOnExit)
        {
            if (!autosaveOnExit) return;
            searchDialog.SaveState(config.States);
        }

        private void BeginOperation()
        {
            availabilityChecker.BeginOperation();
            if (!searchDialog.IsVisible) return;
            searchDialog.Hide();
            searchDialog.Value.Clear();
        }

        public void Dispose()
        {
            searchDialog.Dispose();
        }
    }
}
