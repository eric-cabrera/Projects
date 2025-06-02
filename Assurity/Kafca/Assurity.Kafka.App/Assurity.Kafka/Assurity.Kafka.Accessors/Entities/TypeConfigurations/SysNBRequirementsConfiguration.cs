namespace Assurity.Kafka.Accessors.Entities.TypeConfigurations
{
    using Assurity.Kafka.Accessors.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class SysNBRequirementsConfiguration : IEntityTypeConfiguration<SysNBRequirements>
    {
        public void Configure(EntityTypeBuilder<SysNBRequirements> entity)
        {
            entity.HasKey(e => new { e.POLICYNUMBER, e.REQSEQ, e.IX, e.REQTYPE });
            entity.ToTable("sysNBRequirements", "dbo");
            entity.Property(e => e.POLICYNUMBER)
                .HasMaxLength(12)
                .IsUnicode(false)
                .HasColumnName("PolicyNumber");
            entity.Property(e => e.REQSEQ)
                .HasColumnName("ReqSeq");
            entity.Property(e => e.IX)
                .HasColumnName("IX");
            entity.Property(e => e.REQTYPE)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("ReqType");
        }
    }
}