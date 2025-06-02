import { CaseManagementFilterField } from "@/CaseManagement/models/CaseManagementFilterField";
import type { aFilterData } from "@/components/aFilterDrawer/models/aFilterData";
import type { aFilterItem } from "@/components/aFilterDrawer/models/aFilterItem";
import { aFilterType } from "@/components/aFilterDrawer/models/enums/aFilterType";
import { EventType } from "./enums/EventType";
import { ProductType } from "./enums/ProductType";

export class CaseManagementFilterData {
  public caseManagementFilters: aFilterData[] = [
    {
      display: true,
      text: "Primary Insured Name",
      type: aFilterType.ProductionFilterItems,
      field: CaseManagementFilterField.PrimaryInsuredName,
      selection: null as aFilterItem[] | null,
      items: null as aFilterItem[] | null,
      loading: false,
      active: false,
      isMultiple: false,
    },
    {
      display: true,
      text: "Product",
      type: aFilterType.ProductionFilterItems,
      field: CaseManagementFilterField.ProductTypes,
      selection: null as aFilterItem[] | null,
      items: [
        { id: ProductType.FiveYearTerm, text: "5-Year Renewable Term" },
        { id: ProductType.AccidentalDeath, text: "Accidental Death" },
        {
          id: ProductType.AccidentalDeathDismemberment,
          text: "Accidental Death & Dismemberment",
        },
        { id: ProductType.AccidentInsuranceTiered, text: "Accident Insurance" },
        { id: ProductType.CriticalIllness, text: "Critical Illness" },
        {
          id: ProductType.CentryPlusDisabilityInsurance,
          text: "Century+ Disability Insurance",
        },
        { id: ProductType.IncomeProtection, text: "Income Protection" },
        { id: ProductType.Term, text: "Term Life" },
      ],
      loading: false,
      active: false,
      isTiered: true,
    },
    {
      display: true,
      text: "Status",
      type: aFilterType.ProductionFilterItems,
      field: CaseManagementFilterField.EventTypes,
      selection: null as aFilterItem[] | null,
      items: [
        { id: EventType.ReceivedQuote, text: "Quote Sent" },
        { id: EventType.CaseStarted, text: "Case Started" },
        { id: EventType.InterviewStarted, text: "Interview Started" },
        { id: EventType.InterviewCompleted, text: "Interview Complete" },
        { id: EventType.RecipientSent, text: "Signature Requested" },
        { id: EventType.RecipientDeclined, text: "Signature Declined" },
        { id: EventType.EnvelopeVoided, text: "Signature Expired" },
        { id: EventType.RecipientCompleted, text: "Signature Completed" },
        { id: EventType.ApplicationSubmitted, text: "Application Submitted" },
        { id: EventType.Expired, text: "Expired" },
      ],
      loading: false,
      active: false,
      isTiered: true,
    },
    {
      display: true,
      text: "Created Date",
      type: aFilterType.CreatedDates,
      field: CaseManagementFilterField.CreatedDates,
      selection: null as aFilterItem[] | null,
      items: null as aFilterItem[] | null,
      loading: false,
      active: false,
    },
    {
      display: false,
      text: "View As Agent",
      type: aFilterType.ViewAsAgent,
      field: CaseManagementFilterField.ViewAsAgent,
      selection: null as aFilterItem[] | null,
      items: null as aFilterItem[] | null,
      loading: false,
      active: false,
    },
  ];
}
