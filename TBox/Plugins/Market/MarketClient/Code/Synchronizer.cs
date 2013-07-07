using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using Mnk.Library.Common.Log;
using Mnk.Library.Common.MT;
using Mnk.TBox.Plugins.Market.Client.ServiceReference;

namespace Mnk.TBox.Plugins.Market.Client.Code
{
	static class Synchronizer
	{
		private static readonly ILog Log = LogManager.GetLogger(typeof(Synchronizer));

		public static readonly PluginFiles PluginFiles = new PluginFiles();
		private static string endPoint;
		public static void Init(Config.ClientInfo info)
		{
			if (endPoint == info.EndPoint) return;
			endPoint = info.EndPoint;
		}

		public static event Action<IMarketService> OnReloadData;

		public static IEnumerable<string> Types { get; private set; }
		public static IEnumerable<string> Authors { get; private set; }

		public static bool RefreshTables(IUpdater updater)
		{
			updater.Update("Connecting..", 1);
			var ret = false;
			Do(serv =>
			{
				try
				{
					updater.Update("Refresh tables", 1);
					Types = serv.Type_GetList().OrderBy(x => x.ToLower());
					Authors = serv.Author_GetList().OrderBy(x => x.ToLower());
					OnReloadData(serv);
					ret = true;
				}
				catch (Exception ex)
				{
					Log.Write(ex, "Error refreshing data.");
				}
			});
			return ret;
		}

		public static void Do(Action<IMarketService> worker)
		{
			var factory = new ChannelFactory<IMarketService>(
				new BasicHttpBinding
				{
					MaxReceivedMessageSize = int.MaxValue,
					MessageEncoding = WSMessageEncoding.Mtom,
					TransferMode = TransferMode.Streamed,
					ReceiveTimeout = new TimeSpan(0, 59, 0),
					SendTimeout = new TimeSpan(0, 59, 0),
					OpenTimeout = new TimeSpan(0, 05, 0),
				},
					new EndpointAddress(endPoint));

			var client = factory.CreateChannel();

			worker(client);

			((IClientChannel)client).Close();
			factory.Close();
		}

		public delegate void InstallDelegate(Plugin plugin);

		public static event InstallDelegate OnInstall;
		public static void DoOnInstall(Plugin plugin)
		{
			if (OnInstall != null) OnInstall(plugin);
		}

		public static event InstallDelegate OnUninstall;
		public static void DoOnUninstall(Plugin plugin)
		{
			if (OnUninstall != null) OnUninstall(plugin);
		}
	}
}
