<template>
  <div class="view">
    <div class="view__header">
      <div class="view__heading">
        <h1 class="text-no-wrap view__title">Commissions</h1>
      </div>
    </div>
    <div class="view__filters">
      <DataFiltersDrawer
        v-if="
          commissionsSummaryDataBasedOnSelectedTab.selectedTab ===
          'policyDetails'
        "
        v-model:filters="
          commissionDebtStore.policyDetails.commissionDetails.filters
        "
        v-model:is-filter-tray-open="filterDrawerOpen"
        :result-count="
          commissionDebtStore.policyDetails.commissionDetails.totalRecords
        "
        @date-range-filter="
          (startDate, endDate) =>
            filterDate(startDate, endDate, 'policyDetails')
        "
      />
      <DataFiltersDrawer
        v-if="
          commissionsSummaryDataBasedOnSelectedTab.selectedTab ===
          'writingAgentDetails'
        "
        v-model:filters="commissionDebtStore.writingAgentDetails.filters"
        v-model:is-filter-tray-open="filterDrawerOpen"
        :result-count="commissionDebtStore.writingAgentDetails.totalRecords"
        @date-range-filter="
          (startDate, endDate) =>
            filterDate(startDate, endDate, 'writingAgentDetails')
        "
      />
    </div>
    <div class="view__summary-container">
      <v-row class="view__summary-row">
        <v-col cols="12" lg="4">
          <div
            class="v-sheet h-100"
            :class="{ 'elevation-4 rounded-lg': !isMobile }"
          >
            <v-progress-linear
              v-if="commissionDebtStore.policyDetails.loading"
              color="primary"
              height="4"
              indeterminate
            ></v-progress-linear>
            <div class="v-sheet__content">
              <div class="v-card">
                <div class="v-card__title">
                  <div class="v-card__title-text">
                    <h2 class="v-card__title-text__title">
                      Commission Cycle
                      <span
                        v-atooltip.top="{
                          text: commissionCycleToolTipText,
                        }"
                        class="tooltip-icon__container"
                      >
                        <AssurityIconTooltip />
                      </span>
                    </h2>
                    <p class="v-card__title-text__subtitle">
                      <a
                        v-if="cycleDate"
                        class="cycleCard__cycleDate"
                        @click="filterDrawerOpen = !filterDrawerOpen"
                      >
                        {{ cycleDate }}
                      </a>
                    </p>
                  </div>
                  <div class="v-card__title-icon">
                    <AssurityIconCalendar />
                  </div>
                </div>
                <div class="v-card__content">
                  <v-row>
                    <v-col cols="6">
                      <div class="v-card__content__label">My Commissions</div>
                    </v-col>
                    <v-col cols="6">
                      <div class="money__container">
                        <span class="money__dollar"
                          >{{
                            FormatHelpers.getDollars(
                              commissionsSummaryDataBasedOnSelectedTab
                                .commissionCycle.myCommissions,
                            )
                          }}.</span
                        ><sup class="money__cents">{{
                          FormatHelpers.getCents(
                            commissionsSummaryDataBasedOnSelectedTab
                              .commissionCycle.myCommissions,
                          )
                        }}</sup>
                      </div>
                    </v-col>
                  </v-row>
                  <v-row>
                    <v-col cols="6">
                      <div class="v-card__content__label">First Year</div>
                    </v-col>
                    <v-col cols="6">
                      <div class="money__container">
                        <span class="money__dollar"
                          >{{
                            FormatHelpers.getDollars(
                              commissionsSummaryDataBasedOnSelectedTab
                                .commissionCycle.firstYear,
                            )
                          }}.</span
                        ><sup class="money__cents">{{
                          FormatHelpers.getCents(
                            commissionsSummaryDataBasedOnSelectedTab
                              .commissionCycle.firstYear,
                          )
                        }}</sup>
                      </div>
                    </v-col>
                  </v-row>
                  <v-row>
                    <v-col cols="6">
                      <div class="v-card__content__label">Renewal</div>
                    </v-col>
                    <v-col cols="6">
                      <div class="money__container">
                        <span class="money__dollar"
                          >{{
                            FormatHelpers.getDollars(
                              commissionsSummaryDataBasedOnSelectedTab
                                .commissionCycle.renewal,
                            )
                          }}.</span
                        ><sup class="money__cents">{{
                          FormatHelpers.getCents(
                            commissionsSummaryDataBasedOnSelectedTab
                              .commissionCycle.renewal,
                          )
                        }}</sup>
                      </div>
                    </v-col>
                  </v-row>
                </div>
              </div>
            </div>
          </div>
        </v-col>
        <v-col cols="12" lg="4">
          <div
            class="v-sheet h-100"
            :class="{ 'elevation-4 rounded-lg': !isMobile }"
          >
            <v-progress-linear
              v-if="commissionDebtStore.policyDetails.loading"
              color="primary"
              height="4"
              indeterminate
            ></v-progress-linear>
            <div class="v-sheet__content">
              <div class="v-card">
                <div class="v-card__title">
                  <div class="v-card__title-text">
                    <h2 class="v-card__title-text__title d-none d-md-block">
                      Year to Date Summary
                    </h2>
                    <h2 class="v-card__title-text__title d-md-none">
                      YTD Summary
                    </h2>
                    <p class="v-card__title-text__subtitle">
                      {{ currentYear }}
                    </p>
                  </div>
                  <div class="v-card__title-icon">
                    <AssurityIconProfits />
                  </div>
                </div>
                <div class="v-card__content">
                  <v-row>
                    <v-col cols="7">
                      <div class="v-card__content__label">
                        My Taxable Commissions
                      </div>
                    </v-col>
                    <v-col cols="5">
                      <div class="money__container">
                        <span class="money__dollar"
                          >{{
                            FormatHelpers.getDollars(
                              commissionsSummaryDataBasedOnSelectedTab
                                .commissionSummary.taxableCommissionsTotal,
                            )
                          }}.</span
                        ><sup class="money__cents">{{
                          FormatHelpers.getCents(
                            commissionsSummaryDataBasedOnSelectedTab
                              .commissionSummary.taxableCommissionsTotal,
                          )
                        }}</sup>
                      </div>
                    </v-col>
                  </v-row>
                  <v-row>
                    <v-col cols="6">
                      <div class="v-card__content__label">First Year</div>
                    </v-col>
                    <v-col cols="6">
                      <div class="money__container">
                        <span class="money__dollar"
                          >{{
                            FormatHelpers.getDollars(
                              commissionsSummaryDataBasedOnSelectedTab
                                .commissionSummary
                                .firstYearCommissionsCurrentYear,
                            )
                          }}.</span
                        ><sup class="money__cents">{{
                          FormatHelpers.getCents(
                            commissionsSummaryDataBasedOnSelectedTab
                              .commissionSummary
                              .firstYearCommissionsCurrentYear,
                          )
                        }}</sup>
                      </div>
                    </v-col>
                  </v-row>
                  <v-row>
                    <v-col cols="6">
                      <div class="v-card__content__label">Renewal</div>
                    </v-col>
                    <v-col cols="6">
                      <div class="money__container">
                        <span class="money__dollar"
                          >{{
                            FormatHelpers.getDollars(
                              commissionsSummaryDataBasedOnSelectedTab
                                .commissionSummary
                                .renewalCommissionsCurrentYear,
                            )
                          }}.</span
                        ><sup class="money__cents">{{
                          FormatHelpers.getCents(
                            commissionsSummaryDataBasedOnSelectedTab
                              .commissionSummary.renewalCommissionsCurrentYear,
                          )
                        }}</sup>
                      </div>
                    </v-col>
                  </v-row>
                </div>
              </div>
            </div>
          </div>
        </v-col>
        <v-col cols="12" lg="4">
          <div
            class="v-sheet h-100"
            :class="{ 'elevation-4 rounded-lg': !isMobile }"
          >
            <v-progress-linear
              v-if="commissionDebtStore.policyDetails.loading"
              color="primary"
              height="4"
              indeterminate
            ></v-progress-linear>
            <div class="v-sheet__content">
              <div class="v-card">
                <div class="v-card__title">
                  <div class="v-card__title-text">
                    <h2 class="v-card__title-text__title">Year over Year</h2>
                    <p class="v-card__title-text__subtitle">
                      Commission Comparison
                    </p>
                  </div>
                  <div class="v-card__title-icon">
                    <AssurityIconIncrease
                      class="v-card__title-icon--icon-increase"
                    />
                  </div>
                </div>
                <div class="v-card__content">
                  <v-row>
                    <v-col cols="7">
                      <div class="v-card__content__label">
                        First Year Total for {{ previousYear }}
                      </div>
                      <div class="v-card__content__label">
                        First Year YTD for {{ currentYear }}
                      </div>
                    </v-col>
                    <v-col cols="5">
                      <div class="money__container">
                        <span class="money__dollar"
                          >{{
                            FormatHelpers.getDollars(
                              commissionsSummaryDataBasedOnSelectedTab
                                .commissionSummary
                                .firstYearCommissionsPreviousYear,
                            )
                          }}.</span
                        ><sup class="money__cents">{{
                          FormatHelpers.getCents(
                            commissionsSummaryDataBasedOnSelectedTab
                              .commissionSummary
                              .firstYearCommissionsPreviousYear,
                          )
                        }}</sup>
                      </div>
                      <div class="money__container">
                        <span class="money__dollar"
                          >{{
                            FormatHelpers.getDollars(
                              commissionsSummaryDataBasedOnSelectedTab
                                .commissionSummary
                                .firstYearCommissionsCurrentYear,
                            )
                          }}.</span
                        ><sup class="money__cents">{{
                          FormatHelpers.getCents(
                            commissionsSummaryDataBasedOnSelectedTab
                              .commissionSummary
                              .firstYearCommissionsCurrentYear,
                          )
                        }}</sup>
                      </div>
                    </v-col>
                  </v-row>
                  <v-row>
                    <v-col cols="7">
                      <div class="v-card__content__label">
                        Renewal Total for {{ previousYear }}
                      </div>
                      <div class="v-card__content__label">
                        Renewal YTD for {{ currentYear }}
                      </div>
                    </v-col>
                    <v-col cols="5">
                      <div class="money__container">
                        <span class="money__dollar"
                          >{{
                            FormatHelpers.getDollars(
                              commissionsSummaryDataBasedOnSelectedTab
                                .commissionSummary
                                .renewalCommissionsPreviousYear,
                            )
                          }}.</span
                        ><sup class="money__cents">{{
                          FormatHelpers.getCents(
                            commissionsSummaryDataBasedOnSelectedTab
                              .commissionSummary.renewalCommissionsPreviousYear,
                          )
                        }}</sup>
                      </div>
                      <div class="money__container">
                        <span class="money__dollar"
                          >{{
                            FormatHelpers.getDollars(
                              commissionsSummaryDataBasedOnSelectedTab
                                .commissionSummary
                                .renewalCommissionsCurrentYear,
                            )
                          }}.</span
                        ><sup class="money__cents">{{
                          FormatHelpers.getCents(
                            commissionsSummaryDataBasedOnSelectedTab
                              .commissionSummary.renewalCommissionsCurrentYear,
                          )
                        }}</sup>
                      </div>
                    </v-col>
                  </v-row>
                </div>
              </div>
            </div>
          </div>
        </v-col>
      </v-row>
    </div>
    <div class="commission-details__container">
      <h2 class="view__title-section">Commission Details</h2>
      <aButton
        v-if="canDownload"
        button-style="primary"
        size="s"
        text="Download XLSX"
        class="commission-details__download-button d-none d-md-flex"
        @click="downloadCommissions"
      >
        <template #prepend>
          <AssurityIconDownload class="download-icon--mobile" color="white" />
        </template>
      </aButton>
      <CommissionDetails @on-tab-change="onTabChange"></CommissionDetails>
      <aButton
        v-if="canDownload"
        button-style="primary"
        size="s"
        text="Download XLSX"
        class="commission-details__download-button commission-details__download-button-mobile d-flex d-md-none"
        @click="downloadCommissions"
      >
        <template #prepend>
          <AssurityIconDownload class="download-icon--mobile" color="white" />
        </template>
      </aButton>
    </div>
    <div v-if="showCommissionDates" class="commission-dates__container">
      <h2 class="view__title-section">Commission Dates</h2>
      <v-row class="view__dates-row">
        <v-col cols="12" lg="4">
          <div class="v-sheet" :class="{ 'elevation-4 rounded-lg': !isMobile }">
            <v-progress-linear
              v-if="isLoadingCommissionDates"
              color="primary"
              height="4"
              indeterminate
            ></v-progress-linear>
            <aCollapsible v-else title="Commissions Processed">
              <ul>
                <li
                  v-for="date in commissionDatesData?.commissionsProcessed"
                  :key="date"
                >
                  {{ date }}
                </li>
              </ul>
            </aCollapsible>
          </div>
        </v-col>
        <v-col cols="12" lg="4">
          <div class="v-sheet" :class="{ 'elevation-4 rounded-lg': !isMobile }">
            <v-progress-linear
              v-if="isLoadingCommissionDates"
              color="primary"
              height="4"
              indeterminate
            ></v-progress-linear>
            <aCollapsible v-else title="Statements Available Online">
              <ul>
                <li
                  v-for="date in commissionDatesData?.statementsAvailable"
                  :key="date"
                >
                  {{ date }}
                </li>
              </ul>
            </aCollapsible>
          </div>
        </v-col>
        <v-col cols="12" lg="4">
          <div class="v-sheet" :class="{ 'elevation-4 rounded-lg': !isMobile }">
            <v-progress-linear
              v-if="isLoadingCommissionDates"
              color="primary"
              height="4"
              indeterminate
            ></v-progress-linear>
            <aCollapsible v-else title="Direct Deposit">
              <ul>
                <li
                  v-for="date in commissionDatesData?.directDeposit"
                  :key="date"
                >
                  {{ date }}
                </li>
              </ul>
            </aCollapsible>
          </div>
        </v-col>
      </v-row>
    </div>
    <div class="commission-documents__container">
      <h2 class="view__title-section">Documents</h2>
      <v-row class="commission-documents__row">
        <v-col cols="12" lg="4">
          <AgentStatement
            class="commission-details-documents__item"
            :loading="agentStatementLoading"
            :agent-data="agentStatementAgents"
            :cycle-dates="agentStatementCycleDates"
            :report-types="agentStatementReportTypes"
            :error-message="agentStatementError"
            @submit="
              ($event) =>
                submitAgentStatement(
                  $event.agent,
                  $event.cycleDate,
                  $event.reportType,
                )
            "
          />
        </v-col>
        <v-col cols="12" lg="4">
          <DirectDeposit
            title="Direct Deposit Forms"
            :links="directDepositLinks"
            :is-mobile="isMobile"
          />
        </v-col>
      </v-row>
    </div>
  </div>
</template>
<script setup lang="ts">
import { onMounted, onUnmounted, ref, watch, computed } from "vue";
import { useQuery } from "@tanstack/vue-query";
import axios from "axios";
import dayjs from "dayjs";
import aButton from "@/components/forms/aButton.vue";
import AgentStatement from "@/Commissions/components/AgentStatement.vue";
import DirectDeposit from "@/Commissions/components/DirectDeposit.vue";
import AssurityIconDownload from "@/components/icons/AssurityIconDownload.vue";
import AssurityIconCalendar from "@/components/icons/AssurityIconCalendar.vue";
import AssurityIconProfits from "@/components/icons/AssurityIconProfits.vue";
import AssurityIconIncrease from "@/components/icons/AssurityIconIncrease.vue";
import AssurityIconTooltip from "@/components/icons/AssurityIconTooltip.vue";
import DataFiltersDrawer from "@/components/DataFiltersDrawer.vue";
import aCollapsible from "@/components/aCollapsible/aCollapsible.vue";
import FormatHelpers from "@/Shared/utils/FormatHelpers";
import CommissionDetails from "@/Commissions/views/CommissionDetails.vue";
import { useCommissionDebtStore } from "@/stores/commissionDebtStore";
import type { CommissionDatesResponse } from "@/models/Responses/CommissionsDebtSecuredAdvancesResponse";
import { useUserStore } from "@/stores/userStore";
import { useFeatureStore } from "@/stores/featureStore";
import { FilterField } from "@/models/enums/FilterField";
import { CommissionDetailTypes } from "@/models/CommissionsDebt/CommissionDetailTypes";
import type { Filter } from "@/models/Filters";
import type { Agent } from "@/models/Agent";

const userStore = useUserStore();
const featureStore = useFeatureStore();
const commissionDateFeatureActive = ref(false);

const canDownload = computed(() => {
  switch (currentTab.value) {
    case CommissionDetailTypes.POLICY_DETAILS:
      return (
        !commissionDebtStore.policyDetails.loading &&
        commissionDebtStore.policyDetails.commissionDetails.totalRecords > 0
      );
    case CommissionDetailTypes.WRITING_AGENT_DETAILS:
      return (
        !commissionDebtStore.writingAgentDetails.loading &&
        commissionDebtStore.writingAgentDetails.totalRecords > 0
      );
  }

  return false;
});

async function checkCommissionDateFeature() {
  await featureStore.checkFeature("commissiondates");
  commissionDateFeatureActive.value = featureStore.feature.commissiondates;
}

checkCommissionDateFeature();
const showCommissionDates = computed(() => {
  // feature flag AND data available
  if (
    !commissionDateFeatureActive.value ||
    !commissionDatesData ||
    !commissionDatesData.value?.commissionsProcessed ||
    !commissionDatesData.value?.statementsAvailable ||
    !commissionDatesData.value?.directDeposit ||
    (commissionDatesData.value?.commissionsProcessed.length === 0 &&
      commissionDatesData.value?.statementsAvailable.length === 0 &&
      commissionDatesData.value?.directDeposit.length === 0)
  ) {
    return false;
  }
  return true;
});

const commissionCycleToolTipText = ref(
  "If a future date is selected, estimated commissions are displayed",
);
const isLoading = ref(false);
const isMobile = ref(window.innerWidth <= 960);
const updateIsMobile = () => {
  isMobile.value = window.innerWidth <= 960;
};

const filterDrawerOpen = ref(false);

const currentYear = dayjs().year();
const previousYear = dayjs().subtract(1, "year").year();
const currentTab = ref(CommissionDetailTypes.POLICY_DETAILS);
const directDepositLinks = [
  {
    text: "Direct Deposit Form",
    link: "/forms/175a61e1-9cbc-45a2-97cc-8284444c6d43",
  },
  {
    text: "New York Direct Deposit Form",
    link: "/forms/c53cd305-755f-4262-adf4-9d3964b94df1",
  },
];

const commissionsSummaryDataBasedOnSelectedTab = computed(() => {
  const commissionSummary = {
    commissionCycle: {
      myCommissions: 0,
      firstYear: 0,
      renewal: 0,
      cycleEndDate: "",
      cycleStartDate: "",
    },
    commissionSummary: {
      taxableCommissionsTotal: 0,
      firstYearCommissionsCurrentYear: 0,
      renewalCommissionsCurrentYear: 0,
      firstYearCommissionsPreviousYear: 0,
      renewalCommissionsPreviousYear: 0,
    },
    selectedTab: CommissionDetailTypes.POLICY_DETAILS,
  };
  if (currentTab.value === CommissionDetailTypes.POLICY_DETAILS) {
    commissionSummary.commissionCycle =
      commissionDebtStore.policyDetails.commissionCycle;
    commissionSummary.commissionSummary =
      commissionDebtStore.policyDetails.commissionSummary;
    commissionSummary.selectedTab = CommissionDetailTypes.POLICY_DETAILS;
  } else {
    commissionSummary.commissionCycle =
      commissionDebtStore.writingAgentDetails.commissionCycle;
    commissionSummary.commissionSummary =
      commissionDebtStore.policyDetails.commissionSummary;
    commissionSummary.selectedTab = CommissionDetailTypes.WRITING_AGENT_DETAILS;
  }

  return commissionSummary;
});

const agentStatementLoading = ref(true);
const agentStatementError = ref("");

const commissionDebtStore = useCommissionDebtStore();

onMounted(async () => {
  window.addEventListener("resize", updateIsMobile);

  await getPolicyDetails(filterQueryStringPolicyDetails.value, false);

  await commissionDebtStore.getAgentStatementOptions();
  agentStatementLoading.value = false;
});

onUnmounted(() => {
  window.removeEventListener("resize", updateIsMobile);
});

const filterQueryStringPolicyDetails = computed(() => {
  return generateQueryString(
    commissionDebtStore.policyDetails.commissionDetails.filters,
    userStore.user.viewAsAgentId,
    commissionDebtStore.policyDetails.originalFilters.writingAgentFilterValues,
  );
});

const filterQueryStringWritingAgentDetails = computed(() => {
  return generateQueryString(
    commissionDebtStore.writingAgentDetails.filters,
    userStore.user.viewAsAgentId,
    commissionDebtStore.writingAgentDetails.originalFilters
      .writingAgentFilterValues,
  );
});

const cycleDate = computed(() => {
  if (isLoading.value) {
    return "";
  }

  switch (currentTab.value) {
    case CommissionDetailTypes.POLICY_DETAILS:
      return getCurrentCycleDate(
        commissionDebtStore.policyDetails.commissionDetails.filters,
      );
    case CommissionDetailTypes.WRITING_AGENT_DETAILS:
      return getCurrentCycleDate(
        commissionDebtStore.writingAgentDetails.filters,
      );
    default:
      return "";
  }
});

let isUpdatingPage = false;

watch(
  () => [
    commissionDebtStore.policyDetails.page,
    commissionDebtStore.policyDetails.pageSize,
    commissionDebtStore.policyDetails.orderBy,
    commissionDebtStore.policyDetails.sortDirection,
  ],
  async () => {
    if (isUpdatingPage) {
      return;
    }
    await getPolicyDetails(filterQueryStringPolicyDetails.value);
  },
);

watch(
  () => [
    commissionDebtStore.writingAgentDetails.page,
    commissionDebtStore.writingAgentDetails.pageSize,
    commissionDebtStore.writingAgentDetails.orderBy,
    commissionDebtStore.writingAgentDetails.sortDirection,
  ],
  async () => {
    if (isUpdatingPage) {
      return;
    }
    await getWritingAgentDetails(filterQueryStringWritingAgentDetails.value);
  },
);

watch(
  () => filterQueryStringPolicyDetails.value,
  async (newQueryString) => {
    isLoading.value = true;
    isUpdatingPage = true;
    commissionDebtStore.policyDetails.page = 1;
    await getPolicyDetails(newQueryString);
    isLoading.value = false;
    isUpdatingPage = false;
  },
);

watch(
  () => filterQueryStringWritingAgentDetails.value,
  async (newQueryString) => {
    isLoading.value = true;
    isUpdatingPage = true;
    commissionDebtStore.writingAgentDetails.page = 1;
    await getWritingAgentDetails(newQueryString);
    isLoading.value = false;
    isUpdatingPage = false;
  },
);

// Update data based on tab change.
async function onTabChange(tabId: string) {
  currentTab.value = tabId as CommissionDetailTypes;

  switch (tabId) {
    case CommissionDetailTypes.POLICY_DETAILS:
      isUpdatingPage = true;

      commissionDebtStore.policyDetails.page = 1;
      await getPolicyDetails(filterQueryStringPolicyDetails.value);

      isUpdatingPage = false;

      break;
    case CommissionDetailTypes.WRITING_AGENT_DETAILS:
      isUpdatingPage = true;

      commissionDebtStore.policyDetails.page = 1;
      await getWritingAgentDetails(filterQueryStringWritingAgentDetails.value);

      isUpdatingPage = false;

      break;
  }
}

function getCurrentCycleDate(filters: Filter[]) {
  const cycleDateFilter = filters.filter(
    (filter) => filter.field === FilterField.CycleDate,
  );
  if (cycleDateFilter[0]?.items && cycleDateFilter[0].items.length > 1) {
    return `${FormatHelpers.formatDate(
      cycleDateFilter[0].items.pop(),
    )} - ${FormatHelpers.formatDate(cycleDateFilter[0].items.at(0))}`;
  } else if (
    cycleDateFilter[0]?.items &&
    cycleDateFilter[0].items.length === 1
  ) {
    return FormatHelpers.formatDate(cycleDateFilter[0].items.at(0));
  }
}

async function downloadCommissions() {
  switch (currentTab.value) {
    case CommissionDetailTypes.POLICY_DETAILS:
      await getPolicyDetails(filterQueryStringPolicyDetails.value, true);
      break;
    case CommissionDetailTypes.WRITING_AGENT_DETAILS:
      await getWritingAgentDetails(
        filterQueryStringWritingAgentDetails.value,
        true,
      );
      break;
  }
}

async function getPolicyDetails(
  filterQuery?: string,
  exportReport: boolean = false,
) {
  if (exportReport) {
    await commissionDebtStore.exportPolicyDetailsReport(
      commissionDebtStore.policyDetails.page,
      commissionDebtStore.policyDetails.pageSize,
      commissionDebtStore.policyDetails.orderBy,
      commissionDebtStore.policyDetails.sortDirection,
      filterQuery,
    );
  } else {
    await commissionDebtStore.getPolicyDetails(
      commissionDebtStore.policyDetails.page,
      commissionDebtStore.policyDetails.pageSize,
      commissionDebtStore.policyDetails.orderBy,
      commissionDebtStore.policyDetails.sortDirection,
      true,
      filterQuery,
    );
  }
}

async function getWritingAgentDetails(
  filterQuery?: string,
  exportReport = false,
) {
  if (exportReport) {
    await commissionDebtStore.exportWritingAgentDetailsReport(
      commissionDebtStore.writingAgentDetails.page,
      commissionDebtStore.writingAgentDetails.pageSize,
      commissionDebtStore.writingAgentDetails.orderBy,
      commissionDebtStore.writingAgentDetails.sortDirection,
      filterQuery,
    );
  } else {
    await commissionDebtStore.getWritingAgentDetails(
      commissionDebtStore.writingAgentDetails.page,
      commissionDebtStore.writingAgentDetails.pageSize,
      commissionDebtStore.writingAgentDetails.orderBy,
      commissionDebtStore.writingAgentDetails.sortDirection,
      true,
      filterQuery,
    );
  }
}

function filterDate(
  startDate: string | null,
  endDate: string | null,
  tabId: string,
) {
  if (startDate && endDate) {
    if (tabId === CommissionDetailTypes.POLICY_DETAILS) {
      commissionDebtStore.policyDetails.commissionDetails.filters.forEach(
        (filter) => {
          if (filter.field === FilterField.CycleDate) {
            filter.startDate = startDate;
            filter.endDate = endDate;
          }
        },
      );
    } else if (tabId === CommissionDetailTypes.WRITING_AGENT_DETAILS) {
      commissionDebtStore.writingAgentDetails.filters.forEach((filter) => {
        if (filter.field === FilterField.CycleDate) {
          filter.startDate = startDate;
          filter.endDate = endDate;
        }
      });
    }
  }
}

function generateQueryString(
  filters: Filter[],
  viewAsAgentId: string,
  originalFilters: Agent[] | undefined,
) {
  const queryStringArray: string[] = [];
  const writingAgentIds: string[] = [];

  // Iterate over each filter
  filters.forEach((filter) => {
    if (filter.selection) {
      let filterValue;

      // Handle array selection
      if (Array.isArray(filter.selection)) {
        filterValue = filter.selection.map((item) => encodeURIComponent(item));
        if (filter.field === FilterField.WritingAgentName) {
          const filterValueIds = originalFilters
            ?.filter(
              (item) =>
                Array.isArray(filter.selection) &&
                filter.selection.includes(item.agentName),
            )
            .map((item) => item.agentId);

          if (filterValueIds && filterValueIds?.length > 0) {
            writingAgentIds.push(...filterValueIds);
          }
        } else if (filter.field === FilterField.WritingAgentId) {
          writingAgentIds.push(...filterValue);
        } else {
          queryStringArray.push(
            `${filter.field}s=${filterValue.join(`&${filter.field}s=`)}`,
          );
        }
      } else if (typeof filter.selection === "string") {
        // Handle string selection
        filterValue = encodeURIComponent(filter.selection);
        queryStringArray.push(`${filter.field}=${filterValue}`);
      } else if (
        filter.field === FilterField.ViewAsAgentId &&
        viewAsAgentId !== ""
      ) {
        // Handle view as agent selection
        queryStringArray.push(`AgentId=${viewAsAgentId}`);
      }
    } else {
      if (filter.startDate && filter.endDate) {
        // Handle date range selection
        queryStringArray.push(`CycleBeginDate=${filter.startDate}`);
        queryStringArray.push(`CycleEndDate=${filter.endDate}`);
      }
    }
  });

  if (writingAgentIds && writingAgentIds.length > 0) {
    queryStringArray.push(`WritingAgentIds=${writingAgentIds.join(",")}`);
  }
  // Join all query strings with "&" separator
  const queryString =
    queryStringArray.length > 0 ? `${queryStringArray.join("&")}` : "";

  return queryString;
}

// Agent Statement
const agentStatementReportTypes = [
  { value: "Summary", label: "Summary" },
  { value: "FirstYearDetail", label: "First Year Detail" },
  { value: "RenewalDetail", label: "Renewal Detail" },
];

const agentStatementAgents = computed(() => {
  const agentIdFilter =
    commissionDebtStore.policyDetails.agentStatementOptions.agents;

  if (!agentIdFilter || agentIdFilter.length === 0) {
    return [];
  }

  const agentIds = agentIdFilter?.map((agent) => {
    return {
      value: agent.id,
      label: `${agent.id} - ${agent.name}`,
    };
  });

  return agentIds ?? [];
});

const agentStatementCycleDates = computed(() => {
  const cycleDates =
    commissionDebtStore.policyDetails.agentStatementOptions.cycleDates;

  if (!cycleDates || cycleDates.length === 0) {
    return [];
  }

  const formattedDates = cycleDates.map((date) => {
    return dayjs(date.toString(), "YYYYMMDD").format("MM-DD-YYYY");
  });

  return formattedDates;
});

async function submitAgentStatement(
  agentId: string,
  cycleDate: string,
  reportType: string,
) {
  agentStatementLoading.value = true;
  agentStatementError.value = "";

  const response = await commissionDebtStore.getAgentStatement(
    agentId,
    cycleDate,
    reportType,
  );

  if (response.hasError && response.errorMessage) {
    agentStatementError.value = response.errorMessage;
  }
  agentStatementLoading.value = false;
}

const fetchCommissionDates = async () => {
  const response = await axios.get("/Content/CommissionDates");
  const data = response.data;
  return {
    commissionsProcessed: data.commissionsProcessed || [],
    statementsAvailable: data.statementsAvailable || [],
    directDeposit: data.directDeposit || [],
  };
};

const { data: commissionDatesData, isLoading: isLoadingCommissionDates } =
  useQuery<CommissionDatesResponse>({
    queryKey: ["commissionDates"],
    staleTime: 300000, // 5 minutes
    queryFn: fetchCommissionDates,
  });
</script>
<style scoped lang="pcss">
@import "@/Shared/styles/status-summary.pcss";

.view {
  max-width: 560px;
  margin-left: auto;
  margin-right: auto;

  @media (width >= 960px) {
    max-width: none;
  }
}

.view__dates-row {
  margin-top: 5px;
}

.v-card {
  padding: 15px 20px;
  overflow: visible;
  @media (width >= 600px) {
    padding: 29px 48px;
  }
}

.v-card__title {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 24px;
}

.v-card__content__label {
  font-size: 14px;
  @media (width >= 600px) {
    font-size: 16px;
  }
}

.v-card__title-text__title {
  color: var(--primary-color);
  line-height: 1.2;
  margin-bottom: 0;
  font-size: 1.313rem;
  text-wrap: balance;
  @media (width >= 600px) {
    font-size: 1.75rem;
  }
}
.view__title-section {
  margin-top: 24px;
  font-size: 1.5rem;
  color: var(--primary-color);
  @media (width >= 600px) {
    font-size: 2.063rem;
  }
}

.v-card__title-text__subtitle {
  font-size: 1.125rem;
  margin-bottom: 0;
  @media (width >= 600px) {
    color: var(--primary-color);
    font-size: 1.313rem;
  }
}

.v-card__title-icon {
  font-size: 40px;
}
.v-card__content {
  background: radial-gradient(
    circle at 1px 1px,
    var(--border-grey) 1px,
    rgba(0, 0, 0, 0) 0
  );
  background-size: 6px 1px;
  background-repeat: repeat-x;
  background-position: top;
  padding-top: var(--spacing);
  @media (width >= 600px) {
    padding-top: 0;
    background: none;
  }
}
.v-card__content .v-row {
  @media (width >= 600px) {
    background: radial-gradient(
      circle at 1px 1px,
      var(--border-grey) 1px,
      rgba(0, 0, 0, 0) 0
    );
    background-size: 6px 1px;
    background-repeat: repeat-x;
    background-position: top;
  }
  margin: 0;
}

.v-card__content__value {
  text-align: right;
}

.v-card__content__label {
  text-align: left;
}
.view__summary-container .v-sheet__content .v-card__content__label,
.view__summary-container .money__container .money__dollar {
  @media (width <= 600px) {
    font-size: 1rem;
  }
}

.v-card__content .v-row .v-col {
  padding-left: 0;
  padding-right: 0;
  padding-top: var(--spacing-xs);
  padding-bottom: var(--spacing-xs);
  @media (width >= 600px) {
    padding-top: var(--spacing-l);
    padding-bottom: var(--spacing-l);
  }
}

.v-card__content .v-row .v-col .v-card__content__label {
  padding-right: 0px;
}
.commission-details__container {
  margin-top: var(--spacing-xxl);
  margin-bottom: var(--spacing-xxl);
  position: relative;
}
.commission-details__download-button {
  /* position far right */
  position: absolute;
  top: 0;
  right: 0;
  margin-top: 24px;
  margin-right: 24px;
  &-mobile {
    position: relative;
    width: 100%;
  }
}

.v-sheet.elevation-4 {
  box-shadow: var(--desktop-shadow) !important;
}

@media (width < 600px) {
  .view__summary-row .v-col-md-4,
  .view__dates-row .v-col-md-4 {
    display: flex;
  }
  .view__summary-row .v-col-md-4 .v-sheet,
  .view__dates-row .v-col-md-4 .v-sheet {
    width: 100%;
  }
  .commission-documents__container {
    margin-bottom: var(--spacing-xxl);
  }
  .view__summary-row .v-col-12,
  .commission-documents__container .v-col-12,
  .view__dates-row .v-col-12 {
    padding-left: 0;
    padding-right: 0;
  }
  .v-card__title-icon--icon-increase {
    height: 50px;
  }
  .commission-documents__row {
    display: flex;
  }

  .v-card__content__value--bold {
    font-weight: 600;
  }
}

.cycleCard__cycleDate {
  cursor: pointer;
  text-decoration: underline;
  font-size: 1.2em;
}

.tooltip-icon {
  vertical-align: text-top;
  margin-top: 3px;
  fill: #007b99;
  width: 18px;
  height: 18px;
}

.tooltip-icon__container {
  margin-left: -5px;
}

.commission-dates__container {
  margin-bottom: var(--spacing-xxl);
}

.commission-dates__container ul,
.commission-dates__container ul li {
  list-style-type: none;
  padding-left: 0;
}

.commission-dates__container ul li {
  padding: var(--spacing-s) 0;
}
</style>
<style lang="pcss">
.v-data-table-footer {
  justify-content: center;
}
</style>
