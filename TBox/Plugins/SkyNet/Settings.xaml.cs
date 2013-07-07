using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Mnk.Library.Common.Log;
using Mnk.Library.Common.Tools;
using Mnk.Library.WpfControls;
using Mnk.Library.WpfControls.Dialogs;
using Mnk.TBox.Core.Contracts;
using Mnk.TBox.Core.PluginsShared.ScriptEngine;
using Mnk.Library.ScriptEngine;
using Mnk.TBox.Locales.Localization.Plugins.SkyNet;
using Mnk.TBox.Plugins.SkyNet.Code.Interfaces;
using Mnk.TBox.Plugins.SkyNet.Code.Settings;
using Mnk.TBox.Tools.SkyNet.Common;
using Mnk.Library.WpfControls.Tools;

namespace Mnk.TBox.Plugins.SkyNet
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : ISettings
    {
        private readonly IScriptConfigurator scriptConfigurator;
        private readonly IServicesFacade servicesFacade;
        private readonly IConfigsFacade configsFacade;
        private readonly IScriptsHelper scriptsHelper;
        private readonly ILog log = LogManager.GetLogger<Settings>();

        public Settings()
        {
            InitializeComponent();
        }

        public Settings(IPluginContext context, IScriptConfigurator scriptConfigurator, IServicesFacade servicesFacade, IConfigsFacade configsFacade, IScriptsHelper scriptsHelper)
            : this()
        {
            this.scriptConfigurator = scriptConfigurator;
            this.servicesFacade = servicesFacade;
            this.configsFacade = configsFacade;
            this.scriptsHelper = scriptsHelper;
            FilePaths = scriptsHelper.GetPaths();
            AgentService.ServiceName = Constants.AgentServiceName;
            AgentService.ServicePath = Path.Combine(context.DataProvider.ToolsPath, "Mnk.TBox.Tools.SkyNet.Agent.exe");
            ServerService.ServiceName = Constants.ServerServiceName;
            ServerService.ServicePath = Path.Combine(context.DataProvider.ToolsPath, "Mnk.TBox.Tools.SkyNet.Server.exe");
            AgentSettingsNeedRefresh(null, new DependencyPropertyChangedEventArgs());
            ServerSettingsNeedRefresh(null, new DependencyPropertyChangedEventArgs());
        }

        public IList<string> FilePaths { get; private set; } 
        public UserControl Control { get { return this; } }

        private void ChangeAgentSettingsClick(object sender, RoutedEventArgs e)
        {
            configsFacade.AgentConfig = (AgentConfig)AgentConfiguration.DataContext;
        }

        private void AgentSettingsNeedRefresh(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!AgentConfiguration.IsEnabled)
            {
                AgentConfiguration.DataContext = null;
                return;
            }
            AgentConfiguration.DataContext = configsFacade.AgentConfig;
        }

        private void ChangeServerSettingsClick(object sender, RoutedEventArgs e)
        {
            configsFacade.ServerConfig = (ServerConfig)ServerConfiguration.DataContext;
        }

        private void ServerSettingsNeedRefresh(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!ServerConfiguration.IsEnabled)
            {
                ServerConfiguration.DataContext = null;
                return;
            }
            ServerConfiguration.DataContext = configsFacade.ServerConfig;
        }

        private void RefreshInfoClick(object sender, RoutedEventArgs e)
        {
            DialogsCache.ShowProgress(u => DoRefresh(), 
                SkyNetLang.PluginName, this.GetParentWindow());
        }

        private void DoRefresh()
        {
            try
            {
                var status = servicesFacade.GetStatus();
                Mt.Do(this, () =>
                {
                    ExistTasks.ItemsSource = status.Tasks
                        .Select(x => string.Format("{0}\t {1}\t {2}\t {3}", x.Owner, x.State, x.Progress, x.CreatedTime));
                    ConnectedAgents.ItemsSource = status.Agents
                        .Select(x => string.Format("{0}\t {1}\t {2}", x.Endpoint, x.State, x.TotalCores));
                    AgentInfo.DataContext = status.Task;
                });
            }
            catch (Exception ex)
            {
                log.Write(ex, "Can't retrieve information");
            }
        }

        private void BtnSetOperationParametersClick(object sender, RoutedEventArgs e)
        {
            scriptsHelper.ShowParameters(GetSelectedOperation(sender));
        }

        private SingleFileOperation GetSelectedOperation(object sender)
        {
            var selectedKey = (string)((Button)sender).DataContext;
            var cfg = (Config)DataContext;
            var id = cfg.Operations.GetExistIndexByKeyIgnoreCase(selectedKey);
            return cfg.Operations[id];
        }

        private void OnCheckChangedEvent(object sender, RoutedEventArgs e)
        {
            Ops.OnCheckChangedEvent(sender, e);
        }

    }
}
