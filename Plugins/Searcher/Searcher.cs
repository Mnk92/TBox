using System;
using System.Windows;
using LightInject;
using Mnk.TBox.Core.Contracts;
using Mnk.TBox.Locales.Localization.Plugins.Searcher;
using Mnk.TBox.Plugins.Searcher.Code;
using Mnk.TBox.Plugins.Searcher.Code.Settings;
using Mnk.Library.WpfSyntaxHighlighter;
using Mnk.Library.WpfControls.Menu;
using Mnk.Library.WpfControls.Icons;
using Mnk.Rat;
using Mnk.TBox.Plugins.Searcher.Code.Rat;

namespace Mnk.TBox.Plugins.Searcher
{
    [PluginInfo(typeof(SearcherLang), typeof(Properties.Resources), PluginGroup.Desktop)]
    public sealed class Searcher : ConfigurablePlugin<Settings, Config>
    {
        private IServiceContainer container;
        private IAvailabilityChecker availabilityChecker;
        private readonly Lazy<SearchManager> worker;

        public Searcher()
        {
            worker = new Lazy<SearchManager>(() =>
            {
                var w = container.GetInstance<SearchManager>();
                w.Icon = Icon.ToImageSource();
                w.Fill(Config, Context.DoSync);
                w.InitFolders(Context.DataProvider.WritebleDataPath);
                return w;
            });
        }

        public override void OnRebuildMenu()
        {
            base.OnRebuildMenu();
            Menu = new[]
                       {
                           new UMenuItem
                               {
                                   IsEnabled = availabilityChecker.CanSearch,
                                   Header = SearcherLang.Search,
                                   OnClick = DoSearch
                               },
                            new USeparator(),
                           new UMenuItem
                               {
                                   IsEnabled = availabilityChecker.CanUnload,
                                   Header = SearcherLang.UnloadIndexes,
                                   OnClick = DoUnload
                               },
                           new UMenuItem
                               {
                                   IsEnabled = availabilityChecker.CanRebuild,
                                   Header = SearcherLang.RebuildIndexes,
                                   OnClick = DoRebuild
                               },
                       };
        }

        public override void Init(IPluginContext context)
        {
            base.Init(context);
            container = ServicesRegistrar.Register(ConfigManager, context);
            availabilityChecker = container.GetInstance<IAvailabilityChecker>();
            availabilityChecker.OnChanged += context.RebuildMenu;
            context.AddTypeToWarmingUp(typeof(SyntaxHighlighter));
        }

        public override void Load()
        {
            base.Load();
            if (worker.IsValueCreated) worker.Value.Fill(Config, Context.DoSync);
        }

        public override void Save(bool autoSaveOnExit)
        {
            base.Save(autoSaveOnExit);
            if (worker.IsValueCreated) worker.Value.Save(Config, autoSaveOnExit);
        }

        private void DoUnload(object o)
        {
            worker.Value.UnloadIndexes();
        }

        private void DoRebuild(object o)
        {
            if (o is NonUserRunContext ||
                MessageBox.Show(SearcherLang.AreYouWantRebuild, SearcherLang.PluginName, MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
            {
                worker.Value.RebuildIndexes();
            }
        }

        private void DoSearch(object o)
        {
            worker.Value.ShowSearchDialog();
        }

        public override void Dispose()
        {
            base.Dispose();
            if (worker.IsValueCreated)
                worker.Value.Dispose();
        }
    }

}