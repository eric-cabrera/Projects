namespace Assurity.Kafka.Accessors.Entities.TypeConfigurations
{
    using Assurity.Kafka.Accessors.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class AgentUseQueueConfiguration : IEntityTypeConfiguration<AgentUseQueue>
    {
        public void Configure(EntityTypeBuilder<AgentUseQueue> entity)
        {
            entity.HasKey(e => e.QueueID)
                .HasName("PK_AgentUsedQueue");

            entity.ToTable("AgentUseQueue", "dbo");

            entity.Property(e => e.QueueID).HasColumnName("QueueID");

            entity.Property(e => e.QueueDescription)
                .HasMaxLength(100)
                .IsUnicode(false);
        }
    }
}
