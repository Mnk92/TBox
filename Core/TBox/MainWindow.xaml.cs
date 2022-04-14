using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Interop;
using System.Windows.Media;
using Mnk.Library.CodePlex;
using Mnk.Library.CodePlex.Controls;
using Mnk.Library.Common;
using Mnk.Library.Common.Log;
using Mnk.Library.WpfControls;
using Mnk.TBox.Core.Contracts;
using LightInject;
using Mnk.TBox.Locales.Localization.TBox;
using Mnk.TBox.Core.Application.Code;
using Mnk.TBox.Core.Application.Code.FastStart;
using Mnk.TBox.Core.Application.Code.Menu;
using Mnk.TBox.Core.Application.Code.Objects;
using Mnk.TBox.Core.Application.Forms;
using Mnk.Library.WpfControls.Dialogs;
using Mnk.Library.WpfControls.Dialogs.StateSaver;
using Mnk.Library.WpfControls.Menu;
using Mnk.Library.Common.MT;

namespace Mnk.TBox.Core.Application
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public sealed partial class MainWindow
    {
        private static readonly ILog Log = LogManager.GetLogger<MainWindow>();
        private static readonly ILog InfoLog = LogManager.GetInfoLogger<MainWindow>();
        private readonly Lazy<FastStartDialog> fastStartDialog;
        private readonly UiConfigurator uiConfigurator;
        private Engine engine;
        private ChangesLogDialog changesLogDialog;
        private readonly int startTime = Environment.TickCount;
        internal readonly RecentItemsCollector RecentItemsCollector;
        private readonly IConfigManager<Config> configManager;
        private readonly IServiceContainer container;

        public MainWindow()
        {
            RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly;
            if (!OneInstance.MainWindow.Init(this)) return;
            container = ServicesRegistrar.Register();
            InitializeComponent();
            configManager = container.GetInstance<IConfigManager<Config>>();
            RecentItemsCollector = container.GetInstance<RecentItemsCollector>();
            fastStartDialog = new Lazy<FastStartDialog>(CreateFastStartDialog);
            InfoLog.Write("Init main form time: {0}", Environment.TickCount - startTime);
            uiConfigurator = new UiConfigurator(
                View, PluginsBack, BtnBack, container,
                new[]
                    {
                        new USeparator(),
                        new UMenuItem{Header = TBoxLang.UserActions},
                        new USeparator(),
                        new UMenuItem{Header = TBoxLang.MenuSettings, OnClick = o=>MenuShowSettings(), Icon = Properties.Resources.Settings},
                        new UMenuItem{Header = TBoxLang.MenuFastStart, OnClick = o=>MenuShowFastStart(), Icon = Properties.Resources.Tip},
                        new UMenuItem{Header = TBoxLang.MenuCheckUpdates, OnClick = CheckUpdates, Icon = Properties.Resources.Update},
                        new UMenuItem{Header = TBoxLang.MenuExit, OnClick = o=>MenuClose(), Icon = Properties.Resources.Exit}
                    },
                new[]
                {
                    new EngineSettingsInfo(TBoxLang.SettingsCaption, TBoxLang.SettingsDescription, Properties.Resources.Settings, () => container.GetInstance<PluginsSettings>()),
                    new EngineSettingsInfo(TBoxLang.Information, TBoxLang.Information, Properties.Resources.Icon, () => container.GetInstance<InfoDialog>()),
                    new EngineSettingsInfo(TBoxLang.FastStart, TBoxLang.FastStartDescription, Properties.Resources.Tip,
                        () =>
                        {
                            configManager.Config.FastStartConfig.IsFastStart = true;
                            return fastStartDialog.Value;
                        })
                },
                OnPluginChanged
                );
            ShowProgress(TBoxLang.ProgressInitialize, CreateEngine, false);
            InitFastStartMenu();

            ExceptionsHelper.HandleException(ShowChangeLog, () => TBoxLang.ErrorProcessingChangelog, Log);
            this.SetState(configManager.Config.DialogState);
            if (configManager.Config.StartHidden)
            {
                Show();
                Hide();
                return;
            }
            uiConfigurator.FastStartShower.Show();
        }

        private void InitFastStartMenu()
        {
            uiConfigurator.FastStartShower.Load(configManager.Config.FastStartConfig);
            var visitor = container.GetInstance<MenuCallsVisitor>();
            visitor.ClearHandlers();
            visitor.AddHandler(RecentItemsCollector);
            if (fastStartDialog.IsValueCreated)
            {
                fastStartDialog.Value.Init(configManager, RecentItemsCollector);
            }
        }

        private FastStartDialog CreateFastStartDialog()
        {
            var dialog = new FastStartDialog(
                () =>
                {
                    BackClick(null, null);
                    Close();
                });
            dialog.Init(configManager, RecentItemsCollector);
            return dialog;
        }

        private void CheckUpdates(object o)
        {
            engine.CheckUpdates(o is NonUserRunContext);
        }

        private void ShowChangeLog()
        {
            var cfg = configManager.Config.Update;
            if (!cfg.ShowChanglog) return;
            changesLogDialog = new ChangesLogDialog();
            cfg.LastChanglogPosition = changesLogDialog.ShowChangeLog(cfg.LastChanglogPosition);
        }

        private static string GetCaption()
        {
            var ver = Assembly.GetExecutingAssembly().GetName().Version;
            return $"{TBoxLang.Caption} {ver.Major}.{ver.Minor} - {TBoxLang.Settings}";
        }

        private void OnPluginChanged(EnginePluginInfo info)
        {

            if (string.Equals(info.Name, TBoxLang.Caption))
            {
                Title = GetCaption();
            }
            else
            {
                Title = GetCaption() + " - [" + info.Name + "]";
            }
            Description.Text = info.Description;
        }

        private void ShowProgress(string caption, Action<IUpdater> action, bool withOwner = true)
        {
            DialogsCache.ShowProgress(action, caption, withOwner ? this : null, topmost: false, showInTaskBar: !withOwner, icon: this.Icon);
        }

        public void MenuClose(bool criticalError = false)
        {
            if (changesLogDialog != null)
            {
                changesLogDialog.Dispose();
            }
            CanClose = true;
            uiConfigurator.Dispose();
            if (engine != null)
            {
                Mt.Do(this, () =>
                {
                    configManager.Config.DialogState = this.GetState();
                });
                ShowProgress(TBoxLang.ProgressExit, u => engine.Close(u, criticalError), false);
                container.GetInstance<ILogsSender>().SendIfNeed();
                Mt.Do(this, () => engine.Dispose());
            }
            Close();
        }

        private void CreateEngine(IUpdater updater)
        {
            updater.Update(TBoxLang.ProgressLoadingPlugins, 0);
            engine = new Engine(this, updater, uiConfigurator, container);
            updater.Update(TBoxLang.ProgressFinish, 1);
            InfoLog.Write("Create engine time: {0}", Environment.TickCount - startTime);
        }

        private void MenuShowSettings()
        {
            uiConfigurator.FastStartShower.ShowSettings();
        }

        private void MenuShowFastStart()
        {
            uiConfigurator.FastStartShower.ShowFastStart();
        }

        private void BtnSaveClick(object sender, RoutedEventArgs e)
        {
            ShowProgress(TBoxLang.ProgressApplyChanges, u => engine.Save(u, false));
            if (configManager.Config.HideOnSave) Close();
        }

        private void BtnReloadClick(object sender, RoutedEventArgs e)
        {
            ShowProgress(TBoxLang.ProgressReloadPluginsSettings, u =>
            {
                engine.Load(u);
                InitFastStartMenu();
            });
            if (configManager.Config.HideOnCancel) Close();
        }

        private void SendFeedback(object sender, RoutedEventArgs e)
        {
            var r = DialogsCache.ShowMemoBox(TBoxLang.FeedbackMessage, TBoxLang.FeedbackCaption, configManager.Config.FeedBackMessage, x => !string.IsNullOrEmpty(x), this);
            if (!r.Key) return;
            configManager.Config.FeedBackMessage = r.Value;
            DialogsCache.ShowProgress(u =>
            {
                if (!container.GetInstance<IFeedbackSender>().Send("feedback", configManager.Config.FeedBackMessage))
                {
                    Dispatcher.BeginInvoke(new Action(() => Feedback.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent))));
                }
            }, TBoxLang.FeedbackProgress, this, false);
        }

        public void GoBack()
        {
            PluginsBack.Content = View;
            Description.Text = TBoxLang.DefaultMainViewDescription;
            Title = GetCaption();
            BtnBack.IsEnabled = false;
        }

        private void BackClick(object sender, RoutedEventArgs e)
        {
            GoBack();
            configManager.Config.FastStartConfig.IsFastStart = false;
        }

        private void BtnHelpClick(object sender, RoutedEventArgs e)
        {
            using (Process.Start("https://tbox.codeplex.com")) { }
        }

        private void BtnCloseClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
