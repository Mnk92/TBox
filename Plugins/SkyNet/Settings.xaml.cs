using System.IO;
using System.Windows;
using System.Windows.Controls;
using Mnk.Library.Common.Log;
using Mnk.Library.Common.Tools;
using Mnk.Library.WpfControls;
using Mnk.Library.WpfControls.Dialogs;
using Mnk.TBox.Core.Contracts;
using Mnk.Library.ScriptEngine;
using Mnk.TBox.Locales.Localization.Plugins.SkyNet;
using Mnk.TBox.Plugins.SkyNet.Code.Interfaces;
using Mnk.TBox.Plugins.SkyNet.Code.Settings;
using Mnk.Library.WpfControls.Tools;
using Mnk.TBox.Tools.SkyNet.Contracts;

namespace Mnk.TBox.Plugins.SkyNet
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : ISettings
    {
        private readonly IServicesFacade servicesFacade;
        private readonly IConfigurationProviderFacade configurationProviderFacade;
        private readonly IScriptsHelper scriptsHelper;
        private readonly ILog log = LogManager.GetLogger<Settings>();

        public Settings()
        {
            InitializeComponent();
        }

        public Settings(IPluginContext context, IServicesFacade servicesFacade, IConfigurationProviderFacade configurationProviderFacade, IScriptsHelper scriptsHelper)
            : this()
        {
            this.servicesFacade = servicesFacade;
            this.configurationProviderFacade = configurationProviderFacade;
            this.scriptsHelper = scriptsHelper;
            FilePaths = scriptsHelper.GetPaths();
            AgentService.ServiceName = Constants.AgentServiceName;
            AgentService.ServicePath = Path.Combine(context.DataProvider.ToolsPath, "Mnk.TBox.Tools.SkyNet.Agent.exe");
            ServerService.ServiceName = Constants.ServerServiceName;
            ServerService.ServicePath = Path.Combine(context.DataProvider.ToolsPath, "Mnk.TBox.Tools.SkyNet.Server.exe");
            AgentSettingsNeedRefresh(null, new DependencyPropertyChangedEventArgs());
            ServerSettingsNeedRefresh(null, new DependencyPropertyChangedEventArgs());
        }

        public IList<string> FilePaths { get; }
        public UserControl Control => this;

        private void ChangeAgentSettingsClick(object sender, RoutedEventArgs e)
        {
            configurationProviderFacade.SetAgentConfig((AgentConfig)AgentConfiguration.DataContext).Wait();
        }

        private void AgentSettingsNeedRefresh(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!AgentConfiguration.IsEnabled)
            {
                AgentConfiguration.DataContext = null;
                return;
            }
            AgentConfiguration.DataContext = configurationProviderFacade.GetAgentConfig().Result;
        }

        private void ChangeServerSettingsClick(object sender, RoutedEventArgs e)
        {
            configurationProviderFacade.SetServerConfig((ServerConfig)ServerConfiguration.DataContext).Wait();
        }

        private void ServerSettingsNeedRefresh(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!ServerConfiguration.IsEnabled)
            {
                ServerConfiguration.DataContext = null;
                return;
            }
            ServerConfiguration.DataContext = configurationProviderFacade.GetServerConfig().Result;
        }

        private void RefreshInfoClick(object sender, RoutedEventArgs e)
        {
            DialogsCache.ShowProgress(_ => DoRefresh().Wait(),
                SkyNetLang.PluginName, this.GetParentWindow());
        }

        private async Task DoRefresh()
        {
            try
            {
                var status = await servicesFacade.GetStatus();
                Mt.Do(this, () =>
                {
                    ExistTasks.ItemsSource = status.Tasks
                        .Select(x => $"{x.Owner}\t {x.State}\t {x.Progress}\t {x.CreatedTime}");
                    ConnectedAgents.ItemsSource = status.Agents
                        .Select(x => $"{x.Endpoint}\t {x.State}\t {x.TotalCores}");
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
