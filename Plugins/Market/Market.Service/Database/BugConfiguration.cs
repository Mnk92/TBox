using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mnk.TBox.Plugins.Market.Service.Domain;

namespace Mnk.TBox.Plugins.Market.Service.Database
{
    sealed class BugConfiguration : IEntityTypeConfiguration<Domain.Bug>
    {
        public void Configure(EntityTypeBuilder<Domain.Bug> builder)
        {
            builder.ToTable(nameof(Bug), nameof(Bug));
            builder.HasKey(x => x.BugId);
            builder.Property(x => x.BugId).ValueGeneratedOnAdd().IsRequired();
            builder.Property(x => x.PluginId).IsRequired();
            builder.Property(x => x.Date).IsRequired();
            builder.Property(x => x.Description).HasColumnName(nameof(Bug.Description)).HasMaxLength(255).IsRequired();
        }
    }
}
