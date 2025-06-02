namespace Assurity.Kafka.Accessors.Entities.TypeConfigurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class SystemDataLoadConfiguration : IEntityTypeConfiguration<SystemDataLoad>
    {
        public void Configure(EntityTypeBuilder<SystemDataLoad> entity)
        {
            entity.ToTable("SystemDataLoad", "commdebt");

            entity.HasKey(systemDataLoad => systemDataLoad.Id);

            entity
                .HasOne(systemDataLoad => systemDataLoad.AgentHierarchyChange)
                .WithOne()
                .HasForeignKey<AgentHierarchyChange>("SystemDataLoadId");

            entity.Property(systemDataLoad => systemDataLoad.LoadType)
                .IsRequired()
                .IsUnicode(false)
                .HasMaxLength(7)
                .HasConversion<string>();

            entity.Property(systemDataLoad => systemDataLoad.Status)
                .IsRequired()
                .IsUnicode(false)
                .HasMaxLength(9)
                .HasConversion<string>();

            entity.Property(systemDataLoad => systemDataLoad.InitiatedDate)
                .HasColumnName("Initiated")
                .IsRequired();

            entity.Property(systemDataLoad => systemDataLoad.StartedDate)
                .HasColumnName("Started");

            entity.Property(systemDataLoad => systemDataLoad.FinishedDate)
                .HasColumnName("Finished");
        }
    }
}