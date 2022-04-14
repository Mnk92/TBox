using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mnk.TBox.Plugins.Market.Service.Domain;

namespace Mnk.TBox.Plugins.Market.Service.Database
{
    sealed class PluginTypeConfiguration : IEntityTypeConfiguration<PluginType>
    {
        public void Configure(EntityTypeBuilder<PluginType> builder)
        {
            builder.ToTable(nameof(PluginType), nameof(PluginType));
            builder.HasKey(x => x.PluginTypeId);
            builder.Property(x => x.PluginTypeId).ValueGeneratedOnAdd().IsRequired();
            builder.Property(x => x.Name).HasColumnName(nameof(PluginType.Name)).HasMaxLength(64).IsRequired();
        }
    }
}
