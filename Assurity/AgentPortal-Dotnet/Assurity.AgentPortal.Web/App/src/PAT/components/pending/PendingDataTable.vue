<template>
  <div class="pending-table">
    <div v-if="pageLoading">
      <v-progress-circular
        color="primary"
        indeterminate
        :size="70"
        :width="7"
      ></v-progress-circular>
    </div>
    <div v-else>
      <DataFilters
        v-model:filters="policyStore.pendingData.requirementSummary.filters"
        :result-count="
          policyStore.pendingData.requirementSummary.totalRequirements
        "
        @date-range-filter="
          (startDate, endDate) => filterDate(startDate, endDate)
        "
      >
        <template #desktop>
          <aButton
            v-if="canDownload"
            button-style="primary"
            text="DOWNLOAD XLSX"
            size="s"
            class="active-details__download-button--desktop"
            @click="callDownloadExcelDocument()"
            @keyup.enter="callDownloadExcelDocument()"
            @keyup.space="callDownloadExcelDocument()"
            @keypress="$event.preventDefault()"
          >
            <template #prepend>
              <v-progress-circular v-if="downloadingPolicies" indeterminate />
              <AssurityIconDownload v-else color="white" />
            </template>
          </aButton>
        </template>
      </DataFilters>
      <DataTable
        v-model:header-rows="headers"
        :data-rows="requirements"
        :more-pages="morePages"
        :less-pages="lessPages"
        :page-indicator="pageIndicator"
        :page-size-indicator="pageSizeIndicator"
        :loading="isLoading"
        :error="policyStore.pendingData.error ? true : false"
        @update-sort="updateSort($event)"
        @column-selected="selectedRequirement($event.data)"
        @load-first-page="loadFirstPage()"
        @load-last-page="loadLastPage()"
        @load-next-page="loadNextPage()"
        @load-previous-page="loadPreviousPage()"
        @update-page-size="updatePageSize"
      ></DataTable>
    </div>
    <AssurityModal ref="dialog">
      <span>
        <p>
          <b
            >Unfortunately, the application for this case is not available at
            this time.
          </b>
          For additional information, please contact Customer Connections at
        </p>
        <p><b>800-276-7619, Ext. 4264</b>.</p>
      </span>
    </AssurityModal>
  </div>
</template>
<script setup lang="ts">
import { computed, onMounted, onUnmounted, ref, watch, type Ref } from "vue";
import { useUserStore } from "@/stores/userStore";
import { useUtilityStore } from "@/stores/utilityStore";
import { usePolicyStore } from "@/stores/policystore";
import DataTable from "@/components/DataTable.vue";
import type { TableColumn } from "@/models/components/TableColumn";
import FormatHelpers from "@/Shared/utils/FormatHelpers";
import DataFilters from "@/components/DataFilters.vue";
import { useRouter } from "vue-router";
import aButton from "@/components/forms/aButton.vue";
import AssurityIconDownload from "@/components/icons/AssurityIconDownload.vue";
import AssurityModal from "@/components/AssurityModal.vue";

import { SortOptions } from "@/models/enums/SortOptions";
import type { RequirementSummary } from "@/models/RequirementSummary";
import { RequirementStatus } from "@/models/enums/RequirementStatus";
import type { Requirement } from "@/models/Requirement";
import { FilterField } from "@/models/enums/FilterField";
import type { AssignedAgent } from "@/models/AssignedAgent";

const userStore = useUserStore();
const policyStore = usePolicyStore();
const router = useRouter();
const utilityStore = useUtilityStore();

const pageLoading = ref(true);
const isLoading = ref(false);
const downloadingPolicies = ref(false);
const httpErrorIgnoredEndpoint = "/API/PolicyInfo/Application/*/Export";

defineEmits([
  "firstPageClick",
  "previousPageClick",
  "nextPageClick",
  "lastPageClick",
]);

const dialog: Ref<{ dialog: HTMLDialogElement } | null> = ref(null);

onUnmounted(() => {
  utilityStore.removeEndpointToIgnore(httpErrorIgnoredEndpoint);
});

const canDownload = computed(() => {
  return (
    !isLoading.value &&
    policyStore.pendingData.requirementSummary.totalRequirements > 0
  );
});

const hideEmployer = computed(() => {
  const employerFilter =
    policyStore.pendingData.requirementSummary.filters.filter((filter) => {
      if (filter.field === FilterField.Employer) {
        return filter;
      }
    });

  if (employerFilter && employerFilter.length > 0) {
    return !employerFilter[0].display;
  }

  return false;
});

const hideAdditionalAgents = computed(() => {
  const agentFilter = policyStore.pendingData.requirementSummary.filters.filter(
    (filter) => {
      if (filter.field === FilterField.AgentId) {
        return filter;
      }
    },
  );

  if (agentFilter && agentFilter.length > 0) {
    return !agentFilter[0].display;
  }

  return false;
});

const dateSort = computed(() => {
  if (policyStore.pendingData.requirementSummary.headers.length > 0) {
    return (
      policyStore.pendingData.requirementSummary.headers.filter(
        (header) => header.heading === "Date",
      )[0].sortValue || SortOptions.none
    );
  }

  return SortOptions.none;
});

const metSort = computed(() => {
  if (policyStore.pendingData.requirementSummary.headers.length > 0) {
    return (
      policyStore.pendingData.requirementSummary.headers.filter(
        (header) => header.heading === "Met",
      )[0].sortValue || SortOptions.none
    );
  }

  return SortOptions.none;
});

const actionSort = computed(() => {
  if (policyStore.pendingData.requirementSummary.headers.length > 0) {
    return (
      policyStore.pendingData.requirementSummary.headers.filter(
        (header) => header.heading === "Action Needed",
      )[0].sortValue || SortOptions.none
    );
  }

  return SortOptions.none;
});

const sortQueryString = computed(() => {
  let sortString = "";

  if (policyStore.pendingData.requirementSummary.headers.length) {
    // If data hasn't loaded yet, set default sort order to prevent re-sorting when data loads
    const metSortString = getSortString("Requirements.Status", metSort.value);
    const dateSortString = getSortString(
      "Requirements.AddedDate",
      dateSort.value,
    );
    const actionSortString = getSortString(
      "Requirements.ActionType",
      actionSort.value,
    );

    if (metSortString) {
      if (sortString.length > 0) {
        sortString = sortString + `,${metSortString}`;
      } else {
        sortString = sortString + `${metSortString}`;
      }
    }

    if (actionSortString) {
      if (sortString.length > 0) {
        sortString = sortString + `,${actionSortString}`;
      } else {
        sortString = sortString + `${actionSortString}`;
      }
    }

    if (dateSortString) {
      if (sortString.length > 0) {
        sortString = sortString + `,${dateSortString}`;
      } else {
        sortString = sortString + `${dateSortString}`;
      }
    }
  } else {
    sortString = "Requirements.AddedDate_desc";
  }

  return sortString;
});

const morePages = computed(() => {
  if (
    policyStore.pendingData.requirementSummary.currentPage <
    policyStore.pendingData.requirementSummary.totalPages
  ) {
    return true;
  }
  return false;
});

const lessPages = computed(() => {
  if (policyStore.pendingData.requirementSummary.currentPage === 1) {
    return false;
  }
  return true;
});

const pageIndicator = computed(() => {
  const currentPage = policyStore.pendingData.requirementSummary.currentPage;
  const totalRequirements =
    policyStore.pendingData.requirementSummary.totalRequirements;
  const currentItem =
    currentPage * pageSizeIndicator.value - (pageSizeIndicator.value - 1);

  const pageMessage = `${currentItem} - ${
    currentItem + requirements.value.length - 1
  }  of ${totalRequirements}`;

  return pageMessage;
});

const pageSizeIndicator = computed(() => {
  return policyStore.pendingData.requirementSummary.pageSize
    ? policyStore.pendingData.requirementSummary.pageSize
    : 10;
});

const requirements = computed(() => {
  return policyStore.pendingData.requirementSummary.summaries.map((summary) =>
    mapRequirementSummaries(summary),
  );
});

const headers = computed(() => {
  return policyStore.pendingData.requirementSummary.headers.filter((header) => {
    switch (header.filterField) {
      case FilterField.AgentId:
      case FilterField.AgentLastName:
        if (!hideAdditionalAgents.value) {
          return header;
        }
        break;
      case FilterField.Employer:
        if (!hideEmployer.value) {
          return header;
        }
        break;
      default:
        return header;
    }
  });
});

function updateSort(event: TableColumn) {
  policyStore.pendingData.requirementSummary.headers.forEach((header) => {
    if (header.id === event.id) {
      Object.assign(header, event);
    } else {
      header.sortValue = SortOptions.none;
    }
  });
}

const filterQueryString = computed(() => {
  let queryString = "";
  policyStore.pendingData.requirementSummary.filters.map((filter) => {
    if (filter.selection) {
      let filterValue;

      if (typeof filter.selection === "string") {
        filterValue = encodeURIComponent(filter.selection);
      } else {
        filterValue = filter.selection;
      }

      queryString = queryString + `&${filter.field}=${filterValue}`;
    } else if (filter.startDate && filter.endDate) {
      queryString =
        queryString +
        `&StartDate=${filter.startDate}&EndDate=${filter.endDate}`;
    } else if (
      filter.field === "ViewAsAgentId" &&
      userStore.user.viewAsAgentId !== ""
    ) {
      queryString =
        queryString + `&${filter.field}=${userStore.user.viewAsAgentId}`;
    }
  });
  return queryString;
});

watch(
  () => sortQueryString.value,
  async (newValue) => {
    isLoading.value = true;
    await policyStore.getPendingPolicyRequirementsSummary(
      policyStore.pendingData.requirementSummary.currentPage,
      pageSizeIndicator.value,
      newValue,
      filterQueryString.value,
    );
    isLoading.value = false;
  },
);

watch(
  () => filterQueryString.value,
  async (newValue) => {
    isLoading.value = true;
    policyStore.pendingData.requirementSummary.currentPage = 1;
    await policyStore.getPendingPolicyRequirementsSummary(
      policyStore.pendingData.requirementSummary.currentPage,
      pageSizeIndicator.value,
      sortQueryString.value,
      newValue,
      true,
    );

    isLoading.value = false;
  },
);

function getServicingAgent(agents: AssignedAgent[]): AssignedAgent | undefined {
  return agents.filter((agent) => agent.isServicingAgent)[0];
}

function mapRequirementSummaries(summary: RequirementSummary) {
  const servicingAgent = summary?.assignedAgents
    ? getServicingAgent(summary.assignedAgents)
    : undefined;

  let requirements = [
    {
      id: "date",
      heading: "Date",
      value: FormatHelpers.formatDate(
        summary.requirement.addedDate || new Date(),
      ),
    },
    {
      id: "agentName",
      heading: "Agent",
      value: servicingAgent?.assignedAgentName
        ? servicingAgent?.assignedAgentName.toLowerCase()
        : "",
      subValue: servicingAgent?.assignedAgentId,
      sortable: false,
      filterField: FilterField.AgentLastName,
    },
    {
      id: "primaryInsured",
      heading: "Primary Insured",
      value: summary.primaryInsuredName
        ? summary.primaryInsuredName.toLowerCase()
        : "See Details",
      selectable: true,
      data: summary.policyNumber,
    },
    {
      id: "policyNumber",
      heading: "Policy Number",
      value: summary.policyNumber,
      selectable: true,
      data: summary.policyNumber,
      icon: "pdf",
    },
    {
      id: "employer",
      heading: "Employer",
      value: summary.employerName ? summary.employerName.toLowerCase() : "",
      filterField: FilterField.Employer,
    },
    {
      id: "productType",
      heading: "Product Type",
      value: summary.productCategory,
    },
    {
      id: "requirement",
      heading: "Requirement",
      value: summary.requirement.name.toLowerCase(),
      format: "capitalize",
    },
    {
      id: "met",
      heading: "Met",
      value: getRequirementMetText(summary.requirement.status),
    },
    {
      id: "actionNeeded",
      heading: "Action Needed",
      value: actionNeededText(summary.requirement), // need action type from api
      isButton: true,
      display: showActionButton(summary.requirement),
      data: summary.policyNumber,
    },
  ] as TableColumn[];

  if (hideAdditionalAgents.value) {
    requirements = requirements.filter(
      (req) =>
        (req.filterField as FilterField) !== FilterField.AgentId &&
        (req.filterField as FilterField) !== FilterField.AgentLastName,
    );
  }

  if (hideEmployer.value) {
    requirements = requirements.filter(
      (req) => req.filterField !== FilterField.Employer,
    );
  }

  return requirements;
}

async function loadFirstPage() {
  // TODO: Move loading spinner to table component <tbody> (leave headers visible during loading)
  // cancel previous call if user makes multiple sort / filter requests in succession
  // add error message display to table components if API call fails
  isLoading.value = true;
  await policyStore.getPendingPolicyRequirementsSummary(
    1,
    pageSizeIndicator.value,
    sortQueryString.value,
    filterQueryString.value,
  );
  isLoading.value = false;
}

async function loadPreviousPage() {
  isLoading.value = true;
  policyStore.pendingData.requirementSummary.currentPage =
    policyStore.pendingData.requirementSummary.currentPage - 1;
  await policyStore.getPendingPolicyRequirementsSummary(
    policyStore.pendingData.requirementSummary.currentPage,
    pageSizeIndicator.value,
    sortQueryString.value,
    filterQueryString.value,
  );
  isLoading.value = false;
}

async function loadNextPage() {
  isLoading.value = true;

  policyStore.pendingData.requirementSummary.currentPage =
    policyStore.pendingData.requirementSummary.currentPage + 1;
  await policyStore.getPendingPolicyRequirementsSummary(
    policyStore.pendingData.requirementSummary.currentPage,
    pageSizeIndicator.value,
    sortQueryString.value,
    filterQueryString.value,
  );
  isLoading.value = false;
}

async function loadLastPage() {
  isLoading.value = true;

  await policyStore.getPendingPolicyRequirementsSummary(
    policyStore.pendingData.requirementSummary.totalPages,
    pageSizeIndicator.value,
    sortQueryString.value,
    filterQueryString.value,
  );
  isLoading.value = false;
}

function updatePageSize(pageSize: number) {
  if (policyStore.pendingData.requirementSummary.pageSize === pageSize) return;

  policyStore.pendingData.requirementSummary.pageSize = pageSize;

  loadFirstPage();
}

async function selectedRequirement(column: TableColumn) {
  const policyNumber = column.data;

  if (policyNumber) {
    if (column.id.toLowerCase() === "policynumber") {
      const success = await policyStore.downloadApplication(policyNumber);
      if (success === false) {
        if (dialog.value) {
          dialog.value.dialog.showModal();
        }
      }
    } else {
      policyStore.selectedPolicy.policyNumber = policyNumber;
      if (column.isButton) {
        router.push({
          name: "policy-actions",
          params: { id: policyNumber },
        });
      } else {
        router.push({
          name: "policy-details",
          params: { id: policyNumber },
        });
      }
    }
  }
}

function filterDate(startDate: string | null, endDate: string | null) {
  if (startDate && endDate) {
    policyStore.pendingData.requirementSummary.filters.forEach((filter) => {
      if (filter.field === FilterField.Date) {
        filter.startDate = startDate;
        filter.endDate = endDate;
      }
    });
  }
}

function getSortString(field: string, sort: SortOptions) {
  switch (sort) {
    case SortOptions.asc:
      return field;
    case SortOptions.desc:
      return `${field}_desc`;
    default:
      return null;
  }
}

function getRequirementMetText(status: string) {
  switch (status) {
    case RequirementStatus.Met:
      return "Yes";
    case RequirementStatus.Unmet:
      return "No";
    case RequirementStatus.Waived:
      return "Waived";
    default:
      return "None";
  }
}

function actionNeededText(requirement: Requirement) {
  if (requirement.actionType && requirement.fulfillingParty === "Agent") {
    return "Take Action";
  }

  return "";
}

function showActionButton(requirement: Requirement) {
  if (
    requirement.actionType &&
    requirement.fulfillingParty === "Agent" &&
    requirement.status === RequirementStatus.Unmet
  ) {
    return true;
  }

  return false;
}

async function callDownloadExcelDocument() {
  downloadingPolicies.value = true;
  await policyStore.downloadExcelDocument("pending", filterQueryString.value);
  downloadingPolicies.value = false;
}

onMounted(async () => {
  utilityStore.addEndpointToIgnore(httpErrorIgnoredEndpoint);

  await policyStore.getPendingPolicyRequirementsSummary(
    policyStore.pendingData.requirementSummary.currentPage,
    pageSizeIndicator.value,
    sortQueryString.value,
    filterQueryString.value,
    true,
  );

  pageLoading.value = false;
});
</script>
<style scoped lang="pcss">
.pending-table__table {
  background-color: white;
}

.pending-table__link {
  color: var(--accent-color);
  cursor: pointer;
}

:deep(.v-table > .v-table__wrapper > table > tbody > tr > td) {
  height: var(--spacing-xxxl);
}

:deep() {
  .data-table__value--requirement,
  .data-table__value--primaryInsured,
  .data-table__value--agentName,
  .data-table__value--requirement,
  .data-table__value--employer {
    text-transform: capitalize;
  }

  /* styles specific to the pending version of the mobile table */
  .mobile-table .mobile-table__column--date {
    font-size: 0.875rem;
  }

  .mobile-table .mobile-table__column--actionNeeded {
    position: absolute;
    top: var(--spacing-l);
    right: var(--spacing-l);
    margin: 0;
  }

  .mobile-table .mobile-table__column--actionNeeded:before {
    display: none;
  }
}
</style>
