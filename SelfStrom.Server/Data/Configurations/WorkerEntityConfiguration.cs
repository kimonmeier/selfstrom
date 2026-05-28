using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SelfStrom.Server.Data.Entities;

namespace SelfStrom.Server.Data.Configurations;

public class WorkerEntityConfiguration : IEntityTypeConfiguration<Worker>
{
    public void Configure(EntityTypeBuilder<Worker> builder)
    {
        builder
            .ToTable(nameof(Worker));

        builder
            .HasKey(x => x.Id);

        builder
            .Property(x => x.Id)
            .ValueGeneratedOnAdd();

        builder
            .Property(x => x.WorkerName)
            .HasMaxLength(254);

        builder
            .Property(x => x.HomeId);

        builder
            .HasOne(x => x.Home)
            .WithMany()
            .HasForeignKey(x => x.HomeId);

        builder
            .HasIndex(x => x.HomeId);
    }
}
