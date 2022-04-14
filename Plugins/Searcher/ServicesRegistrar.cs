using System.Drawing;
using LightInject;
using Mnk.TBox.Core.Contracts;
using Mnk.TBox.Plugins.Searcher.Code.Rat;
using Mnk.TBox.Plugins.Searcher.Code.Settings;
using IDataProvider = Mnk.Rat.IDataProvider;

namespace Mnk.TBox.Plugins.Searcher
{
    static class ServicesRegistrar
    {
        public static IServiceContainer Register(IConfigManager<Config> cm, IPluginContext context)
        {
            var container = Rat.ServicesRegistrar.Register();
            container.RegisterInstance(cm);
            container.RegisterInstance(context.PathResolver);
            container.Register<IDataProvider, IDataProvider>(new PerContainerLifetime());
            container.Register<Rat.IIndexContextBuilder, IndexContextBuilder>(new PerContainerLifetime());
            container.Register<IFilter, Filter>(new PerContainerLifetime());
            container.Register<SearchManager>(new PerContainerLifetime());
            return container;
        }
    }
}
