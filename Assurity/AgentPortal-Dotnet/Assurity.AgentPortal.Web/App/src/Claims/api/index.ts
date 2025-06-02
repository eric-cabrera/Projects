import type { Ref } from "vue";
import { useQuery } from "@tanstack/vue-query";
import type { UseQueryReturnType } from "@tanstack/vue-query";
import type { ClaimsParameters } from "@/models/ClaimsParameters";
import type { ClaimsResponse } from "@/models/ClaimsResponse";
import type { ClaimItem } from "@/models/ClaimItem";
import { ClaimsService } from "@assurity/newassurelink-client";

import type { ApiError } from "@assurity/newassurelink-client";
import type { Assurity_AgentPortal_Contracts_Claims_Claim as ClaimsFromApi } from "@assurity/newassurelink-client";
import type { Assurity_AgentPortal_Contracts_Claims_ClaimDetail as ClaimDetailsFromApi } from "@assurity/newassurelink-client";
import dayjs from "dayjs";

const fetchClaimsData = async (
  params: ClaimsParameters,
): Promise<ClaimsResponse> => {
  try {
    const response = await ClaimsService.getApiClaims(params);

    const myClaimsData: ClaimsResponse = {
      claims: getClaims(response.claims) as ClaimItem[],
      page: response.page ?? 1,
      pageSize: response.pageSize ?? 10,
      totalRecords: response.totalRecords ?? 0,
    };

    return myClaimsData;
  } catch (error) {
    const apiError = error as ApiError;

    // Catch 404 NotFound errors and return an empty ClaimsResponse.
    if (apiError.status === 404) {
      return { claims: [], page: 1, pageSize: 10, totalRecords: 0 };
    }

    // Rethrow all other errors.
    throw error;
  }
};

export const useClaimsData = (
  queryParams: Ref<ClaimsParameters>,
  isEnabled: boolean,
): UseQueryReturnType<ClaimsResponse, Error> => {
  const queryData = useQuery<ClaimsResponse>({
    enabled: isEnabled,
    queryKey: ["getApiClaims", queryParams],
    staleTime: 300000,
    queryFn: () => fetchClaimsData(queryParams.value),
  });

  return queryData;
};

function getClaimDetails(
  claimDetailsFromApi: ClaimDetailsFromApi[] | null | undefined,
) {
  return claimDetailsFromApi?.map((item) => ({
    benefitDate: item.benefitDate,
    benefitDescription: item.benefitDescription,
    deliveryMethod: item.deliveryMethod,
    paymentAmount: item.paymentAmount,
    paymentDate: item.paymentDate,
    policyNumber: item.policyNumber,
    status: item.status,
  }));
}

function getClaims(claimsFromApi: ClaimsFromApi[] | null | undefined) {
  const claims = claimsFromApi?.map((item) => ({
    claimNumber: item.claimNumber,
    claimant: {
      firstName: item.claimant?.firstName,
      lastName: item.claimant?.lastName,
    },
    dateReported: item.dateReported,
    paymentAmount: item.paymentAmount,
    policyNumber: item.policyNumber,
    policyType: item.policyType,
    status: item.status,
    statusReason: item.statusReason,
    details: getClaimDetails(item.details),
  }));
  if (claims) {
    return claims.sort((a, b) =>
      dayjs(b.dateReported ?? "").diff(dayjs(a.dateReported ?? "")),
    );
  }
  return [];
}
