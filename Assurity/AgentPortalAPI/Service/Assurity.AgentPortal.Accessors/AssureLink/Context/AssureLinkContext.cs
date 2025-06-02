namespace Assurity.AgentPortal.Accessors.AssureLink.Context;

using Assurity.AgentPortal.Accessors.AssureLink.Entities;
using Assurity.AgentPortal.Accessors.AssureLink.Entities.EntityTypeConfigurations;
using Microsoft.EntityFrameworkCore;

public class AssureLinkContext : DbContext
{
    public AssureLinkContext()
    {
    }

    public AssureLinkContext(DbContextOptions<AssureLinkContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AgentOptions> AgentOptions { get; set; }

    public virtual DbSet<AgentLinkOptions> AgentLinkOptions { get; set; }

    public virtual DbSet<AgentPersonalInformation> AgentPersonalInformation { get; set; }

    public virtual DbSet<DistributionList> DistributionLists { get; set; }

    public virtual DbSet<DistributionMaster> DistributionMasters { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AgentLinkOptionsEntityTypeConfiguration).Assembly);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AgentOptionsEntityTypeConfiguration).Assembly);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AgentPersonalInformationEntityTypeConfiguration).Assembly);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DistributionListEntityTypeConfiguration).Assembly);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DistributionMasterEntityTypeConfiguration).Assembly);
    }
}
