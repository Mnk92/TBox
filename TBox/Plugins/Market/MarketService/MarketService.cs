using System;
using System.Linq;
using Mnk.Library.Common.Log;
using Mnk.TBox.Plugins.Market.Interfaces;
using Mnk.TBox.Plugins.Market.Interfaces.Contracts;
using Mnk.TBox.Plugins.Market.Service.Service;

namespace Mnk.TBox.Plugins.Market.Service
{
	sealed class MarketService : IMarketService, IDisposable
	{
		private static readonly ILog Log = LogManager.GetLogger<MarketService>();
		private readonly PluginService pluginService;
		private readonly BugService bugService;
		private readonly MarketEntities marketEntities = new MarketEntities();
		public MarketService()
		{
			bugService = new BugService(marketEntities);
			pluginService = new PluginService(marketEntities);
		}

		public Mnk.TBox.Plugins.Market.Interfaces.Plugin[] Plugin_GetList(Mnk.TBox.Plugins.Market.Interfaces.Plugin filter, int offset, int count, bool? onlyPlugins)
		{
			return pluginService.GetList(filter, offset, count, onlyPlugins);
		}

		public int Plugin_GetListCount(Mnk.TBox.Plugins.Market.Interfaces.Plugin filter)
		{
			return pluginService.GetListCount(filter);
		}

		public DataContract Plugin_Download(DownloadContract body)
		{
			return pluginService.Download(body);
		}

		public UploadContract Plugin_Upload(PluginUploadContract body)
		{
			return pluginService.Upload(body);
		}

		public UploadContract Plugin_Upgrade(PluginUploadContract body)
		{
			return pluginService.Upgrade(body);
		}

		public bool Plugin_Delete(Mnk.TBox.Plugins.Market.Interfaces.Plugin plugin)
		{
			return pluginService.Delete(plugin);
		}

		public bool Plugin_Exist(Mnk.TBox.Plugins.Market.Interfaces.Plugin plugin)
		{
			return pluginService.Exist(plugin);
		}



		public Mnk.TBox.Plugins.Market.Interfaces.Bug[] Bug_GetList(long uid, int offset, int count)
		{
			return bugService.GetList(uid, offset, count);
		}

		public int Bug_GetListCount(long uid)
		{
			return bugService.GetListCount(uid);
		}

		public void Bug_Send(Mnk.TBox.Plugins.Market.Interfaces.Bug bug)
		{
			bugService.Send(bug);
		}



		public string[] Author_GetList()
		{
			var ret = new string[0];
			Shared.Do("Author GetList", () =>
				ret = marketEntities.Authors.Select(
					x => x.Name).ToArray(), Log
				);
			return ret;
		}



		public string[] Type_GetList()
		{
			var ret = new string[0];
			Shared.Do("Type GetList", () =>
				ret = marketEntities.Types.Select(
					x => x.Name).ToArray(), Log
				);
			return ret;
		}

		public void Dispose()
		{
			marketEntities.Dispose();
		}
	}
}
