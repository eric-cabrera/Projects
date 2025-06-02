namespace Assurity.Kafka.Accessors.Context
{
    using Assurity.Kafka.Accessors.Entities;
    using Assurity.Kafka.Accessors.Entities.TypeConfigurations;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// The database context used to access the GlobalData database.
    /// </summary>
    public partial class GlobalDataContext : DbContext
    {
        public GlobalDataContext(DbContextOptions<GlobalDataContext> options)
            : base(options)
        {
        }

        public virtual DbSet<QUEUES> Queues { get; set; } = null!;

        public virtual DbSet<SysACAgentData> SysACAgentData { get; set; } = null!;

        public virtual DbSet<SysACAgentMarketCodes> SysACAgentMarketCodes { get; set; } = null!;

        public virtual DbSet<SysNBRequirements> SysNBRequirements { get; set; } = null!;

        public virtual DbSet<SysZ9Process> SysZ9processes { get; set; } = null!;

        public virtual DbSet<VAttributesunionArc> VAttributesunionArcs { get; set; } = null!;

        public virtual DbSet<Attributes> Attributes { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(QueuesConfiguration).Assembly);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(SysACAgentDataConfiguration).Assembly);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(SysAcagentMarketCodeConfiguration).Assembly);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(SysNBRequirementsConfiguration).Assembly);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(SysZ9ProcessConfiguration).Assembly);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(VAttributesunionArcConfiguration).Assembly);
        }
    }
}
