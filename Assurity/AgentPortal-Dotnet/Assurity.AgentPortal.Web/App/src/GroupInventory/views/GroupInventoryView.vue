<template>
  <div class="view__filters">
    <aFilterDrawer
      ref="groupInventoryFiltersDrawer"
      v-model:is-filter-tray-open="filterDrawerOpen"
      v-model:filters="filters"
      :date-compare="false"
      :is-loading="groupSummaryData.isLoading.value"
      @effective-date-change="effectiveDateChange"
      @filter-change="filterChange"
    />
  </div>
  <div class="view__header group-inventory">
    <div class="view__header-left">
      <div class="view__heading">
        <h1 class="text-no-wrap view__title">Group Inventory</h1>
      </div>
    </div>
    <div class="view__header-right">
      <div class="active-policies">
        <AssurityIconFlag class="active-policies__flag" />
        <span>{{ groupSummaryData.data.value?.totalSummaries || 0 }}</span>
        <strong>Groups Enrolled</strong>
      </div>
    </div>
  </div>
  <div class="group-inventory-header">
    <div class="group-inventory-header__filters">
      <div>
        <aDropdownV2
          id="agentId"
          class="group-inventory-header__filters__view-as"
          label="View As"
          bg-color="white"
          :value="viewAsAgentId"
          :options="viewAsAgentValues"
          @update:model-value="viewAsChange"
        />
      </div>

      <aButton
        button-style="primary"
        :text="`FILTER ${selectedFilterCount ? `(${selectedFilterCount})` : ''}`"
        size="s"
        class="active-details__filter-button--desktop"
        @click="filterDrawerOpen = !filterDrawerOpen"
      >
        <template #prepend>
          <AssurityIconFilter class="filters__filter-icon" color="white" />
        </template>
      </aButton>
      <div
        v-if="selectedFilters?.length"
        class="group-inventory__header-selected-filters"
      >
        <aSelectedFilters
          :selected-filters="selectedFilters"
          :selected-filter-count="selectedFilterCount"
          @reset-filters="resetFilters"
          @filter-value-changed="onRemoveSelection"
        />
      </div>
    </div>
    <div class="group-inventory-header-download">
      <aButton
        v-if="canDownload"
        button-style="primary"
        text="DOWNLOAD XLSX"
        size="s"
        class="align-bottom"
        @click="callDownloadDocument()"
        @keyup.enter="callDownloadDocument()"
        @keyup.space="callDownloadDocument()"
        @keypress="$event.preventDefault()"
      >
        <template #prepend>
          <v-progress-circular v-if="isDownloading" indeterminate />
          <AssurityIconDownload v-else color="white" />
        </template>
      </aButton>
    </div>
  </div>
  <ContainerBlock dense class="group-inventory__table-container">
    <aTableDesktop
      :headers="groupInventoryTableHeaders"
      :items="groupInventoryRows"
      :show-row-number="false"
      :loading="groupSummaryData.isLoading.value"
      class="group-inventory__table"
      sorting="server"
      pagination="server"
      :current-page="pageingOptions.page"
      :total-items="totalItems"
      @update:sort-column="updateSorting"
      @update:current-page="updatePaging"
      @update:items-per-page="updateItemsPerPage"
      @link-click="
        (key, value) => {
          onActionClick(value);
        }
      "
    />
    <aTableMobile
      :headers="groupInventoryTableHeaders"
      :items="groupInventoryRowsMobile"
      :show-row-number="false"
      :loading="groupSummaryData.isLoading.value"
      class="group-inventory__table"
      sorting="server"
      pagination="server"
      :hide-first-label="true"
      :current-page="pageingOptions.page"
      :total-items="totalItems"
      :get-expanded-component="getExpandedComponent"
      @update:sort-column="updateSorting"
      @update:current-page="updatePaging"
      @update:items-per-page="updateItemsPerPage"
      @link-click="
        (key, value) => {
          onActionClick(value);
        }
      "
    />
    <div class="group-inventory-mobile-download">
      <aButton
        v-if="canDownload"
        button-style="primary"
        text="DOWNLOAD XLSX"
        size="s"
        class="align-bottom"
        @click="callDownloadDocument()"
        @keyup.enter="callDownloadDocument()"
        @keyup.space="callDownloadDocument()"
        @keypress="$event.preventDefault()"
      >
        <template #prepend>
          <v-progress-circular v-if="isDownloading" indeterminate />
          <AssurityIconDownload v-else color="white" />
        </template>
      </aButton>
    </div>
  </ContainerBlock>
</template>

<script setup lang="ts">
import { computed, onMounted, ref, watch } from "vue";
import aDropdownV2 from "@/components/forms/aDropdownV2.vue";
import { useGroupSummaryData } from "../api/groupInventoryApi";
import type {
  $OpenApiTs,
  Assurity_AgentPortal_Contracts_GroupInventory_Response_GroupSummary as GroupSummary,
  Assurity_AgentPortal_Contracts_Enums_SortDirection as SortDirection,
} from "@assurity/newassurelink-client";
import type { GroupInventoryFilterOptions } from "../models/GroupInventoryFilterOptions";
import ContainerBlock from "@/components/content/ContainerBlock.vue";
import aTableDesktop from "@/components/aTable/desktop/aTableDesktop.vue";
import aTableMobile from "@/components/aTable/mobile/aTableMobile.vue";
import {
  SortOptions,
  type Cell,
  type Row,
} from "@/components/aTable/definitions";
import dayjs from "dayjs";
import { useUserStore } from "@/stores/userStore";
import AssurityIconDownload from "@/components/icons/AssurityIconDownload.vue";
import axios from "axios";
import { useRouter } from "vue-router";
import { FileHelper } from "@/Shared/utils/FileHelper";
import type { aFilterData } from "@/components/aFilterDrawer/models/aFilterData";
import { aFilterType } from "@/components/aFilterDrawer/models/enums/aFilterType";
import { aFilterField } from "@/components/aFilterDrawer/models/enums/aFilterField";
import GroupInventoryFilterHelpers from "@/GroupInventory/utils/groupInventoryFilterHelpers";
import { GroupInventoryFilterData } from "../models/GroupInventoryFilterData";
import aButton from "@/components/forms/aButton.vue";
import AssurityIconFilter from "@/components/icons/AssurityIconFilter.vue";
import AssurityIconFlag from "@/components/icons/AssurityIconFlag.vue";
import aFilterDrawer from "@/components/aFilterDrawer/aFilterDrawer.vue";
import type { Assurity_AgentPortal_Contracts_GroupInventory_Response_GroupSummaryFilters as GroupSummaryFilters } from "@assurity/newassurelink-client";
import GroupInventoryMobileHeader from "./GroupInventoryMobileHeader.vue";
import aSelectedFilters from "@/components/aFilterDrawer/aSelectedFilters.vue";

const getExpandedComponent = (componentName: string) => {
  switch (componentName) {
    case "GroupInventoryMobileHeader":
      return GroupInventoryMobileHeader;
    default:
      return null;
  }
};

const isDownloading = ref(false);
const router = useRouter();

onMounted(() => {
  resetFilters();
});

const canDownload = computed(() => {
  return (
    !groupSummaryData.isLoading.value && groupInventoryRows.value.length > 0
  );
});

async function callDownloadDocument() {
  isDownloading.value = true;
  const params: $OpenApiTs["/API/GroupInventory/GroupSummary/Export"]["get"]["req"] =
    {
      groupName: filterOptions.value.groupName,
      groupNumber: filterOptions.value.groupNumber,
      groupStatus: filterOptions.value.groupStatus,
      orderBy: sortingOptions.value.orderBy,
      sortDirection: sortingOptions.value.sortDirection,
      viewAsAgentId: filterOptions.value.viewAsAgentId,
      groupEffectiveEndDate: filterOptions.value.groupEffectiveEndDate,
      groupEffectiveStartDate: filterOptions.value.groupEffectiveStartDate,
    };
  await callWithAxios(
    params,
    "/API/GroupInventory/GroupSummary/Export",
    "Group_Summary",
  );

  isDownloading.value = false;
}

async function callWithAxios(
  params: $OpenApiTs["/API/GroupInventory/GroupSummary/Export"]["get"]["req"],
  url: string,
  fileName: string,
) {
  try {
    const response = await axios.get(url, {
      params,
      responseType: "blob",
    });
    const file = new FileHelper(
      fileName,
      response.data,
      response.headers["content-disposition"],
    );
    file.downloadFile();
  } catch (error) {
    console.error(error);
    isDownloading.value = false;
  }
}

const userStore = useUserStore();

class ActionData {
  groupNumber?: string;
}
const onActionClick = async (value: object) => {
  const data = value as ActionData;

  router.push({
    path: `/group-inventory/${data.groupNumber}`,
  });
};

const filterOptions = ref<GroupInventoryFilterOptions>({});

const viewAsAgentId = ref("");
const viewAsAgents = ref<string[]>([]);

const sortingOptions = ref({
  orderBy: "GroupEffectiveDate",
  sortDirection: "DESC" as SortDirection,
});

const pageingOptions = ref({
  page: 1,
  pageSize: 10,
});

const updateSorting = (newSort: {
  key: string;
  sortKey: string;
  sortDirection: string;
}) => {
  sortingOptions.value.orderBy = newSort.sortKey;
  sortingOptions.value.sortDirection = (
    newSort.sortDirection as string
  ).toUpperCase() as SortDirection;
};

const updatePaging = (newPage: number): void => {
  pageingOptions.value.page = newPage;
};

const updateItemsPerPage = (newItemsPerPage: number): void => {
  pageingOptions.value.pageSize = newItemsPerPage;
};

const viewAsChange = (value: string | number | undefined) => {
  filterOptions.value.viewAsAgentId = value?.toString() ?? "";
};
viewAsAgentId.value = userStore.user.agentId ?? "";
viewAsChange(viewAsAgentId.value);

const effectiveDateChange = (filters: aFilterData[]) => {
  filters
    .filter(
      (filter) =>
        filter.type !== aFilterType.EffectiveDates && filter.selection?.length,
    )
    .forEach(({ field, selection }) => {
      selection?.forEach(({ id }) => {
        if (groupInventoryFiltersDrawer.value) {
          groupInventoryFiltersDrawer.value.onRemoveSelectionEmitted({
            filterName: field,
            itemId: id,
          });
        }
      });
    });

  const effectiveDateFilter = filters.find(
    (filter) => filter.type === aFilterType.EffectiveDates,
  );

  if (
    effectiveDateFilter == null ||
    effectiveDateFilter == undefined ||
    effectiveDateFilter.effectiveDateStart == undefined ||
    effectiveDateFilter.effectiveDateEnd == undefined ||
    effectiveDateFilter.effectiveDateStart?.toString() == "Invalid Date" ||
    effectiveDateFilter.effectiveDateEnd?.toString() == "Invalid Date"
  ) {
    filterOptions.value.groupEffectiveStartDate = "";
    filterOptions.value.groupEffectiveEndDate = "";
  } else {
    filterOptions.value.groupEffectiveStartDate = dayjs(
      effectiveDateFilter.effectiveDateStart,
    ).format("MM/DD/YYYY");
    filterOptions.value.groupEffectiveEndDate = dayjs(
      effectiveDateFilter.effectiveDateEnd,
    ).format("MM/DD/YYYY");
  }

  processFilters = true;
};

const queryParams = computed(() => ({
  ...filterOptions.value,
  ...sortingOptions.value,
  ...pageingOptions.value,
}));

const groupSummaryData = useGroupSummaryData(queryParams);

const selectedFilters = computed(() => {
  if (groupInventoryFiltersDrawer.value) {
    return groupInventoryFiltersDrawer.value.selectedFilters || [];
  }
  return [];
});

const selectedFilterCount = computed(() => {
  if (groupInventoryFiltersDrawer.value) {
    return groupInventoryFiltersDrawer.value.selectedFilterCount;
  }
  return 0;
});
function onRemoveSelection(value: {
  filterName: string;
  itemId: string | null | undefined;
}) {
  if (groupInventoryFiltersDrawer.value) {
    groupInventoryFiltersDrawer.value.onRemoveSelectionEmitted(value);
  }
  return [];
}

function resetFilters() {
  if (groupInventoryFiltersDrawer.value) {
    groupInventoryFiltersDrawer.value.resetFilters();
  }
  return [];
}

const totalItems = ref(0);
const groupInventoryRows = ref<Row[]>([]);
const groupInventoryRowsMobile = ref<Row[]>([]);
const processGroupSummaryData = (groups: GroupSummary[] | null | undefined) => {
  const newRows: Row[] = [];
  groups?.forEach((group) => {
    const mainRow: Cell[] = [
      {
        key: "group-name",
        text: group.name ?? "",
        onClickReturnValue: {
          groupNumber: group.number,
        },
        buttonStyle: "plain",
        isSortable: true,
        sortDirection: SortOptions.none,
        sortKey: "GroupName",
        className: "group-name-table-button",
      },
      { key: "group-number", text: group.number ?? "" },
      {
        key: "status",
        text: group.status ?? "",
        isSortable: true,
        sortDirection: SortOptions.none,
        sortKey: "GroupStatus",
      },
      {
        key: "policy-count",
        text: group.policyCount ?? "",
        isSortable: true,
        sortDirection: SortOptions.none,
        sortKey: "PolicyCount",
      },
      {
        key: "group-effective-date",
        text: dayjs(group?.groupEffectiveDate).format("MM/DD/YYYY"),
        isSortable: true,
        sortDirection: SortOptions.none,
        sortKey: "GroupEffectiveDate",
      },
      {
        key: "action",
        text: "GROUP DETAILS",
        onClickReturnValue: {
          groupNumber: group.number,
        },
        buttonStyle: "primary",
        buttonSize: "s",
      },
    ];
    newRows.push(mainRow);
  });

  groupInventoryRows.value = newRows;
};

const processGroupSummaryDataMobile = (
  groups: GroupSummary[] | null | undefined,
) => {
  const newRows: Row[] = [];
  groups?.forEach((group) => {
    const mainRow: Cell[] = [
      {
        key: "group-name",
        componentName: "GroupInventoryMobileHeader",
        isSortable: true,
        sortDirection: SortOptions.none,
        sortKey: "GroupName",
        componentProps: {
          groupNumber: group.number,
          groupName: group.name,
        },
      },
      { key: "group-number", text: group.number ?? "" },
      { key: "empty-column", text: "" },
      {
        key: "group-effective-date",
        text: dayjs(group?.groupEffectiveDate).format("MM/DD/YYYY"),
        isSortable: true,
        sortDirection: SortOptions.none,
        sortKey: "GroupEffectiveDate",
      },
      { key: "empty-column", text: "" },
      {
        key: "status",
        text: group.status ?? "",
        isSortable: true,
        sortDirection: SortOptions.none,
        sortKey: "GroupStatus",
      },
      { key: "empty-column", text: "" },
      {
        key: "policy-count",
        text: group.policyCount ?? "",
        isSortable: true,
        sortDirection: SortOptions.none,
        sortKey: "PolicyCount",
      },
      {
        key: "action",
        text: "GROUP DETAILS",
        onClickReturnValue: {
          groupNumber: group.number,
        },
        buttonStyle: "primary",
        buttonSize: "s",
      },
    ];
    newRows.push(mainRow);
  });

  groupInventoryRowsMobile.value = newRows;
};
const filters = ref<aFilterData[]>([]);
const filterModel = new GroupInventoryFilterData();
filters.value = filterModel.groupInventoryFilters;

const groupInventoryFiltersDrawer = ref<typeof aFilterDrawer | null>();

const filterDrawerOpen = ref(false);

const filterChange = (filters: aFilterData[]) => {
  const selections = filters.map((filter: aFilterData) => {
    if (filter.field === aFilterField.ViewAsAgent) {
      if (filter.selection && filter.selection?.length > 0) {
        filter.selection[0].id = filter.selection.at(0)?.id as string;
      }
    }
    return { field: filter.field, selection: filter.selection };
  });

  const newFilterOptions = GroupInventoryFilterHelpers.mapSelections(
    selections.filter((item) => item.selection !== null),
  );

  filterOptions.value = {
    ...filterOptions.value,
    ...newFilterOptions,
  };
};

const groupInventoryTableHeaders = ref<Cell[]>([]);
groupInventoryTableHeaders.value = [
  {
    key: "group-name",
    text: "Group Name",
    className: "group-inventory-table__header--wideColumn",
    isSortable: true,
    sortDirection: SortOptions.none,
    sortKey: "GroupName",
  },
  { key: "group-number", text: "Group Number" },
  {
    key: "status",
    text: "Status",
    isSortable: true,
    sortDirection: SortOptions.none,
    sortKey: "GroupStatus",
  },
  {
    key: "policy-count",
    text: "Policy Count",
    isSortable: true,
    sortDirection: SortOptions.none,
    sortKey: "PolicyCount",
  },
  {
    key: "group-effective-date",
    text: "Effective Date",
    isSortable: true,
    sortDirection: SortOptions.none,
    sortKey: "GroupEffectiveDate",
  },
  {
    key: "action",
    text: "",
  },
];

const viewAsAgentValues = computed(() => {
  return viewAsAgents.value.map((agent) => {
    return {
      value: agent ?? "",
      label: agent ?? "",
    };
  });
});

const processViewAsData = (viewAsAgentsData: string[]) => {
  viewAsAgents.value = viewAsAgentsData;
};

const processFilterItems = (filterData: GroupSummaryFilters | undefined) => {
  filters.value =
    GroupInventoryFilterHelpers.getFilterItems(
      filters.value,
      filterOptions.value,
      filterData,
    ) ?? [];

  processFilters = false;
};

let processFilters = true;

watch(groupSummaryData.data, (data) => {
  if (data) {
    processGroupSummaryData(data?.groupSummaries);
    processGroupSummaryDataMobile(data?.groupSummaries);

    if (processFilters) {
      processFilterItems(data.filters);
    }

    if (data.filters) {
      processViewAsData(data?.filters?.viewAsAgents ?? []);
    }

    totalItems.value = data.totalSummaries ?? 0;
  }
});
</script>

<style lang="pcss" scoped>
@import "@/Shared/styles/status-summary.pcss";
@import "@/Shared/styles/filter-styles.pcss";
.group-inventory {
  color: var(--text-grey);
  &__table-container {
    padding-top: 1em;
  }
  &__header-selected-filters {
    display: flex;
    gap: var(--spacing);
    flex: 1 1 auto;
    flex-wrap: wrap;
    min-height: 2em;
    margin: auto;
  }
}

.group-inventory-header {
  display: flex;
  justify-content: space-between;
  &__filters {
    display: flex;
    gap: 1em;
    flex-wrap: wrap;
    &__view-as {
      .custom-select-wrapper {
        height: 52px;
        max-height: 52em;
        min-width: 140px;
      }
    }
  }
}

.group-inventory-header-download {
  justify-content: end;
  display: flex;
  @media (width <= 960px) {
    display: none;
  }
}

.group-inventory-mobile-download {
  display: none;
  @media (width <= 960px) {
    display: block;
    padding: 1em;
  }
}

.group-inventory-mobile-download button {
  @media (width <= 960px) {
    width: 100%;
  }
}

.mobile-data-table__cell-data--first span {
  font-size: 1.2em;
  font-weight: bold;
}

.view__heading h1 {
  @media (width < 960px) {
    margin-bottom: 0;
  }
}

.active-policies {
  font-size: 1rem;
  font-weight: bold;
  color: var(--text-grey-dark);
  padding-bottom: 16px;

  @media (width >= 960px) {
    padding-bottom: 0;
  }

  & span {
    font-size: 18px;
    padding-right: 0.25rem;

    @media (width >= 960px) {
      font-size: 1.75rem;
    }
  }

  &__flag {
    color: var(--primary-color);
    width: 18px;
    top: 2px;
    position: relative;

    @media (width >= 960px) {
      width: 36px;
      top: 6px;
    }
  }
}

.group-inventory .custom-select-wrapper {
  min-height: 52px;
  width: 235px;
}
</style>

<style lang="pcss">
.group-inventory {
  color: var(--text-grey);
  &__table-container {
    padding-top: 1em;
    padding-bottom: 2em;
  }
}

.group-name-table-button .table-desktop__button {
  padding-left: 0;
  justify-content: left;
  text-align: left;
  padding: 0;
}

.group-inventory__table-container
  .table-desktop__wrapper
  table
  > tbody
  > tr
  td.table-desktop__cell {
  padding-top: 8px;
  padding-bottom: 8px;
}
</style>
