<template>
  <div class="tab__container">
    <div class="commissions__debt-tab__header">
      <div class="tab__heading">
        <h2 class="tab__title">
          Total:
          {{
            FormatHelpers.formatMoney(
              commissionDebtStore.debtUnsecuredAdvances.totalDebt || 0,
            )
          }}
        </h2>
      </div>
      <div v-if="commissionDebtStore.debtUnsecuredAdvances.error">
        An error has occurred, and no data is currently available. Please try
        refreshing the page or check back later.
      </div>
      <div class="debt-details__container">
        <DataFiltersDrawer
          v-model:filters="commissionDebtStore.debtUnsecuredAdvances.filters"
          :result-count="commissionDebtStore.debtUnsecuredAdvances.totalRecords"
        />
        <div class="debt-details__download-button_container d-none d-md-block">
          <aButton
            v-if="canDownload"
            button-style="primary"
            size="s"
            text="Download XLSX"
            class="debt-details__download-button"
            @click="downloadDebtUnsecuredAdvances"
          >
            <template #prepend>
              <AssurityIconDownload
                class="download-icon--mobile"
                color="white"
              />
            </template>
          </aButton>
        </div>
      </div>
    </div>
    <v-sheet :elevation="isMobile ? '0' : '4'" :rounded="isMobile ? '' : 'lg'">
      <v-data-table-server
        v-model:expanded="expanded"
        :headers="unsecuredAdvancesHeaders"
        :items="unsecuredAdvancesItems"
        :items-length="commissionDebtStore.debtUnsecuredAdvances.totalRecords"
        :loading="loading"
        item-value="agentId"
        show-expand
        class="debt-details-table"
        :class="{ 'server-table-mobile debt-details-table-mobile': isMobile }"
        :hide-headers="isMobile"
        pagination.sync="pagination"
        @update:options="updateOptions"
      >
        <template #headers="{ columns, toggleSort }">
          <tr>
            <template v-for="column in columns" :key="column.key">
              <th
                :class="{
                  'data-table__table-heading--right': column.align === 'end',
                }"
              >
                <span
                  class="data-table__table-heading"
                  :class="{
                    'data-table__table-heading--sortable': column.sortable,
                    'data-table__table-heading--not-sortable': !column.sortable,
                  }"
                  @click="() => toggleSort(column)"
                  >{{ column.title }}</span
                >
                <component
                  :is="getSortIcon(column.value as string)"
                  v-if="column.sortable"
                  class="data-table__table-heading-icon"
                  @click="() => toggleSort(column)"
                ></component>
              </th>
            </template>
          </tr>
        </template>
        <template v-if="isMobile" #item="{ item }">
          <v-progress-linear
            v-if="loading"
            indeterminate
            color="#007b99"
            class="mobile-table__loading-indicator"
            title="Loading..."
          >
          </v-progress-linear>
          <button
            v-else
            class="mobile-details-table__custom-row"
            @click="toggleExpanded(item.id, $event)"
          >
            <p class="custom-cell">
              Agent Name <span>{{ item.agentName }}</span>
            </p>
            <p class="custom-cell">
              Agent ID <span>{{ item.agentId }}</span>
            </p>
            <p class="custom-cell">
              Unsecured Advance Owed
              <span>{{ item.unsecuredAdvanceOwed }}</span>
            </p>
            <p class="custom-cell">
              Reversed Commission Owed
              <span>{{ item.reversedCommissionOwed }}</span>
            </p>

            <p class="custom-cell">
              Balance Owed <span>{{ item.balanceOwed }}</span>
            </p>
            <v-icon class="right-chevron">mdi-chevron-right</v-icon>
          </button>
          <AssurityDrawer
            v-model="expandedItems[item.id]"
            :item="item"
            :open-event="expandedTrayOpenEvent"
            class="expanded__details-drawer"
            anchor="left"
            :overlay="true"
          >
            <div class="expanded-content">
              <div class="expanded-content__header">
                <v-icon
                  class="expanded-content__close-icon"
                  color="secondary"
                  size="38"
                  @click="toggleExpanded(item.id, $event)"
                  >mdi-chevron-left</v-icon
                >
                <h3 class="expanded-content__title">Unsecured Advances</h3>
              </div>
              <aHorizontalRule class="expanded-content__hr" />
              <div class="expanded-content__header-details">
                <div>
                  <p class="expanded-content__agent-name">
                    Agent Name <span>{{ item.agentName }}</span>
                  </p>
                  <p class="expanded-content__agent-id">
                    Agent ID <span>{{ item.agentId }}</span>
                  </p>
                </div>
                <div class="expanded-content__header-details__icon">
                  <AssurityIconAgentProfile />
                </div>
              </div>
              <div class="expanded-content__details">
                <ExpandedInfoTableComponent
                  :expanded-headers="item.expandedInfo.expandedHeaders"
                  :expanded-items="item.expandedInfo.expandedItems"
                />
              </div>
            </div>
          </AssurityDrawer>
        </template>
        <template v-if="!isMobile" #expanded-row="{ columns, item }">
          <tr>
            <td :colspan="columns.length" class="debt-details-expanded">
              <ExpandedInfoTableComponent
                :expanded-headers="item.expandedInfo.expandedHeaders"
                :expanded-items="item.expandedInfo.expandedItems"
              />
            </td>
          </tr>
        </template>
        <template #bottom>
          <div
            v-if="commissionDebtStore.debtUnsecuredAdvances.totalRecords > 10"
            class="table-footer"
          >
            <div class="table-footer__pagination">
              <div class="table-footer__pagination-page-nav">
                <v-icon @click="prevPage">mdi-chevron-left</v-icon>
                <span class="px-2">{{ currentPage }} of {{ totalPages }}</span>
                <v-icon @click="nextPage">mdi-chevron-right</v-icon>
              </div>

              <div class="table-footer__pagination-num_of_items pl-4">
                <v-select
                  v-model="pagination.itemsPerPage"
                  hide-details
                  variant="outlined"
                  :items="[10, 25, 50, 100]"
                  density="compact"
                  class="ml-2"
                  @update:model-value="pageSizeChanged"
                ></v-select>
                <span class="pl-2">Per page</span>
              </div>
            </div>
          </div>
        </template>
      </v-data-table-server>
    </v-sheet>
    <div
      class="debt-details__download-button_container debt-details__download-button_container-mobile d-block d-md-none"
    >
      <aButton
        v-if="canDownload"
        button-style="primary"
        size="s"
        text="Download XLSX"
        class="debt-details__download-button"
        @click="downloadDebtUnsecuredAdvances"
      >
        <template #prepend>
          <AssurityIconDownload class="download-icon--mobile" color="white" />
        </template>
      </aButton>
    </div>
  </div>
</template>
<script setup lang="ts">
import {
  ref,
  watch,
  computed,
  isProxy,
  toRaw,
  onMounted,
  onUnmounted,
  reactive,
} from "vue";
import type { Ref } from "vue";
import type {
  DebtUnsecuredAdvancesAgent,
  DebtUnsecuredAdvancesAgentPolicy,
} from "@/models/Responses/CommissionsDebtUnsecuredAdvancesResponse";
import { useCommissionDebtStore } from "@/stores/commissionDebtStore";
import FormatHelpers from "@/Shared/utils/FormatHelpers";
import ExpandedInfoTableComponent from "@/Commissions/components/ExpandedDetailsTable.vue";
import aButton from "@/components/forms/aButton.vue";
import DataFiltersDrawer from "@/components/DataFiltersDrawer.vue";
import AssurityIconDownload from "@/components/icons/AssurityIconDownload.vue";
import AssurityDrawer from "@/components/AssurityDrawer.vue";
import aHorizontalRule from "@/components/aHorizontalRule.vue";
import AssurityIconAgentProfile from "@/components/icons/AssurityIconAgentProfile.vue";

import AssurityIconSortDescending from "@/components/icons/AssurityIconSortDescending.vue";
import AssurityIconSortAscending from "@/components/icons/AssurityIconSortAscending.vue";
import AssurityIconSortDefault from "@/components/icons/AssurityIconSortDefault.vue";

const commissionDebtStore = useCommissionDebtStore();

const expandedTrayOpenEvent: Ref<
  MouseEvent | KeyboardEvent | Event | undefined
> = ref();
const expanded = ref([]);

const isMobile = ref(window.innerWidth <= 960);
const updateIsMobile = () => {
  isMobile.value = window.innerWidth <= 960;
};

onMounted(() => {
  window.addEventListener("resize", updateIsMobile);
});

onUnmounted(() => {
  window.removeEventListener("resize", updateIsMobile);
});

const canDownload = computed(() => {
  return (
    !loading.value && commissionDebtStore.debtSecuredAdvances.agents.length > 0
  );
});

const loading = computed(() => {
  return commissionDebtStore.debtUnsecuredAdvances.loading;
});

// pagination:
const pagination = ref({
  itemsPerPage: commissionDebtStore.debtUnsecuredAdvances.pageSize,
  sortBy: commissionDebtStore.debtUnsecuredAdvances.orderBy
    ? [
        {
          key: commissionDebtStore.debtUnsecuredAdvances.orderBy,
          order: commissionDebtStore.debtUnsecuredAdvances.sortDirection,
        },
      ]
    : [],
});

const internalPage = ref(commissionDebtStore.debtUnsecuredAdvances.page);

const paginationPage = computed({
  get: () => commissionDebtStore.debtUnsecuredAdvances.page,
  set: (newValue) => {
    internalPage.value = newValue;
  },
});

const prevPage = () => {
  if (paginationPage.value > 1) {
    paginationPage.value--;
  }
  getDataFromServer();
};

const nextPage = () => {
  if (paginationPage.value < totalPages.value) {
    paginationPage.value++;
  }
  getDataFromServer();
};

const totalPages = computed(() => {
  const total = commissionDebtStore.debtUnsecuredAdvances.totalRecords;
  const perPage = commissionDebtStore.debtUnsecuredAdvances.pageSize;
  return Math.ceil(total / perPage);
});

const currentPage = computed(() => {
  return commissionDebtStore.debtUnsecuredAdvances.page;
});

const pageSizeChanged = (newPageSize: number) => {
  paginationPage.value = 1;
  pagination.value.itemsPerPage = newPageSize;
  getDataFromServer();
};

const updateOptions = (options: {
  page: number;
  itemsPerPage: number;
  sortBy: unknown;
}) => {
  pagination.value.sortBy = options.sortBy as { key: string; order: string }[];
  getDataFromServer();
};

const getDataFromServer = () => {
  updateItems({
    page: internalPage.value,
    itemsPerPage: pagination.value.itemsPerPage,
    sortBy: pagination.value.sortBy,
  });
};

function getSortIcon(columnValue: string) {
  const sort = pagination.value.sortBy?.find((s) => s.key === columnValue);
  if (sort) {
    switch (sort.order) {
      case "desc":
        return AssurityIconSortDescending;
      case "asc":
        return AssurityIconSortAscending;
      default:
        return AssurityIconSortDefault;
    }
  }
  return AssurityIconSortDefault;
}

const unsecuredAdvancesHeaders = [
  { title: "", key: "data-table-expand", align: "start" },
  {
    title: "Agent Name",
    key: "agentName",
    cellProps: { class: "debt-details-table__value--bold" },
  },
  { title: "Agent ID", key: "agentId" },
  {
    title: "Agent Status",
    key: "status",
  },
  {
    title: "Unsecured Advance Owed",
    key: "unsecuredAdvanceOwed",
    align: "end",
  },
  {
    title: "Reversed Commission Owed",
    key: "reversedCommissionOwed",
    align: "end",
  },
  {
    title: "Balance Owed",
    key: "balanceOwed",
    cellProps: { class: "debt-details-table__value--bold" },
    align: "end",
  },
];

const unsecuredAdvancesItems = computed(() => {
  const agents = commissionDebtStore.debtUnsecuredAdvances.agents;

  if (agents && agents.length > 0) {
    return agents.map(mapWritingAgentsForUnsecuredTable);
  }

  return [];
});

function mapWritingAgentsForUnsecuredTable(
  writingAgent: DebtUnsecuredAdvancesAgent,
) {
  return {
    id: crypto.randomUUID(),
    agentName: FormatHelpers.sentenceCase(writingAgent.agentName),
    agentId: writingAgent.agentId,
    status: writingAgent.agentStatus,
    unsecuredAdvanceOwed: FormatHelpers.formatMoney(
      writingAgent.unsecuredAdvanceOwed,
    ),
    reversedCommissionOwed: FormatHelpers.formatMoney(
      writingAgent.reversedCommissionOwed,
    ),
    balanceOwed: FormatHelpers.formatMoney(writingAgent.balanceOwed),
    expandedInfo: {
      expandedHeaders: [
        { title: "Policy Number", key: "policyNumber" },
        {
          title: "Insured Name",
          key: "insuredName",
          emphasizeValue: true,
        },
        { title: "Application Date", key: "applicationDate" },
        { title: "Paid to Date", key: "paidToDate" },
        {
          title: "Unsecured Advance Owed",
          key: "unsecuredAdvanceOwed",
          emphasizeValue: true,
        },
      ],
      expandedItems: writingAgent.policies.map(
        mapUnsecuredAdvancesExpandedInfoForTable,
      ),
    },
  };
}

function mapUnsecuredAdvancesExpandedInfoForTable(
  policyDetail: DebtUnsecuredAdvancesAgentPolicy,
) {
  return {
    policyNumber: policyDetail.policyNumber,
    insuredName: FormatHelpers.sentenceCase(policyDetail.insuredName),
    applicationDate: FormatHelpers.formatDate(policyDetail.applicationDate),
    paidToDate: FormatHelpers.formatDate(policyDetail.paidToDate),
    unsecuredAdvanceOwed: FormatHelpers.formatMoney(
      Number(policyDetail.unsecuredAdvanceOwed),
    ),
  } as DebtUnsecuredAdvancesAgentPolicy;
}

async function downloadDebtUnsecuredAdvances() {
  await commissionDebtStore.exportDebtUnsecuredAdvancesReport(
    commissionDebtStore.debtUnsecuredAdvances.page,
    commissionDebtStore.debtUnsecuredAdvances.pageSize,
    commissionDebtStore.debtUnsecuredAdvances.orderBy,
    commissionDebtStore.unsecuredAdvancesQueryString,
  );
}

const emit = defineEmits(["update:expanded"]);

// Update the expanded array when the v-model:expanded is modified
watch(expanded, (newValue) => {
  emit("update:expanded", newValue);
});

const expandedItems: Record<string, boolean> = reactive({});

const toggleExpanded = ref((id: string, $event: Event) => {
  if (expandedItems[id]) {
    expandedItems[id] = false;
  } else {
    expandedItems[id] = true;
  }
  expandedTrayOpenEvent.value = $event;
});

async function updateItems(options: {
  page: number;
  itemsPerPage: number;
  sortBy: unknown;
}) {
  if (isProxy(options.sortBy)) {
    const sort = toRaw(options.sortBy) as { key: string; order: string }[];
    if (sort.length > 0) {
      commissionDebtStore.debtUnsecuredAdvances.orderBy = sort[0].key;
      commissionDebtStore.debtUnsecuredAdvances.sortDirection = sort[0].order;
    } else {
      commissionDebtStore.debtUnsecuredAdvances.orderBy = "";
      commissionDebtStore.debtUnsecuredAdvances.sortDirection = "";
    }
  }
  if (options.page > 0) {
    commissionDebtStore.debtUnsecuredAdvances.page = options.page;
  }

  if (options.itemsPerPage > 0) {
    commissionDebtStore.debtUnsecuredAdvances.pageSize = options.itemsPerPage;
  }
}
</script>
<style lang="pcss" scoped>
@import "../styles/commissions.pcss";
</style>
<style scoped lang="pcss">
.commissions__debt-tab__header {
  padding: 0 var(--spacing-l);
  @media (width >= 600px) {
    padding: 0;
  }
}
.debt-details-table {
  font-size: 1rem;
  color: var(--text-grey);
}
:deep(.v-data-table-header__content) {
  font-size: 16px;
  font-weight: 600;
  color: var(--text-grey);
}
:deep(.debt-details-table__value--bold) {
  font-weight: 600;
}
:deep(.debt-details-table .v-data-table-header__sort-icon) {
  opacity: 1 !important;
  font-size: 1rem;
}

.debt-details-table td.debt-details-expanded {
  padding-top: 8px !important;
  padding-bottom: 8px !important;
  background-color: #f1f7f9;
}

:deep(.debt-details-table .v-btn__content i.mdi-chevron-down),
:deep(.debt-details-table .v-btn__content i.mdi-chevron-up) {
  font-size: 32px;
  color: var(--accent-color);
}

.tab__title {
  margin-top: 24px;
  margin-bottom: 24px;
  font-size: 33px;
  color: var(--primary-color);
}
.debt-details__container {
  display: flex;
  flex-direction: row;
  margin-bottom: 24px;
}
.debt-details__download-button {
  /* position far right */
  margin-left: auto;
}
.debt-details__download-button_container {
  margin-left: auto;
}
.debt-details__download-button_container-mobile {
  width: 100%;
  .debt-details__download-button {
    margin-top: var(--spacing-l);
    width: 100%;
  }
}

.debt-details-table .mobile-details-table__custom-row {
  display: flex;
  flex-direction: column;
  padding: var(--spacing-l) 0;
  background-color: var(--white);
  position: relative;
  text-align: left;
  width: 100%;
  .custom-cell {
    display: flex;
    flex-direction: column;
    padding: 0 var(--spacing-l) 0;
    background-color: var(--white);
    font-size: 14px;
    & span {
      display: block;
      font-size: 18px;
      font-weight: 600;
      margin-bottom: var(--spacing-s);
    }
  }
  .custom-cell-date {
    background-color: var(--white);
    font-size: 14px;
    font-weight: 600;
    padding-bottom: var(--spacing-s);
  }
  .right-chevron {
    position: absolute;
    right: 8px;
    top: 50%;
    transform: translateY(-50%);
    font-size: 46px;
  }
}
.data-table__table-heading {
  font-size: 1rem;
  font-weight: bold !important;
  white-space: nowrap;
  margin-right: var(--spacing-xs);
}
.data-table__table-heading-icon {
  height: 20px;
  vertical-align: middle;
  line-height: 1;
}
.data-table__table-heading--sortable,
.data-table__table-heading-icon {
  cursor: pointer;
}
.data-table__table-heading--not-sortable {
  cursor: default;
  pointer-events: none;
}
.debt-details-table.v-table--density-default
  > .v-table__wrapper
  > table
  > thead
  > tr
  > td {
  white-space: nowrap;
  border: none;
  padding-bottom: 2px;
  background-image: radial-gradient(
    circle at 1px 1px,
    var(--border-grey) 1px,
    rgba(0, 0, 0, 0) 0
  );
  background-repeat: repeat-x;
  background-size: 6px 4px;
  background-position: bottom;
  margin: 0;
}
.debt-details-table.v-table--density-default
  > .v-table__wrapper
  > table
  > thead
  > tr
  > th {
  height: var(--spacing-xxxl);
  padding-top: var(--spacing);
}
</style>
