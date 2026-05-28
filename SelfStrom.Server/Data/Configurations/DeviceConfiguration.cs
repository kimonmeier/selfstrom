using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SelfStrom.Server.Data.Entities;

namespace SelfStrom.Server.Data.Configurations;

public class DeviceConfiguration : IEntityTypeConfiguration<Device>
{
    public void Configure(EntityTypeBuilder<Device> builder)
    {
        builder
            .ToTable(nameof(Device));

        builder
            .HasKey(x => x.Id);

        builder
            .Property(x => x.Id)
            .ValueGeneratedOnAdd();

        builder
            .HasOne(x => x.Room)
            .WithMany()
            .HasForeignKey(x => x.RoomId);

        builder
            .Property(x => x.IsDeleted)
            .HasDefaultValue(false);

        builder
            .HasIndex(x => x.RoomId);
    }
}
