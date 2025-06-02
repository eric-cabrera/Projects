namespace Assurity.AgentPortal.Accessors.AssureLink.Entities.EntityTypeConfigurations
{
    using Assurity.AgentPortal.Accessors.AssureLink.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class AgentPersonalInformationEntityTypeConfiguration : IEntityTypeConfiguration<AgentPersonalInformation>
    {
        public void Configure(EntityTypeBuilder<AgentPersonalInformation> entity)
        {
            entity.ToTable("AgentPersonalInformation", "Authentication");

            // Specify the primary key
            entity.HasKey(e => e.AgentId);

            // Optional: Configure other properties and defaults (e.g., DirectDepositIndicator)
            entity.Property(e => e.DirectDepositIndicator).HasDefaultValue(false);
        }
    }
}
