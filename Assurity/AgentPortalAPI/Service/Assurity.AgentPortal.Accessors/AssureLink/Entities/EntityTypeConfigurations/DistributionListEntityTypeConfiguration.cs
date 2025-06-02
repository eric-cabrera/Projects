namespace Assurity.AgentPortal.Accessors.AssureLink.Entities.EntityTypeConfigurations;

using Assurity.AgentPortal.Accessors.DataStore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class DistributionListEntityTypeConfiguration : IEntityTypeConfiguration<DistributionList>
{
    public void Configure(EntityTypeBuilder<DistributionList> entity)
    {
        entity.ToTable("DistributionList", "AssureLink");

        entity.HasKey(e => e.Id).HasName("spSyncDistributionList_AssureLink");

        entity.Property(e => e.AgentId).HasMaxLength(12);
        entity.Property(e => e.Email).HasMaxLength(250);
    }
}
