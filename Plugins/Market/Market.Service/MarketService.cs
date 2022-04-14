using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Mnk.TBox.Plugins.Market.Contracts;
using Mnk.TBox.Plugins.Market.Service.Database;

namespace Mnk.TBox.Plugins.Market.Service
{
    public sealed class MarketService : Contracts.MarketService.MarketServiceBase
    {
        private readonly AuthorRepository authors;
        private readonly PluginTypeRepository types;
        private readonly BugRepository bugRepository;
        private readonly PluginRepository pluginRepository;

        public MarketService(Context.Context context)
        {
            authors = new AuthorRepository(context);
            types = new PluginTypeRepository(context);
            bugRepository = new BugRepository(context);
            pluginRepository = new PluginRepository(context, authors, types);
        }

        public override async Task<PluginSearchResult> GetPluginsList(PluginSearch request, ServerCallContext context)
        {
            var items = await pluginRepository.GetList(request.Author, request.Type, (int)request.Offset, (int)request.Count, request.OnlyPlugins).ToArrayAsync();
            var result = new PluginSearchResult();
            result.Items.AddRange(items);
            return result;
        }

        public override async Task<CountResult> GetPluginsListCount(PluginSearch request, ServerCallContext context)
        {
            var count = await pluginRepository.GetListCount(request.Author, request.Type);
            return new CountResult { Count = (ulong)count };
        }

        public override async Task DownloadPlugin(DownloadPluginMessage request, IServerStreamWriter<ResultStream> responseStream, ServerCallContext context)
        {
            var result = await pluginRepository.Download(request.PluginId);
            await responseStream.WriteAsync(result);
        }

        public override async Task<BoolResult> UploadPlugin(IAsyncStreamReader<UploadPluginStream> requestStream, ServerCallContext context)
        {
            var result = await pluginRepository.Upload(requestStream.Current);
            return new BoolResult { Success = result };
        }

        public override async Task<BoolResult> UpgradePlugin(IAsyncStreamReader<UploadPluginStream> requestStream, ServerCallContext context)
        {
            var result = await pluginRepository.Upgrade(requestStream.Current);
            return new BoolResult { Success = result };
        }

        public override async Task<BoolResult> DeletePlugin(Plugin request, ServerCallContext context)
        {
            var result = await pluginRepository.Delete(request.PluginId);
            return new BoolResult { Success = result };
        }

        public override async Task<BoolResult> ExistPlugin(Plugin request, ServerCallContext context)
        {
            var result = await pluginRepository.Exist(request.PluginId);
            return new BoolResult { Success = result };
        }

        public override async Task<BugSearchResult> GetBugList(BugSearch request, ServerCallContext context)
        {
            var items = await bugRepository.GetList(request.PluginId, (int)request.Offset, (int)request.Count).ToArrayAsync();
            var result = new BugSearchResult();
            result.Items.AddRange(items);
            return result;
        }

        public override async Task<CountResult> GetBugListCount(BugSearch request, ServerCallContext context)
        {
            var count = await bugRepository.GetListCount(request.PluginId);
            return new CountResult { Count = (ulong)count };
        }

        public override async Task<Empty> SendBug(Bug request, ServerCallContext context)
        {
            await bugRepository.Send(request);
            return new Empty();
        }

        public override async Task<StringResult> GetAuthorList(Empty request, ServerCallContext context)
        {
            var items = await authors.Queryable.Select(x => x.Name).ToArrayAsync();
            var result = new StringResult();
            result.Items.AddRange(items);
            return result;
        }

        public override async Task<StringResult> GetTypeList(Empty request, ServerCallContext context)
        {
            var items = await types.Queryable.Select(x => x.Name).ToArrayAsync();
            var result = new StringResult();
            result.Items.AddRange(items);
            return result;
        }
    }
}
