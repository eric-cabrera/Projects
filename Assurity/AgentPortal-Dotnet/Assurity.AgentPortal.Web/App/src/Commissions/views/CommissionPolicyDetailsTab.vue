<template>
  <div class="tab__container">
    <v-sheet :elevation="isMobile ? '0' : '4'" :rounded="isMobile ? '' : 'lg'">
      <v-data-table-server
        v-model:expanded="expanded"
        :headers="headers"
        :items="policyItems"
        :items-length="
          commissionDebtStore.policyDetails.commissionDetails.totalRecords
        "
        :loading="loading"
        item-value="id"
        show-expand
        class="policy-details-table"
        :class="{ 'server-table-mobile': isMobile }"
        :sort-icon="true"
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
            <p class="custom-cell custom-cell-date">{{ item.date }}</p>
            <p class="custom-cell">
              Writing Agent <span>{{ item.writingAgent }}</span>
            </p>
            <p class="custom-cell">
              Agent ID <span>{{ item.writingAgentId }}</span>
            </p>
            <p class="custom-cell">
              Primary Insured <span>{{ item.primaryinsured }}</span>
            </p>
            <p class="custom-cell">
              Commission <span>{{ item.commission }}</span>
            </p>
            <v-icon class="right-chevron">mdi-chevron-right</v-icon>
          </button>
          <aHorizontalRule class="expanded-content__hr" />
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
                <h3 class="expanded-content__title">Policy Details</h3>
              </div>
              <aHorizontalRule class="expanded-content__hr" />
              <div class="expanded-content__header-details">
                <div>
                  <p class="expanded-content__agent-name">
                    Agent Name: <span>{{ item.writingAgent }}</span>
                  </p>
                  <p class="expanded-content__agent-id">
                    Agent ID: <span>{{ item.writingAgentId }}</span>
                  </p>
                </div>
                <div class="expanded-content__header-details__icon">
                  <AssurityIconAgentProfile />
                </div>
              </div>
              <div class="expanded-content__details">
                <ExpandedInfoComponent :items="item.expandedInfo" />
              </div>
            </div>
          </AssurityDrawer>
        </template>
        <template v-if="!isMobile" #expanded-row="{ columns, item }">
          <tr>
            <td :colspan="columns.length" class="policy-details-expanded">
              <ExpandedInfoComponent :items="item.expandedInfo" />
            </td>
          </tr>
        </template>
        <template #bottom>
          <div
            v-if="
              commissionDebtStore.policyDetails.commissionDetails.totalRecords >
              10
            "
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
  </div>
</template>

<script setup lang="ts">
import {
  ref,
  watch,
  computed,
  toRaw,
  isProxy,
  reactive,
  onMounted,
  onUnmounted,
} from "vue";
import type { Ref } from "vue";
import type { CommissionsPolicyDetail } from "@/models/Responses/CommissionsPolicyDetailsResponse";
import { useCommissionDebtStore } from "@/stores/commissionDebtStore";
import FormatHelpers from "@/Shared/utils/FormatHelpers";
import ExpandedInfoComponent from "@/Commissions/components/ExpandedDetails.vue";
import AssurityDrawer from "@/components/AssurityDrawer.vue";
import aHorizontalRule from "@/components/aHorizontalRule.vue";
import AssurityIconAgentProfile from "@/components/icons/AssurityIconAgentProfile.vue";

import AssurityIconSortDescending from "@/components/icons/AssurityIconSortDescending.vue";
import AssurityIconSortAscending from "@/components/icons/AssurityIconSortAscending.vue";
import AssurityIconSortDefault from "@/components/icons/AssurityIconSortDefault.vue";

const commissionDebtStore = useCommissionDebtStore();

const expandedTrayOpenEvent: Ref<Event | null> = ref(null);

// pagination:
const pagination = ref({
  itemsPerPage: commissionDebtStore.policyDetails.pageSize,
  sortBy: commissionDebtStore.policyDetails.orderBy
    ? [
        {
          key: commissionDebtStore.policyDetails.orderBy,
          order: commissionDebtStore.policyDetails.sortDirection,
        },
      ]
    : [],
});

const internalPage = ref(commissionDebtStore.policyDetails.page);

const paginationPage = computed({
  get: () => commissionDebtStore.policyDetails.page,
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
  const total =
    commissionDebtStore.policyDetails.commissionDetails.totalRecords;
  const perPage = commissionDebtStore.policyDetails.pageSize;
  return Math.ceil(total / perPage);
});

const currentPage = computed(() => {
  return commissionDebtStore.policyDetails.page;
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

const getDataFromServer = () => {
  loadItems({
    page: internalPage.value,
    itemsPerPage: pagination.value.itemsPerPage,
    sortBy: pagination.value.sortBy,
  });
};

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

const loading = computed(() => {
  return commissionDebtStore.policyDetails.loading;
});

const headers = [
  { key: "data-table-expand", align: "start" },
  { title: "Cycle Date", key: "date", sortable: false },
  {
    title: "Writing Agent",
    key: "writingAgent",
    cellProps: { class: "policy-details-table__value--bold" },
  },
  { title: "Agent ID", key: "writingAgentId" },
  {
    title: "Primary Insured",
    key: "primaryinsured",
  },
  { title: "Policy Number", key: "policyNumber", sortable: false },
  {
    title: "Commission",
    key: "commission",
    align: "end",
    cellProps: { class: "policy-details-table__value--bold" },
  },
];

const policyItems = computed(() => {
  const policies =
    commissionDebtStore.policyDetails.commissionDetails.policyDetails;
  return policies.map(mapPoliciesForTable);
});

function mapPoliciesForTable(policy: CommissionsPolicyDetail) {
  return {
    id: crypto.randomUUID(),
    date: FormatHelpers.formatDate(policy.paymentDate),
    writingAgent: FormatHelpers.sentenceCase(policy.writingAgent),
    writingAgentId: policy.writingAgentId,
    policyNumber: policy.policyNumber,
    primaryinsured: FormatHelpers.sentenceCase(policy.primaryInsured),
    commission: FormatHelpers.formatMoney(policy.netCommission),
    expandedInfo: [
      {
        label: "Premium Due Date:",
        value: FormatHelpers.formatDate(policy.premiumDueDate),
      },
      { label: "Product Description:", value: policy.productDescription },
      { label: "Mode:", value: policy.mode },
      { label: "Commission Rate:", value: `${policy.commissionRate}%` },
      {
        label: "Cycle Date:",
        value: FormatHelpers.formatDate(policy.paymentDate),
      },
      {
        label: "Employer Name/ID:",
        value: `${policy.employerName} - ${policy.employerId}`,
      },
      {
        label: "Mode Premium:",
        value: FormatHelpers.formatMoney(policy.modePremium),
      },
      {
        label: "Advance Recovery:",
        value: FormatHelpers.formatMoney(policy.advanceRecovery),
      },
      { label: "Contract:", value: policy.contract },
      { label: "Chargeback Reason:", value: policy.chargebackReason },
      {
        label: "Net Commission:",
        value: FormatHelpers.formatMoney(policy.netCommission),
      },
      { label: "Line of Business:", value: policy.lineOfBusiness },
      {
        label: "Transaction Date:",
        value: FormatHelpers.formatDate(policy.transactionDate),
      },
      { label: "Commission Type:", value: policy.commissionType },
    ],
  };
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
function loadItems(options: {
  page: number;
  itemsPerPage: number;
  sortBy: unknown;
}) {
  if (isProxy(options.sortBy)) {
    const sort = toRaw(options.sortBy) as { key: string; order: string }[];
    if (sort.length > 0) {
      commissionDebtStore.policyDetails.orderBy = sort[0].key;
      commissionDebtStore.policyDetails.sortDirection = sort[0].order;
    } else {
      commissionDebtStore.policyDetails.orderBy = "";
      commissionDebtStore.policyDetails.sortDirection = "";
    }
  }

  if (options.page > 0) {
    commissionDebtStore.policyDetails.page = options.page;
  }

  if (options.itemsPerPage > 0) {
    commissionDebtStore.policyDetails.pageSize = options.itemsPerPage;
  }
}
</script>
<style lang="pcss" scoped>
@import "../styles/commissions.pcss";
</style>
<style lang="pcss">
.policy-details-table {
  font-size: 1rem;
  color: var(--text-grey);
}
.policy-details-table .v-data-table-header__content {
  font-size: 1rem;
  font-weight: 600;
  color: var(--text-grey);
}
.policy-details-table .v-data-table-header__sort-icon {
  opacity: 1 !important;
  font-size: 1rem;
}
.policy-details-table__value--bold {
  font-weight: 600;
}
.policy-details-table td.policy-details-expanded {
  padding-top: var(--spacing-s) !important;
  padding-bottom: var(--spacing-s) !important;
  background-color: #f1f7f9;
}
.policy-details-table td.policy-details-expanded .v-row {
  margin-left: 80px;
  margin-bottom: 0;
  margin-top: 0;
}

.server-table-mobile thead {
  display: none;
}

.assurity-drawer__container {
  &.expanded__details-drawer {
    padding: var(--spacing-l) 0 0;
    max-width: 100%;
    width: 100%;
    scrollbar-gutter: stable;
    top: 45px;
    background-color: var(--bg-grey);
  }
}

/* Adding align="end" in the header options for the data to be right aligned, needs to be fixed for the header itself */
.policy-details-table
  .v-table__wrapper
  > table
  > thead
  > tr
  th.v-data-table-column--align-end
  .v-data-table-header__content {
  flex-direction: initial;
  justify-content: flex-end;
}

.data-table__table-heading--right {
  text-align: right !important;
}

.policy-details-table .v-btn__content i.mdi-chevron-down,
.policy-details-table .v-btn__content i.mdi-chevron-up {
  font-size: 32px;
  color: var(--accent-color);
}

.policy-details-table .mobile-details-table__custom-row {
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
.expanded-content__header {
  display: flex;
  align-items: center;
  padding: 0 0 var(--spacing-l);
  background-color: var(--bg-grey);
  border-bottom: 0;
  margin-top: calc(var(--spacing-xl) * -1);
}

.expanded-content .expanded-content__hr {
  margin: 0 var(--spacing-l);
  width: auto;
}

.expanded-content__header-details {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: var(--spacing-l);
  background-color: var(--bg-grey);
  font-size: 14px;
  & span {
    display: block;
    font-size: 18px;
    font-weight: 600;
    margin-bottom: var(--spacing-s);
  }
}
.expanded-content__title {
  color: var(--primary-color);
  line-height: 1.2;
  margin-bottom: 0;
  font-size: 1.5rem;
}
.expanded-content__details {
  background-color: var(--white);
  box-shadow: var(--mobile-shadow) !important;
  padding: var(--spacing-l);
  overflow-y: auto;
  .v-row {
    display: block;
    margin-bottom: var(--spacing-l);
    .v-col {
      max-width: 100% !important; /*override the max-width set by vuetify */
    }
    .v-card__content__label {
      font-size: 14px;
      font-weight: 300;
    }
    .v-card__content__value {
      font-weight: 600;
      font-size: 18px;
    }
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
.policy-details-table.v-table--density-default
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
.policy-details-table.v-table--density-default
  > .v-table__wrapper
  > table
  > thead
  > tr
  > th {
  height: var(--spacing-xxxl);
  padding-top: var(--spacing);
}
.v-sheet.elevation-4 {
  box-shadow: var(--desktop-shadow) !important;
}
</style>
