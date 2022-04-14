using DotNetCore.EntityFrameworkCore;
using Google.Protobuf.WellKnownTypes;
using Microsoft.EntityFrameworkCore;
using Mnk.Library.Common.Log;

namespace Mnk.TBox.Plugins.Market.Service.Database
{
    sealed class BugRepository : EFRepository<Domain.Bug>
    {
        private static readonly ILog Log = LogManager.GetLogger<BugRepository>();

        public BugRepository(Context.Context context) : base(context)
        {
        }

        public IQueryable<Contracts.Bug> GetList(ulong pluginId, int offset, int count)
        {
            return Queryable.Where(bug => bug.PluginId == pluginId).Skip(offset).Take(count).
                   Select(bug =>
                       new Contracts.Bug
                       {
                           Description = bug.Description,
                           BugId = bug.BugId,
                           PluginId = bug.PluginId,
                           Date = Timestamp.FromDateTime(bug.Date)
                       }
                   );
        }

        public Task<int> GetListCount(ulong pluginId)
        {
            return Queryable.CountAsync(bug => bug.PluginId == pluginId);
        }

        public async Task Send(Contracts.Bug bug)
        {
            await AddAsync(new Domain.Bug(bug.BugId, bug.PluginId, bug.Description, DateTime.Now));
            Log.Write($"Bug was sent: '{bug.Description}', for plugin: {bug.PluginId}");
        }
    }
}
