<template>
  <DynamicContent>
    <template #header>
      <div class="claims__header">
        <div class="claims__header-left">
          <h1 class="heading--1">Claim Results</h1>
          <div>
            <RouterLink to="/claims" class="claims__back-button">
              <AssurityIconChevronLeft
                class="chevron"
              ></AssurityIconChevronLeft>
              <span>New Search</span>
            </RouterLink>
          </div>
        </div>
        <div class="claims__header-right">
          <div class="claims-overview__container">
            <span v-if="headerClaimCount" class="claims-overview__claim-count">
              <AssurityIconFlag class="claims-overview__flag" />
              {{ headerClaimCount }} claims found for
            </span>
            <div class="claims-overview__searched-by">
              {{ headerSearchedBy }}
            </div>
          </div>
        </div>
      </div>
    </template>
    <div class="col-span-12">
      <ContainerBlock dense>
        <aTableMobile
          :headers="claimsHeaders"
          :items="claimsItems"
          :show-nested-rows-details="true"
          :default-items-per-page="10"
          class="col-span-12 claims-results__table"
          :loading="isLoading"
          hide-first-label
          pagination="client"
        />
        <aTableDesktop
          :headers="claimsHeaders"
          :items="claimsItems"
          :default-items-per-page="10"
          class="col-span-12 claims-results__table"
          :loading="isLoading"
          pagination="client"
          :pad-for-nested-row="true"
        />
      </ContainerBlock>
    </div>
  </DynamicContent>
</template>

<script setup lang="ts">
import { computed, ref, watchEffect } from "vue";
import DynamicContent from "@/layouts/DynamicContent.vue";
import AssurityIconChevronLeft from "@/components/icons/AssurityIconChevronLeft.vue";
import ContainerBlock from "@/components/content/ContainerBlock.vue";
import aTableDesktop from "@/components/aTable/desktop/aTableDesktop.vue";
import aTableMobile from "@/components/aTable/mobile/aTableMobile.vue";
import type { Cell, Row } from "@/components/aTable/definitions";
import FormatHelpers from "@/Shared/utils/FormatHelpers";
import AssurityIconFlag from "@/components/icons/AssurityIconFlag.vue";
import { useClaimsData } from "@/Claims/api";
import type { Assurity_AgentPortal_Contracts_Claims_ClaimsResponse as ClaimsResponse } from "@assurity/newassurelink-client";

const props = defineProps<{
  policyNumber?: string;
  claimNumber?: string;
  claimantFirstName?: string;
  claimantLastName?: string;
}>();

const claimsHeaders: Cell[] = [
  { key: "dateReported", text: "Reported" },
  {
    key: "claimNumber",
    text: "Claim Number",
    tooltip:
      "If the Claim Number is blank, the claim has not been reviewed. Until the initial review is complete, the claimants name will not be shown.",
  },
  { key: "claimant", text: "Claimant" },
  { key: "policyNumber", text: "Policy Number" },
  { key: "policyType", text: "Policy Type" },
  {
    key: "status",
    text: "Status",
    tooltip:
      "Claims labeled as processed may show a benefit amount of $0 even if the claim was approved. The payment amount will appear the next business day.",
  },
  { key: "statusReason", text: "Status Reason" },
  {
    key: "paymentAmount",
    text: "Payment",
    align: "right",
    tooltip: "Reflects all processed payments for this claim.",
  },
  {
    details: [
      { key: "benefitDate", text: "Benefit Date" },
      { key: "paymentDate", text: "Payment Date" },
      { key: "deliveryMethod", text: "Delivery Method" },
      { key: "benefitDescription", text: "Benefit Description" },
      {
        key: "paymentAmount",
        text: "Amount",
        tooltip: "Reflects payments processed within the last 24 months.",
      },
    ],
  },
];

const claimsItems = ref<Row[]>([]);
const headerClaimCount = ref<string | undefined | null>("");
const headerSearchedBy = ref<string | undefined | null>("");

const queryParams = computed(() => ({
  policyNumber: props.policyNumber,
  claimNumber: props.claimNumber,
  claimantFirstName: props.claimantFirstName,
  claimantLastName: props.claimantLastName,
  pageSize: 999,
}));

const processClaimData = (claimsData: ClaimsResponse) => {
  if (claimsData) {
    headerClaimCount.value = claimsData.totalRecords?.toString();
    headerSearchedBy.value = determineHeaderSearchedBy(props);

    claimsItems.value = claimsData.claims?.map((item) => {
      const isUmClaim = item.claimNumber?.toUpperCase().startsWith("UM");

      return [
        {
          key: "dateReported",
          text: FormatHelpers.formatDate(
            item.dateReported ? item.dateReported : "",
          ),
        },
        {
          key: "claimNumber",
          text: isUmClaim ? "" : item.claimNumber,
        },
        {
          key: "claimant",
          text: isUmClaim
            ? ""
            : (
                item.claimant?.firstName +
                " " +
                item.claimant?.lastName
              ).toLocaleLowerCase(),
          isBold: true,
        },
        { key: "policyNumber", text: item.policyNumber },
        { key: "policyType", text: item.policyType },
        { key: "status", text: item.status?.toLocaleLowerCase() },
        { key: "statusReason", text: item.statusReason },
        {
          key: "paymentAmount",
          text: FormatHelpers.formatMoney(item.paymentAmount ?? 0),
          align: "right",
        },
        {
          details:
            item.details && item.details.length > 0
              ? item.details.map((detailItem) => [
                  {
                    key: "benefitDate",
                    text: FormatHelpers.formatDate(
                      detailItem.benefitDate ? detailItem.benefitDate : "",
                    ),
                  },
                  {
                    key: "paymentDate",
                    text: FormatHelpers.formatDate(
                      detailItem.paymentDate ? detailItem.paymentDate : "",
                    ),
                  },
                  {
                    key: "deliveryMethod",
                    text: detailItem.deliveryMethod?.toLocaleLowerCase(),
                  },
                  {
                    key: "benefitDescription",
                    text: detailItem.benefitDescription,
                  },
                  {
                    key: "paymentAmount",
                    text: FormatHelpers.formatMoney(
                      detailItem.paymentAmount ?? 0,
                    ),
                  },
                ])
              : (item.paymentAmount ?? 0 > 0)
                ? [
                    [
                      {
                        key: "nopayments",
                        text: "No payments to show",
                        colspan: 5,
                        align: "center",
                      },
                    ],
                  ]
                : [],
        },
      ];
    }) as Row[];
  }
};

const { data, isLoading } = useClaimsData(queryParams, true);

watchEffect(() => {
  if (data.value) {
    processClaimData(data.value);
  }
});

function determineHeaderSearchedBy(props: {
  policyNumber?: string;
  claimNumber?: string;
  claimantFirstName?: string;
  claimantLastName?: string;
}) {
  if (props.policyNumber) {
    return `Policy ${props.policyNumber}`;
  }
  if (props.claimNumber) {
    return `Claim ${props.claimNumber}`;
  }
  if (props.claimantFirstName || props.claimantLastName) {
    return `${props.claimantFirstName || ""} ${props.claimantLastName || ""}`.trim();
  }
  return ""; // Default case if none of the conditions are met
}
</script>
<style>
svg.tooltip-icon {
  fill: #007b99;
  width: 1.25em;
  height: 1.25em;
}
</style>
<style scoped lang="pcss">
.claims__header {
  margin: var(--spacing) 0 0;

  @media (width >= 960px) {
    display: flex;
    justify-content: space-between;
    margin: var(--spacing-xxxl) 0 0;
  }
}

.heading--1 {
  margin-bottom: 0px;
}

.claims__back-button {
  .chevron {
    width: 30px;
    margin-left: -10px;
    margin-top: -9px;
  }

  span {
    display: inline-block;
    vertical-align: middle;
    padding-bottom: var(--spacing-s);
    margin-top: -10px;
  }
}

.table-desktop .tooltip-icon {
  fill: #007b99;
}

.claims-overview {
  &__container {
    position: relative;
    align-items: center;
    font-size: 1.25rem;
    line-height: 1.25;
    margin: 0 0 var(--spacing-s);
    @media (width >= 960px) {
      text-align: right;
      margin: 0;
      padding-left: 3em;
      padding-top: var(--spacing);
    }
  }
  &__claim-count {
    width: 100%;
    color: var(--text-grey);
    @media (width >= 960px) {
      font-size: 1.8rem;
    }
  }
  &__searched-by {
    font-weight: bold;
    text-transform: capitalize;
    color: var(--text-grey);
    @media (width >= 960px) {
      font-size: 2.0625rem;
    }
  }
  &__flag {
    color: var(--primary-color);
    width: 18px;
    @media (width >= 960px) {
      width: 36px;
      position: relative;
      top: 6px;
    }
  }
}
</style>
