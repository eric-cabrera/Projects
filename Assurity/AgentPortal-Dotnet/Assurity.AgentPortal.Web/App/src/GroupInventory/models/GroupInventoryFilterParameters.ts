import type { Assurity_AgentPortal_Contracts_Enums_SortDirection } from "@assurity/newassurelink-client";

export interface GroupInventoryFilterParameters {
  viewAsAgentId?: string;
  groupNumber?: string;
  groupName?: string;
  groupEffectiveStartDate?: string;
  groupEffectiveEndDate?: string;
  groupStatus?: string;
  orderBy?: string;
  sortDirection?: Assurity_AgentPortal_Contracts_Enums_SortDirection;
  page?: number;
  pageSize?: number;
}
