namespace Assurity.AgentPortal.Accessors.AssureLink.Entities.EntityTypeConfigurations
{
    using Assurity.AgentPortal.Accessors.AssureLink.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class AgentOptionsEntityTypeConfiguration : IEntityTypeConfiguration<AgentOptions>
    {
        public void Configure(EntityTypeBuilder<AgentOptions> entity)
        {
            entity.ToTable("AgentOptions", "DocumentConnect");

            entity.HasKey(e => new { e.AgentId, e.MarketCode, e.OptionType })
                  .HasName("PK_DocumentConnect.AgentOptions");

            entity.Property(e => e.AgentId)
                  .HasMaxLength(255)
                  .IsRequired();
            entity.Property(e => e.MarketCode)
                  .HasMaxLength(12)
                  .IsRequired();
            entity.Property(e => e.OptionType)
                  .HasMaxLength(32)
                  .IsRequired();

            entity.Property(e => e.OptOutForEDelivery)
                  .HasDefaultValue(false)
                  .IsRequired();

            entity.Property(e => e.AgentLinkSelected)
                  .HasDefaultValue(false)
                  .IsRequired();

            entity.Property(e => e.IncludeDownline)
                  .HasDefaultValue(false)
                  .IsRequired();

            entity.Property(e => e.DateUpdated)
                  .IsRequired(false);
        }
    }
}
