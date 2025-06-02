namespace Assurity.AgentPortal.Accessors.DataStore.Entities.EntityTypeConfigurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class FiservDistributionChannelEntityTypeConfiguration : IEntityTypeConfiguration<FiservDistributionChannel>
{
    public void Configure(EntityTypeBuilder<FiservDistributionChannel> entity)
    {
        entity.ToTable("FiservDistributionChannel");

        entity.HasKey(distributionChannel => distributionChannel.AccessCode);

        entity.Property(distributionChannel => distributionChannel.AccessCode)
            .HasMaxLength(15)
            .IsRequired()
            .IsUnicode(false);

        entity.Property(distributionChannel => distributionChannel.MarketCodes)
            .HasMaxLength(25)
            .IsRequired()
            .IsUnicode(false);
    }
}
