namespace Assurity.Kafka.Accessors.Context
{
    using Assurity.Kafka.Accessors.Entities;
    using Assurity.Kafka.Accessors.Entities.TypeConfigurations;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// The database context used to access the SupportData database.
    /// </summary>
    public partial class SupportDataContext : DbContext
    {
        public SupportDataContext(DbContextOptions<SupportDataContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AgentUseQueue> AgentUseQueues { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AgentUseQueueConfiguration).Assembly);
        }
    }
}
