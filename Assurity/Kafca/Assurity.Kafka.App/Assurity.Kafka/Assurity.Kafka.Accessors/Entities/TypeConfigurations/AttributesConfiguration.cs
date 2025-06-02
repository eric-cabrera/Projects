namespace Assurity.Kafka.Accessors.Entities.TypeConfigurations
{
    using Assurity.Kafka.Accessors.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class AttributesConfiguration : IEntityTypeConfiguration<Attributes>
    {
        public void Configure(EntityTypeBuilder<Attributes> entity)
        {
            entity.HasKey(e => e.OBJECT_ID);

            entity.ToTable("Attributes", "dbo");

            entity.Property(e => e.OBJECT_ID)
                .HasMaxLength(44)
                .IsUnicode(false)
                .HasColumnName("OBJECT_ID");

            entity.Property(e => e.OBJECT_TYPE)
                .HasColumnName("OBJECT_TYPE");

            entity.Property(e => e.AgentID)
                .HasMaxLength(12)
                .IsUnicode(false)
                .HasColumnName("AgentID");

            entity.Property(e => e.OBJECT_NAME)
                .HasMaxLength(44)
                .IsUnicode(false)
                .HasColumnName("OBJECT_NAME");

            entity.Property(e => e.TransTypeDetail)
               .HasMaxLength(25)
               .IsUnicode(false)
               .HasColumnName("TransTypeDetail");
        }
    }
}
