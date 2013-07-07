using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using Mnk.Library.CodePlex;
using Mnk.Library.Common.OS;
using Mnk.Library.Common.Tools;
using Mnk.Library.Common.UI.ModelsContainers;
using Mnk.Library.WpfControls;
using Mnk.TBox.Locales.Localization.TBox;
using Mnk.TBox.Core.Application.Code;
using Mnk.TBox.Core.Application.Code.FastStart;
using Mnk.TBox.Core.Application.Code.HotKeys;
using Mnk.TBox.Core.Application.Code.Menu;
using Mnk.TBox.Core.Application.Code.Objects;
using Mnk.TBox.Core.Application.Code.Shelduler;
using Mnk.Library.WpfControls.Components;

namespace Mnk.TBox.Core.Application.Forms
{
    /// <summary>
    /// Interaction logic for PluginsSettings.xaml
    /// </summary>
    sealed partial class PluginsSettings : IDisposable
    {
        private readonly IThemesManager themesManager;
        private static readonly string FilePath = Process.GetCurrentProcess().MainModule.FileName;
        public CheckableDataCollection<EnginePluginInfo> Collection { get; private set; }
        private IAutoUpdater appUpdater;
        public event EventHandler EnableHotKeys;
        private readonly HotkeysManager hotkeysManager;
        private readonly SchedulerManager schedulerManager;
        private readonly UserActionsManager userActionsManager;

        public PluginsSettings(IMenuItemsProvider menuItemsProvider, IThemesManager themesManager)
        {
            this.themesManager = themesManager;
            InitializeComponent();
            Panel.ItemsSource = Collection = new CheckableDataCollection<EnginePluginInfo>();
            PanelButtons.View = Panel;
            hotkeysManager = new HotkeysManager(HotKeysView, menuItemsProvider);
            schedulerManager = new SchedulerManager(ScheldulerView, menuItemsProvider);
            userActionsManager = new UserActionsManager(UserActionsView, menuItemsProvider);
            Themes.ItemsSource = themesManager.AvailableThemes;
            cbLanguage.ItemsSource = new[] { "en" }.Concat(new DirectoryInfo("Localization").GetDirectories().Select(x => x.Name)).ToArray();
        }

        public void Init(IAutoUpdater updater)
        {
            appUpdater = updater;
        }

        public void Load(Config cfg)
        {
            DataContext = cfg;
            OnConfigUpdated(cfg);
        }

        public void Save(Config cfg)
        {
            OnConfigUpdated(cfg);
        }

        private void OnConfigUpdated(Config cfg)
        {
            Mt.Do(this, Panel.Refresh);
            hotkeysManager.OnConfigUpdated(cfg.HotKeys);
            schedulerManager.OnConfigUpdated(cfg.SchedulerTasks);
            userActionsManager.OnConfigUpdated(cfg.FastStartConfig);
            RenderOptions.ProcessRenderMode = cfg.EnableGPUAccelerationForUi
                ? RenderMode.Default
                : RenderMode.SoftwareOnly;
        }

        private void ShortcutToDesktopClick(object sender, RoutedEventArgs e)
        {
            Shortcuts.CreateOnDesktop(FilePath);
        }

        private void ShortcutToAutorunClick(object sender, RoutedEventArgs e)
        {
            Shortcuts.CreateOnStartup(FilePath);
        }

        private void OnCheckChangedEvent(object sender, RoutedEventArgs e)
        {
            Panel.OnCheckChangedEvent(sender, e);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (appUpdater.TryUpdate(true) == false)
                MessageBox.Show(TBoxLang.MessageNoUpdatesFound, TBoxLang.Caption, MessageBoxButton.OK, MessageBoxImage.Asterisk);
        }

        private void HotkeysEnabledChecked(object sender, RoutedEventArgs e)
        {
            if (EnableHotKeys != null) EnableHotKeys(sender, e);
        }

        private void SheldulerEnabledChecked(object sender, RoutedEventArgs e)
        {
            if (EnableHotKeys != null) EnableHotKeys(sender, e);
        }

        private void ButtonClearHotkeyClick(object sender, RoutedEventArgs e)
        {
            ((HotkeyBox)((DockPanel)((Button)sender).Parent).Children[2]).GlobalHotkey = null;
        }

        private void ThemeChanged(object sender, RoutedEventArgs e)
        {
            themesManager.Load(Themes.Value);
        }

        public void Dispose()
        {
            hotkeysManager.Dispose();
            schedulerManager.Dispose();
        }

        private void ScheldulerCheckChanged(object sender, RoutedEventArgs e)
        {
            ScheldulerView.OnCheckChangedEvent(sender, e);
        }

        private void UserActionsCheckChanged(object sender, RoutedEventArgs e)
        {
            UserActionsView.OnCheckChangedEvent(sender, e);
        }

        private void BtnEditUserActionClick(object sender, RoutedEventArgs e)
        {
            var selectedKey = ((TextBlock)((DockPanel)((Button)sender).Parent).Children[2]).Text;
            var cfg = Config.FastStartConfig.MenuItemsSequence.GetExistByKeyIgnoreCase(selectedKey);
            userActionsManager.ShowDialog(cfg);
        }

        private Config Config
        {
            get { return ((Config)DataContext); }
        }

        private void ButtonClearHistoryClick(object sender, RoutedEventArgs e)
        {
            Config.FastStartConfig.MenuItems.Clear();
        }

        private void LanguageChanged(object sender, RoutedEventArgs e)
        {
        }

        private void AliasesCheckChanged(object sender, RoutedEventArgs e)
        {
            Aliases.OnCheckChangedEvent(sender,e);
        }
    }
}
