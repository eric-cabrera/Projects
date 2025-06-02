<template>
  <div class="active-table">
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
        v-model:filters="policyStore.activeData.policySummary.filters"
        :result-count="policyStore.activeData.policySummary.totalPolicies"
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
        class="d-none d-md-block"
        :data-rows="policies"
        :more-pages="morePages"
        :less-pages="lessPages"
        :page-indicator="pageIndicator"
        :page-size-indicator="pageSizeIndicator"
        :loading="isLoading"
        :error="policyStore.activeData.error ? true : false"
        @update-sort="updateSort($event)"
        @column-selected="selectedPolicy($event.data)"
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
            >Unfortunately, we cannot display the policy or certificate in full
            detail online.
          </b>
          Please submit the owner’s signed request on the
          <a
            class="dialog-box__link"
            href="https://www.assurity.com/forms/DuplicatePolicyCertificateRequest.pdf"
            target="_blank"
            >Duplicate Policy / Certificate Request form</a
          >
          to us by fax at 888-255-2060, secure email or to our mailing address.
          It only takes about 2 weeks for the owner to receive the copy.
        </p>
        <p>
          If you only need a description of the policy benefits, contact
          Customer Connections at
          <b>800-869-0355, Ext. 4279</b> and we can get that information to you
          within 5 - 7 business days.
        </p>
      </span>
    </AssurityModal>
  </div>
</template>
<script setup lang="ts">
import { computed, onMounted, onUnmounted, ref, watch, type Ref } from "vue";
import { useRouter } from "vue-router";
import { useUserStore } from "@/stores/userStore";
import { useUtilityStore } from "@/stores/utilityStore";
import { usePolicyStore } from "@/stores/policystore";
import { PolicyStatus } from "@/models/enums/PolicyStatus";
import type { PolicySummary } from "@/models/PolicySummary";
import type { TableColumn } from "@/models/components/TableColumn";
import FormatHelpers from "@/Shared/utils/FormatHelpers";
import AssurityModal from "@/components/AssurityModal.vue";
import DataTable from "@/components/DataTable.vue";
import DataFilters from "@/components/DataFilters.vue";
import { SortOptions } from "@/models/enums/SortOptions";
import aButton from "@/components/forms/aButton.vue";
import AssurityIconDownload from "@/components/icons/AssurityIconDownload.vue";
import { FilterField } from "@/models/enums/FilterField";
import type { AssignedAgent } from "@/models/AssignedAgent";

const userStore = useUserStore();
const policyStore = usePolicyStore();
const router = useRouter();
const utilityStore = useUtilityStore();

const pageLoading = ref(true);
const isLoading = ref(false);
const httpErrorIgnoredEndpoint = "/API/PolicyInfo/Policies/*/Export";

defineEmits([
  "firstPageClick",
  "previousPageClick",
  "nextPageClick",
  "lastPageClick",
]);

const dialog: Ref<{ dialog: HTMLDialogElement } | null> = ref(null);
const pastDueFilter = ref(false);
const downloadingPolicies = ref(false);

onUnmounted(() => {
  utilityStore.removeEndpointToIgnore(httpErrorIgnoredEndpoint);
});

const canDownload = computed(() => {
  return (
    !isLoading.value && policyStore.activeData.policySummary.totalPolicies > 0
  );
});

const hideEmployer = computed(() => {
  const employerFilter = policyStore.activeData.policySummary.filters.filter(
    (filter) => {
      if (filter.field === FilterField.Employer) {
        return filter;
      }
    },
  );
  if (employerFilter && employerFilter.length > 0) {
    return !employerFilter[0].display;
  }
  return false;
});
const hideAdditionalAgents = computed(() => {
  const agentFilter = policyStore.activeData.policySummary.filters.filter(
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

// TODO: apply DRY principles
const issueDateSort = computed(() => {
  if (policyStore.activeData.policySummary.headers.length > 0) {
    return (
      policyStore.activeData.policySummary.headers.filter(
        (header) => header.id === "issueDate",
      )[0].sortValue || SortOptions.none
    );
  }

  return SortOptions.none;
});

const paidToDateSort = computed(() => {
  if (policyStore.activeData.policySummary.headers.length > 0) {
    return (
      policyStore.activeData.policySummary.headers.filter(
        (header) => header.id === "paidToDate",
      )[0].sortValue || SortOptions.none
    );
  }

  return SortOptions.none;
});

const billingModeSort = computed(() => {
  if (policyStore.activeData.policySummary.headers.length > 0) {
    return (
      policyStore.activeData.policySummary.headers.filter(
        (header) => header.id === "billingMode",
      )[0].sortValue || SortOptions.none
    );
  }

  return SortOptions.none;
});

const sortQueryString = computed(() => {
  let sortString = "";

  if (policyStore.activeData.policySummary.headers.length) {
    const billingModeSortString = getSortString(
      "BillingMode",
      billingModeSort.value,
    );
    const issueDateSortString = getSortString("IssueDate", issueDateSort.value);
    const paidToDateSortString = getSortString(
      "PaidToDate",
      paidToDateSort.value,
    );

    // TODO: Apply DRY principles
    // Extract sorting code into new helper, import into summary views.
    if (billingModeSortString) {
      if (sortString.length > 0) {
        sortString = sortString + `,${billingModeSortString}`;
      } else {
        sortString = sortString + `${billingModeSortString}`;
      }
    }

    if (issueDateSortString) {
      if (sortString.length > 0) {
        sortString = sortString + `,${issueDateSortString}`;
      } else {
        sortString = sortString + `${issueDateSortString}`;
      }
    }

    if (paidToDateSortString) {
      if (sortString.length > 0) {
        sortString = sortString + `,${paidToDateSortString}`;
      } else {
        sortString = sortString + `${paidToDateSortString}`;
      }
    }
  } else {
    sortString = "IssueDate_desc";
  }

  return sortString;
});

const filterQueryString = computed(() => {
  let queryString = "";
  policyStore.activeData.policySummary.filters.map((filter) => {
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
    await policyStore.getPolicySummaries(
      PolicyStatus.Active,
      policyStore.activeData.policySummary.currentPage,
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
    policyStore.activeData.policySummary.currentPage = 1;
    await policyStore.getPolicySummaries(
      PolicyStatus.Active,
      policyStore.activeData.policySummary.currentPage,
      pageSizeIndicator.value,
      sortQueryString.value,
      newValue,
      true,
    );

    isLoading.value = false;
  },
);

watch(
  () => pastDueFilter.value,
  (newValue) => {
    policyStore.activeData.policySummary.filters.forEach((filter) => {
      if (filter.field === FilterField.PastDue) {
        filter.selection = newValue;
      }
    });
  },
);

const morePages = computed(() => {
  if (
    policyStore.activeData.policySummary.currentPage <
    policyStore.activeData.policySummary.totalPages
  ) {
    return true;
  }
  return false;
});

const lessPages = computed(() => {
  if (policyStore.activeData.policySummary.currentPage === 1) {
    return false;
  }
  return true;
});

const policies = computed(() => {
  const summaries = policyStore.activeData.policySummary.summaries;
  return summaries.map((summary) => mapPolicySummaries(summary));
});

const headers = computed(() => {
  return policyStore.activeData.policySummary.headers.filter((header) => {
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
  policyStore.activeData.policySummary.headers.forEach((header) => {
    if (header.id === event.id) {
      Object.assign(header, event);
    } else {
      header.sortValue = SortOptions.none;
    }
  });
}

const pageIndicator = computed(() => {
  const currentPage = policyStore.activeData.policySummary.currentPage;
  const totalPolicies = policyStore.activeData.policySummary.totalPolicies;
  const currentItem =
    currentPage * pageSizeIndicator.value - (pageSizeIndicator.value - 1);

  const pageMessage = `${currentItem} - ${
    currentItem + policies.value.length - 1
  } of ${totalPolicies}`;

  return pageMessage;
});

const pageSizeIndicator = computed(() => {
  return policyStore.activeData.policySummary.pageSize
    ? policyStore.activeData.policySummary.pageSize
    : 10;
});

function getServicingAgent(agents: AssignedAgent[]): AssignedAgent | undefined {
  return agents.filter((agent) => agent.isServicingAgent)[0];
}

function mapPolicySummaries(summary: PolicySummary) {
  const servicingAgent = summary?.assignedAgents
    ? getServicingAgent(summary.assignedAgents)
    : undefined;

  let policy = [
    {
      id: "agentName",
      heading: "Agent",
      value: servicingAgent?.assignedAgentName
        ? servicingAgent?.assignedAgentName.toLowerCase()
        : "",
      subValue: servicingAgent?.assignedAgentId,
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
      id: "billingMode",
      heading: "Billing Mode",
      value: summary.billingMode,
    },
    {
      id: "issueDate",
      heading: "Issued Date",
      value: FormatHelpers.formatDate(summary.issueDate),
    },
    {
      id: "paidToDate",
      heading: "Paid to Date",
      flagStatus: summary.pastDue ? "error" : null,
      flagContent: summary.pastDue ? "PAST DUE" : null,
      value: FormatHelpers.formatDate(summary.paidToDate),
    },
  ] as TableColumn[];

  if (hideAdditionalAgents.value) {
    policy = policy.filter((column) => column.id !== "agentName");
  }

  if (hideEmployer.value) {
    policy = policy.filter(
      (column) => column.filterField !== FilterField.Employer,
    );
  }

  return policy;
}

// TODO: consider if we can move pagination logic into a reusable helper. DRY it out
async function loadFirstPage() {
  // TODO: Move loading spinner to table component <tbody> (leave headers visible during loading)
  // cancel previous call if user makes multiple sort / filter requests in succession
  // add error message display to table components if API call fails
  isLoading.value = true;
  await policyStore.getPolicySummaries(
    PolicyStatus.Active,
    1,
    pageSizeIndicator.value,
    sortQueryString.value,
    filterQueryString.value,
  );
  isLoading.value = false;
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

async function loadPreviousPage() {
  isLoading.value = true;
  policyStore.activeData.policySummary.currentPage =
    policyStore.activeData.policySummary.currentPage - 1;
  await policyStore.getPolicySummaries(
    PolicyStatus.Active,
    policyStore.activeData.policySummary.currentPage,
    pageSizeIndicator.value,
    sortQueryString.value,
    filterQueryString.value,
  );
  isLoading.value = false;
}

async function loadNextPage() {
  isLoading.value = true;

  policyStore.activeData.policySummary.currentPage =
    policyStore.activeData.policySummary.currentPage + 1;
  await policyStore.getPolicySummaries(
    PolicyStatus.Active,
    policyStore.activeData.policySummary.currentPage,
    pageSizeIndicator.value,
    sortQueryString.value,
    filterQueryString.value,
  );
  isLoading.value = false;
}

async function loadLastPage() {
  isLoading.value = true;

  await policyStore.getPolicySummaries(
    PolicyStatus.Active,
    policyStore.activeData.policySummary.totalPages,
    pageSizeIndicator.value,
    sortQueryString.value,
    filterQueryString.value,
  );
  isLoading.value = false;
}

function updatePageSize(pageSize: number) {
  if (policyStore.activeData.policySummary.pageSize == pageSize) return;

  policyStore.activeData.policySummary.pageSize = pageSize;

  loadFirstPage();
}

async function selectedPolicy(column: TableColumn) {
  const policyNumber = column.data;

  if (policyNumber) {
    if (column.id.toLowerCase() === "policynumber") {
      const success = await policyStore.downloadPolicy(policyNumber);
      if (success === false) {
        if (dialog.value) {
          dialog.value.dialog.showModal();
        }
      }
    } else {
      policyStore.selectedPolicy.policyNumber = policyNumber;
      router.push({
        name: "policy-details",
        params: { id: policyNumber },
      });
    }
  }
}

function filterDate(startDate: string | null, endDate: string | null) {
  if (startDate && endDate) {
    policyStore.activeData.policySummary.filters.forEach((filter) => {
      if (filter.field === FilterField.Date) {
        filter.startDate = startDate;
        filter.endDate = endDate;
      }
    });
  }
}

async function callDownloadExcelDocument() {
  downloadingPolicies.value = true;
  await policyStore.downloadExcelDocument("active", filterQueryString.value);
  downloadingPolicies.value = false;
}

onMounted(async () => {
  utilityStore.addEndpointToIgnore(httpErrorIgnoredEndpoint);

  await policyStore.getPolicySummaries(
    PolicyStatus.Active,
    policyStore.activeData.policySummary.currentPage,
    pageSizeIndicator.value,
    sortQueryString.value,
    filterQueryString.value,
    true,
  );

  pageLoading.value = false;
});
</script>
<style scoped>
.active-table__table {
  background-color: white;
}

.active-table__link {
  color: var(--accent-color);
  cursor: pointer;
}

/* styles specific to the active version of the mobile table */
:deep(.mobile-active-table .mobile-table__column--date) {
  font-size: 0.75rem;
}

:deep(.mobile-active-table .mobile-table__column--actionNeeded) {
  position: absolute;
  top: var(--spacing-l);
  right: var(--spacing-l);
  margin: 0;
}

:deep(.mobile-active-table .mobile-table__column--date:before),
:deep(.mobile-active-table .mobile-table__column--actionNeeded:before) {
  display: none;
}
:deep(.data-table__column--paidToDate),
:deep(.data-table__column--primaryInsured),
:deep(.data-table__column--policyNumber) {
  white-space: nowrap;
}

:deep(.mobile-table__column--paidToDate) {
  text-align: right;
  position: absolute;
  right: var(--spacing);
  bottom: var(--spacing);
}

:deep(.mobile-table__column--policyNumber) {
  font-size: 1rem;
}

:deep(.mobile-table__column--policyNumber .mobile-table__link-icon) {
  display: none;
}

:deep() {
  .data-table__value--primaryInsured,
  .data-table__value--agentName,
  .data-table__value--employer {
    text-transform: capitalize;
  }
}
</style>
