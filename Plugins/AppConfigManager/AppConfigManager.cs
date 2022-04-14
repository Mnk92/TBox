using System;
using System.Collections.Generic;
using System.Linq;
using Mnk.Library.WpfControls.Menu;
using Mnk.TBox.Core.Contracts;
using Mnk.TBox.Locales.Localization.Plugins.AppConfigManager;
using Mnk.TBox.Core.PluginsShared.Tools;

namespace Mnk.TBox.Plugins.AppConfigManager
{
    [PluginInfo(typeof(AppConfigManagerLang), typeof(Properties.Resources), PluginGroup.Desktop)]
    public class AppConfigManager : ConfigurablePlugin<Settings, Config>
    {
        private readonly Lazy<FeatureToggler> executor = new Lazy<FeatureToggler>(() => new FeatureToggler());

        public override void OnRebuildMenu()
        {
            base.OnRebuildMenu();
            Menu = Config.Profiles
                .Where(p => p.Files.Any(o => o.IsChecked) && p.Options.Any(o => o.IsChecked))
                .Select(p => new UMenuItem
                {
                    Header = p.Key,
                    OnClick = o => executor.Value.Execute(
                        p.Files.CheckedItems.Select(x => Context.PathResolver.Resolve(x.Key)),
                        p.Options.CheckedItems.Select(x => new KeyValuePair<string, string>(x.Key, x.Value)).ToArray())
                }).ToArray();
        }
    }
}
