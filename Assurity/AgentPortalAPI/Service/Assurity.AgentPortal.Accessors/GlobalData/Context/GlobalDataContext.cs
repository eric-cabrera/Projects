namespace Assurity.AgentPortal.Accessors.GlobalData.Context;

using Assurity.AgentPortal.Accessors.GlobalData.Entities;
using Assurity.AgentPortal.Accessors.GlobalData.Entities.EntityTypeConfigurations;
using Microsoft.EntityFrameworkCore;

public class GlobalDataContext : DbContext
{
    public GlobalDataContext(DbContextOptions<GlobalDataContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Attributes> Attributes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("dbo");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AttributesEntityTypeConfiguration).Assembly);
    }
}