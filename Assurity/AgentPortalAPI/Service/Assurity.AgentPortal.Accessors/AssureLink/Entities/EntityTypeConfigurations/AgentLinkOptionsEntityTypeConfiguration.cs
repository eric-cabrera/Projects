namespace Assurity.AgentPortal.Accessors.AssureLink.Entities.EntityTypeConfigurations
{
    using Assurity.AgentPortal.Accessors.AssureLink.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class AgentLinkOptionsEntityTypeConfiguration : IEntityTypeConfiguration<AgentLinkOptions>
    {
        public void Configure(EntityTypeBuilder<AgentLinkOptions> entity)
        {
            entity.ToTable("AgentLinkOptions", "DocumentConnect");

            entity.HasKey(e => new { e.AgentId, e.MarketCode, e.OptionType })
                  .HasName("PK_AssureLink.AgentLinkOptions");

            entity.Property(e => e.AgentId).HasMaxLength(255);
            entity.Property(e => e.MarketCode).HasMaxLength(12);
            entity.Property(e => e.OptionType).HasMaxLength(32);
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.AccessCode).HasMaxLength(100);
            entity.Property(e => e.DateUpdated).HasColumnType("datetime");
        }
    }
}
