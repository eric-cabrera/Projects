namespace Assurity.Kafka.Accessors.Context
{
    using Assurity.Kafka.Accessors.Entities;
    using Microsoft.EntityFrameworkCore;

    public class BaseContext : DbContext
    {
        public BaseContext(DbContextOptions<DataStoreContext> options)
            : base(options)
        {
        }

        public BaseContext(DbContextOptions<LifeProContext> options)
            : base(options)
        {
        }

        public virtual DbSet<PPOLC> PPOLC { get; set; }

        public virtual DbSet<PPOLM_POLICY_BENEFIT_MISC> PPOLM_POLICY_BENEFIT_MISC { get; set; }

        public virtual DbSet<PACTG> PACTG { get; set; }

        public virtual DbSet<PICDA_WAIVER_DETAILS> PICDA_WAIVER_DETAILS { get; set; }

        public virtual DbSet<ProductDescription> ProductDescription { get; set; }

        public virtual DbSet<PPEND_NEW_BUSINESS_PENDING> PPEND_NEW_BUSINESS_PENDING { get; set; }

        public virtual DbSet<PPEND_NEW_BUS_PEND_UNDERWRITING> PPEND_NEW_BUS_PEND_UNDERWRITING { get; set; }

        public virtual DbSet<PPBEN_POLICY_BENEFITS> PPBEN_POLICY_BENEFITS { get; set; }

        public virtual DbSet<PRELA_RELATIONSHIP_MASTER> PRELA_RELATIONSHIP_MASTER { get; set; }

        public virtual DbSet<PMEDR> PMEDR { get; set; }

        public virtual DbSet<PAGNT_AGENT_MASTER> PAGNT_AGENT_MASTER { get; set; }

        public virtual DbSet<PMUIN_MULTIPLE_INSUREDS> PMUIN_MULTIPLE_INSUREDS { get; set; }

        public virtual DbSet<PNAME> PNAME { get; set; }

        public virtual DbSet<PNALK> PNALK { get; set; }

        public virtual DbSet<PRQRM> PRQRM { get; set; }

        public virtual DbSet<PRQRMTBL> PRQRMTBL { get; set; }

        public virtual DbSet<PADDR> PADDR { get; set; }

        public virtual DbSet<PPBEN_POLICY_BENEFITS_TYPES_BA_OR> PPBEN_POLICY_BENEFITS_TYPES_BA_OR { get; set; }

        public virtual DbSet<PPBEN_POLICY_BENEFITS_TYPES_BF> PPBEN_POLICY_BENEFITS_TYPES_BF { get; set; }

        public virtual DbSet<PPBEN_POLICY_BENEFITS_TYPES_SU> PPBEN_POLICY_BENEFITS_TYPES_SU { get; set; }

        public virtual DbSet<PPBEN_POLICY_BENEFITS_TYPES_SL> PPBEN_POLICY_BENEFITS_TYPES_SL { get; set; }

        public virtual DbSet<PPBEN_POLICY_BENEFITS_TYPES_SP> PPBEN_POLICY_BENEFITS_TYPES_SP { get; set; }

        public virtual DbSet<PKDEF_KEY_DEFINITION> PKDEF_KEY_DEFINITION { get; set; }

        public virtual DbSet<PCEXP_COVERAGE_EXPANSION> PCEXP_COVERAGE_EXPANSION { get; set; }

        public virtual DbSet<PCEXP_COVERAGE_EXPANSION_UWCLS_DETAILS> PCEXP_COVERAGE_EXPANSION_UWCLS_DETAILS { get; set; }

        public virtual DbSet<PCOMC_COMMISSION_CONTROL> PCOMC_COMMISSION_CONTROL { get; set; }

        public virtual DbSet<PCOMC_COMMISSION_CONTROL_TYPE_S> PCOMC_COMMISSION_CONTROL_TYPE_S { get; set; }

        public virtual DbSet<PCOVR_PRODUCT_COVERAGES> PCOVR_PRODUCT_COVERAGES { get; set; }

        public virtual DbSet<PGRUP_GROUP_MASTER> PGRUP_GROUP_MASTER { get; set; }

        public virtual DbSet<PHIER_AGENT_HIERARCHY> PHIER_AGENT_HIERARCHY { get; set; }

        public virtual DbSet<PBDRV> PBDRV { get; set; }

        public virtual DbSet<PACON_ANNUITY_POLICY> PACON_ANNUITY_POLICY { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PPOLC>().HasKey(ppolc =>
                new { ppolc.COMPANY_CODE, ppolc.POLICY_NUMBER });
            modelBuilder.Entity<PRELA_RELATIONSHIP_MASTER>().HasKey(prela =>
                new { prela.NAME_ID, prela.RELATE_CODE, prela.IDENTIFYING_ALPHA, prela.BENEFIT_SEQ_NUMBER });
            modelBuilder.Entity<PNALK>().HasKey(pnalk =>
                new { pnalk.NAME_ID, pnalk.ADDRESS_CODE, pnalk.ADDRESS_ID });
            modelBuilder.Entity<PICDA_WAIVER_DETAILS>().HasKey(picda =>
                new { picda.TYPE_CODE, picda.KEY_DATA, picda.RECORD_SEQUENCE });
            modelBuilder.Entity<PRQRM>().HasKey(prqrm =>
                new { prqrm.COMPANY_CODE, prqrm.POLICY_NUMBER, prqrm.NAME_ID, prqrm.REQ_SEQUENCE });
            modelBuilder.Entity<PRQRMTBL>().HasKey(prqrmtbl =>
                new { prqrmtbl.COMPANY_CODE, prqrmtbl.POLICY_NUMBER, prqrmtbl.NAME_ID, prqrmtbl.REQ_SEQUENCE, prqrmtbl.SEQ });
            modelBuilder.Entity<PMEDR>().HasKey(pmedr =>
               new { pmedr.RECORD_TYPE, pmedr.REQ_NUMBER });
            modelBuilder.Entity<PPEND_NEW_BUS_PEND_UNDERWRITING>().HasKey(pendu =>
                new { pendu.PEND_ID, pendu.IDX });
            modelBuilder.Entity<PCEXP_COVERAGE_EXPANSION_UWCLS_DETAILS>().HasKey(pcexpd =>
                new { pcexpd.CEXP_ID, pcexpd.IDX });
            modelBuilder.Entity<PHIER_AGENT_HIERARCHY>().HasKey(phier =>
                new { phier.COMPANY_CODE, phier.AGENT_NUM, phier.AGENT_LEVEL, phier.MARKET_CODE, phier.STOP_DATE });
            modelBuilder.Entity<PMUIN_MULTIPLE_INSUREDS>().HasKey(pmuin =>
                new { pmuin.COMPANY_CODE, pmuin.POLICY_NUMBER, pmuin.BENEFIT_SEQ, pmuin.STOP_DATE, pmuin.NAME_ID });
            modelBuilder.Entity<SysNBRequirements>().HasKey(sysnbreq =>
                new { sysnbreq.POLICYNUMBER, sysnbreq.REQSEQ, sysnbreq.IX, sysnbreq.REQTYPE, sysnbreq.REQNOTE });
            modelBuilder.Entity<PACTG>().HasKey(pactg =>
                new { pactg.COMPANY_CODE, pactg.POLICY_NUMBER, pactg.BENEFIT_SEQ, pactg.DATE_ADDED, pactg.TIME_ADDED, pactg.RECORD_SEQUENCE, pactg.LIFEPRO_ID });
            modelBuilder.Entity<PBDRV>().HasKey(pbdrv =>
                new { pbdrv.DESCRIPTION, pbdrv.STATUS_CODE, pbdrv.BATCH_START_DATE, pbdrv.BATCH_STOP_DATE });
            modelBuilder.Entity<PACON_ANNUITY_POLICY>().HasKey(pacon =>
                new { pacon.COMPANY_CODE, pacon.POLICY_NUMBER });
            modelBuilder.Entity<PCOMC_COMMISSION_CONTROL_TYPE_S>().HasKey(pcomc =>
                new { pcomc.COMC_ID, pcomc.I });

            modelBuilder.Entity<PPBEN_POLICY_BENEFITS>()
                .HasOne(ppben => ppben.PCOVR_PRODUCT_COVERAGES)
                .WithOne()
                .HasForeignKey<PCOVR_PRODUCT_COVERAGES>(pco => pco.COVERAGE_ID)
                .HasPrincipalKey<PPBEN_POLICY_BENEFITS>(ppben => ppben.PLAN_CODE);

            modelBuilder.Entity<PPBEN_POLICY_BENEFITS>()
                .HasOne(ppben => ppben.PPBEN_POLICY_BENEFITS_TYPES_BA_OR)
                .WithOne()
                .HasForeignKey<PPBEN_POLICY_BENEFITS_TYPES_BA_OR>(pco => pco.PBEN_ID)
                .HasPrincipalKey<PPBEN_POLICY_BENEFITS>(ppben => ppben.PBEN_ID);

            modelBuilder.Entity<PPBEN_POLICY_BENEFITS>()
                .HasOne(ppben => ppben.PPBEN_POLICY_BENEFITS_TYPES_BF)
                .WithOne()
                .HasForeignKey<PPBEN_POLICY_BENEFITS_TYPES_BF>(type => type.PBEN_ID)
                .HasPrincipalKey<PPBEN_POLICY_BENEFITS>(ppben => ppben.PBEN_ID);

            modelBuilder.Entity<PPBEN_POLICY_BENEFITS>()
                .HasOne(ppben => ppben.PPBEN_POLICY_BENEFITS_TYPES_SU)
                .WithOne()
                .HasForeignKey<PPBEN_POLICY_BENEFITS_TYPES_SU>(type => type.PBEN_ID)
                .HasPrincipalKey<PPBEN_POLICY_BENEFITS>(ppben => ppben.PBEN_ID);

            modelBuilder.Entity<PPBEN_POLICY_BENEFITS>()
                .HasOne(ppben => ppben.PPBEN_POLICY_BENEFITS_TYPES_SL)
                .WithOne()
                .HasForeignKey<PPBEN_POLICY_BENEFITS_TYPES_SL>(type => type.PBEN_ID)
                .HasPrincipalKey<PPBEN_POLICY_BENEFITS>(ppben => ppben.PBEN_ID);

            modelBuilder.Entity<PPBEN_POLICY_BENEFITS>()
                .HasOne(ppben => ppben.PPBEN_POLICY_BENEFITS_TYPES_SP)
                .WithOne()
                .HasForeignKey<PPBEN_POLICY_BENEFITS_TYPES_SP>(type => type.PBEN_ID)
                .HasPrincipalKey<PPBEN_POLICY_BENEFITS>(ppben => ppben.PBEN_ID);

            modelBuilder.Entity<PPBEN_POLICY_BENEFITS>()
                .HasMany(ppben => ppben.PMUIN_MULTIPLE_INSUREDs)
                .WithOne()
                .HasForeignKey(pmuin => new { pmuin.POLICY_NUMBER, pmuin.COMPANY_CODE, pmuin.BENEFIT_SEQ })
                .HasPrincipalKey(ppben => new { ppben.POLICY_NUMBER, ppben.COMPANY_CODE, ppben.BENEFIT_SEQ });

            modelBuilder.Entity<PPBEN_POLICY_BENEFITS>()
                .HasOne(ppben => ppben.PCEXP_COVERAGE_EXPANSION)
                .WithOne()
                .HasForeignKey<PCEXP_COVERAGE_EXPANSION>(exp => exp.COVERAGE_ID)
                .HasPrincipalKey<PPBEN_POLICY_BENEFITS>(ppben => ppben.PLAN_CODE);

            modelBuilder.Entity<PCEXP_COVERAGE_EXPANSION>()
                .HasMany(ppben => ppben.PCEXP_COVERAGE_EXPANSION_UWCLS_DETAILS)
                .WithOne()
                .HasForeignKey(pcexp => pcexp.CEXP_ID)
                .HasPrincipalKey(exp => exp.CEXP_ID);

            modelBuilder.Entity<PPOLC>()
                .HasKey(ppolc => new { ppolc.COMPANY_CODE, ppolc.POLICY_NUMBER });

            modelBuilder.Entity<PPOLC>()
                .HasOne(ppolc => ppolc.PACON_ANNUITY_POLICY)
                .WithOne()
                .HasForeignKey<PACON_ANNUITY_POLICY>(pacon => new { pacon.COMPANY_CODE, pacon.POLICY_NUMBER })
                .HasPrincipalKey<PPOLC>(ppolc => new { ppolc.COMPANY_CODE, ppolc.POLICY_NUMBER })
                .IsRequired(false);

            modelBuilder.Entity<PPOLC>()
                .HasOne(ppolc => ppolc.PGRUP_GROUP_MASTER)
                .WithMany()
                .HasForeignKey(ppolc => ppolc.GROUP_NUMBER)
                .HasPrincipalKey(pgrup => pgrup.GROUP_NUMBER)
                .IsRequired(false);

            modelBuilder.Entity<PPOLC>()
                .HasOne(ppolc => ppolc.PPEND_NEW_BUSINESS_PENDING)
                .WithOne()
                .HasForeignKey<PPEND_NEW_BUSINESS_PENDING>(ppend => ppend.POLICY_NUMBER)
                .HasPrincipalKey<PPOLC>(ppolc => ppolc.POLICY_NUMBER)
                .IsRequired(false);

            modelBuilder.Entity<PGRUP_GROUP_MASTER>()
                .HasOne(pgrup => pgrup.PNAME)
                .WithOne()
                .HasForeignKey<PGRUP_GROUP_MASTER>(pgrup => pgrup.NAME_ID)
                .HasPrincipalKey<PNAME>(pname => pname.NAME_ID);

            modelBuilder.Entity<PRELA_RELATIONSHIP_MASTER>()
                .HasKey(prela => new { prela.NAME_ID, prela.RELATE_CODE, prela.IDENTIFYING_ALPHA, prela.BENEFIT_SEQ_NUMBER });

            modelBuilder.Entity<PRELA_RELATIONSHIP_MASTER>()
                .HasOne(prela => prela.PNAME)
                .WithMany()
                .HasForeignKey(prela => prela.NAME_ID)
                .HasPrincipalKey(pname => pname.NAME_ID);

            modelBuilder.Entity<PNAME>()
                .HasMany(prela => prela.PNALKs)
                .WithOne()
                .HasForeignKey(prela => prela.NAME_ID)
                .HasPrincipalKey(pname => pname.NAME_ID)
                .IsRequired(false);

            modelBuilder.Entity<PNALK>()
                .HasKey(pnalk => new { pnalk.NAME_ID, pnalk.ADDRESS_CODE, pnalk.ADDRESS_ID });

            modelBuilder.Entity<PNALK>()
                .HasOne(pnalk => pnalk.PADDR)
                .WithOne()
                .HasForeignKey<PADDR>(paddr => paddr.ADDRESS_ID)
                .HasPrincipalKey<PNALK>(pnalk => pnalk.ADDRESS_ID);

            modelBuilder.Entity<PCOMC_COMMISSION_CONTROL>()
                .HasOne(pcomc => pcomc.PCOMC_COMMISSION_CONTROL_TYPE_S)
                .WithOne()
                .HasForeignKey<PCOMC_COMMISSION_CONTROL_TYPE_S>(pcomcs => pcomcs.COMC_ID)
                .HasPrincipalKey<PCOMC_COMMISSION_CONTROL>(pcomc => pcomc.COMC_ID);

            modelBuilder.Entity<PCOMC_COMMISSION_CONTROL_TYPE_S>()
                .HasMany(pcomc => pcomc.PAGNT_AGENT_MASTERs)
                .WithOne()
                .HasForeignKey(pagent => pagent.AGENT_NUMBER)
                .HasPrincipalKey(pcomc => pcomc.AGENT);

            modelBuilder.Entity<PAGNT_AGENT_MASTER>()
                .HasOne(agent => agent.PNAME)
                .WithOne()
                .HasForeignKey<PNAME>(pname => pname.NAME_ID)
                .HasPrincipalKey<PAGNT_AGENT_MASTER>(agent => agent.NAME_ID);
        }
    }
}