namespace Assurity.AgentPortal.Accessors.DataStore.Entities.EntityTypeConfigurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class PhierAgentHierarchyEntityTypeConfiguration : IEntityTypeConfiguration<PhierAgentHierarchy>
{
    public void Configure(EntityTypeBuilder<PhierAgentHierarchy> entity)
    {
        entity.HasKey(e => new { e.CompanyCode, e.AgentNum, e.MarketCode, e.AgentLevel, e.StopDate }).HasName("KPHIER");

        entity.ToTable("PHIER_AGENT_HIERARCHY", "lifepro_r");

        entity.HasIndex(e => new { e.AgentNum, e.MarketCode, e.AgentLevel, e.StopDate, e.StartDate }, "IX_lifepro_r_PHIER_AGENT_HIERARCHY__AGENT_NUM__MARKET_CODE__AGENT_LEVEL__STOP_DATE__START_DATE__INCL");

        entity.HasIndex(e => new { e.HierarchyAgent, e.HierMarketCode, e.HierAgentLevel, e.StopDate, e.StartDate }, "IX_lifepro_r_PHIER_AGENT_HIERARCHY__HIERARCHY_AGENT__HIER_MARKET_CODE__HIER_AGENT_LEVEL__STOP_DATE__START_DATE");

        entity.HasIndex(e => new { e.StopDate, e.StartDate }, "IX_lifepro_r_PHIER_AGENT_HIERARCHY__STOP_DATE__START_DATE__INCL");

        entity.HasIndex(e => new { e.CompanyCode, e.AgentNum, e.StopDate, e.StartDate }, "lp_PHIER_X_1");

        entity.HasIndex(e => new { e.CompanyCode, e.HierarchyAgent, e.HierMarketCode, e.HierAgentLevel, e.StopDate }, "lp_PHIER_X_2");

        entity.Property(e => e.CompanyCode)
            .HasMaxLength(2)
            .IsUnicode(false)
            .IsFixedLength()
            .HasColumnName("COMPANY_CODE");
        entity.Property(e => e.AgentNum)
            .HasMaxLength(12)
            .IsFixedLength()
            .HasColumnName("AGENT_NUM");
        entity.Property(e => e.MarketCode)
            .HasMaxLength(10)
            .IsFixedLength()
            .HasColumnName("MARKET_CODE");
        entity.Property(e => e.AgentLevel)
            .HasMaxLength(2)
            .IsUnicode(false)
            .IsFixedLength()
            .HasColumnName("AGENT_LEVEL");
        entity.Property(e => e.StopDate).HasColumnName("STOP_DATE");
        entity.Property(e => e.AddlCommInd)
            .HasMaxLength(1)
            .IsUnicode(false)
            .IsFixedLength()
            .HasColumnName("ADDL_COMM_IND");
        entity.Property(e => e.AddlDealCode)
            .HasMaxLength(5)
            .IsFixedLength()
            .HasColumnName("ADDL_DEAL_CODE");
        entity.Property(e => e.AddlUpHierId)
            .HasMaxLength(1)
            .IsUnicode(false)
            .IsFixedLength()
            .HasColumnName("ADDL_UP_HIER_ID");
        entity.Property(e => e.AdvPayFrequency)
            .HasMaxLength(1)
            .IsUnicode(false)
            .IsFixedLength()
            .HasColumnName("ADV_PAY_FREQUENCY");
        entity.Property(e => e.AdvanceIndicator)
            .HasMaxLength(1)
            .IsUnicode(false)
            .IsFixedLength()
            .HasColumnName("ADVANCE_INDICATOR");
        entity.Property(e => e.AdvancePointer)
            .HasMaxLength(5)
            .IsFixedLength()
            .HasColumnName("ADVANCE_POINTER");
        entity.Property(e => e.AgencyCode)
            .HasMaxLength(5)
            .IsFixedLength()
            .HasColumnName("AGENCY_CODE");
        entity.Property(e => e.AllocRenewalAmt).HasColumnName("ALLOC_RENEWAL_AMT");
        entity.Property(e => e.AllocRenewalPct)
            .HasColumnType("decimal(5, 2)")
            .HasColumnName("ALLOC_RENEWAL_PCT");
        entity.Property(e => e.AllocatedAgent)
            .HasMaxLength(12)
            .IsFixedLength()
            .HasColumnName("ALLOCATED_AGENT");
        entity.Property(e => e.AllocatedAmount).HasColumnName("ALLOCATED_AMOUNT");
        entity.Property(e => e.AllocatedPercent)
            .HasColumnType("decimal(5, 2)")
            .HasColumnName("ALLOCATED_PERCENT");
        entity.Property(e => e.AllocationOption)
            .HasMaxLength(1)
            .IsUnicode(false)
            .IsFixedLength()
            .HasColumnName("ALLOCATION_OPTION");
        entity.Property(e => e.ApplToUnsecPolc)
            .HasMaxLength(1)
            .IsUnicode(false)
            .IsFixedLength()
            .HasColumnName("APPL_TO_UNSEC_POLC");
        entity.Property(e => e.ApplUnsecNonpolc)
            .HasMaxLength(1)
            .IsUnicode(false)
            .IsFixedLength()
            .HasColumnName("APPL_UNSEC_NONPOLC");
        entity.Property(e => e.ApplyToUnsecBal)
            .HasMaxLength(1)
            .IsUnicode(false)
            .IsFixedLength()
            .HasColumnName("APPLY_TO_UNSEC_BAL");
        entity.Property(e => e.AssignmentFlag)
            .HasMaxLength(1)
            .IsUnicode(false)
            .IsFixedLength()
            .HasColumnName("ASSIGNMENT_FLAG");
        entity.Property(e => e.BonusAllowedFlag)
            .HasMaxLength(1)
            .IsUnicode(false)
            .IsFixedLength()
            .HasColumnName("BONUS_ALLOWED_FLAG");
        entity.Property(e => e.BonusCode)
            .HasMaxLength(5)
            .IsFixedLength()
            .HasColumnName("BONUS_CODE");
        entity.Property(e => e.BonusLevel).HasColumnName("BONUS_LEVEL");
        entity.Property(e => e.CommType)
            .HasMaxLength(1)
            .IsUnicode(false)
            .IsFixedLength()
            .HasColumnName("COMM_TYPE");
        entity.Property(e => e.ControlId).HasColumnName("CONTROL_ID");
        entity.Property(e => e.Ctrl).HasColumnName("CTRL");
        entity.Property(e => e.DealCode)
            .HasMaxLength(5)
            .IsFixedLength()
            .HasColumnName("DEAL_CODE");
        entity.Property(e => e.DebitAmt).HasColumnName("DEBIT_AMT");
        entity.Property(e => e.DebitBalIntFlag)
            .HasMaxLength(1)
            .IsUnicode(false)
            .IsFixedLength()
            .HasColumnName("DEBIT_BAL_INT_FLAG");
        entity.Property(e => e.DebitBalIntPct)
            .HasColumnType("decimal(5, 2)")
            .HasColumnName("DEBIT_BAL_INT_PCT");
        entity.Property(e => e.DeferredAmount).HasColumnName("DEFERRED_AMOUNT");
        entity.Property(e => e.DeferredPercent)
            .HasColumnType("decimal(5, 2)")
            .HasColumnName("DEFERRED_PERCENT");
        entity.Property(e => e.DfltCommOptn)
            .HasMaxLength(2)
            .IsUnicode(false)
            .IsFixedLength()
            .HasColumnName("DFLT_COMM_OPTN");
        entity.Property(e => e.DivisionIndicator)
            .HasMaxLength(1)
            .IsUnicode(false)
            .IsFixedLength()
            .HasColumnName("DIVISION_INDICATOR");
        entity.Property(e => e.FicaInd)
            .HasMaxLength(1)
            .IsUnicode(false)
            .IsFixedLength()
            .HasColumnName("FICA_IND");
        entity.Property(e => e.FicaMaximum).HasColumnName("FICA_MAXIMUM");
        entity.Property(e => e.FicaPercent)
            .HasColumnType("decimal(5, 2)")
            .HasColumnName("FICA_PERCENT");
        entity.Property(e => e.FinancialAgent)
            .HasMaxLength(12)
            .IsFixedLength()
            .HasColumnName("FINANCIAL_AGENT");
        entity.Property(e => e.HierAgentLevel)
            .HasMaxLength(2)
            .IsUnicode(false)
            .IsFixedLength()
            .HasColumnName("HIER_AGENT_LEVEL");
        entity.Property(e => e.HierMarketCode)
            .HasMaxLength(10)
            .IsFixedLength()
            .HasColumnName("HIER_MARKET_CODE");
        entity.Property(e => e.HierarchyAgent)
            .HasMaxLength(12)
            .IsFixedLength()
            .HasColumnName("HIERARCHY_AGENT");
        entity.Property(e => e.LastChangeDate).HasColumnName("LAST_CHANGE_DATE");
        entity.Property(e => e.LastChangeTime).HasColumnName("LAST_CHANGE_TIME");
        entity.Property(e => e.MailSortKey)
            .HasMaxLength(12)
            .IsFixedLength()
            .HasColumnName("MAIL_SORT_KEY");
        entity.Property(e => e.Mga).HasColumnName("MGA");
        entity.Property(e => e.NetComm)
            .HasMaxLength(1)
            .IsUnicode(false)
            .IsFixedLength()
            .HasColumnName("NET_COMM");
        entity.Property(e => e.PayCode)
            .HasMaxLength(1)
            .IsUnicode(false)
            .IsFixedLength()
            .HasColumnName("PAY_CODE");
        entity.Property(e => e.PayFrequency)
            .HasMaxLength(1)
            .IsUnicode(false)
            .IsFixedLength()
            .HasColumnName("PAY_FREQUENCY");
        entity.Property(e => e.PayReason)
            .HasMaxLength(2)
            .IsUnicode(false)
            .IsFixedLength()
            .HasColumnName("PAY_REASON");
        entity.Property(e => e.ProdPct)
            .HasColumnType("decimal(5, 2)")
            .HasColumnName("PROD_PCT");
        entity.Property(e => e.RecovAmt).HasColumnName("RECOV_AMT");
        entity.Property(e => e.RecovPercent)
            .HasColumnType("decimal(5, 2)")
            .HasColumnName("RECOV_PERCENT");
        entity.Property(e => e.RegionCode)
            .HasMaxLength(6)
            .IsFixedLength()
            .HasColumnName("REGION_CODE");
        entity.Property(e => e.ReportDesc)
            .HasMaxLength(3)
            .IsUnicode(false)
            .IsFixedLength()
            .HasColumnName("REPORT_DESC");
        entity.Property(e => e.ReportForm)
            .HasMaxLength(4)
            .IsUnicode(false)
            .IsFixedLength()
            .HasColumnName("REPORT_FORM");
        entity.Property(e => e.ReserveFlag)
            .HasMaxLength(1)
            .IsUnicode(false)
            .IsFixedLength()
            .HasColumnName("RESERVE_FLAG");
        entity.Property(e => e.ReserveFlatAmt)
            .HasColumnType("decimal(9, 2)")
            .HasColumnName("RESERVE_FLAT_AMT");
        entity.Property(e => e.ReserveMaxBal)
            .HasColumnType("decimal(9, 2)")
            .HasColumnName("RESERVE_MAX_BAL");
        entity.Property(e => e.ReserveMinBal)
            .HasColumnType("decimal(9, 2)")
            .HasColumnName("RESERVE_MIN_BAL");
        entity.Property(e => e.ReserveRateId)
            .HasMaxLength(10)
            .IsFixedLength()
            .HasColumnName("RESERVE_RATE_ID");
        entity.Property(e => e.ServiceFeeDeal)
            .HasMaxLength(5)
            .IsFixedLength()
            .HasColumnName("SERVICE_FEE_DEAL");
        entity.Property(e => e.ServicingAgency)
            .HasMaxLength(12)
            .IsFixedLength()
            .HasColumnName("SERVICING_AGENCY");
        entity.Property(e => e.StartDate).HasColumnName("START_DATE");
        entity.Property(e => e.StatementAgent)
            .HasMaxLength(12)
            .IsFixedLength()
            .HasColumnName("STATEMENT_AGENT");
        entity.Property(e => e.StatementInd)
            .HasMaxLength(1)
            .IsUnicode(false)
            .IsFixedLength()
            .HasColumnName("STATEMENT_IND");
    }
}
