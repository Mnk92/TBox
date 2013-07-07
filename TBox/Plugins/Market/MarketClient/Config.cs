using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Mnk.TBox.Plugins.Market.Client.ServiceReference;

namespace Mnk.TBox.Plugins.Market.Client
{
	[Serializable]
	public class Config
	{
		[Serializable]
		public sealed class ClientInfo
		{
			public string EndPoint = "http://localhost:8080/MarketService";
			public int ItemsPerPage = 50;
		}
		public ClientInfo Client = new ClientInfo();

		[Serializable]
		public sealed class History
		{
			[Serializable]
			public struct Info
			{
				[XmlAttribute]
				public string Name;
				[XmlAttribute]
				public string Author;
				[XmlAttribute]
				public DateTime Date;
				[XmlAttribute]
				public bool Installed;
			}
			public List<Info> Items = new List<Info>();
			public int MaxItemsInHistory = 50;
		}
		public History HistoryConfig = new History();

		[Serializable]
		public sealed class InstalledPluginsConfig
		{
			public List<Plugin> Plugins = new List<Plugin>();
		}
		public InstalledPluginsConfig InstalledPlugins = new InstalledPluginsConfig();
	}
}
