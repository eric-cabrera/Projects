import { type Ref } from "vue";
import { useQuery } from "@tanstack/vue-query";
import type { UseQueryReturnType } from "@tanstack/vue-query";
import { CaseManagementService } from "@assurity/newassurelink-client";

import type {
  Assurity_AgentPortal_Contracts_CaseManagement_CaseManagementResponse as CaseManagementResponse,
  Assurity_AgentPortal_Contracts_CaseManagement_CaseManagementFilters as CaseManagementFilters,
} from "@assurity/newassurelink-client";

import type { CaseManagementFilterOptions } from "../models/CaseManagementFilterOptions";

const fetchCaseManagementData = async (
  params: CaseManagementFilterOptions,
): Promise<CaseManagementResponse> => {
  return CaseManagementService.getApiCaseManagementCases(params);
};

export const useCaseManagementData = (
  queryParams: Ref<CaseManagementFilterOptions>,
): UseQueryReturnType<CaseManagementResponse, Error> => {
  const queryInfo = useQuery<CaseManagementResponse>({
    queryKey: ["CaseManagement", queryParams],
    staleTime: 300000,
    queryFn: () => fetchCaseManagementData(queryParams.value),
  });

  return queryInfo;
};

const fetchCaseManagementFilterOptions =
  async (): Promise<CaseManagementFilters> => {
    return CaseManagementService.getApiCaseManagementFilterOptions();
  };

export const useCaseManagementFilterOptions = (): UseQueryReturnType<
  CaseManagementFilters,
  Error
> => {
  const queryInfo = useQuery<CaseManagementFilters>({
    queryKey: ["CaseManagementFilters"],
    staleTime: 300000,
    queryFn: () => fetchCaseManagementFilterOptions(),
  });

  return queryInfo;
};

export const sendCaseManagementResendEmail = async (
  envelopeId: string | undefined,
) => {
  CaseManagementService.getApiCaseManagementResendEmail({
    envelopeId: envelopeId,
  });
};
