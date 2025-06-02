import type { Assurity_AgentPortal_Contracts_AgentContracts_Requirement as Requirement } from "@assurity/newassurelink-client";

export interface AgentAgencyRow {
  agentNumber?: string | null;
  marketCode?: string | null;
  agentLevel?: string | null;
  companyCode?: string | null;
  name?: string | null;
  emailAddress?: string | null;
  phoneNumber?: string | null;
  level?: string | null;
  contractStatus?: string | null;
  pendingRequirements?: Requirement[] | null;
  detailsOpen?: boolean;
  hasBranches?: boolean;
  selected?: boolean;
  depth?: number;
}
