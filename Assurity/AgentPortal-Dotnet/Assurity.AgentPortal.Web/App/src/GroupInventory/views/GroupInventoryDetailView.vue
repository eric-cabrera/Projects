<template>
  <div class="view__filters">
    <aFilterDrawer
      ref="groupInventoryFiltersDrawer"
      v-model:is-filter-tray-open="filterDrawerOpen"
      v-model:filters="filters"
      :date-compare="false"
      :is-loading="groupDetailData.isLoading.value"
      @created-date-change="issueDateChange"
      @filter-change="filterChange"
    />
  </div>
  <div class="view__header group-inventory">
    <div class="view__heading">
      <h1 class="text-no-wrap view__title" @click="backToGroupSummary">
        <AssurityIconChevronLeft class="back-chevron" />
        Group Details
      </h1>
      <aHorizontalRule />
      <h2 class="tab__title">{{ groupName }}</h2>
    </div>

    <div class="view__header-row">
      <div class="view__header-left">
        <div><strong>Group Number: </strong>{{ groupNumber }}</div>
        <div>
          <strong>Status: </strong
          >{{ groupDetailData.data.value?.group?.status }}
        </div>
        <div>
          <strong>Effective Date: </strong
          >{{ groupDetailData.data.value?.group?.effectiveDate }}
        </div>
      </div>
      <div class="view__header-right">
        <div class="active-policies">
          <AssurityIconFlag class="active-policies__flag" />
          <span>{{
            groupDetailData.data.value?.group?.activePolicyCount || 0
          }}</span>
          <strong>Active Policies</strong>
        </div>
      </div>
    </div>

    <!-- Mobile Layout -->
    <div class="view__header-row__mobile">
      <div class="view__header-left">
        <div class="detail_group">
          <div>Group Number</div>
          <strong>{{ groupNumber }}</strong>
        </div>
        <br />
        <div class="detail_group">
          <div>Status</div>
          <strong>{{ groupDetailData.data.value?.group?.status }}</strong>
        </div>
      </div>
      <div class="view__header-right">
        <div class="detail_group">
          <div>Effective Date</div>
          <strong>{{
            groupDetailData.data.value?.group?.effectiveDate
          }}</strong>
        </div>
        <br />
        <div class="active-policies">
          <AssurityIconFlag class="active-policies__flag" />
          <span>{{
            groupDetailData.data.value?.group?.activePolicyCount || 0
          }}</span>
          <strong>Active Policies</strong>
        </div>
      </div>
    </div>
  </div>

  <div class="group-inventory-header">
    <div class="group-inventory-header__filters">
      <aButton
        button-style="primary"
        :text="`FILTER ${selectedFilterCount ? `(${selectedFilterCount})` : ''}`"
        size="s"
        class="group-inventory-header__filter-button"
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

  <ContainerBlock dense class="group-inventory-details__table-container">
    <aTableDesktop
      :headers="groupInventoryDetailTableHeaders"
      :items="groupInventoryDetailRows"
      :show-row-number="false"
      :loading="groupDetailData.isLoading.value"
      class="group-inventory-details__table"
      sorting="server"
      pagination="server"
      :current-page="pageingOptions.page"
      :total-items="totalItems"
      :get-expanded-component="getExpandedComponent"
      @update:sort-column="updateSorting"
      @update:current-page="updatePaging"
      @update:items-per-page="updateItemsPerPage"
    />
    <aTableMobile
      :headers="groupInventoryDetailTableHeaders"
      :items="groupInventoryDetailRowsMobile"
      :show-row-number="false"
      :loading="groupDetailData.isLoading.value"
      class="group-inventory-details__table"
      sorting="server"
      pagination="server"
      :current-page="pageingOptions.page"
      :total-items="totalItems"
      :get-expanded-component="getExpandedComponent"
      @update:sort-column="updateSorting"
      @update:current-page="updatePaging"
      @update:items-per-page="updateItemsPerPage"
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
import { useRoute, useRouter } from "vue-router";
import axios from "axios";
import type {
  $OpenApiTs,
  Assurity_AgentPortal_Contracts_GroupInventory_Response_Policy as ResponsePolicy,
  Assurity_AgentPortal_Contracts_Enums_SortDirection as SortDirection,
  Assurity_AgentPortal_Contracts_GroupInventory_Response_GroupDetailFilters as GroupDetailFilters,
} from "@assurity/newassurelink-client";
import aHorizontalRule from "@/components/aHorizontalRule.vue";
import AssurityIconFlag from "@/components/icons/AssurityIconFlag.vue";
import ContainerBlock from "@/components/content/ContainerBlock.vue";
import aTableDesktop from "@/components/aTable/desktop/aTableDesktop.vue";
import aTableMobile from "@/components/aTable/mobile/aTableMobile.vue";
import {
  SortOptions,
  type Cell,
  type Row,
} from "@/components/aTable/definitions";
import dayjs from "dayjs";
import { FileHelper } from "@/Shared/utils/FileHelper";
import { useGroupDetailData } from "../api/groupInventoryApi";
import aButton from "@/components/forms/aButton.vue";
import AssurityIconDownload from "@/components/icons/AssurityIconDownload.vue";
import AssurityIconChevronLeft from "@/components/icons/AssurityIconChevronLeft.vue";
import GroupInventoryBenefits from "./GroupInventoryBenefits.vue";
import GroupInventoryBenefitsMobile from "./GroupInventoryBenefitsMobile.vue";
import type { aFilterData } from "@/components/aFilterDrawer/models/aFilterData";
import { GroupInventoryDetailFilterData } from "../models/GroupInventoryDetailFilterData";
import { aFilterType } from "@/components/aFilterDrawer/models/enums/aFilterType";
import AssurityIconFilter from "@/components/icons/AssurityIconFilter.vue";
import aFilterDrawer from "@/components/aFilterDrawer/aFilterDrawer.vue";
import { aFilterField } from "@/components/aFilterDrawer/models/enums/aFilterField";
import GroupInventoryFilterHelpers from "@/GroupInventory/utils/groupInventoryFilterHelpers";
import type { GroupInventoryDetailFilterOptions } from "../models/GroupInventoryDetailFilterOptions";
import aSelectedFilters from "@/components/aFilterDrawer/aSelectedFilters.vue";

onMounted(() => {
  resetFilters();
});

const getExpandedComponent = (componentName: string) => {
  switch (componentName) {
    case "GroupInventoryBenefits":
      return GroupInventoryBenefits;
    case "GroupInventoryBenefitsMobile":
      return GroupInventoryBenefitsMobile;
    default:
      return null;
  }
};

const isDownloading = ref(false);
const route = useRoute();
const router = useRouter();
const groupNumber = computed(() => route.params.id as string);
const groupName = computed(() => groupDetailData.data.value?.group?.name);

const filters = ref<aFilterData[]>([]);
const filterModel = new GroupInventoryDetailFilterData();
filters.value = filterModel.groupInventoryDetailsFilters;

const filterDrawerOpen = ref(false);

const issueDateChange = (filters: aFilterData[]) => {
  filters
    .filter(
      (filter) =>
        filter.type !== aFilterType.CreatedDates && filter.selection?.length,
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

  const issueDateFilter = filters.find(
    (filter) => filter.type === aFilterType.CreatedDates,
  );
  if (
    issueDateFilter == null ||
    issueDateFilter == undefined ||
    issueDateFilter.createdDateBegin == undefined ||
    issueDateFilter.createdDateEnd == undefined ||
    issueDateFilter.createdDateBegin?.toString() == "Invalid Date" ||
    issueDateFilter.createdDateEnd?.toString() == "Invalid Date"
  ) {
    filterOptions.value.issueStartDate = "";
    filterOptions.value.issueEndDate = "";
  } else {
    filterOptions.value.issueStartDate = dayjs(
      issueDateFilter.createdDateBegin,
    ).format("MM/DD/YYYY");
    filterOptions.value.issueEndDate = dayjs(
      issueDateFilter.createdDateEnd,
    ).format("MM/DD/YYYY");
  }
};

const filterChange = (filters: aFilterData[]) => {
  const selections = filters.map((filter: aFilterData) => {
    if (filter.field === aFilterField.ViewAsAgent) {
      if (filter.selection && filter.selection?.length > 0) {
        filter.selection[0].id = filter.selection.at(0)?.id as string;
      }
    }
    return { field: filter.field, selection: filter.selection };
  });

  const newFilterOptions = GroupInventoryFilterHelpers.mapDetailSelections(
    selections.filter((item) => item.selection !== null),
  );

  filterOptions.value = {
    ...filterOptions.value,
    ...newFilterOptions,
  };
};

const filterOptions = ref<GroupInventoryDetailFilterOptions>({
  groupNumber: groupNumber.value,
});

const backToGroupSummary = async () => {
  router.back();
};

const canDownload = computed(() => {
  return (
    !groupDetailData.isLoading.value &&
    groupInventoryDetailRows.value.length > 0
  );
});

async function callDownloadDocument() {
  isDownloading.value = true;

  const params: $OpenApiTs["/API/GroupInventory/GroupDetails/Export"]["get"]["req"] =
    {
      groupNumber: groupNumber.value,
      issueDateEndDate: queryParams.value.issueEndDate,
      issueDateStartDate: queryParams.value.issueStartDate,
      orderBy: queryParams.value.orderBy,
      policyNumber: queryParams.value.policyNumber,
      policyOwnerName: queryParams.value.policyOwnerName,
      policyStatus: queryParams.value.policyStatus,
      productDescription: queryParams.value.productDescription,
      sortDirection: queryParams.value.sortDirection,
    };
  await callWithAxios(
    params,
    "/API/GroupInventory/GroupDetails/Export",
    "Group_Details",
  );

  isDownloading.value = false;
}

async function callWithAxios(
  params: $OpenApiTs["/API/GroupInventory/GroupDetails/Export"]["get"]["req"],
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

const sortingOptions = ref({
  orderBy: "PrimaryOwner",
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

const queryParams = computed(() => ({
  ...filterOptions.value,
  ...sortingOptions.value,
  ...pageingOptions.value,
}));

const groupDetailData = useGroupDetailData(queryParams);
const totalItems = ref(0);
const groupInventoryDetailRows = ref<Row[]>([]);
const groupInventoryDetailRowsMobile = ref<Row[]>([]);

const groupInventoryFiltersDrawer = ref<typeof aFilterDrawer | null>();

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

const processGroupDetailData = (
  policies: ResponsePolicy[] | null | undefined,
) => {
  const newRows: Row[] = [];
  policies?.forEach((policy) => {
    const mainRow: Cell[] = [
      {
        key: "primary-owner",
        text: policy.primaryOwner ?? "",
      },
      { key: "policy-number", text: policy.number ?? "" },
      {
        key: "policy-status",
        text: policy.status ?? "",
        isSortable: true,
        sortDirection: SortOptions.none,
        sortKey: "GroupStatus",
      },
      {
        key: "issue-date",
        text: policy?.issueDate ? policy?.issueDate : "",
        isSortable: true,
        sortDirection: SortOptions.none,
        sortKey: "IssueDate",
      },
      {
        key: "paid-to-date",
        text: policy?.paidToDate ? policy?.paidToDate : "",
        isSortable: true,
        sortDirection: SortOptions.none,
        sortKey: "PaidToDate",
      },
      {
        key: "annual-premium",
        text: policy.annualPremium ? policy.annualPremium : "",
        align: "right",
      },
      {
        key: "mode-premium",
        text: policy.modePremium ? policy.modePremium : "",
        align: "right",
      },
      { key: "mode", text: policy.mode ?? "" },
      { key: "product-description", text: policy.productDescription ?? "" },
      {
        key: "coverage-type",
        text: policy.coverageType ?? "",
      },
      { key: "details", details: getDetailBenefits(policy) },
    ];
    newRows.push(mainRow);
  });

  groupInventoryDetailRows.value = newRows;
};

const processGroupDetailDataMobile = (
  policies: ResponsePolicy[] | null | undefined,
) => {
  const newRows: Row[] = [];
  policies?.forEach((policy) => {
    const mainRow: Cell[] = [
      {
        key: "primary-owner",
        text: policy.primaryOwner ?? "",
      },
      { key: "policy-number", text: policy.number ?? "" },
      {
        key: "policy-status",
        text: policy.status ?? "",
        isSortable: true,
        sortDirection: SortOptions.none,
        sortKey: "GroupStatus",
      },
      {
        key: "issue-date",
        text: policy?.issueDate ? policy?.issueDate : "",
        isSortable: true,
        sortDirection: SortOptions.none,
        sortKey: "IssueDate",
      },
      {
        key: "paid-to-date",
        text: policy?.paidToDate ? policy?.paidToDate : "",
        isSortable: true,
        sortDirection: SortOptions.none,
        sortKey: "PaidToDate",
      },
      {
        key: "annual-premium",
        text: policy.annualPremium ? policy.annualPremium : "",
        align: "right",
      },
      {
        key: "mode-premium",
        text: policy.modePremium ? policy.modePremium : "",
        align: "right",
      },
      { key: "mode", text: policy.mode ?? "" },
      { key: "product-description", text: policy.productDescription ?? "" },
      {
        key: "coverage-type",
        text: policy.coverageType ?? "",
      },
      { key: "details", details: getDetailBenefitsMobile(policy) },
    ];
    newRows.push(mainRow);
  });

  groupInventoryDetailRowsMobile.value = newRows;
};

const getDetailBenefits = (policy: ResponsePolicy) => {
  const newRows: Row[] = [];
  const mainRow: Cell[] = [
    {
      key: "column-1",
      componentName: "GroupInventoryBenefits",
      componentProps: {
        policy: policy,
      },
      colspan: 10,
    },
  ];
  newRows.push(mainRow);
  return newRows;
};

const getDetailBenefitsMobile = (policy: ResponsePolicy) => {
  const newRows: Row[] = [];
  const mainRow: Cell[] = [
    {
      key: "column-1",
      componentName: "GroupInventoryBenefitsMobile",
      componentProps: {
        policy: policy,
      },
      colspan: 10,
    },
  ];
  newRows.push(mainRow);
  return newRows;
};

const groupInventoryDetailTableHeaders = ref<Cell[]>([]);
groupInventoryDetailTableHeaders.value = [
  {
    key: "primary-owner",
    text: "Primary Owner",
    className: "group-inventory-details-table__header--wideColumn",
    isSortable: true,
    sortDirection: SortOptions.none,
    sortKey: "PolicyOwner",
  },
  { key: "policy-number", text: "Policy Number" },
  {
    key: "policy-status",
    text: "Policy Status",
    className: "group-inventory-details-table__header__PolicyColumn",
    isSortable: true,
    sortDirection: SortOptions.none,
    sortKey: "PolicyStatus",
  },
  {
    key: "issue-date",
    text: "Issue Date",
    isSortable: true,
    sortDirection: SortOptions.none,
    sortKey: "IssueDate",
  },
  {
    key: "paid-to-date",
    text: "Paid to Date",
    isSortable: true,
    sortDirection: SortOptions.none,
    sortKey: "PaidToDate",
  },
  {
    key: "annual-premium",
    text: "Annual Premium",
    className: "wrap-group-header-text",
  },
  {
    key: "mode-premium",
    text: "Mode Premium",
    className: "wrap-group-header-text",
  },
  { key: "mode", text: "Mode" },
  { key: "product-description", text: "Product Description" },
  { key: "coverage-type", text: "Coverage Type" },
  {
    details: [{ key: "details" }],
  },
];

const processFilterItems = (filterData: GroupDetailFilters | undefined) => {
  filters.value =
    GroupInventoryFilterHelpers.getDetailFilterItems(
      filters.value,
      filterOptions.value,
      filterData,
    ) ?? [];

  processFilters = false;
};

let processFilters = true;

watch(groupDetailData.data, (data) => {
  if (data) {
    processGroupDetailData(data.policies);
    processGroupDetailDataMobile(data.policies);

    if (processFilters) {
      processFilterItems(data.filters);
    }

    totalItems.value =
      data.filters?.totalPageCount && data.filters?.pageSize
        ? parseInt(data.filters?.totalPageCount) *
          parseInt(data.filters?.pageSize)
        : 0;
  }
});
</script>

<style lang="pcss" scoped>
@import "@/Shared/styles/status-summary.pcss";
.tab__title {
  font-size: 1.2em;
  font-weight: bold;
  margin: 1em 0 0;
  text-align: center;

  @media (width >= 960px) {
    margin: 0;
    font-size: 33px;
    color: var(--primary-color);
    text-align: left;
  }
}

.group-inventory-header {
  display: block;

  @media (width >= 960px) {
    display: flex;
    justify-content: space-between;
  }

  &__filters {
    display: flex;
    gap: 1em;
    flex-wrap: wrap;
  }

  &__filter-button {
    width: 100%;
    margin-top: 2em;

    @media (width >= 960px) {
      margin-top: 0;
      width: 200px;
    }
  }
}

.group-inventory__header-selected-filters {
  display: flex;
  flex-wrap: wrap;
  gap: 1em;
  align-items: center;
}

.group-inventory-header-download {
  display: none;

  @media (width >= 960px) {
    display: flex;
    justify-content: flex-end;
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

:deep(.group-inventory-details-table__header--wideColumn) {
  min-width: 13em;
}

:deep(.group-inventory-details-table__header__PolicyColumn) {
  min-width: 90px;

  & .table-desktop__header-text {
    text-wrap: auto;
  }
}

.view__title {
  cursor: pointer;
  justify-content: flex-end;
  display: flex;
}

.view__header {
  display: flex;
  flex-direction: column;

  & hr {
    @media (width >= 960px) {
      display: none;
    }
  }
}

.view__heading {
  display: flex;
  flex-direction: column;
  align-items: flex-start;

  & h1 svg {
    margin-left: -11px;

    @media (width >= 960px) {
      margin-left: -24px;
    }
  }
}

.detail_group > div:first-of-type {
  font-size: 14px;
}

.view__header-row {
  display: none;
  justify-content: space-between;
  margin-bottom: 1rem;

  @media (width >= 960px) {
    display: flex;
  }

  &__mobile {
    display: grid;
    grid-template-columns: 1fr 1fr;
    grid-gap: 1rem;
    margin-top: 1rem;

    @media (width >= 960px) {
      display: none;
    }
  }
}

.view__header-left {
  flex: 1;
}

.view__header-right {
  flex-shrink: 0;
  text-align: left;

  @media (width >= 960px) {
    text-align: right;
  }
}

.active-policies {
  font-size: 1rem;
  font-weight: bold;
  color: var(--text-grey-dark);
  padding-top: 21px;

  @media (width >= 960px) {
    padding-top: 0;
    top: -48px;
    position: relative;
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

.back-chevron {
  width: 35px;

  @media (width >= 960px) {
    width: 75px;
  }
}

.group-inventory .custom-select-wrapper {
  min-height: 52px;
  width: 235px;
}

:deep(.table-desktop__cell) {
  &:not(:first-child) {
    padding-left: 0;
  }

  &:first-child .table-desktop__text {
    color: var(--text-grey);
    font-weight: bold;
  }
}
</style>

<style lang="pcss">
.group-inventory {
  color: var(--text-grey);
  margin-bottom: 36px;
}

.group-inventory-details__table-container {
  padding-top: 1em;

  @media (width >= 960px) {
    & .wrap-group-header-text {
      text-align: right;

      & span {
        text-wrap: auto;
      }
    }
  }
}

div.table-desktop__detail-table {
  min-width: 75vw;
}
</style>
