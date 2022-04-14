using DotNetCore.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Mnk.TBox.Plugins.Market.Service.Domain;

namespace Mnk.TBox.Plugins.Market.Service.Database
{
    sealed class PluginTypeRepository : EFRepository<PluginType>
    {
        public PluginTypeRepository(DbContext context) : base(context)
        {
        }
    }
}
