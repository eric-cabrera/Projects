namespace Assurity.AgentPortal.Accessors.DataStore.Entities;

public partial class PhierAgentHierarchy
{
    public string CompanyCode { get; set; } = null!;

    public string AgentNum { get; set; } = null!;

    public string MarketCode { get; set; } = null!;

    public string AgentLevel { get; set; } = null!;

    public int StopDate { get; set; }

    public int StartDate { get; set; }

    public string ReportDesc { get; set; } = null!;

    public string RegionCode { get; set; } = null!;

    public string DealCode { get; set; } = null!;

    public string AddlCommInd { get; set; } = null!;

    public string AddlDealCode { get; set; } = null!;

    public string PayCode { get; set; } = null!;

    public string PayFrequency { get; set; } = null!;

    public string AdvanceIndicator { get; set; } = null!;

    public string AdvPayFrequency { get; set; } = null!;

    public string HierarchyAgent { get; set; } = null!;

    public string HierMarketCode { get; set; } = null!;

    public string HierAgentLevel { get; set; } = null!;

    public string FinancialAgent { get; set; } = null!;

    public string AllocatedAgent { get; set; } = null!;

    public decimal AllocatedPercent { get; set; }

    public int AllocatedAmount { get; set; }

    public int DebitAmt { get; set; }

    public decimal RecovPercent { get; set; }

    public int RecovAmt { get; set; }

    public decimal DeferredPercent { get; set; }

    public int DeferredAmount { get; set; }

    public string AdvancePointer { get; set; } = null!;

    public string FicaInd { get; set; } = null!;

    public string AddlUpHierId { get; set; } = null!;

    public string ReportForm { get; set; } = null!;

    public string ApplToUnsecPolc { get; set; } = null!;

    public string ApplUnsecNonpolc { get; set; } = null!;

    public string ApplyToUnsecBal { get; set; } = null!;

    public decimal FicaPercent { get; set; }

    public int FicaMaximum { get; set; }

    public string StatementAgent { get; set; } = null!;

    public string StatementInd { get; set; } = null!;

    public decimal AllocRenewalPct { get; set; }

    public int AllocRenewalAmt { get; set; }

    public string PayReason { get; set; } = null!;

    public string AgencyCode { get; set; } = null!;

    public int Mga { get; set; }

    public int Ctrl { get; set; }

    public short ControlId { get; set; }

    public string ServicingAgency { get; set; } = null!;

    public string MailSortKey { get; set; } = null!;

    public string NetComm { get; set; } = null!;

    public short BonusLevel { get; set; }

    public string CommType { get; set; } = null!;

    public string DivisionIndicator { get; set; } = null!;

    public string DebitBalIntFlag { get; set; } = null!;

    public decimal DebitBalIntPct { get; set; }

    public string ServiceFeeDeal { get; set; } = null!;

    public decimal ProdPct { get; set; }

    public string DfltCommOptn { get; set; } = null!;

    public string AllocationOption { get; set; } = null!;

    public string ReserveFlag { get; set; } = null!;

    public string ReserveRateId { get; set; } = null!;

    public decimal ReserveFlatAmt { get; set; }

    public decimal ReserveMaxBal { get; set; }

    public decimal ReserveMinBal { get; set; }

    public string BonusAllowedFlag { get; set; } = null!;

    public string BonusCode { get; set; } = null!;

    public string AssignmentFlag { get; set; } = null!;

    public int LastChangeDate { get; set; }

    public int LastChangeTime { get; set; }
}
