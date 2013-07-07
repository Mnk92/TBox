using System.Data.Objects;
using System.Linq;
using Mnk.Library.Common.Log;
using Mnk.TBox.Plugins.Market.Interfaces.Contracts;
using IPlugin = Mnk.TBox.Plugins.Market.Interfaces.Plugin;

namespace Mnk.TBox.Plugins.Market.Service.Service
{
	class PluginService
	{
		private static readonly ILog Log = LogManager.GetLogger<PluginService>();
		private readonly MarketEntities marketEntities;
		private readonly ObjectSet<Plugin> plugins;
		private readonly ObjectSet<Author> authors;
		private readonly ObjectSet<Type> types;
		private readonly FileMan fileMan;
		public PluginService(MarketEntities marketEntities)
		{
			this.marketEntities = marketEntities;
			plugins = this.marketEntities.Plugins;
			authors = this.marketEntities.Authors;
			types = this.marketEntities.Types;
			fileMan = new FileMan(Shared.Plugins);
		}

		private class Data
		{
			public Plugin Plugin { get; set; }
			public Author Author { get; set; }
			public Type Type { get; set; }
		}

		private IQueryable<Data> GetFilteredUpdateData(string author, string name)
		{
			return from p in plugins
				   join a in authors on p.Author equals a.UID
				   join t in types on p.Type equals t.UID
				   where (a.Name == author) && (p.Name == name)
				   select new Data { Plugin = p, Author = a, Type = t };
		}

		private IQueryable<Data> GetFilteredUpdateData(IPlugin filter)
		{
			return GetFilteredUpdateData(filter.Author, filter.Name);
		}

		private IQueryable<Data> GetFilteredUpdateData(DownloadContract filter)
		{
			return GetFilteredUpdateData(filter.Author, filter.Name);
		}

		public DataContract Download(DownloadContract body)
		{
			var result = new DataContract();
			Shared.Do("Plugins Download", () =>
			{
				var target = GetFilteredUpdateData(body).First();
				if (target != null)
				{
					var ret = fileMan.Read(body.Author, body.Name);
					if (ret != null && ret.Length > 0)
					{
						++target.Plugin.Downloads;
						marketEntities.SaveChanges();
						result = ret;
					}
					else
					{
						Log.Write("Error while read plugin: '{0}-{1}'", body.Author, body.Name);
					}
				}
			}, Log);
			return result;
		}

		private void OnDoAfterDel(long author, long type)
		{
			if (!plugins.Any(p => p.Author == author) && authors.Any(a => a.UID == author))
			{
				authors.DeleteObject(authors.First(a => a.UID == author));
			}
			if (!plugins.Any(p => p.Type == type) && types.Any(t => t.UID == type))
			{
				types.DeleteObject(types.First(t => t.UID == type));
			}
		}

		private class PluginInfo
		{
			public long Author { get; set; }
			public long Type { get; set; }
		}

		private PluginInfo OnDoBeforeAdd(PluginUploadContract contract)
		{
			var item = contract.Item;
			var info = new PluginInfo
			{
				Author = authors.Any(a => a.Name == item.Author) ?
						authors.First(a => a.Name == item.Author).UID : -1,
				Type = types.Any(t => t.Name == item.Type) ?
						types.First(t => t.Name == item.Type).UID : -1,
			};
			OnDoAfterDel(info.Author, info.Type);
			if (!authors.Any(a => a.Name == item.Author))
			{
				if (authors.Any()) info.Author = authors.Max(p => p.UID) + 1;
				else info.Author = 1;
				authors.AddObject(new Author { Name = item.Author, UID = info.Author });
			}
			if (!types.Any(t => t.Name == item.Type))
			{
				if (types.Any()) info.Type = types.Max(p => p.UID) + 1;
				else info.Type = 1;
				types.AddObject(new Type { Name = item.Type, UID = info.Type });
			}
			return info;
		}


		public bool Delete(IPlugin item)
		{
			var result = false;
			Shared.Do("Plugins Delete", () =>
			{
				var target = GetFilteredUpdateData(item);
				if (!target.Any()) return;
				if (target.Count() > 1)
				{
					Log.Write("More that one plugin for: {0}-{1}. Deleting all!", item.Author, item.Name);
				}
				foreach (var p in target)
				{
					plugins.DeleteObject(p.Plugin);
					marketEntities.SaveChanges();

					fileMan.Delete(p.Author.Name, p.Plugin.Name);

					OnDoAfterDel(p.Plugin.Author, p.Plugin.Type);
					marketEntities.SaveChanges();
					result = true;
				}
			}, Log);
			return result;
		}

		private Plugin CreateUploadInfo(PluginUploadContract body, PluginInfo info)
		{
			var item = body.Item;
			return new Plugin
			{
				UID = plugins.Any() ? plugins.Max(p => p.UID) + 1 : 1,
				Date = item.Date,
				Description = item.Description,
				//Dependenses = m_strArr.ToLine(item.Dependenses),
				Type = info.Type,
				Author = info.Author,
				Name = item.Name,
				Size = body.Length,
				Downloads = 0,
				Uploads = 1,
				IsPlugin = item.IsPlugin,
			};
		}

		private static bool Validate(IPlugin plugin)
		{
			return !string.IsNullOrEmpty(plugin.Name) &&
				   !string.IsNullOrEmpty(plugin.Author) &&
				   !string.IsNullOrEmpty(plugin.Type) &&
				   !string.IsNullOrEmpty(plugin.Description);
		}

		public UploadContract Upload(PluginUploadContract body)
		{
			var item = body.Item;
			var ret = new UploadContract { Success = false, Exist = false };
			Shared.Do("Plugins Uploading", () =>
			{
				if (GetFilteredUpdateData(item).Any())
				{
					ret.Exist = true;
				}
				else if (Validate(item) && fileMan.Save(item.Author, item.Name, body))
				{
					plugins.AddObject(CreateUploadInfo(body, OnDoBeforeAdd(body)));
					marketEntities.SaveChanges();
					ret.Success = true;
				}
			}, Log);
			return ret;
		}

		private void Upgrade(PluginUploadContract body, Plugin exist, PluginInfo info)
		{
			var item = body.Item;
			exist.Date = item.Date;
			exist.Description = item.Description;
			//exist.Dependenses = m_strArr.ToLine(item.Dependenses);
			exist.Type = info.Type;
			exist.Author = info.Author;
			exist.Name = item.Name;
			if (body.Length != 0) exist.Size = body.Length;
			++exist.Uploads;
		}

		public UploadContract Upgrade(PluginUploadContract body)
		{
			var item = body.Item;
			var ret = new UploadContract { Success = false, Exist = true };
			Shared.Do("Plugins Upgrading", () =>
			{
				var target = GetFilteredUpdateData(item);
				if (!target.Any())
				{
					ret.Exist = false;
				}
				else if (target.Count() > 1)
				{
					Log.Write("More that one plugin for: {0}-{1}!", item.Author, item.Name);
				}
				else if (Validate(item) && body.Length == 0 || fileMan.Save(item.Author, item.Name, body))
				{
					Upgrade(body, target.First().Plugin, OnDoBeforeAdd(body));
					marketEntities.SaveChanges();
					ret.Success = true;
				}
			}, Log);
			return ret;
		}

		private IQueryable<Data> GetFilteredData(IPlugin filter)
		{
			var authorEmpty = string.IsNullOrEmpty(filter.Author);
			var typeEmpty = string.IsNullOrEmpty(filter.Type);
			var nameEmpty = string.IsNullOrEmpty(filter.Name);
			return from p in plugins
				   join a in authors on p.Author equals a.UID
				   join t in types on p.Type equals t.UID
				   where
					(authorEmpty || a.Name == filter.Author) &&
					(typeEmpty || t.Name == filter.Type) &&
					(nameEmpty || p.Name == filter.Name)
				   select new Data { Plugin = p, Author = a, Type = t };
		}

		public bool Exist(IPlugin item)
		{
			return GetFilteredData(item).Any();
		}

		public int GetListCount(IPlugin filter)
		{
			return GetFilteredData(filter).Count();
		}

		public IPlugin[] GetList(IPlugin filter, int offset, int count, bool? onlyPlugins)
		{
			var ret = new IPlugin[0];
			Shared.Do("Plugins GetList", () =>
			{
				ret = GetFilteredData(filter).
						Where(p => onlyPlugins == null || p.Plugin.IsPlugin == onlyPlugins).
						OrderBy(p => p.Plugin.UID).
						Select(p =>
						new IPlugin
						{
							Author = p.Author.Name,
							Date = p.Plugin.Date,
							Description = p.Plugin.Description,
							Downloads = p.Plugin.Downloads,
							Uploads = p.Plugin.Uploads,
							Name = p.Plugin.Name,
							Size = p.Plugin.Size,
							Type = p.Type.Name,
							IsPlugin = p.Plugin.IsPlugin,
						}).Take(count + offset).AsEnumerable().Skip(offset).ToArray();
			}, Log);
			return ret;
		}
	}
}
