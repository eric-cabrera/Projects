<template>
  <div class="tab__container">
    <v-sheet v-if="!isMobile" elevation="4" rounded="lg">
      <v-data-table-server
        :headers="headers"
        :items="agentItems"
        :items-length="commissionDebtStore.writingAgentDetails.totalRecords"
        :loading="loading"
        :row-props="rowProps"
        item-value="name"
        class="writing-agent-details-table"
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
        <template #bottom>
          <div
            v-if="commissionDebtStore.writingAgentDetails.totalRecords > 10"
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
    <v-data-table-server
      v-else
      :headers="headers"
      :items="agentItems"
      :items-length="commissionDebtStore.writingAgentDetails.totalRecords"
      :loading="loading"
      item-value="name"
      class="writing-agent-details-table"
      :class="{ 'server-table-mobile': isMobile }"
      :hide-headers="isMobile"
      pagination.sync="pagination"
      @update:options="loadItems"
    >
      <template v-for="(item, index) in agentItems">
        <div
          v-if="index === 0"
          :key="'first-' + index"
          class="mobile-details-table__custom-row mobile-details-table__custom-row--totals"
        >
          <div class="totals-header">
            <h2 class="v-card__title-text__title">Totals</h2>
            <div class="v-card__title-icon">
              <AssurityIconMoney />
            </div>
          </div>
          <aHorizontalRule class="expanded-content__hr" />
          <div class="writing-agent-details">
            <p class="custom-cell">
              Cycle Paid First Year <span>{{ item.cyclepaidfirstyear }}</span>
            </p>
            <p class="custom-cell">
              Cycle Paid Renewal <span>{{ item.cyclepaidrenewal }}</span>
            </p>
            <p class="custom-cell">
              YTD Paid First Year <span>{{ item.ytdpaidfirstyear }}</span>
            </p>
            <p class="custom-cell">
              YTD Paid Renewal <span>{{ item.ytdpaidrenewal }}</span>
            </p>
          </div>
        </div>
        <div
          v-else
          :key="'other-' + index"
          class="mobile-details-table__custom-row"
        >
          <p class="custom-cell">
            Writing Agent <span>{{ item.writingagent }}</span>
          </p>
          <p class="custom-cell">
            Agent ID <span>{{ item.agentid }}</span>
          </p>
          <div class="writing-agent-details">
            <p class="custom-cell">
              Cycle Paid First Year <span>{{ item.cyclepaidfirstyear }}</span>
            </p>
            <p class="custom-cell">
              Cycle Paid Renewal <span>{{ item.cyclepaidrenewal }}</span>
            </p>
            <p class="custom-cell">
              YTD Paid First Year <span>{{ item.ytdpaidfirstyear }}</span>
            </p>
            <p class="custom-cell">
              YTD Paid Renewal <span>{{ item.ytdpaidrenewal }}</span>
            </p>
          </div>
          <aHorizontalRule class="expanded-content__hr" />
        </div>
      </template>

      <template #bottom>
        <div
          v-if="commissionDebtStore.writingAgentDetails.totalRecords > 10"
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
                v-model="pagination.pageSize"
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
  </div>
</template>

<script setup lang="ts">
import { ref, computed, toRaw, isProxy, onMounted, onUnmounted } from "vue";
import type { WritingAgentCommissionDetail } from "@/models/Responses/CommissionsWritingAgentDetailsResponse";
import { useCommissionDebtStore } from "@/stores/commissionDebtStore";
import FormatHelpers from "@/Shared/utils/FormatHelpers";
import aHorizontalRule from "@/components/aHorizontalRule.vue";
import AssurityIconMoney from "@/components/icons/AssurityIconMoney.vue";

import AssurityIconSortDescending from "@/components/icons/AssurityIconSortDescending.vue";
import AssurityIconSortAscending from "@/components/icons/AssurityIconSortAscending.vue";
import AssurityIconSortDefault from "@/components/icons/AssurityIconSortDefault.vue";

const commissionDebtStore = useCommissionDebtStore();

// pagination:
const pagination = ref({
  itemsPerPage: commissionDebtStore.writingAgentDetails.pageSize,
  sortBy: commissionDebtStore.writingAgentDetails.orderBy
    ? [
        {
          key: commissionDebtStore.writingAgentDetails.orderBy,
          order: commissionDebtStore.writingAgentDetails.sortDirection,
        },
      ]
    : [],
});

const internalPage = ref(commissionDebtStore.writingAgentDetails.page);

const paginationPage = computed({
  get: () => commissionDebtStore.writingAgentDetails.page,
  set: (value: number) => {
    internalPage.value = value;
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
  const total = commissionDebtStore.writingAgentDetails.totalRecords;
  const perPage = commissionDebtStore.writingAgentDetails.pageSize;
  return Math.ceil(total / perPage);
});

const currentPage = computed(() => {
  return commissionDebtStore.writingAgentDetails.page;
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
  return commissionDebtStore.writingAgentDetails.loading;
});

const headers = [
  {
    title: "Writing Agent",
    key: "writingagent",
    cellProps: { class: "writing-agent-details-table__value--bold" },
  },
  {
    title: "Agent ID",
    key: "agentid",
  },
  {
    title: "Cycle Paid First Year",
    key: "cyclepaidfirstyear",
    align: "end",
  },
  {
    title: "Cycle Paid Renewal",
    key: "cyclepaidrenewal",
    align: "end",
  },
  {
    title: "YTD Paid First Year",
    key: "ytdpaidfirstyear",
    align: "end",
  },
  {
    title: "YTD Paid Renewal",
    key: "ytdpaidrenewal",
    align: "end",
  },
];

const agentItems = computed(() => {
  const comissionDetailItems =
    commissionDebtStore.writingAgentDetails.writingAgentCommissions;
  return [
    {
      writingagent: "Totals",
      agentid: "",
      cyclepaidfirstyear: FormatHelpers.formatMoney(
        commissionDebtStore.writingAgentDetails.cyclePaidFirstYearTotal,
      ),
      cyclepaidrenewal: FormatHelpers.formatMoney(
        commissionDebtStore.writingAgentDetails.cyclePaidRenewalTotal,
      ),
      ytdpaidfirstyear: FormatHelpers.formatMoney(
        commissionDebtStore.writingAgentDetails.yearTotDateFirstYearTotal,
      ),
      ytdpaidrenewal: FormatHelpers.formatMoney(
        commissionDebtStore.writingAgentDetails.yearToDateRenewalTotal,
      ),
    },
    ...comissionDetailItems.map(mapComissionDetailsForTable),
  ];
});

function mapComissionDetailsForTable(
  comissionDetail: WritingAgentCommissionDetail,
) {
  return {
    writingagent: FormatHelpers.sentenceCase(comissionDetail.agentName),
    agentid: comissionDetail.agentId,
    cyclepaidfirstyear: FormatHelpers.formatMoney(
      comissionDetail.cycleFirstYearCommissions,
    ),
    cyclepaidrenewal: FormatHelpers.formatMoney(
      comissionDetail.cycleRenewalCommissions,
    ),
    ytdpaidfirstyear: FormatHelpers.formatMoney(
      comissionDetail.firstYearCommissionsYtd,
    ),
    ytdpaidrenewal: FormatHelpers.formatMoney(
      comissionDetail.renewalCommissionsYtd,
    ),
  };
}

async function loadItems(options: {
  page: number;
  itemsPerPage: number;
  sortBy: unknown;
}) {
  if (isProxy(options.sortBy)) {
    const sort = toRaw(options.sortBy) as { key: string; order: string }[];
    if (sort.length > 0) {
      commissionDebtStore.writingAgentDetails.orderBy = sort[0].key;
      commissionDebtStore.writingAgentDetails.sortDirection = sort[0].order;
    } else {
      commissionDebtStore.writingAgentDetails.orderBy = "";
      commissionDebtStore.writingAgentDetails.sortDirection = "";
    }
  }

  if (options.page > 0) {
    commissionDebtStore.writingAgentDetails.page = options.page;
  }

  if (options.itemsPerPage > 0) {
    commissionDebtStore.writingAgentDetails.pageSize = options.itemsPerPage;
  }
}

function rowProps(item: { index: number }) {
  if (item && item.index !== undefined && item.index === 0) {
    return { class: "writing-agent-details-table__value--bold" };
  }
}
</script>

<style lang="pcss" scoped>
@import "../styles/commissions.pcss";
</style>
<style lang="pcss">
.writing-agent-details-table {
  font-size: 1rem;
  color: var(--text-grey);
}
.writing-agent-details-table .v-data-table-header__content {
  font-size: 1rem;
  font-weight: 600;
  color: var(--text-grey);
}
.writing-agent-details-table .v-data-table-header__sort-icon {
  opacity: 1 !important;
  font-size: 1rem;
}

.writing-agent-details-table__value--bold {
  font-weight: 600;
}

.server-table-mobile thead {
  display: none;
}

.writing-agent-details-table.server-table-mobile {
  margin-top: calc(var(--spacing-l) * -1);
  padding-top: var(--spacing-xs);
}
.data-table__table-heading--right {
  text-align: right;
}
.writing-agent-details-table .mobile-details-table__custom-row {
  display: flex;
  flex-direction: column;
  padding: var(--spacing) 0 0 0;
  background-color: var(--white);
  position: relative;
  .custom-cell {
    display: flex;
    flex-direction: column;
    padding: 0 var(--spacing-l);
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
  .writing-agent-details {
    display: grid;
    grid-template-columns: repeat(2, 1fr);
    grid-gap: 10px;
    padding-bottom: var(--spacing-s);
  }
  &.mobile-details-table__custom-row--totals {
    background-color: var(--bg-grey);
    padding-top: 0;
    .custom-cell {
      background-color: var(--bg-grey);
    }
    .totals-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      .v-card__title-icon {
        padding-right: var(--spacing-l);
      }
    }
    .writing-agent-details {
      padding-top: var(--spacing-l);
    }
  }
  .v-card__title-text__title {
    padding-left: var(--spacing-l);
    color: var(--primary-color);
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
.writing-agent-details-table.v-table--density-default
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

.writing-agent-details-table.v-table--density-default
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
