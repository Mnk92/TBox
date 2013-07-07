using System.IO;
using System.Linq;
using Mnk.TBox.Core.Contracts;
using Mnk.TBox.Locales.Localization.Plugins.DirectoryProcessor;
using Mnk.Library.WpfWinForms;
using Mnk.TBox.Plugins.DirectoryProcessor.Code;

namespace Mnk.TBox.Plugins.DirectoryProcessor
{
    [PluginInfo(typeof(DirectoryProcessorLang), 4, PluginGroup.Desktop)]
    public sealed class DirectoryProcessor : ConfigurablePlugin<Settings, Config>
    {
        private Executor executor;

        public override void OnRebuildMenu()
        {
            base.OnRebuildMenu();
            executor = new Executor(Context.PathResolver);
            Menu = Config.Directories
                .Where(x => x.IsChecked)
                .Select(x => new UMenuItem { Header = Path.GetFileName(Context.PathResolver.Resolve(x.Key)), Items = executor.Process(x) })
                .ToArray();
        }
    }
}
