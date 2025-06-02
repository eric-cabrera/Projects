import type { Assurity_AgentPortal_Contracts_Enums_SortDirection } from "@assurity/newassurelink-client";

export interface GroupInventoryDetailFilterParameters {
  groupNumber?: string;
  policyNumber?: string;
  productDescription?: string;
  policyOwnerName?: string;
  issueStartDate?: string;
  issueEndDate?: string;
  policyStatus?: string;
  orderBy?: string;
  sortDirection?: Assurity_AgentPortal_Contracts_Enums_SortDirection;
  page?: number;
  pageSize?: number;
}
