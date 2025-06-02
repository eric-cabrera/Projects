namespace Assurity.AgentPortal.Accessors.AssureLink.Entities.EntityTypeConfigurations;

using Assurity.AgentPortal.Accessors.DataStore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class DistributionMasterEntityTypeConfiguration : IEntityTypeConfiguration<DistributionMaster>
{
    public void Configure(EntityTypeBuilder<DistributionMaster> entity)
    {
        entity.ToTable("DistributionMaster", "AssureLink");

        entity.HasKey(e => e.Id).HasName("spSyncDistributionMaster_AssureLink");

        entity.Property(e => e.AgentId).HasMaxLength(12);
    }
}
