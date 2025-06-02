import { useQuery } from "@tanstack/vue-query";
import type { UseQueryReturnType } from "@tanstack/vue-query";
import { GroupInventoryService } from "@assurity/newassurelink-client";

import type {
  Assurity_AgentPortal_Contracts_GroupInventory_Response_GroupSummaryResponse as GroupSummaryResponse,
  Assurity_AgentPortal_Contracts_GroupInventory_Response_GroupDetailResponse as GroupDetailResponse,
} from "@assurity/newassurelink-client";
import type { GroupInventoryFilterParameters } from "../models/GroupInventoryFilterParameters";
import type { Ref } from "vue";
import type { GroupInventoryDetailFilterParameters } from "../models/GroupInventoryDetailFilterParameters";

const fetchGroupSummaryInformation = async (
  params: GroupInventoryFilterParameters,
): Promise<GroupSummaryResponse> => {
  const results = await GroupInventoryService.getGroupSummary({
    viewAsAgentId: params.viewAsAgentId,
    groupEffectiveStartDate: params.groupEffectiveStartDate,
    groupEffectiveEndDate: params.groupEffectiveEndDate,
    groupNumber: params.groupNumber,
    groupStatus: params.groupStatus as
      | ""
      | "Active"
      | "Suspended"
      | "Terminated"
      | undefined,
    orderBy: params.orderBy as
      | ""
      | "GroupStatus"
      | "PolicyCount"
      | "GroupEffectiveDate"
      | undefined,
    sortDirection: params.sortDirection,
    page: params.page?.toString(),
    pageSize: params.pageSize?.toString(),
  });
  return results;
};

export const useGroupSummaryData = (
  queryParams: Ref<GroupInventoryFilterParameters>,
): UseQueryReturnType<GroupSummaryResponse, Error> => {
  const queryInfo = useQuery<GroupSummaryResponse>({
    queryKey: ["GroupSummaryInformation", queryParams],
    staleTime: 300000,
    queryFn: () => fetchGroupSummaryInformation(queryParams.value),
  });

  return queryInfo;
};

const fetchGroupDetailInformation = async (
  params: GroupInventoryDetailFilterParameters,
): Promise<GroupDetailResponse> => {
  const results = await GroupInventoryService.getGroupDetail({
    groupNumber: params.groupNumber as "",
    policyNumber: params.policyNumber as "",
    policyOwnerName: params.policyOwnerName as "",
    policyStatus: params.policyStatus as "",
    issueStartDate: params.issueStartDate as "",
    issueEndDate: params.issueEndDate as "",
    productDescription: params.productDescription as "",
    orderBy: params.orderBy as
      | ""
      | "PolicyOwner"
      | "PolicyStatus"
      | "IssueDate"
      | "PaidToDate"
      | undefined,
    sortDirection: params.sortDirection,
    page: params.page?.toString(),
    pageSize: params.pageSize?.toString(),
  });
  return results;
};

export const useGroupDetailData = (
  queryParams: Ref<GroupInventoryDetailFilterParameters>,
): UseQueryReturnType<GroupDetailResponse, Error> => {
  const queryInfo = useQuery<GroupDetailResponse>({
    queryKey: ["GroupDetailInformation", queryParams],
    staleTime: 300000,
    queryFn: () => fetchGroupDetailInformation(queryParams.value),
  });

  return queryInfo;
};
