using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mnk.TBox.Plugins.Market.Service.Domain;

namespace Mnk.TBox.Plugins.Market.Service.Database
{
    sealed class AuthorConfiguration : IEntityTypeConfiguration<Author>
    {
        public void Configure(EntityTypeBuilder<Author> builder)
        {
            builder.ToTable(nameof(Author), nameof(Author));
            builder.HasKey(x => x.AuthorId);
            builder.Property(x => x.AuthorId).ValueGeneratedOnAdd().IsRequired();
            builder.Property(x => x.Name).HasColumnName(nameof(Author.Name)).HasMaxLength(128).IsRequired();
        }
    }
}
