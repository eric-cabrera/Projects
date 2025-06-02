namespace Assurity.Kafka.Accessors.Entities.TypeConfigurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class AgentHierarchyChangeConfiguration : IEntityTypeConfiguration<AgentHierarchyChange>
    {
        public void Configure(EntityTypeBuilder<AgentHierarchyChange> entity)
        {
            entity.ToTable("AgentHierarchyChange", "commdebt");

            entity.HasKey(agentHierarchyChange => agentHierarchyChange.Id);

            entity.Property(agentHierarchyChange => agentHierarchyChange.SystemDataLoadId)
                .IsRequired();

            entity.Property(agentHierarchyChange => agentHierarchyChange.ChangeType)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(6)
                .IsUnicode(false)
                .HasConversion<string>();

            entity.Property(agentHierarchyChange => agentHierarchyChange.CompanyCode)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(2)
                .IsUnicode(false);

            entity.Property(agentHierarchyChange => agentHierarchyChange.AgentId)
                .IsRequired()
                .HasMaxLength(12)
                .IsUnicode(false);

            entity.Property(agentHierarchyChange => agentHierarchyChange.MarketCode)
                .IsRequired()
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.Property(agentHierarchyChange => agentHierarchyChange.AgentLevel)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(2)
                .IsUnicode(false);

            entity.Property(agentHierarchyChange => agentHierarchyChange.StartDate)
                .IsRequired();

            entity.Property(agentHierarchyChange => agentHierarchyChange.StopDate)
                .IsRequired();

            entity.Property(agentHierarchyChange => agentHierarchyChange.UplineAgentId)
                .IsRequired()
                .HasMaxLength(12)
                .IsUnicode(false);

            entity.Property(agentHierarchyChange => agentHierarchyChange.UplineMarketCode)
                .IsRequired()
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.Property(agentHierarchyChange => agentHierarchyChange.UplineAgentLevel)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(2)
                .IsUnicode(false);

            entity.Property(agentHierarchyChange => agentHierarchyChange.BeforeAgentId)
                .HasMaxLength(12)
                .IsUnicode(false);
        }
    }
}