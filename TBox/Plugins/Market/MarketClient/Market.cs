using Mnk.TBox.Core.Contracts;
using Mnk.TBox.Locales.Localization.Plugins.Market;
using Mnk.TBox.Plugins.Market.Client.Code;

namespace Mnk.TBox.Plugins.Market.Client
{
	[PluginInfo(typeof(MarketLang), 96, PluginGroup.Other)]
	public class Market : ConfigurablePlugin<Settings, Config>
	{
		protected override void OnConfigUpdated()
		{
			Synchronizer.Init(Config.Client);
		}

		public override void Save(bool autoSaveOnExit)
		{
			base.Save(false);
		}
	}
}
