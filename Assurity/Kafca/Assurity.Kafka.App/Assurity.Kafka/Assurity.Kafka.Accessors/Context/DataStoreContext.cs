namespace Assurity.Kafka.Accessors.Context
{
    using Assurity.Kafka.Accessors.Entities;
    using Assurity.Kafka.Accessors.Entities.TypeConfigurations;
    using Microsoft.EntityFrameworkCore;

    public class DataStoreContext : BaseContext
    {
        public DataStoreContext(DbContextOptions<DataStoreContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AgentHierarchyChange> AgentHierarchyChanges { get; set; }

        public virtual DbSet<SystemDataLoad> SystemDataLoads { get; set; }

        /// <summary>
        /// DataStore context configuration.
        /// </summary>
        /// <remarks>
        /// DataStore accessor resources are for the MigrationWorker.
        /// It tends to be working with lage amounts of data and has been optimized
        /// using EF Navigation Properties to perform joins and pull larger amounts of data
        /// than the LifeProAccessor.
        /// </remarks>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasDefaultSchema("lifepro_r");
            modelBuilder.Entity<AgentQueryEntity>().HasNoKey();
            modelBuilder.Entity<BenefitDataQueryEntity>().HasNoKey();
            modelBuilder.Entity<ExtendedKeyQueryEntity>().HasNoKey();
            modelBuilder.Entity<ProductDescription>().ToTable("ProductDescription", "alic");

            modelBuilder.ApplyConfiguration(new AgentHierarchyChangeConfiguration());
            modelBuilder.ApplyConfiguration(new SystemDataLoadConfiguration());
        }
    }
}
