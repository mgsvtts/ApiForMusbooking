using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

internal sealed class ArtistConfiguration : IEntityTypeConfiguration<ServiceObject>
{
    public void Configure(EntityTypeBuilder<ServiceObject> builder)
    {
        builder.HasKey(item => item.Id);

        builder.Property(item => item.Name)
               .HasMaxLength(100);

        builder.HasIndex(item => item.Name);
    }
}
