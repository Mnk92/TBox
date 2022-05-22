using System.IO;
using Mnk.Library.AutoUpdateAndFeedback;
using Mnk.Library.Common.AutoUpdate;
using Mnk.Library.Common.Log;
using Mnk.Library.WpfControls;
using Mnk.TBox.Core.Contracts;
using LightInject;
using Mnk.TBox.Core.Application.Code.Configs;
using Mnk.TBox.Core.Application.Code.FastStart;
using Mnk.TBox.Core.Application.Code.Managers;
using Mnk.TBox.Core.Application.Code.Menu;
using Mnk.TBox.Core.Application.Code.Objects;
using Mnk.TBox.Core.Application.Forms;
using Mnk.Library.WpfControls.Code.Log;
using Mnk.Library.WpfControls.Icons;
using Mnk.TBox.Locales.Localization.TBox;

namespace Mnk.TBox.Core.Application.Code
{
    static class ServicesRegistrar
    {
        private static IServiceContainer container;
        private static readonly string LogsFolder = Path.Combine(Folders.UserRootFolder, "Logs");
        private static readonly string ErrorsLogsPath = Path.Combine(LogsFolder, "TBox.Errors.log");

        static ServicesRegistrar()
        {
            if (!Directory.Exists(LogsFolder)) Directory.CreateDirectory(LogsFolder);
            LogManager.Init(
                new MultiplyLog(new FileLog(ErrorsLogsPath), new MessageBoxLog()),
                new FileLog(Path.Combine(LogsFolder, "TBox.Info.log"))
                );
        }

        public static IServiceContainer Register()
        {
            container = new ServiceContainer();
            var cm = new ConfigManager();
            container.RegisterInstance<IConfigManager<Config>>(cm);
            container.RegisterInstance<IConfigsManager>(cm);
            container.RegisterInstance<IPathResolver>(new PathResolver(cm));
            container.Register<IMenuItemsProvider, MenuItemsProvider>(new PerContainerLifetime());
            container.Register<MenuCallsVisitor>(new PerContainerLifetime());
            container.Register<RecentItemsCollector>(new PerContainerLifetime());
            container.Register<InfoDialog>(new PerContainerLifetime());
            container.Register<PluginsSettings>(new PerContainerLifetime());

            container.Register<IconsCache>(new PerContainerLifetime());
            container.Register<IconsExtractor>(new PerContainerLifetime());
            container.Register<WarmingUpManager>(new PerContainerLifetime());
            container.Register<PluginsContextShared>(new PerContainerLifetime());

            container.RegisterInstance<IApplicationUpdater>(new Mnk.Library.AutoUpdateAndFeedback.ApplicationUpdater("tbox"));
            container.Register<IAutoUpdater, AutoUpdate.ApplicationUpdater>(new PerContainerLifetime());
            container.Register<IThemesManager, ThemesManager>(new PerContainerLifetime());
            container.RegisterInstance<IFeedbackSender>(new FeedbackSender(TBoxLang.AppName, "http://mnk92.cloudapp.net:61234"));
            container.RegisterInstance<ILogsSender>(new LogsSender(ErrorsLogsPath, container.GetInstance<IFeedbackSender>()));

            return container;
        }

        public static IServiceContainer Container => container;
    }
}
