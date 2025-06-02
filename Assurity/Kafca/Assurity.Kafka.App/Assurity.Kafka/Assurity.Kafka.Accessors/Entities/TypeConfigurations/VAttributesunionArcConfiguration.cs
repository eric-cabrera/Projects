namespace Assurity.Kafka.Accessors.Entities.TypeConfigurations
{
    using Assurity.Kafka.Accessors.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class VAttributesunionArcConfiguration : IEntityTypeConfiguration<VAttributesunionArc>
    {
        public void Configure(EntityTypeBuilder<VAttributesunionArc> entity)
        {
            entity.HasKey(entity => entity.ObjectId);

            entity.ToView("vATTRIBUTESUnionARC", "dbo");

            entity.Property(e => e.ObjectId)
                .HasMaxLength(44)
                .IsUnicode(false)
                .HasColumnName("OBJECT_ID");

            entity.Property(e => e.ObjectType).HasColumnName("OBJECT_TYPE");

            entity.Property(e => e.PolicyNumber)
                .HasMaxLength(12)
                .IsUnicode(false);
        }
    }
}
