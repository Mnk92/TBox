using System.Windows.Media;
using LightInject;
using Mnk.Library.Common.Tools;
using Mnk.Library.ScriptEngine.Core;
using Mnk.Library.ScriptEngine.Core.Interfaces;
using Mnk.Library.WpfControls;
using Mnk.TBox.Core.Contracts;
using Mnk.TBox.Core.PluginsShared.ScriptEngine;
using Mnk.TBox.Plugins.SkyNet.Code.Interfaces;
using Mnk.TBox.Plugins.SkyNet.Forms;
using Mnk.TBox.Tools.SkyNet.Common;

namespace Mnk.TBox.Plugins.SkyNet.Code
{
    static class ServicesRegistrar
    {
        public static IServiceContainer Register(IPluginContext context, Func<ImageSource> iconGetter)
        {
            var container = new ServiceContainer();
            container.Register<IScriptCompiler<ISkyScript>, ScriptCompiler<ISkyScript>>(new PerContainerLifetime());
            container.Register<ICopyDirGenerator, CopyDirGenerator>();
            container.Register<IDataPacker, DataPacker>();
            container.Register<ITaskExecutor, TaskExecutor>();
            container.Register<IConfigurationProviderFacade, ConfigurationProviderFacade>();
            container.Register<IServicesFacade, ServicesFacade>();
            container.Register<IScriptsHelper, ScriptsHelper>();
            container.Register<IScriptConfigurator, SkyNetScriptConfigurator>();
            container.Register<TaskDialog>();
            container.Register<ISettings, Plugins.SkyNet.Settings>();
            container.RegisterInstance(new LazyDialog<ScriptsConfigurator>(
                () => new SingleFileScriptConfigurator { Context = context, Icon = iconGetter() }));
            container.RegisterInstance(new LazyDialog<EditorDialog>(
                () => new EditorDialog { Context = context, Icon = iconGetter() }));
            container.RegisterInstance(new LazyDialog<TaskDialog>(
                () => CreateTaskDialog(container, iconGetter())));

            container.RegisterInstance(context);

            return container;
        }

        private static TaskDialog CreateTaskDialog(ServiceContainer container, ImageSource icon)
        {
            var d = container.GetInstance<TaskDialog>();
            d.Icon = icon;
            return d;
        }
    }
}
