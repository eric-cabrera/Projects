namespace Assurity.AgentPortal.Accessors.GlobalData.Entities.EntityTypeConfigurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class AttributesEntityTypeConfiguration : IEntityTypeConfiguration<Attributes>
{
    public void Configure(EntityTypeBuilder<Attributes> entity)
    {
        entity.ToTable("ATTRIBUTES");

        entity.HasKey(attributes => attributes.ObjectId);

        entity.Property(attributes => attributes.ObjectId)
            .HasColumnName("OBJECT_ID")
            .HasMaxLength(44)
            .IsRequired()
            .IsUnicode(false);

        entity.Property(attributes => attributes.ObjectClass)
            .HasColumnName("OBJECT_CLASS")
            .HasMaxLength(19)
            .IsRequired()
            .IsUnicode(false);

        entity.Property(attributes => attributes.DocType)
            .HasMaxLength(25)
            .IsUnicode(false);

        entity.Property(attributes => attributes.PrintInclude)
            .HasMaxLength(1)
            .IsUnicode(false);

        entity.Property(attributes => attributes.ObjectType)
            .HasColumnName("OBJECT_TYPE")
            .IsRequired();

        entity.Property(attributes => attributes.PolicyNumber)
            .HasMaxLength(12)
            .IsUnicode(false);
    }
}