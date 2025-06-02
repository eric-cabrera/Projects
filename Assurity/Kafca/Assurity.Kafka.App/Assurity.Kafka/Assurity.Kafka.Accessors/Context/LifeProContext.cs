namespace Assurity.Kafka.Accessors.Context
{
    using Assurity.Kafka.Accessors.Entities;
    using Microsoft.EntityFrameworkCore;

    public class LifeProContext : BaseContext
    {
        public LifeProContext(DbContextOptions<LifeProContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// LifePro context configuration.
        /// </summary>
        /// <remarks>
        /// The navigation properties configured in the shared entities namespace will affect
        /// the LifeProAccessor queries if not ".Ignore()"d as they are below.
        /// </remarks>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasDefaultSchema("dbo");
            modelBuilder.Entity<AgentQueryEntity>().HasNoKey();
            modelBuilder.Entity<BenefitDataQueryEntity>().HasNoKey();
            modelBuilder.Entity<ExtendedKeyQueryEntity>().HasNoKey();
            modelBuilder.Entity<ProductDescription>().ToTable("ProductDescription", "dbo");
        }
    }
}
