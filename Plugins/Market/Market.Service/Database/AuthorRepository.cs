using DotNetCore.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Mnk.TBox.Plugins.Market.Service.Domain;

namespace Mnk.TBox.Plugins.Market.Service.Database
{
    sealed class AuthorRepository : EFRepository<Author>
    {
        public AuthorRepository(DbContext context) : base(context)
        {
        }
    }
}
