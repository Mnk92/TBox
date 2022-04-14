using Google.Protobuf.WellKnownTypes;
using DotNetCore.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Mnk.TBox.Plugins.Market.Contracts;
using Mnk.TBox.Plugins.Market.Service.Domain;

namespace Mnk.TBox.Plugins.Market.Service.Database
{
    class PluginRepository : EFRepository<Domain.Plugin>
    {
        private readonly PluginContentManager pluginContentManager;
        private readonly AuthorRepository authors;
        private readonly PluginTypeRepository types;
        public PluginRepository(Context.Context context, AuthorRepository authors, PluginTypeRepository types) : base(context)
        {
            pluginContentManager = new PluginContentManager(PluginContentManager.Plugins);
            this.authors = authors;
            this.types = types;
        }

        public async Task<ResultStream> Download(ulong pluginId)
        {
            var target = await Queryable.SingleAsync(x => x.PluginId == pluginId);
            var ret = pluginContentManager.Read(target.Author.Name, target.Name);
            if (ret != null)
            {
                ++target.Downloads;
                await UpdatePartialAsync(target);
            }
            return ret;
        }

        sealed class PluginDependencies
        {
            public Author Author { get; set; }
            public PluginType PluginType { get; set; }
        }

        private async Task<PluginDependencies> CollectPluginDependencies(UploadPluginStream details)
        {
            var item = details.Plugin;
            var dependencies = new PluginDependencies
            {
                Author = await authors.Queryable.FirstOrDefaultAsync(a => a.Name == item.Author),
                PluginType = await types.Queryable.FirstOrDefaultAsync(t => t.Name == item.Type),
            };
            if (dependencies.Author == null)
            {
                dependencies.Author = new Author(0, item.Author);
                await authors.AddAsync(dependencies.Author);
            }
            if (dependencies.PluginType == null)
            {
                dependencies.PluginType = new PluginType(0, item.Type);
                await types.AddAsync(dependencies.PluginType);
            }
            return dependencies;
        }

        private static Domain.Plugin CreateUploadInfo(UploadPluginStream details, PluginDependencies dependencies)
        {
            var plugin = details.Plugin;
            return new Domain.Plugin
            {
                PluginId = plugin.PluginId,
                Date = plugin.Date.ToDateTime(),
                Description = plugin.Description,
                PluginType = dependencies.PluginType,
                Author = dependencies.Author,
                Name = plugin.Name,
                Size = (ulong)details.Length,
                Downloads = 0,
                Uploads = 1,
                IsPlugin = plugin.IsPlugin,
            };
        }

        private static bool Validate(Contracts.Plugin plugin)
        {
            return !string.IsNullOrEmpty(plugin.Name) &&
                   !string.IsNullOrEmpty(plugin.Author) &&
                   !string.IsNullOrEmpty(plugin.Type) &&
                   !string.IsNullOrEmpty(plugin.Description);
        }

        public async Task<bool> Upload(UploadPluginStream details)
        {
            var domain = await Queryable.SingleOrDefaultAsync(x => x.PluginId == details.Plugin.PluginId);
            if (domain == null &&
                Validate(details.Plugin) &&
                pluginContentManager.Save(details))
            {
                await AddAsync(CreateUploadInfo(details, await CollectPluginDependencies(details)));
                return true;
            }
            return false;
        }

        public async Task<bool> Upgrade(UploadPluginStream details)
        {
            var plugin = details.Plugin;
            var domain = await Queryable.SingleOrDefaultAsync(x => x.PluginId == details.Plugin.PluginId);
            if (domain != null &&
                Validate(plugin) &&
                pluginContentManager.Save(details))
            {
                await ClearDependencies(domain.PluginTypeId, domain.AuthorId);
                var dependencies = await CollectPluginDependencies(details);
                domain.Date = plugin.Date.ToDateTime();
                domain.Description = plugin.Description;
                domain.PluginType = dependencies.PluginType;
                domain.Author = dependencies.Author;
                domain.Name = plugin.Name;
                domain.Size = (ulong)details.Length;
                ++domain.Uploads;
                return true;
            }
            return false;
        }

        private IQueryable<Domain.Plugin> GetFilteredData(string typeName, string authorName)
        {
            var result = Queryable;
            if (typeName.Length > 0)
            {
                result = result.Where(x => x.PluginType.Name == typeName);
            }
            if (authorName.Length > 0)
            {
                result = result.Where(x => x.Author.Name == authorName);
            }
            return result;
        }

        public async Task<bool> Delete(ulong pluginId)
        {
            var target = await Queryable.FirstOrDefaultAsync(x => x.PluginId == pluginId);
            if (target?.PluginId != pluginId) return false;
            await DeleteAsync(target);
            await ClearDependencies(target.PluginTypeId, target.AuthorId);
            pluginContentManager.Delete(target.Author.Name, target.Name);
            return true;
        }

        private async Task ClearDependencies(ulong pluginTypeId, ulong authorId)
        {
            if (!await Queryable.AnyAsync(p => p.AuthorId == authorId))
            {
                await authors.DeleteAsync(x => x.AuthorId == authorId);
            }

            if (!await Queryable.AnyAsync(p => p.PluginTypeId == pluginTypeId))
            {
                await types.DeleteAsync(x => x.PluginTypeId == pluginTypeId);
            }
        }

        public Task<bool> Exist(ulong pluginId)
        {
            return Queryable.AnyAsync(x => x.PluginId == pluginId);
        }

        public Task<int> GetListCount(string typeName, string authorName)
        {
            return GetFilteredData(typeName, authorName).CountAsync();
        }

        public IQueryable<Contracts.Plugin> GetList(string typeName, string authorName, int offset, int count, bool? onlyPlugins)
        {
            var result = GetFilteredData(typeName, authorName);
            if (onlyPlugins.HasValue)
            {
                result = result.Where(x => x.IsPlugin == onlyPlugins.Value);
            }
            return result
                .OrderByDescending(p => p.Date)
                .Take(count)
                .Skip(offset)
                .Select(x => new Contracts.Plugin
                {
                    Author = x.Author.Name,
                    Type = x.PluginType.Name,
                    Date = Timestamp.FromDateTime(x.Date),
                    Description = x.Description,
                    Downloads = x.Downloads,
                    Uploads = x.Uploads,
                    Name = x.Name,
                    Size = x.Size,
                    IsPlugin = x.IsPlugin,
                });
        }
    }
}
