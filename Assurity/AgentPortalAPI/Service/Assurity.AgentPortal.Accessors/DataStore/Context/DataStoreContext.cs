namespace Assurity.AgentPortal.Accessors.DataStore.Context;

using Assurity.AgentPortal.Accessors.DataStore.Entities;
using Assurity.AgentPortal.Accessors.DataStore.Entities.EntityTypeConfigurations;
using Microsoft.EntityFrameworkCore;

public class DataStoreContext : DbContext
{
    public DataStoreContext()
    {
    }

    public DataStoreContext(DbContextOptions<DataStoreContext> options)
        : base(options)
    {
    }

    public virtual DbSet<FiservDistributionChannel> FiservDistributionChannel { get; set; }

    public virtual DbSet<LifePortraits> LifePortraits { get; set; }

    public virtual DbSet<PhierAgentHierarchy> PhierAgentHierarchies { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("integrations");

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FiservDistributionChannelEntityTypeConfiguration).Assembly);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LifePortraits).Assembly);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PhierAgentHierarchyEntityTypeConfiguration).Assembly);
    }
}
