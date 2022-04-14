using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mnk.TBox.Plugins.Market.Service.Domain;

namespace Mnk.TBox.Plugins.Market.Service.Database
{
    sealed class PluginConfiguration : IEntityTypeConfiguration<Plugin>
    {
        public void Configure(EntityTypeBuilder<Plugin> builder)
        {
            builder.ToTable(nameof(Plugin), nameof(Plugin));
            builder.HasKey(x => x.PluginId);
            builder.Property(x => x.PluginId).ValueGeneratedOnAdd().IsRequired();
            builder.Property(x => x.Name).HasColumnName(nameof(Plugin.Name)).HasMaxLength(128).IsRequired();
            builder.Property(x => x.Description).HasColumnName(nameof(Plugin.Description)).HasMaxLength(256).IsRequired();
            builder.Property(x => x.Date).IsRequired();
            builder.Property(x => x.Downloads).IsRequired();
            builder.Property(x => x.Uploads).IsRequired();
            builder.Property(x => x.Size).IsRequired();
            builder.Property(x => x.IsPlugin).IsRequired();
            builder.HasOne(x => x.Author).WithOne().HasForeignKey<Author>(nameof(Plugin.AuthorId)).IsRequired();
            builder.HasOne(x => x.PluginType).WithOne().HasForeignKey<PluginType>(nameof(Plugin.PluginTypeId)).IsRequired();
            builder.HasIndex(nameof(Plugin.AuthorId)).IsUnique();
            builder.HasIndex(nameof(Plugin.PluginTypeId)).IsUnique();
        }
    }
}
