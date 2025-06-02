<template>
  <DynamicContent>
    <template #header>
      <h1 class="heading--1">List Bill</h1>
      <div class="filter-info">
        <h3>Filter List Bills by Group Name or Number</h3>
        <AssurityIconInfo
          id="popoverModalConfig.uniqueId"
          class="info"
          @click="handleShowPopOver"
        ></AssurityIconInfo>
      </div>
      <PopoverModal
        v-if="isShowingPopover"
        class="list-bill"
        :unique-id="popoverModalConfig.uniqueId"
        :header-text="popoverModalConfig.headerText"
        :icon="popoverModalConfig.icon"
        :close-btn="popoverModalConfig.closeBtn"
      >
        <p>
          <strong>Subgroups</strong> are often used for large groups or groups
          that have multiple locations or billing modes. Groups that have
          subgroups can be located by searching for a set of shared characters
          at the end or beginning of the group number.
          <br />
          <br />
          <em>Example:</em> Main group number is 0800000<span class="highlight"
            >471</span
          >
          with 080000A<span class="highlight">471</span> and 080000B<span
            class="highlight"
            >471</span
          >
          subgroups.<br />
          Enter shared characters <span class="highlight">471</span> to pull up
          all subgroups.
          <br />
          <br />
          <em>Large Group Example:</em> Main group number is
          <span class="highlight">18CPS</span>00001 with
          <span class="highlight">18CPS</span>000A1 as subgroup. In addition,
          has different location subgroups such as
          <span class="highlight">18CPS</span>000139 and
          <span class="highlight">18CPS</span>000140.
          <br />
          Enter shared characters <span class="highlight">18CPS</span> to pull
          up all subgroups.
        </p>
      </PopoverModal>
      <div class="filters">
        <aSearch
          id="group-name"
          placeholder="Group Name"
          tool-tip-text="Group Name"
          class="filters__header-group-name"
          :clearable="true"
          :value="groupNameQuery"
          :options="groupSuggestions"
          :min-length="groupNameSearchMin"
          :search-query="groupNameQuery"
          :suggestions="groupSuggestions"
          @update:model-value="groupNameChange"
          @update:clear="() => clearGroupName()"
          @update:search-query:input="
            (newValue: string) => {
              groupNameQuery = newValue;
            }
          "
          @update:search-query:suggestion="
            (newValue: string) => {
              groupNameQuery = newValue;
            }
          "
          @update:search-query:clear="
            () => {
              groupNameQuery = '';
            }
          "
          @focus="
            () => {
              groupNumberQuery = '';
              listBillsQueryId = '';
            }
          "
        >
        </aSearch>
        <aSearch
          id="group-number"
          placeholder="Group Number"
          class="filters__header-group-number"
          :clearable="true"
          :value="groupNumberQuery"
          :options="groupNumberSuggestions"
          :min-length="groupNumberSearchMin"
          :search-query="groupNumberQuery"
          :suggestions="groupNumberSuggestions"
          @update:model-value="groupNumberChange"
          @update:clear="() => clearGroupNumber()"
          @update:search-query:input="
            (newValue: string) => {
              groupNumberQuery = newValue;
              if (newValue === '') {
                listBillsQueryId = '';
              }
            }
          "
          @update:search-query:suggestion="
            (newValue: string) => {
              groupNumberQuery = newValue;
              listBillsQueryId = newValue;
            }
          "
          @update:search-query:clear="
            () => {
              groupNumberQuery = '';
              listBillsQueryId = '';
            }
          "
          @focus="() => (groupNameQuery = '')"
        >
        </aSearch>
      </div>
      <aHorizontalRule class="horizontal-rule" />
    </template>

    <div class="col-span-8">
      <h2 class="heading--2 col-span-12">Group</h2>
      <ContainerBlock dense>
        <aTableMobile
          ref="mobileGroupsTable"
          class="col-span-12"
          pagination="client"
          :headers="groupsTableHeaders"
          :items="groupsView"
          :loading="groupsLoading"
          @click="
            (values: any[]) => {
              let groupId = values.find((value) => value.key == 'id');
              if (groupId) {
                groupNameQuery = '';
                groupSelected(groupId.value);
              }
            }
          "
        />
        <aTableDesktop
          ref="desktopGroupsTable"
          class="col-span-12"
          pagination="client"
          :headers="groupsTableHeaders"
          :items="groupsView"
          :loading="groupsLoading"
          @link-click="
            (key: string, rowSelectedValue: string) => {
              if (key == 'id') {
                groupNameQuery = '';
                groupSelected(rowSelectedValue);
              }
            }
          "
        />
      </ContainerBlock>
    </div>

    <div class="col-span-4 list-bills-table">
      <h2 class="heading--2 col-span-12">List Bill Results</h2>
      <ContainerBlock dense>
        <aTableMobile
          v-if="listBillsView.length"
          class="col-span-12 list-bill-results-table"
          :headers="listBillsHeaders"
          :items="listBillsView"
          :loading="listBillsLoading"
          @click="
            (values: any[]) => {
              downloadDocument(
                values.find((value) => value.key == 'document')?.value,
              );
            }
          "
        />
        <aTableDesktop
          v-if="listBillsView.length"
          class="col-span-12"
          :headers="listBillsHeaders"
          :items="listBillsView"
          :loading="listBillsLoading"
          @link-click="
            (key: string, listBillId: string) => {
              if (key == 'document') downloadDocument(listBillId);
            }
          "
        />
        <div v-if="isDownloadingListBill" class="download-message">
          <v-progress-circular
            class="progress-circle"
            color="primary"
            indeterminate
          />
          <h4>Downloading ListBill</h4>
        </div>
        <div
          v-else-if="listBillsView.length < 1"
          class="col-span-12 list-bill-placeholder"
        >
          <img
            src="/src/assets/Icon-Blue-Circle___Folder.svg"
            width="100px"
            height="100px"
          />
          <h3>Select a group to view List Bills</h3>
          <div class="underline"></div>
        </div>
      </ContainerBlock>
    </div>
  </DynamicContent>
</template>
<script setup lang="ts">
import { useUserStore } from "@/stores/userStore";
import { formatGroupsData, formatListBillData } from "../utils/tableUtils";
import { FileHelper } from "@/Shared/utils/FileHelper";
import type {
  $OpenApiTs,
  Assurity_AgentPortal_Contracts_ListBill_GroupsResponse as GroupsResponse,
  Assurity_AgentPortal_Contracts_ListBill_ListBillsResponse as ListBillsResponse,
} from "@assurity/newassurelink-client";
import { ListBillsService } from "@assurity/newassurelink-client";

import DynamicContent from "@/layouts/DynamicContent.vue";
import ContainerBlock from "@/components/content/ContainerBlock.vue";

import aHorizontalRule from "@/components/aHorizontalRule.vue";
import aSearch from "@/components/aSearch/aSearch.vue";
import aTableDesktop from "@/components/aTable/desktop/aTableDesktop.vue";
import aTableMobile from "@/components/aTable/mobile/aTableMobile.vue";
import type { Row } from "@/components/aTable/definitions";
import AssurityIconInfo from "@/components/icons/AssurityIconInfo.vue";
import PopoverSearchIcon from "@/components/icons/AssurityIconPopoverInfo.vue";
import PopoverModal from "@/components/PopoverModal.vue";

import { computed, nextTick, ref } from "vue";
import { useQuery } from "@tanstack/vue-query";
import axios from "axios";

const groupNameSearchMin = 3;
const groupNumberSearchMin = 7;
const userStore = useUserStore();
const desktopGroupsTable = ref(null);
const mobileGroupsTable = ref(null);

const popoverModalUUID = crypto.randomUUID();
const isShowingPopover = ref(false);
const popoverModalConfig = {
  uniqueId: "popoverModal" + popoverModalUUID,
  headerText: "Finding Subgroups",
  icon: PopoverSearchIcon,
  closeBtn: false,
};

function handleShowPopOver() {
  isShowingPopover.value = true;

  nextTick(() => {
    document.getElementById(popoverModalConfig.uniqueId)?.showPopover();
  });
}

const { data: groupsData, isLoading: groupsLoading } = useQuery<GroupsResponse>(
  {
    queryKey: [userStore.user.agentId],
    staleTime: 300000,
    queryFn: () => ListBillsService.getGroups(),
  },
);

const groupsView = computed(() => {
  const rowData = formatGroupsData(groupsData.value?.groups ?? []);

  return rowData.filter(
    (groupData) =>
      ((groupNumberQuery.value.length < groupNumberSearchMin &&
        (groupNumberQuery.value === "" ||
          groupNumberQuery.value !== listBillsQueryId.value)) ||
        groupData.findIndex(
          (cell) =>
            cell.key === "id" &&
            cell.onClickReturnValue === groupNumberQuery.value,
        ) > -1) &&
      (groupNameQuery.value.length < groupNameSearchMin ||
        groupData.findIndex(
          (cell) =>
            cell.key === "name" &&
            `${cell.text}`
              .toLowerCase()
              .includes(groupNameQuery.value.toLowerCase()),
        ) > -1),
  );
});

const groupSuggestions = computed(() => {
  const unsortedSuggestions = (groupsData.value?.groups ?? []).map(
    (group) => `${group.name}`,
  );

  unsortedSuggestions.sort((a, b) => {
    return a < b ? -1 : a == b ? 0 : 1;
  });

  return unsortedSuggestions;
});

const groupNumberSuggestions = computed(() => {
  return (groupsData.value?.groups ?? []).map((group) => `${group.id}`);
});

const groupNameQuery = ref("");
const groupNumberQuery = ref("");
const groupsTableHeaders: Row = [
  { key: "id", text: "Number" },
  { key: "name", text: "Name", className: "group-name" },
  { key: "city", text: "City" },
  { key: "state", text: "State" },
];

const listBillsQueryId = ref("");
const listBillsHeaders: Row = [{ key: "document", text: "Download by Date" }];

const groupSelected = (groupId: string) => {
  groupNumberQuery.value = groupId;
  listBillsQueryId.value = groupId;
  desktopGroupsTable.value.setTablePagetoOne();
  mobileGroupsTable.value.setTablePagetoOne();
};

const { data: listBillsData, isLoading: listBillsLoading } =
  useQuery<ListBillsResponse>({
    queryKey: [listBillsQueryId],
    staleTime: 300000,
    queryFn: () => {
      return listBillsQueryId.value !== ""
        ? ListBillsService.getListBills({ groupId: listBillsQueryId.value })
        : {};
    },
  });

const listBillsView = computed(() => {
  return formatListBillData(listBillsData.value?.listBills ?? []);
});

const isDownloadingListBill = ref(false);
async function downloadDocument(listBillId: string | undefined) {
  if (listBillId === undefined) {
    return;
  }

  isDownloadingListBill.value = true;
  const params: $OpenApiTs["/API/ListBills/listBills/{listBillId}"]["get"]["req"] =
    {
      listBillId,
    };

  try {
    const response = await axios.get(`/API/ListBills/listBills/${listBillId}`, {
      params,
      responseType: "blob",
    });

    const file = new FileHelper(
      `ListBill_${listBillId}`,
      response.data,
      response.headers["content-disposition"],
    );
    file.downloadFile();
  } catch (error) {
    console.error(error);
    isDownloadingListBill.value = false;
  }

  isDownloadingListBill.value = false;
}

const groupNameChange = (value: string | number | undefined) => {
  if (value) {
    const newValue: string = value.toString();
    groupNameQuery.value = newValue;
  }
};

const groupNumberChange = (value: string | number | undefined) => {
  if (value) {
    const newValue: string = value.toString();
    groupNumberQuery.value = newValue;
    listBillsQueryId.value = newValue;
  }
  if (value === "") {
    listBillsQueryId.value = "";
  }
};

const clearGroupNumber = () => {
  groupNumberQuery.value = "";
};

const clearGroupName = () => {
  groupNameQuery.value = "";
};
</script>
<style scoped lang="pcss">
.heading--1 {
  margin-bottom: 0px;
}
.popover {
  max-width: 720px;
}
.filter-info {
  display: flex;
  align-items: center;
  font-size: 0.8rem;
  color: var(--text-grey);
  @media (width >= 600px) {
    font-size: 1rem;
  }
}

.horizontal-rule {
  display: none;
  @media (width >= 600px) {
    display: flex;
  }
}

.heading--2 {
  margin: var(--spacing) 0 0;
}

:deep(.list-bills-table .table-desktop) {
  &__body {
    display: block;
    margin: var(--spacing-xs) 0 var(--spacing-xs) 10px;
  }

  &__row::before {
    content: url(/src/assets/Assurity-Logo-List-Bill.svg);
    width: 20px;
    height: 20px;
    display: flex;
    padding-top: var(--spacing-xxs);
    margin: 0 -6px var(--spacing-xxs) var(--spacing-xs);
  }
}

:deep(.group-name .mobile-data-table) {
  &__cell-data {
    min-width: 315px;
  }
}

:deep(.list-bills-table .mobile-data-table) {
  &__cell-heading {
    font-weight: bold;
  }

  &__cell-data {
    padding-top: 10px;
    justify-content: flex-start;
    text-decoration: underline;
    color: var(--accent-color);
    &:before {
      content: url(/src/assets/Assurity-Logo-List-Bill.svg);
      display: flex;
      align-items: center;
      margin-right: var(--spacing-s);
    }
  }
}

.filters {
  display: flex;
  flex-direction: column;
  flex-wrap: wrap;
  margin: var(--spacing) 0 var(--spacing);
  column-gap: var(--spacing);
  row-gap: var(--spacing);
  @media (width >= 600px) {
    flex-direction: row;
  }

  :deep(&__header-group-name .input) {
    min-width: 260px;
    @media (width >= 600px) {
      min-width: 300px;
    }
  }

  :deep(&__header-group-number .input) {
    min-width: 275px;
    @media (width >= 600px) {
      min-width: 150px;
    }
  }

  :deep(.filters__header-group-name .custom-select-options) {
    border-bottom: 1px solid var(--border-grey);
    z-index: 10;
  }

  :deep(.filters__header-group-number .custom-select-options) {
    border-bottom: 1px solid var(--border-grey);
    z-index: 9;
  }
}

:deep(.list-bill-group-table .mobile-data-table) {
  &__cell-data--first {
    color: var(--accent-color);
  }
}

:deep(.list-bill-results-table .mobile-data-table) {
  &__cell-data {
    color: var(--accent-color);
    text-decoration: underline;
  }
}

.list-bill-placeholder {
  width: 100%;
  display: flex;
  flex-direction: column;
  align-items: center;
}

:popover-open {
  max-height: 150vw;

  @media (width >= var(--bp-phone-portrait)) {
    width: 689px;
  }
}

:deep(.list-bill .popover) {
  &__header-text {
    padding-left: var(--spacing-xxs);
    font-size: 1.3125rem;
    @media (width >= var(--bp-phone-portrait)) {
      font-size: 1.75rem;
      padding-left: 6px;
    }
  }

  &__header {
    .popover__icon {
      width: 65px;
      margin-left: -10px;

      @media (width >= var(--bp-phone-portrait)) {
        width: 80px;
      }
    }
  }

  &__popover__icon {
    width: 65px;
    height: 65px;
  }

  &__content {
    font-size: 1rem;
    color: var(--text-grey);
    @media (width >= var(--bp-phone-portrait)) {
      font-size: 1.125rem;
      max-width: 900px;
    }
  }

  &__close-button {
    position: absolute;
    top: 10px;
    right: 10px;
    color: #707070;
    margin: 0;
    cursor: pointer;
  }

  &__close-icon {
    width: 20px;

    @media (width >= var(--bp-phone-portrait)) {
      width: 35px;
    }
  }

  &__footer {
    padding-top: 20px;
  }

  &__footer button {
    width: 123px;
    height: 34px;
    border: 1px solid #003b4d;
    border-radius: var(--spacing-xxs);

    @media (width >= var(--bp-phone-portrait)) {
      width: 114px;
      height: 42px;
    }
  }
}

.info {
  margin-left: 10px;
  height: 20px;
  width: 20px;
  cursor: pointer;
}

.highlight {
  background-color: lightgrey;
}

.list-bill-placeholder > .underline {
  margin-top: 15px;
  width: 150px;
  height: 10px;
  border-radius: var(--spacing-xxs);
  background: rgba(0, 0, 0, 0.1);
}

.download-message {
  display: flex;
  text-wrap: nowrap;
}

.progress-circle {
  height: 25px;
  width: 25px;
  margin-right: var(--spacing);
  margin-left: var(--spacing);
}
</style>
