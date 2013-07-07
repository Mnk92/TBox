using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Mnk.Library.Common;
using Mnk.Library.Common.Log;
using Mnk.Library.Common.MT;
using Mnk.Library.WpfControls;
using Mnk.Library.WpfControls.Dialogs;
using Mnk.TBox.Core.PluginsShared.LongPaths;
using Mnk.TBox.Locales.Localization.Plugins.Searcher;
using Mnk.TBox.Plugins.Searcher.Code.Settings;
using Mnk.Library.WpfControls.Tools;
using Mnk.Rat;
using Mnk.Rat.Checkers;
using Mnk.Rat.Finders.Parsers;
using Mnk.Rat.Finders.Search;
using Mnk.Rat.Settings;
using Mnk.TBox.Core.Contracts;
using ZetaLongPaths;

namespace Mnk.TBox.Plugins.Searcher.Components
{
    /// <summary>
    /// Interaction logic for SearchDialog.xaml
    /// </summary>
    sealed partial class SearchDialog 
    {
        private static readonly ILog Log = LogManager.GetLogger<SearchDialog>();
        private Config config;
        private readonly DataTable filesTable = new DataTable();
        private ISearchEngine searchEngine;
        private bool disableSearch = false;
        private readonly Stopwatch sw = new Stopwatch();

        public SearchDialog()
        {
            InitializeComponent();
            filesTable.Columns.Add("Name", typeof(String));
            filesTable.Columns.Add("Ext", typeof(String));
            filesTable.Columns.Add("Path", typeof(String));
            filesTable.DefaultView.Sort = "Path ASC";
            lvResult.ItemsSource = filesTable.DefaultView;
            lvResult.Loaded += (o, e) =>
            {
                lvResult.Columns[1].Width = 48;
            };
            cbCount.Items.AddRange("16", "64", "128", "256", "512", "1024");
            FileTypesPanel.View = FileTypes;
        }

        public override void Dispose()
        {
            filesTable.Dispose();
            base.Dispose();
        }

        public void Load(Config main)
        {
            disableSearch = true;
            DataContext = main;
            config = main;
            FileTypes.ItemsSource = config.Index.FileTypes;
            disableSearch = false;
        }

        internal void ShowDialog(ISearchEngine engine, IPathResolver pathResolver)
        {
            disableSearch = true;
            if (!IsVisible)
            {
                ExceptionsHelper.HandleException(
                    ()=>FillFolderFilter(pathResolver),
                    () => "Can't enumerate directories",
                    Log
                    );
            }
            searchEngine = engine;
            edText.Clear();
            SearchText.SetFocus();
            ShowAndActivate();
            btnSearch.IsEnabled = SearchText.Text.Length > 0;
            disableSearch = false;
            if (btnSearch.IsEnabled && !config.Search.FullTextSearch)
            {
                DoSearch();
            }
        }

        private void FillFolderFilter(IPathResolver pathResolver)
        {
            FoldersFilter.Items.Clear();
            FoldersFilter.Items.Add(string.Empty);
            foreach (var folder in config.Index.FileNames.CheckedItems.Select(x=>pathResolver.Resolve(x.Key)).Where(ZlpIOHelper.DirectoryExists))
            {
                AddPath(folder);
                foreach (var subFolder in new ZlpDirectoryInfo(folder).SafeEnumerateDirectories(Log))
                {
                    AddPath(subFolder.FullName);
                }
            }
        }

        private void AddPath(string path)
        {
            if (!string.IsNullOrEmpty(path) && !path.EndsWith("\\"))
            {
                path += "\\";
            }
            FoldersFilter.Items.Add(path);
        }

        private DataRow GetSelectedCollection()
        {
            return ((DataRowView)lvResult.SelectedItem).Row;
        }

        private string GetSelectedFilePath()
        {
            return GetSelectedCollection().ItemArray[2].ToString();
        }

        private string GetSelectedFileExt()
        {
            return GetSelectedCollection().ItemArray[1].ToString();
        }

        private void LvResultMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lvResult.SelectedIndex < 0 ||
                lvResult.SelectedIndex >= filesTable.Rows.Count) return;
            Cmd.Start(config.Result.OpenWith, Log, string.Format("\"{0}\"", GetSelectedFilePath()), false, true);
        }

        private void LvResultSelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvResult.SelectedIndex < 0 ||
                lvResult.SelectedIndex >= filesTable.Rows.Count ||
                config.Result.AutoLoadFile != true) return;
            try
            {
                sw.Restart();
                var path = GetSelectedFilePath();
                Title = string.Format("{1} - [ {0} ]", path, SearcherLang.PluginName);
                edText.Clear();
                edText.Format = GetSelectedFileExt();
                using (var s = LongPathExtensions.OpenRead(path))
                {
                    edText.Read(s);
                }
                sbiWords.Content = edText.MarkAll(SearchText.Text,
                    config.Search.MatchCase);
                sbiLoadFileTime.Content = sw.ElapsedMilliseconds / 1000.0;
            }
            catch (Exception ex)
            {
                Log.Write(ex, "Internal error.");
                edText.Text = SearcherLang.ErrorLoadingFile;
                edText.Format = string.Empty;
                sbiLoadFileTime.Content = string.Empty;
            }
        }

        private void PrintResults(ICollection<int> files)
        {
            sbiSearchTime.Content = sw.ElapsedMilliseconds / 1000.0;
            sw.Restart();
            lvResult.Visibility = Visibility.Hidden;
            filesTable.Rows.Clear();
            if (files.Count != 0)
            {
                var results = files.Select(i => searchEngine.FileInformer.GetFilePath(i));
                if (FoldersFilter.SelectedValue != null)
                {
                    var folder = FoldersFilter.SelectedValue.ToString();
                    if (!string.IsNullOrEmpty(folder))
                    {
                        results = results.Where(x => x.StartsWith(folder));
                    }
                }
                foreach (var path in results)
                {
                    filesTable.Rows.Add(
                        ZlpPathHelper.GetFileNameWithoutExtension(path),
                        GetFileExtension(path),
                        path
                        );
                }
            }
            lvResult.Visibility = Visibility.Visible;
            CollectionViewSource.GetDefaultView(lvResult.ItemsSource).Refresh();

            sbiFiles.Content = filesTable.Rows.Count;
            sbiFillResultsTime.Content = sw.ElapsedMilliseconds / 1000.0;
        }

        private static string GetFileExtension(string path)
        {
            var ext = ZlpPathHelper.GetExtension(path);
            return !string.IsNullOrEmpty(ext) ? ext.Remove(0, 1) : ext;
        }

        private void SearchClick(object sender, RoutedEventArgs e)
        {
            if (disableSearch) return;
            ExceptionsHelper.HandleException(
                SafeSearch,
                () => "Search internal error",
                Log);
        }

        private void SafeSearch()
        {
            /*
            sw.Restart();
            var searchText = SearchText.Text.Trim();
            Title = "Searcher";
            edText.Text = string.Empty;
            sbiFiles.Content = string.Empty;
            sbiSearchTime.Content = string.Empty;
            sbiFillResultsTime.Content = string.Empty;
            sbiWords.Content = string.Empty;
            sbiLoadFileTime.Content = string.Empty;
            var files = new HashSet<int>();
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                var searchAdder = new SearchAdder();
                if (config.Search.SearchMode == SearchMode.FileNames)
                {
                    searchAdder.Words.Add(searchText);
                }
                else
                {
                    var parser = new Parser(searchAdder, config.Index);
                    parser.ParseFileData(searchText, 0);
                }
                if (!searchAdder.Words.Any())
                {
                    MessageBox.Show(SearcherLang.TextDontContainSearchableSymbols, Title, MessageBoxButton.OK, MessageBoxImage.Hand);
                    return;
                }
                if (searchAdder.Words.Count < 2 && (!config.Search.FullTextSearch || config.Search.SearchMode == SearchMode.FileNames))
                {
                    Search(searchAdder.Words.FirstOrDefault(), files, config.Search.FileCount);
                }
                else
                {
                    var firstWord = true;
                    foreach (var word in searchAdder.Words)
                    {
                        var tmp = new HashSet<int>();
                        Search(word, tmp, int.MaxValue);
                        if (firstWord)
                        {
                            files = tmp;
                            firstWord = false;
                        }
                        else
                        {
                            files.IntersectWith(tmp);
                        }
                    }
                    var items = (IEnumerable<int>)files;
                    if (config.Search.FullTextSearch && config.Search.SearchMode == SearchMode.FileData)
                    {
                        DialogsCache.ShowProgress(
                            u => DoFullTextSearch(files, u, searchText), 
                            SearcherLang.FullTextSearch, 
                            this, icon: Icon);
                        return;
                    }
                    PrintResults(items.Take(config.Search.FileCount).ToArray());
                    return;
                }
            }
            PrintResults(files);
            */
        }

        private void DoFullTextSearch(HashSet<int> files, IUpdater u, string searchText)
        {
            var count = (float)Math.Min(files.Count, config.Search.FileCount);
            var i = 0;
            var founded = files.AsParallel()
                .Where(x => CheckFile(u, searchText, x, count, ref i))
                .Take(config.Search.FileCount)
                .ToArray();
            if (u.UserPressClose) return;
            Mt.Do(this, () => PrintResults(founded));
        }

        private bool CheckFile(IUpdater u, string searchText, int x, float count, ref int i)
        {
            if (u.UserPressClose || !FileContains(x, searchText)) return false;
            u.Update(++i/count);
            return true;
        }

        private bool FileContains(int i, string searchText)
        {
            try
            {
                var path = searchEngine.FileInformer.GetFilePath(i);
                if (!ZlpIOHelper.FileExists(path)) return false;
                var comparationType = config.Search.MatchCase ?
                    StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
                using (var f = LongPathExtensions.OpenRead(path))
                {
                    using (var s = new StreamReader(f, Encoding.UTF8))
                    {
                        while (!s.EndOfStream)
                        {
                            var line = s.ReadLine();
                            if (!string.IsNullOrEmpty(line) && line.IndexOf(searchText, comparationType) > -1)
                            {
                                return true;
                            }
                        }
                        return false;
                    } 
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void Search(string searchText, HashSet<int> files, int fileCount)
        {
            var checker = CreateChecker(searchText);
            switch (config.Search.SearchMode)
            {
                case SearchMode.FileNames:
                    searchEngine.FileInformer.Find(
                        GetTypes(), checker, fileCount, files);
                    break;
                case SearchMode.FileData:
                    searchEngine.WordsFinder.Find(
                        GetTypes(), checker, fileCount, files);
                    break;
                default:
                    throw new ArgumentException("Unknown search mode: " + config.Search.SearchMode);
            }
        }

        private void DoSearch()
        {
            if (!IsVisible || config.Search.FullTextSearch) return;
            SearchClick(null, null);
        }

        private void CbSearchTextKeyUp(object sender, KeyEventArgs e)
        {
            btnSearch.IsEnabled = SearchText.Text.Length > 0;
            if (!btnSearch.IsEnabled) return;
            if (e.Key == Key.Return)
            {
                SearchClick(sender, e);
            }
        }

        private IFileChecker CreateChecker(string searchText)
        {
            return FileCheckerFactory.Create(
                config.Search.CompareType, config.Search.MatchCase, searchText);
        }

        private ISet<string> GetTypes()
        {
            return new HashSet<string>(config.Index.FileTypes.CheckedItems.Select(x => x.Key));
        }

        public void Clear()
        {
            filesTable.Rows.Clear();
        }

        private void NeedSearch(object sender, RoutedEventArgs e)
        {
            DoSearch();
        }

        private void SelectedTextChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                btnSearch.IsEnabled = e.AddedItems[0].ToString().Length > 0;
            }
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ContextMenuGoToFile(object sender, RoutedEventArgs e)
        {
            if (lvResult.SelectedItem == null) return;
            var path = GetSelectedFilePath();
            using (Process.Start("explorer.exe", "/select, " + path))
            {
            }
        }

        private void ContextMenuEdit(object sender, RoutedEventArgs e)
        {
            LvResultMouseDoubleClick(sender, null);
        }

        private void ContextMenuCopyFilePath(object sender, RoutedEventArgs e)
        {
            if (lvResult.SelectedItem == null) return;
            var path = GetSelectedFilePath();
            Clipboard.SetText(path);
        }

        private void ContextMenuCopyFileName(object sender, RoutedEventArgs e)
        {
            if (lvResult.SelectedItem == null) return;
            var path = GetSelectedFilePath();
            Clipboard.SetText(Path.GetFileName(path));
        }
    }
}
