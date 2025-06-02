namespace Assurity.Kafka.Accessors.Entities.TypeConfigurations
{
    using Assurity.Kafka.Accessors.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class QueuesConfiguration : IEntityTypeConfiguration<QUEUES>
    {
        public void Configure(EntityTypeBuilder<QUEUES> entity)
        {
            entity.HasKey(e => new { e.ID })
                .HasName("QUEUE_INSTANCE_WFLID");

            entity.ToTable("QUEUES", "dbo");

            entity.Property(e => e.ID)
                .HasMaxLength(44)
                .IsUnicode(false)
                .HasColumnName("ID");

            entity.Property(e => e.QUEUE)
                .HasMaxLength(44)
                .IsUnicode(false)
                .HasColumnName("QUEUE");
        }
    }
}
