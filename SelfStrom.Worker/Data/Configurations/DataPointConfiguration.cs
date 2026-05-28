using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SelfStrom.Worker.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfStrom.Worker.Data.Configurations;

internal class DataPointConfiguration : IEntityTypeConfiguration<DataPoint>
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
            .HasIndex(x => x.DeviceId);
    }
}
