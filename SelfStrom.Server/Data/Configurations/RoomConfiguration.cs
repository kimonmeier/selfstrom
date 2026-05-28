using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SelfStrom.Server.Data.Entities;

namespace SelfStrom.Server.Data.Configurations;

public class RoomConfiguration : IEntityTypeConfiguration<Room>
{
    public void Configure(EntityTypeBuilder<Room> builder)
    {
        builder
            .ToTable(nameof(Room));

        builder
            .HasKey(x => x.Id);

        builder
            .Property(x => x.Id)
            .ValueGeneratedOnAdd();

        builder
            .Property(x => x.Name)
            .HasMaxLength(254);

        builder
            .Property(x => x.HomeId);

        builder
            .HasOne(x => x.Home)
            .WithMany()
            .HasForeignKey(x => x.HomeId);

        builder
            .Property(x => x.IsDeleted)
            .HasDefaultValue(true);

        builder
            .HasIndex(x => x.HomeId);
    }
}
