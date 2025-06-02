namespace Assurity.Kafka.Accessors.Entities.TypeConfigurations
{
    using Assurity.Kafka.Accessors.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class SysACAgentDataConfiguration : IEntityTypeConfiguration<SysACAgentData>
    {
        public void Configure(EntityTypeBuilder<SysACAgentData> entity)
        {
            entity.HasKey(e => e.FOLDERID);

            entity.ToTable("sysACAgentData", "dbo");

            entity.Property(e => e.FOLDERID)
                .HasMaxLength(44)
                .IsUnicode(false)
                .HasColumnName("FOLDERID");

            entity.Property(e => e.AGENTID)
                .HasMaxLength(12)
                .IsUnicode(false)
                .HasColumnName("AGENTID");

            entity.Property(e => e.FIRSTNAME)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("FIRSTNAME");

            entity.Property(e => e.MIDDLENAME)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("MIDDLENAME");

            entity.Property(e => e.LASTNAME)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("LASTNAME");

            entity.Property(e => e.BUSINESSNAME)
                .HasMaxLength(70)
                .IsUnicode(false)
                .HasColumnName("BUSINESSNAME");
        }
    }
}
