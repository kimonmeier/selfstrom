using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SelfStrom.Worker.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfStrom.Worker.Data.Configurations;

internal class ConnectedDevicesConfiguration : IEntityTypeConfiguration<ConnectedDevices>
{
    public void Configure(EntityTypeBuilder<ConnectedDevices> builder)
    {
        builder
            .ToTable(nameof(ConnectedDevices));

        builder
            .HasKey(x => x.Id);

        builder
            .Property(x => x.Id)
            .ValueGeneratedOnAdd();

        builder
            .HasIndex(x => x.MacAdress)
            .IsUnique();
    }
}
