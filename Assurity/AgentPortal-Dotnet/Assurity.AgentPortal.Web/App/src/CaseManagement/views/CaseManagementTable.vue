<template>
  <ContainerBlock dense class="col-span-12 case-management__table">
    <aTableDesktop
      :headers="caseManagementHeaders"
      :items="caseManagementRows"
      :default-items-per-page="10"
      class="col-span-12 case-management__table"
      :loading="isLoading"
      :total-items="totalItems"
      sorting="server"
      pagination="server"
      :current-page="pageingOptions.pageNumber"
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
    <aTableMobile
      :headers="caseManagementHeadersMobile"
      :items="caseManagementRowsMobile"
      :show-row-number="false"
      class="col-span-12 case-management__table"
      :loading="isLoading"
      :total-items="totalItems"
      sorting="server"
      pagination="server"
      :current-page="pageingOptions.pageNumber"
      :show-nested-rows-details="true"
      :get-expanded-component="getExpandedComponent"
      details-heading="Case Details"
      @update:current-page="updatePaging"
      @update:items-per-page="updateItemsPerPage"
      @link-click="
        (key, value) => {
          onActionClick(value);
        }
      "
    />
  </ContainerBlock>
  <StatusModal
    :unique-id="statusModalUniqueId"
    header-text="What your application status means"
  ></StatusModal>
  <MessageModal
    :unique-id="emailModalUniqueId"
    class="case-management__message_modal"
    header-text="Email Sent"
    body-text="An Email has been sent"
  ></MessageModal>
</template>

<script setup lang="ts">
import { computed, ref, watch, watchEffect } from "vue";
import aTableDesktop from "@/components/aTable/desktop/aTableDesktop.vue";
import ContainerBlock from "@/components/content/ContainerBlock.vue";
import {
  SortOptions,
  type Cell,
  type Row,
} from "@/components/aTable/definitions";
import aTableMobile from "@/components/aTable/mobile/aTableMobile.vue";
import {
  type Assurity_AgentPortal_Contracts_CaseManagement_CaseManagementCase as Case,
  type Assurity_AgentPortal_Contracts_Enums_SortDirection as SortDirection,
} from "@assurity/newassurelink-client";

import {
  sendCaseManagementResendEmail,
  useCaseManagementData,
} from "@/CaseManagement/api/caseManagementApi";
import StatusModal from "@/CaseManagement/views/StatusModal.vue";
import { useCaseManagementStore } from "../caseManagementStore";
import type { ActionData } from "../models/ActionData";
import EnvironHelpers from "@/Shared/utils/environHelpers";

import StatusHistoryName from "./StatusHistoryName.vue";
import StatusHistoryDate from "./StatusHistoryDate.vue";
import StatusHistoryAction from "./StatusHistoryAction.vue";
import SectionHeading from "./SectionHeading.vue";
import StatusHistoryMobile from "./StatusHistoryMobile.vue";
import PrimaryInsuredPerson from "./PrimaryInsuredPerson.vue";
import SpousePerson from "./SpousePerson.vue";
import ChildPerson from "./ChildPerson.vue";
import RelatedPerson from "./RelatedPerson.vue";
import ClientSectionMobile from "./ClientSectionMobile.vue";
import PrimaryInsuredProduct from "./PrimaryInsuredProduct.vue";
import SpouseProduct from "./SpouseProduct.vue";
import ChildProduct from "./ChildProduct.vue";
import ProductSectionMobile from "./ProductSectionMobile.vue";
import { ComponentName } from "../models/enums/ComponentName";
import {
  getClientInfoRows,
  getClientInfoRowsMobile,
  getMainRow,
  getProductInfoRow,
  getProductInfoRowsMobile,
  getStatusHistoryRow,
  getStatusHistoryRowMobile,
} from "../utils/tableUtils";
import { EventType } from "../models/enums/EventType";
import { NextView } from "../models/enums/NextView";
import MessageModal from "./MessageModal.vue";
import { useUserStore } from "@/stores/userStore";

const getExpandedComponent = (componentName: string) => {
  switch (componentName) {
    case ComponentName.StatusHistoryName:
      return StatusHistoryName;
    case ComponentName.StatusHistoryDate:
      return StatusHistoryDate;
    case ComponentName.StatusHistoryAction:
      return StatusHistoryAction;
    case ComponentName.SectionHeading:
      return SectionHeading;
    case ComponentName.StatusHistoryMobile:
      return StatusHistoryMobile;
    case ComponentName.PrimaryInsuredPerson:
      return PrimaryInsuredPerson;
    case ComponentName.SpousePerson:
      return SpousePerson;
    case ComponentName.ChildPerson:
      return ChildPerson;
    case ComponentName.RelatedPerson:
      return RelatedPerson;
    case ComponentName.ClientSectionMobile:
      return ClientSectionMobile;
    case ComponentName.PrimaryInsuredProduct:
      return PrimaryInsuredProduct;
    case ComponentName.SpouseProduct:
      return SpouseProduct;
    case ComponentName.ChildProduct:
      return ChildProduct;
    case ComponentName.ProductSectionMobile:
      return ProductSectionMobile;
    default:
      return null;
  }
};

const caseManagementStore = useCaseManagementStore();
const userStore = useUserStore();

const environ = EnvironHelpers.getEnviron();

const linkBaseUrl = `https://${environ}quickstart.assurity.com/`;

const sortingOptions = ref({
  sortColumn: "Events.0.CreatedDateTime",
  sortOrder: "DESC" as SortDirection,
});

const pageingOptions = ref({
  pageNumber: 0,
  pageSize: 10,
});

const totalItems = ref(0);

const updateSorting = (newSort: {
  key: string;
  sortKey: string;
  sortDirection: string;
}) => {
  sortingOptions.value.sortColumn = newSort.sortKey;
  sortingOptions.value.sortOrder = (
    newSort.sortDirection as string
  ).toUpperCase() as SortDirection;
};

const updatePaging = (newPage: number): void => {
  pageingOptions.value.pageNumber = newPage;
};

const updateItemsPerPage = (newItemsPerPage: number): void => {
  pageingOptions.value.pageSize = newItemsPerPage;
};

const deepFilterOptions = computed(() => caseManagementStore.filterOptions);

watch(
  deepFilterOptions,
  () => {
    pageingOptions.value.pageNumber = 1;
  },
  { deep: true },
);

const queryParams = computed(() => ({
  ...caseManagementStore.filterOptions,
  ...sortingOptions.value,
  ...pageingOptions.value,
}));

const { data, isLoading } = useCaseManagementData(queryParams);

const caseManagementRows = ref<Row[]>([]);
const caseManagementRowsMobile = ref<Row[]>([]);
const modalUUID = crypto.randomUUID();

const statusModalUniqueId = "statusModal" + modalUUID;
const emailModalUniqueId = "emailModal" + modalUUID;

document.getElementById(statusModalUniqueId)?.hidePopover();
document.getElementById(emailModalUniqueId)?.hidePopover();

const onActionClick = async (value: object) => {
  const data = value as ActionData;
  const eventType = data.eventType;

  if (eventType === EventType.RecipientSent) {
    sendCaseManagementResendEmail(data.envelopeId);
    document.getElementById(emailModalUniqueId)?.showPopover();
  } else {
    let nextView = "";
    switch (eventType) {
      case EventType.InterviewCompleted:
        nextView = NextView.PreliminaryDecisionScreen;
        break;
      case EventType.CaseStarted:
        nextView = NextView.CreateCase;
        break;
      case EventType.ReceivedQuote:
        nextView = NextView.RecustomizeQuote;
        break;
      default:
        nextView = NextView.Interview;
    }
    const url =
      linkBaseUrl +
      "resume?expKey=" +
      data.experienceKey +
      "&productId=" +
      data.product +
      "&nextView=" +
      nextView +
      "&cacheId=" +
      data.cacheId +
      "&quoteId=" +
      data.quoteId;
    window.open(url, "_blank");
  }
};

const processCaseManagementData = (cases: Case[] | null | undefined) => {
  const newRows: Row[] = [];
  cases?.forEach((item) => {
    const mainRow: Row = getMainRow(
      item,
      userStore.user.isSubaccount,
      userStore.user.isHomeOfficeImpersonating,
    );
    let nestedRows: Row[] = [];
    nestedRows.push(getStatusHistoryRow(item));
    nestedRows = nestedRows.concat(getClientInfoRows(item));
    nestedRows.push(getProductInfoRow(item));

    const nestedRowContainer = { nestedRow: nestedRows } as Cell;
    mainRow.push(nestedRowContainer);
    newRows.push(mainRow);
  });

  caseManagementRows.value = newRows;
};

const processCaseManagementDataMobile = (cases: Case[] | null | undefined) => {
  const newRows: Row[] = [];
  cases?.forEach((item) => {
    const mainRow: Row = getMainRow(
      item,
      userStore.user.isSubaccount,
      userStore.user.isHomeOfficeImpersonating,
    );
    let nestedRows: Row[] = [];
    nestedRows.push(getStatusHistoryRowMobile(item));
    nestedRows = nestedRows.concat(getClientInfoRowsMobile(item));
    nestedRows = nestedRows.concat(getProductInfoRowsMobile(item));
    const nestedRowContainer = { nestedRow: nestedRows } as Cell;
    mainRow.push(nestedRowContainer);
    newRows.push(mainRow);
  });

  caseManagementRowsMobile.value = newRows;
};

watchEffect(() => {
  if (data.value) {
    processCaseManagementData(data.value.cases);
    processCaseManagementDataMobile(data.value.cases);
    totalItems.value = data.value.totalRecords ?? 0;
  }
});

const caseManagementHeaders = [
  {
    key: "column-1",
    text: "Name",
    isSortable: true,
    sortDirection: SortOptions.none,
    sortKey: "PrimaryInsured.LastName",
  },
  {
    key: "column-2",
    text: "Product",
    isSortable: true,
    sortDirection: SortOptions.none,
    sortKey: "Product",
  },
  {
    key: "column-3",
    text: "Status",
    tooltipModal: statusModalUniqueId,
  },
  {
    key: "column-4",
    text: "Created Date",
    isSortable: true,
    sortDirection: SortOptions.none,
    sortKey: "Events.at(-1).CreatedDateTime", // Case's first event
  },
  {
    key: "column-5",
    text: "Action",
  },
];

const caseManagementHeadersMobile = [
  {
    key: "column-1",
    text: "Name",
    isSortable: true,
    sortDirection: SortOptions.none,
    sortKey: "Name",
  },
  {
    key: "column-2",
    text: "Product",
    isSortable: true,
    sortDirection: SortOptions.none,
    sortKey: "Product",
  },
  {
    key: "column-3",
    text: "Status",
    tooltipModal: statusModalUniqueId,
  },
  {
    key: "column-4",
    text: "Created Date",
    isSortable: true,
    sortDirection: SortOptions.none,
    sortKey: "Events.at(-1).CreatedDateTime", // Case's first event
  },
  {
    key: "column-5",
  },
];
</script>
<style>
.popover__content {
  @media (width < 960px) {
    max-height: 70vh;
    overflow-y: scroll;
  }
}
.case-management {
  &__check-box {
    width: 1.5em;
  }
  &__table .table-desktop__wrapper table {
    width: 100%;
    .tooltip-icon__container svg {
      width: 1.25em;
      margin: 0 0 -0.25em 0.25em;
    }
    & thead > tr.table-desktop__row-header th:first-child {
      padding-left: 3em;
    }
    & tbody > tr.table-desktop__row--nested td {
      vertical-align: top;
      width: 200px;
      div .case-management__table {
        &__insured {
          display: flex;
          flex-direction: column;
          table {
            margin: 0.5em 0;
            th {
              padding: 1rem 0;
            }
            td {
              border: none;
            }
          }
          &__name {
            font-weight: bold;
            padding-top: 0.5em;
          }
        }

        &__insured-demograpics {
          td:first-child {
            font-weight: bold;
          }
        }
        &__product {
          display: flex;
          flex-direction: column;
          table {
            margin-bottom: 1em;
            td {
              border: none;
              padding-right: 0.5em;
            }
          }
        }
        &__product-table {
          margin-top: 1.3em;
          td {
            padding: 0.05em;
          }
          td:first-child {
            font-weight: bold;
            width: 60%;
          }
        }
        &__section-heading {
          border: none;
          font-weight: bold;
          padding-top: 1.5em;
        }
        &__status {
          td {
            border-bottom: none;
            padding-bottom: 1em;
          }
          th {
            padding: 1em 0;
          }
          th:first-child:after {
            background: none;
          }
        }
      }
    }
  }
  &__table__status-mobile {
    width: 100%;
    .cm-td {
      border: none;
      padding-bottom: 0.5em;
    }
    td:first-child {
      font-weight: bold;
    }
    th {
      border: none;
      padding-bottom: 0.5em;
    }
  }
  &__table__section-heading-mobile {
    font-size: 1.2rem;
    font-weight: bold;
    border-bottom: 1px solid var(--bg-grey-dark);
    padding: 1em 0 0.8em 0;
  }
  &__table__mobile-section {
    hr {
      margin: 1em 0 2em 0;
    }
    .case-management__table {
      &__insured {
        table {
          margin: 0 0 0.5em 0;
        }
      }

      &__insured-demograpics {
        td:first-child {
          font-weight: bold;
          padding-right: 1em;
        }
      }
      &__product-table {
        td:first-child {
          font-weight: bold;
          padding-right: 1em;
        }
      }
    }
  }
  &__message_modal {
    & .popover__content {
      overflow: auto;
      width: 300px;
    }
  }
}

.mobile-data-table__cell:nth-child(2) {
  display: grid;
  flex: 1 0 calc(100% - var(--number-size) - var(--list-column-gap));
}
.mobile-data-table__cell:nth-child(3) {
  flex: 1 0 calc(100% - var(--number-size) - var(--list-column-gap));
}
.mobile-data-table__list {
  font-size: 1rem;
  li:last-child {
    hr {
      display: none;
    }
  }
}
.mobile-data-table__cell-heading {
  button {
    float: right;
    margin-top: -70px;
  }
}
.tooltip-icon__container {
  svg {
    vertical-align: top;
    height: 20px;
  }
}
.table-desktop {
  &__wrapper {
    td {
      border: none;
    }
  }
}
.popover {
  @media (min-width: 960px) {
    max-width: 960px;
  }
}
.table-desktop__row {
  height: 52px;
}
.table-desktop__button {
  height: 34px;
}
.page-content {
  color: var(--text-grey);
}
</style>
