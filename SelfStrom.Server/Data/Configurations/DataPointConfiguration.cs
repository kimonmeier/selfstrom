using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SelfStrom.Server.Data.Entities;

namespace SelfStrom.Server.Data.Configurations;

public class DataPointConfiguration : IEntityTypeConfiguration<DataPoint>
{
    public void Configure(EntityTypeBuilder<DataPoint> builder)
    {
        builder
            .ToTable(nameof(DataPoint));

        builder
            .HasKey(x => x.Id);

        builder
            .Property(x => x.Id)
            .ValueGeneratedOnAdd();

        builder
            .HasOne(x => x.Device)
            .WithMany()
            .HasForeignKey(x => x.DeviceId);

        builder
            .HasIndex([nameof(DataPoint.DeviceId), nameof(DataPoint.Timestamp)]);
    }
}
