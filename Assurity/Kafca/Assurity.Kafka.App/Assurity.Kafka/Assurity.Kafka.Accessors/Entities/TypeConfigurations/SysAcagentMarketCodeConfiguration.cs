namespace Assurity.Kafka.Accessors.Entities.TypeConfigurations
{
    using Assurity.Kafka.Accessors.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class SysAcagentMarketCodeConfiguration : IEntityTypeConfiguration<SysACAgentMarketCodes>
    {
        public void Configure(EntityTypeBuilder<SysACAgentMarketCodes> entity)
        {
            entity.HasKey(e => new { e.FOLDERID, e.MARKETCODE, e.CONTRACTLEVEL });

            entity.ToTable("sysACAgentMarketCodes", "dbo");

            entity.Property(e => e.FOLDERID)
                .HasMaxLength(44)
                .IsUnicode(false)
                .HasColumnName("FOLDERID");

            entity.Property(e => e.MARKETCODE)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("MARKETCODE");

            entity.Property(e => e.CONTRACTLEVEL)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("CONTRACTLEVEL");

            entity.Property(e => e.PENDINGRPTDISABLED).HasColumnName("PENDINGRPTDISABLED");

            entity.Property(e => e.UPLINEAGENTID)
                .HasMaxLength(12)
                .IsUnicode(false)
                .HasColumnName("UPLINEAGENTID");

            entity.Property(e => e.UPLINECONTRACTLEVEL)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("UPLINECONTRACTLEVEL");

            entity.Property(e => e.UPLINEMARKETCODE)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("UPLINEMARKETCODE");
        }
    }
}
