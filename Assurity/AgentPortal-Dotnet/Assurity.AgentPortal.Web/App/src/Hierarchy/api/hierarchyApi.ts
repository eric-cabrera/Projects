import { computed, type Ref } from "vue";
import { useQuery } from "@tanstack/vue-query";
import type { UseQueryReturnType } from "@tanstack/vue-query";
import { AgentHierarchyService } from "@assurity/newassurelink-client";

import type {
  Assurity_AgentPortal_Contracts_AgentContracts_AgentContractInformation as AgentContractInformation,
  Assurity_AgentPortal_Contracts_AgentContracts_PendingRequirementsHierarchyResponse as PendingRequirementsHierarchyResponse,
  Assurity_AgentPortal_Contracts_AgentContracts_ActiveHierarchyResponse as ActiveHierarchyResponse,
  Assurity_AgentPortal_Contracts_AgentContracts_AgentAppointmentResponse as AgentAppointmentResponse,
  Assurity_AgentPortal_Contracts_AgentContracts_DropdownOption as DropdownOption,
  Assurity_AgentPortal_Contracts_AgentContracts_ContractStatus as ContractStatus,
} from "@assurity/newassurelink-client";

import type { HierarchyFilterOptions } from "../models/HierarchyFilterOptions";
import type { AppointmentFilterOptions } from "../models/AppointmentFilterOptions";

const fetchViewAsFilterInformation = async (): Promise<DropdownOption[]> => {
  const results = await AgentHierarchyService.getViewAsFilterInformation();
  return results;
};

export const useViewAsData = (): UseQueryReturnType<
  DropdownOption[],
  Error
> => {
  const queryInfo = useQuery<DropdownOption[]>({
    queryKey: ["ViewAsFilterInformation"],
    staleTime: 300000,
    queryFn: () => fetchViewAsFilterInformation(),
  });

  return queryInfo;
};

const fetchActiveHierarchy = async (
  params: HierarchyFilterOptions,
): Promise<ActiveHierarchyResponse> => {
  if (!params.agentNumber) return {};
  const results = await AgentHierarchyService.getActiveHierarchy({
    agentLevel: params.agentLevel ?? "",
    agentNumber: params.agentNumber ?? "",
    marketCode: params.marketCode ?? "",
    companyCode: params.companyCode ?? "",
    contractStatus: params.contractStatus as ContractStatus,
  });
  return results;
};

export const useActiveHierarchyData = (
  queryParams: Ref<HierarchyFilterOptions>,
): UseQueryReturnType<ActiveHierarchyResponse, Error> => {
  const queryKey = computed(() => ["ActiveHierarchy", queryParams.value]);

  const queryInfo = useQuery<ActiveHierarchyResponse>({
    queryKey: queryKey,
    queryFn: () => fetchActiveHierarchy(queryParams.value),
    staleTime: 300000,
  });
  return queryInfo;
};

const fetchHierarchyWithPendingRequirements = async (
  params: HierarchyFilterOptions,
): Promise<PendingRequirementsHierarchyResponse> => {
  if (!params.agentNumber) return {};
  const results =
    await AgentHierarchyService.getHierarchyWithPendingRequirements({
      agentLevel: params.agentLevel ?? "",
      agentNumber: params.agentNumber ?? "",
      marketCode: params.marketCode ?? "",
      companyCode: params.companyCode ?? "",
    });
  return results;
};

export const useHierarchyWithPendingRequirementsData = (
  queryParams: Ref<HierarchyFilterOptions>,
): UseQueryReturnType<PendingRequirementsHierarchyResponse, Error> => {
  const queryKey = computed(() => [
    "HierarchyWithPendingRequirements",
    queryParams.value,
  ]);
  const queryInfo = useQuery<PendingRequirementsHierarchyResponse>({
    queryKey: queryKey,
    staleTime: 300000,
    queryFn: () => fetchHierarchyWithPendingRequirements(queryParams.value),
  });

  return queryInfo;
};

const fetchActiveAppointments = async (
  params: AppointmentFilterOptions,
): Promise<AgentAppointmentResponse> => {
  if (!params.viewAsAgentLevel) return {};
  const results = await AgentHierarchyService.getActiveAppointments({
    viewAsAgentLevel: params.viewAsAgentLevel ?? "",
    viewAsAgentNumber: params.viewAsAgentNumber ?? "",
    viewAsMarketCode: params.viewAsMarketCode ?? "",
    viewAsCompanyCode: params.viewAsCompanyCode ?? "",
    downlineAgentNumber:
      params.downlineAgentNumber ?? params.viewAsAgentNumber ?? "",
    downlineMarketCode:
      params.downlineMarketCode ?? params.viewAsMarketCode ?? "",
    downlineAgentLevel:
      params.downlineAgentLevel ?? params.viewAsAgentLevel ?? "",
    downlineCompanyCode:
      params.downlineCompanyCode ?? params.viewAsCompanyCode ?? "",
  });
  return results;
};

export const useActiveAppointmentsData = (
  queryParams: Ref<AppointmentFilterOptions>,
): UseQueryReturnType<AgentAppointmentResponse, Error> => {
  const queryInfo = useQuery<AgentAppointmentResponse>({
    queryKey: computed(() => ["ActiveAppointments", queryParams.value]),
    staleTime: 300000,
    queryFn: () => fetchActiveAppointments(queryParams.value),
    enabled: computed(() => !!queryParams.value.viewAsAgentNumber),
  });

  return queryInfo;
};

const fetchAgentInformation = async (
  downlineParams: HierarchyFilterOptions,
  viewAsParams: HierarchyFilterOptions,
): Promise<AgentContractInformation> => {
  const agentInfoParams = {
    downlineAgentLevel: downlineParams.agentLevel ?? "",
    downlineAgentNumber: downlineParams.agentNumber ?? "",
    downlineCompanyCode: downlineParams.companyCode ?? "",
    downlineMarketCode: downlineParams.marketCode ?? "",
    viewAsAgentLevel: viewAsParams.agentLevel ?? "",
    viewAsAgentNumber: viewAsParams.agentNumber ?? "",
    viewAsCompanyCode: viewAsParams.companyCode ?? "",
    viewAsMarketCode: viewAsParams.marketCode ?? "",
  };
  const results =
    await AgentHierarchyService.getAgentContractInformation(agentInfoParams);
  return results;
};

export const useAgentInformationData = (
  downlineParams: Ref<HierarchyFilterOptions>,
  viewAsParams: Ref<HierarchyFilterOptions>,
): UseQueryReturnType<AgentContractInformation, Error> => {
  const queryInfo = useQuery<AgentContractInformation>({
    queryKey: ["AgentInformation", downlineParams.value],
    staleTime: 300000,
    queryFn: () =>
      fetchAgentInformation(downlineParams.value, viewAsParams.value),
  });

  return queryInfo;
};
