namespace Assurity.Kafka.Accessors.Entities.TypeConfigurations
{
    using Assurity.Kafka.Accessors.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class SysZ9ProcessConfiguration : IEntityTypeConfiguration<SysZ9Process>
    {
        public void Configure(EntityTypeBuilder<SysZ9Process> entity)
        {
            entity.HasKey(e => e.RECORDID);

            entity.ToTable("sysZ9Process", "dbo");

            entity.HasIndex(e => e.NBFOLDEROBJID, "IX_sysZ9Process");

            entity.Property(e => e.RECORDID)
                .HasColumnType("numeric(18, 0)")
                .ValueGeneratedOnAdd()
                .HasColumnName("RecordID");

            entity.Property(e => e.AGENTID)
                .HasMaxLength(12)
                .IsUnicode(false)
                .HasColumnName("AgentID");

            entity.Property(e => e.AGENTMARKETCODE)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("AgentMarketCode");

            entity.Property(e => e.AGENTLEVEL)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("AgentLevel");

            entity.Property(e => e.NBFOLDEROBJID)
                .HasMaxLength(44)
                .IsUnicode(false)
                .HasColumnName("NBFolderObjID");
        }
    }
}
