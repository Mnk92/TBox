using LightInject;
using Mnk.Rat.Finders;
using Mnk.Rat.Finders.Parsers;
using Mnk.Rat.Finders.Scanner;
using Mnk.Rat.Search;

namespace Mnk.Rat
{
    public static class ServicesRegistrar
    {
        public static IServiceContainer Register()
        {
            var container = new ServiceContainer();
            container.Register<IParser, Parser>(new PerContainerLifetime());
            container.Register<IScanner, Scanner>(new PerContainerLifetime());
            container.Register<IWordsGenerator, WordsGenerator>(new PerContainerLifetime());
            container.Register<IFileInformer, FileInformer>(new PerContainerLifetime());
            container.Register<IWordsFinder, WordsFinder>(new PerContainerLifetime());
            container.Register<IAvailabilityChecker, AvailabilityChecker>(new PerContainerLifetime());
            container.Register<ISearchEngine, SearchEngine>(new PerContainerLifetime());
            return container;
        }
    }
}
