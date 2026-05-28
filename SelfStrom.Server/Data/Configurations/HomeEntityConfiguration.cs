using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SelfStrom.Server.Data.Entities;

namespace SelfStrom.Server.Data.Configurations;

public class HomeEntityConfiguration : IEntityTypeConfiguration<Home>
{
    public void Configure(EntityTypeBuilder<Home> builder)
    {
        builder
            .ToTable(nameof(Home));

        builder
            .HasKey(x => x.Id);

        builder
            .Property(x => x.Id)
            .ValueGeneratedOnAdd();

        builder
            .Property(x => x.Name)
            .HasMaxLength(254);

        builder
            .HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId);

        builder
            .Property(x => x.IsDeleted)
            .HasDefaultValue(false);

        builder
            .HasIndex(x => x.UserId);
    }
}
