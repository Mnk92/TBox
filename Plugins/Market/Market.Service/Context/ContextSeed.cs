using Microsoft.EntityFrameworkCore;
using Mnk.TBox.Plugins.Market.Service.Domain;

namespace Mnk.TBox.Plugins.Market.Service.Context
{
    static class ContextSeed
    {
        public static void Seed(this ModelBuilder builder)
        {
            builder.SeedPluginTypes();
        }
        public static void SeedPluginTypes(this ModelBuilder builder)
        {
            builder.Entity<PluginType>(t =>
            {
                t.HasData(new PluginType(0, "Other"));
                t.HasData(new PluginType(1, "Desktop"));
                t.HasData(new PluginType(2, "Web"));
                t.HasData(new PluginType(3, "Database"));
                t.HasData(new PluginType(4, "Development"));
            });
        }
    }
}
