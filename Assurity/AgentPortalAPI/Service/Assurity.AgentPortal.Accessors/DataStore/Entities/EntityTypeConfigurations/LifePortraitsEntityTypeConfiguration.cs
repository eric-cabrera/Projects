namespace Assurity.AgentPortal.Accessors.DataStore.Entities.EntityTypeConfigurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class LifePortraitsEntityTypeConfiguration : IEntityTypeConfiguration<LifePortraits>
{
    public void Configure(EntityTypeBuilder<LifePortraits> entity)
    {
        entity.ToTable("LifePortraits");

        entity.HasKey(e => e.SSOUserId);

        entity.Property(e => e.UserName)
            .HasMaxLength(255)
            .IsRequired();

        entity.Property(e => e.LoweredUserName)
            .HasMaxLength(255)
            .IsRequired();
    }
}
