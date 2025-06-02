import type { Assurity_AgentPortal_Contracts_Enums_SortDirection } from "@assurity/newassurelink-client";

export interface CaseManagementFilterOptions {
  createdDateBegin?: string;
  createdDateEnd?: string;
  primaryInsuredName?: string;
  productTypes?: string;
  eventTypes?: string;
  viewAsAgentId?: string;
  sortColumn?: string;
  sortOrder?: Assurity_AgentPortal_Contracts_Enums_SortDirection;
  pageNumber?: number;
  pageSize?: number;
}
