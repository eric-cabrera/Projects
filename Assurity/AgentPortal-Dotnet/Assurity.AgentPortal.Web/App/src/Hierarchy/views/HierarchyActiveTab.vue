<template>
  <div>
    <div class="hierarchy__active hierarchy__header">
      <div>
        <div class="filterIcon hierarchy__active__filter-icon">
          <AssurityIconFilter class="filters__filter-icon" />
          <span>FILTER</span>
          <a v-if="agentNameQuery" class="clear-link" @click="clearAgentName()"
            >CLEAR ALL</a
          >
        </div>
        <aSearch
          id="agent-name"
          class="agent-name__dropdown"
          placeholder="Agent Name or ID"
          tool-tip-text="Agent Name or ID"
          bg-color="white"
          :clearable="true"
          :value="selectedAgent"
          :options="agentOptions"
          :min-length="agentNameSearchMin"
          :search-query="agentNameQuery"
          :suggestions="agentSuggestions"
          @update:model-value="agentChange"
          @update:clear="() => clearAgentName()"
          @update:search-query:input="
            (newValue: string) => {
              agentNameQuery = newValue;
            }
          "
          @update:search-query:suggestion="
            (newValue: string) => {
              selectedAgent = newValue;
              agentNameQuery = newValue;
            }
          "
          @update:search-query:clear="clearAgentName()"
          @focus="() => {}"
        >
        </aSearch>
        <aDropdownV2
          id="contractStatus"
          label="Contract Status"
          class="contract_dropdown"
          bg-color="white"
          :value="contractStatusSelect"
          :options="filterItems.contractFilterValues"
          @update:model-value="contractStatusChange"
        />
      </div>
      <div class="hierarchy__header__download">
        <aButton
          v-if="canDownload"
          button-style="primary"
          text="DOWNLOAD XLSX"
          size="s"
          class="align-bottom"
          @click="callDownloadActiveDocument()"
          @keyup.enter="callDownloadActiveDocument()"
          @keyup.space="callDownloadActiveDocument()"
          @keypress="$event.preventDefault()"
        >
          <template #prepend>
            <v-progress-circular v-if="isDownloading" indeterminate />
            <AssurityIconDownload v-else color="white" />
          </template>
        </aButton>
      </div>
    </div>
    <div ref="hierarchyTable" class="hierarchy__active-tab__hierarchy-table">
      <ContainerBlock nosidepadding>
        <AgentAgencyTable
          :agent-agencies="agentAgencies"
          :is-loading="activeHierarchyQuery.isLoading.value"
          :is-pending="false"
        >
        </AgentAgencyTable>
        <div class="hierarchy__footer__download">
          <aButton
            v-if="canDownload"
            button-style="primary"
            text="DOWNLOAD XLSX"
            size="s"
            class="align-bottom"
            @click="callDownloadActiveDocument()"
            @keyup.enter="callDownloadActiveDocument()"
            @keyup.space="callDownloadActiveDocument()"
            @keypress="$event.preventDefault()"
          >
            <template #prepend>
              <v-progress-circular v-if="isDownloading" indeterminate />
              <AssurityIconDownload v-else color="white" />
            </template>
          </aButton>
        </div>
      </ContainerBlock>
    </div>
    <h3 class="title">Agent Appointment Information</h3>
    <aHorizontalRule class="horizontal-rule" />

    <div class="hierarchy__header">
      <div>
        <div class="filterIcon">
          <AssurityIconFilter class="filters__filter-icon" />
          <span>FILTER</span>
        </div>

        <div v-if="selectedFilterData?.length" class="header-selected-filters">
          <aSelectedFilters
            :selected-filters="selectedFilterData"
            :selected-filter-count="1"
            @reset-filters="clearAgentName"
            @filter-value-changed="removeFilter"
          />
        </div>

        <aDropdownV2
          id="state"
          label="State"
          bg-color="white"
          class="state_filter"
          :value="stateSelect"
          :options="filterItems.stateFilterValues"
          @update:model-value="stateChange"
        />
      </div>
      <div class="hierarchy__header__download">
        <aButton
          v-if="canAppointmentDownload"
          button-style="primary"
          text="DOWNLOAD XLSX"
          size="s"
          class="align-bottom"
          @click="callDownloadAppointmentDocument()"
          @keyup.enter="callDownloadAppointmentDocument()"
          @keyup.space="callDownloadAppointmentDocument()"
          @keypress="$event.preventDefault()"
        >
          <template #prepend>
            <v-progress-circular v-if="isDownloading" indeterminate />
            <AssurityIconDownload v-else color="white" />
          </template>
        </aButton>
      </div>
    </div>

    <div class="hierarchy_appointment-table">
      <ContainerBlock dense>
        <aTableDesktop
          :headers="hierarchyTableHeaders"
          :items="hierarchyAppointmentItems"
          :show-row-number="false"
          :loading="appointmentIsLoading"
          class="col-span-12"
          pagination="client"
          :total-items="totalItems"
          :current-page="1"
        />
        <aTableMobile
          :headers="hierarchyDetailTableHeadersMobile"
          :items="hierarchyAppointmentItems"
          :show-row-number="false"
          :loading="appointmentIsLoading"
          pagination="client"
          :total-items="totalItems"
          :current-page="1"
        />
        <div class="hierarchy__footer__download">
          <aButton
            v-if="canAppointmentDownload"
            button-style="primary"
            text="DOWNLOAD XLSX"
            size="s"
            class="align-bottom"
            @click="callDownloadAppointmentDocument()"
            @keyup.enter="callDownloadAppointmentDocument()"
            @keyup.space="callDownloadAppointmentDocument()"
            @keypress="$event.preventDefault()"
          >
            <template #prepend>
              <v-progress-circular v-if="isDownloading" indeterminate />
              <AssurityIconDownload v-else color="white" />
            </template>
          </aButton>
        </div>
      </ContainerBlock>
    </div>
  </div>
</template>

<script setup lang="ts">
import type { Cell, Row } from "@/components/aTable/definitions";
import type { aFilterData } from "@/components/aFilterDrawer/models/aFilterData";
import { ref, computed, reactive, watch } from "vue";
import axios from "axios";
import { FileHelper } from "@/Shared/utils/FileHelper";
import aTableDesktop from "@/components/aTable/desktop/aTableDesktop.vue";
import aTableMobile from "@/components/aTable/mobile/aTableMobile.vue";
import aSearch from "@/components/aSearch/aSearch.vue";
import type { Option } from "@/components/aSearch/aSearch.vue";
import aHorizontalRule from "@/components/aHorizontalRule.vue";
import aDropdownV2 from "@/components/forms/aDropdownV2.vue";
import aSelectedFilters from "@/components/aFilterDrawer/aSelectedFilters.vue";
import aButton from "@/components/forms/aButton.vue";
import AssurityIconDownload from "@/components/icons/AssurityIconDownload.vue";
import AssurityIconFilter from "@/components/icons/AssurityIconFilter.vue";
import { aFilterType } from "@/components/aFilterDrawer/models/enums/aFilterType";
import ContainerBlock from "@/components/content/ContainerBlock.vue";
import { SortOptions } from "@/components/aTable/definitions";
import AgentAgencyTable from "@/Hierarchy/views/AgentAgencyTable.vue";
import {
  useActiveHierarchyData,
  useActiveAppointmentsData,
} from "../api/hierarchyApi";
import type {
  $OpenApiTs,
  Assurity_AgentPortal_Contracts_AgentContracts_Agent as Agent,
  Assurity_AgentPortal_Contracts_AgentContracts_AgentHierarchy as AgentHierarchy,
  Assurity_AgentPortal_Contracts_AgentContracts_AgentHierarchyBranch as HierarchyBranch,
  Assurity_AgentPortal_Contracts_AgentContracts_AgentAppointment as AgentAppointment,
  Assurity_AgentPortal_Contracts_AgentContracts_ContractStatus as ContractStatus,
} from "@assurity/newassurelink-client";
import type { AgentAgencyRow } from "../models/AgentAgencyRow";
import { useHierarchyStore } from "../hierarchyStore";
import type { HierarchyFilterOptions } from "../models/HierarchyFilterOptions";
import type { AppointmentFilterOptions } from "../models/AppointmentFilterOptions";

const AGENT_NAME_FILTER_NAME = "agentNameQuery";
const STATE_DEFAULT = "All";
const STATUS_DEFAULT = "Unknown";
const MIN_CHARS_BEFORE_AGENT_ACTION = 3;

const agentNameSearchMin = 3;
const isDownloading = ref(false);
const agentNameQuery = ref("");
const selectedAgent = ref("");
const hierarchyTable = ref<HTMLDivElement>();
const stateSelect = ref(STATE_DEFAULT);
const contractStatusSelect = ref<ContractStatus>(STATUS_DEFAULT);
const agentSuggestions = ref<string[] | undefined>([]);
const agentOptions = ref<Option[] | undefined>([]);
let currentAgent: HTMLElement;
let allAppointmentData: AgentAppointment[];

const agentAgencies = ref<AgentAgencyRow[]>([]);
const hierarchyStore = useHierarchyStore();
const queryParams = ref<HierarchyFilterOptions>({});
const appointmentParams = ref<AppointmentFilterOptions>({});
const appointmentData = useActiveAppointmentsData(appointmentParams);
const appointmentIsLoading = appointmentData.isLoading;

watch(
  appointmentData.data,
  (response) => {
    if (response && response.appointments) {
      allAppointmentData = response.appointments;
      filterItems.stateFilterValues = [];
      hierarchyAppointmentItems.value = response.appointments.map(
        (appointment) => {
          if (appointment.stateAbbreviation) {
            filterItems.stateFilterValues.push(appointment.stateAbbreviation);
          }

          return getAppointmentMainRow(appointment);
        },
      );

      // Sort and unique
      filterItems.stateFilterValues = [
        ...new Set(filterItems.stateFilterValues),
      ];
      filterItems.stateFilterValues.sort((a, b) => a.localeCompare(b));
      filterItems.stateFilterValues.unshift(STATE_DEFAULT);
    }
  },
  { immediate: true },
);

const activeHierarchyQuery = useActiveHierarchyData(queryParams);

watch(
  activeHierarchyQuery.data,
  (response) => {
    agentSuggestions.value = [];
    agentAgencies.value = [];
    if (response?.hierarchy) {
      processAgentNames(response?.filters?.agentNames ?? []);
      processAgentAgencies(response?.hierarchy);

      hierarchyStore.agentCounts.active = response.hierarchy
        .totalActiveAgents as number;

      hierarchyStore.agentCounts.jit = response.hierarchy
        .totalJitAgents as number;
    }
  },
  { immediate: true },
);

const getAppointmentMainRow = (item: AgentAppointment) => {
  const mainRow: Cell[] = [
    {
      key: "name",
      text: item.name ?? "-",
    },
    {
      key: "id",
      text: item.agentNumber ?? "-",
    },
    {
      key: "state",
      text: item.stateAbbreviation ?? "-",
    },
    {
      key: "resident",
      text: item.isResident ?? "-",
    },
    {
      key: "granted",
      text: item.grantedDate ?? "",
    },
    {
      key: "expiration",
      text: item.expirationDate ?? "",
    },
  ];

  return mainRow;
};

const processAgentNames = (agentNames: Agent[] | undefined) => {
  agentSuggestions.value = agentNames?.map((agent) => {
    return agent.displayValue;
  });
  agentOptions.value =
    agentNames?.map((agent) => {
      return {
        label: agent.displayValue,
        value: agent.agentNumber,
      } as Option;
    }) ?? [];
};

const processAgentAgencies = (hierarchy: AgentHierarchy | undefined) => {
  agentAgencies.value.push({
    agentNumber: hierarchy?.agentNumber,
    marketCode: hierarchy?.marketCode,
    agentLevel: hierarchy?.agentLevel,
    companyCode: hierarchy?.companyCode,
    name: hierarchy?.name,
    contractStatus: hierarchy?.contractStatus,
    depth: 0,
    selected: false,
  });
  if (hierarchy?.branches) {
    getBranchRows(hierarchy.branches, 1);
  }
};

const getBranchRows = (branches: HierarchyBranch[], depth: number) => {
  branches.forEach((branch) => {
    agentAgencies.value.push({
      agentNumber: branch.agentNumber,
      marketCode: branch?.marketCode,
      agentLevel: branch?.agentLevel,
      companyCode: branch?.companyCode,
      name: branch.name,
      contractStatus: branch.contractStatus,
      depth: depth,
      hasBranches: (branch.branches?.length ?? 0) > 0,
      selected: false,
    });
    if (branch.branches) {
      getBranchRows(branch.branches, depth + 1);
    }
  });
};

const selectedFilterData = computed((): aFilterData[] => {
  const selectedFilters: aFilterData[] = [];
  if (agentNameQuery.value?.length) {
    selectedFilters.push({
      display: true,
      text: "agent Name",
      type: aFilterType.AutoComplete,
      field: "agentNameQuery",
      selection: [
        {
          text: agentNameQuery.value,
        },
      ],
    });
  }

  return selectedFilters;
});

const filterItems = reactive({
  stateFilterValues: [STATE_DEFAULT],
  contractFilterValues: [
    { label: "All Agents", value: STATUS_DEFAULT },
    { label: "Active Agents", value: "Active" },
    { label: "Just-In-Time Agents", value: "JIT" },
  ],
});

const agentChange = (value: string | number | undefined) => {
  if (value) {
    const newValue: string = value.toString();
    if (newValue.length > MIN_CHARS_BEFORE_AGENT_ACTION) {
      const agentNumber = newValue;

      agentAgencies.value.forEach((agent) => {
        if (agent.detailsOpen) {
          agent.detailsOpen = false;
        }

        if (agent.agentNumber === agentNumber) {
          agent.selected = true;
          agent.detailsOpen = true;
        }
      });

      stateSelect.value = STATE_DEFAULT;
      contractStatusSelect.value = STATUS_DEFAULT;

      if (hierarchyTable?.value) {
        if (currentAgent) {
          currentAgent.classList.remove("font-bold");
        }

        currentAgent = hierarchyTable.value.querySelector(
          "#agent-" + agentNumber,
        ) as HTMLElement;

        if (currentAgent) {
          currentAgent.classList.add("font-bold");

          setTimeout(function () {
            currentAgent.scrollIntoView({
              behavior: "smooth",
              block: "center",
            });
          }, 100);
        }

        const agentAgency = agentAgencies.value.find(
          (agentAgency) => agentAgency.agentNumber === agentNumber,
        );
        hierarchyAppointmentItems.value = [];
        if (agentAgency) {
          appointmentParams.value = {
            ...appointmentParams.value,
            ...{
              downlineAgentNumber: agentNumber,
              downlineAgentLevel: agentAgency.agentLevel ?? undefined,
              downlineCompanyCode: agentAgency.companyCode ?? undefined,
              downlineMarketCode: agentAgency.marketCode ?? undefined,
            },
          };
        }
      }
    }
  }
};

const contractStatusChange = (value: string | number | undefined) => {
  if (value) {
    const newValue: string = value.toString();
    hierarchyStore.filterOptions.contractStatus =
      newValue === STATUS_DEFAULT ? undefined : newValue;
  }
};

const hierarchyTableHeaders = ref<Cell[]>([]);
hierarchyTableHeaders.value = [
  {
    key: "name",
    text: "Agent Name",
    className: "policy-detail-table__header--wideColumn",
    isSortable: false,
    sortDirection: SortOptions.none,
    sortKey: "AgentName",
  },
  {
    key: "id",
    text: "Agent ID",
    className: "policy-detail-table__header--wideColumn",
    isSortable: false,
    sortDirection: SortOptions.none,
    sortKey: "AgentId",
  },
  {
    key: "state",
    text: "State",
    className: "policy-detail-table__header--wideColumn",
    isSortable: false,
    sortDirection: SortOptions.none,
    sortKey: "State",
  },
  {
    key: "resident",
    text: "Resident",
    className: "policy-detail-table__header--wideColumn",
    isSortable: false,
    sortDirection: SortOptions.none,
    sortKey: "Resident",
  },
  {
    key: "granted",
    text: "Granted",
    className: "policy-detail-table__header--wideColumn",
    isSortable: false,
    sortDirection: SortOptions.none,
    sortKey: "Granted",
  },
  {
    key: "expiration",
    text: "Expiration",
    className: "policy-detail-table__header--wideColumn",
    isSortable: false,
    sortDirection: SortOptions.none,
    sortKey: "Expiration",
  },
];

const hierarchyDetailTableHeadersMobile = ref<Cell[]>([]);
hierarchyDetailTableHeadersMobile.value = [
  {
    key: "name",
    text: "Agent Name",
    className: "policy-detail-table__header--wideColumn",
    isSortable: false,
    sortDirection: SortOptions.none,
    sortKey: "AgentName",
  },
  {
    key: "id",
    text: "Agent ID",
    className: "policy-detail-table__mobile--wideColumn",
    isSortable: false,
    sortDirection: SortOptions.none,
    sortKey: "AgentId",
  },
  {
    key: "state",
    text: "State",
    className: "policy-detail-table__mobile--wideColumn",
    isSortable: false,
    sortDirection: SortOptions.none,
    sortKey: "State",
  },
  {
    key: "resident",
    text: "Resident",
    className: "policy-detail-table__mobile--wideColumn",
    isSortable: false,
    sortDirection: SortOptions.none,
    sortKey: "Resident",
  },
  {
    key: "granted",
    text: "Granted",
    className: "policy-detail-table__mobile--wideColumn",
    isSortable: false,
    sortDirection: SortOptions.none,
    sortKey: "Granted",
  },
  {
    key: "expiration",
    text: "Expiration",
    className: "policy-detail-table__mobile--wideColumn",
    isSortable: false,
    sortDirection: SortOptions.none,
    sortKey: "Expiration",
  },
];

const hierarchyAppointmentItems = ref<Row[]>([]);
const totalItems = ref<number | undefined>(0);

const clearAll = () => {
  clearAgentName();
  contractStatusSelect.value = STATUS_DEFAULT;
};

const clearAgentName = () => {
  agentNameQuery.value = "";
  selectedAgent.value = "";
  filterItems.stateFilterValues = [STATE_DEFAULT];
  stateChange(STATE_DEFAULT);

  agentAgencies.value.forEach((agency) => {
    if (agency.detailsOpen) {
      agency.detailsOpen = false;
    }
  });

  if (currentAgent) {
    currentAgent.classList.remove("font-bold");
  }

  appointmentParams.value = {
    ...{
      viewAsAgentLevel: hierarchyStore.filterOptions.agentLevel,
      viewAsCompanyCode: hierarchyStore.filterOptions.companyCode,
      viewAsAgentNumber: hierarchyStore.filterOptions.agentNumber,
      viewAsMarketCode: hierarchyStore.filterOptions.marketCode,
    },
  };
};

const removeFilter = (value: {
  filterName: string;
  itemId: string | null | undefined;
}) => {
  if (value.filterName === AGENT_NAME_FILTER_NAME) {
    clearAgentName();
  }
};

const stateChange = (value: string | number | undefined) => {
  if (value && allAppointmentData) {
    stateSelect.value = value;

    if (value === STATE_DEFAULT) {
      hierarchyAppointmentItems.value = allAppointmentData.map((appointment) =>
        getAppointmentMainRow(appointment),
      );
    } else {
      hierarchyAppointmentItems.value = allAppointmentData
        .filter((appointment) => appointment.stateAbbreviation === value)
        .map((appointment) => getAppointmentMainRow(appointment));
    }
  }
};

async function callDownloadActiveDocument() {
  isDownloading.value = true;
  const params: $OpenApiTs["/API/AgentHierarchy/Hierarchy/Export"]["get"]["req"] =
    {
      agentLevel: queryParams.value.agentLevel ?? "",
      agentNumber: queryParams.value.agentNumber ?? "",
      marketCode: queryParams.value.marketCode ?? "",
      companyCode: queryParams.value.companyCode ?? "",
      contractStatus: hierarchyStore.filterOptions.contractStatus,
    };
  await callWithAxios(
    params,
    "/API/AgentHierarchy/Hierarchy/Export",
    "Active_Hierarchy_Details",
  );

  isDownloading.value = false;
}

async function callDownloadAppointmentDocument() {
  isDownloading.value = true;
  const params: $OpenApiTs["/API/AgentHierarchy/Appointments/Export"]["get"]["req"] =
    {
      downlineAgentLevel:
        appointmentParams.value.downlineAgentLevel ??
        appointmentParams.value.viewAsAgentLevel ??
        "",
      downlineAgentNumber:
        appointmentParams.value.downlineAgentNumber ??
        appointmentParams.value.viewAsAgentNumber ??
        "",
      downlineCompanyCode:
        appointmentParams.value.downlineCompanyCode ??
        appointmentParams.value.viewAsCompanyCode ??
        "",
      downlineMarketCode:
        appointmentParams.value.downlineMarketCode ??
        appointmentParams.value.viewAsMarketCode ??
        "",
      viewAsAgentLevel: appointmentParams.value.viewAsAgentLevel ?? "",
      viewAsAgentNumber: appointmentParams.value.viewAsAgentNumber ?? "",
      viewAsCompanyCode: appointmentParams.value.viewAsCompanyCode ?? "",
      viewAsMarketCode: appointmentParams.value.viewAsMarketCode ?? "",
      state: stateSelect.value,
    };
  await callWithAxios(
    params,
    "/API/AgentHierarchy/Appointments/Export",
    "Active_Appointment_Details",
  );

  isDownloading.value = false;
}

async function callWithAxios(
  params:
    | $OpenApiTs["/API/AgentHierarchy/Hierarchy/Export"]["get"]["req"]
    | $OpenApiTs["/API/AgentHierarchy/Appointments/Export"]["get"]["req"],
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

const canDownload = computed(() => {
  return (
    !activeHierarchyQuery.isLoading.value && agentAgencies.value.length > 0
  );
});

const canAppointmentDownload = computed(() => {
  return (
    !appointmentIsLoading.value && hierarchyAppointmentItems.value.length > 0
  );
});

watch(
  hierarchyStore.filterOptions,
  (newFilterOptions) => {
    queryParams.value = { ...newFilterOptions };
    appointmentParams.value = {
      ...{
        viewAsAgentLevel: newFilterOptions.agentLevel,
        viewAsCompanyCode: newFilterOptions.companyCode,
        viewAsAgentNumber: newFilterOptions.agentNumber,
        viewAsMarketCode: newFilterOptions.marketCode,
      },
    };
    clearAll();
  },
  { immediate: true },
);
</script>

<style scoped lang="pcss">
.hierarchy__active-tab__hierarchy-table {
  margin: 1em 0 2.5em;
}

.title {
  color: var(--primary-color);
}

.horizontal-rule {
  margin-top: var(--spacing-s);
  margin-bottom: var(--spacing-s);
}

.header-selected-filters {
  display: flex;
  gap: var(--spacing);
  flex: 1 1 auto;
  flex-wrap: wrap;
  padding-top: var(--spacing);
  padding-bottom: var(--spacing);
}

.filterIcon {
  color: var(--text-grey);

  & .filters__filter-icon {
    margin-right: 10px;
  }

  & span {
    vertical-align: super;
    font-weight: bold;
  }
}

.hierarchy__header {
  padding-top: var(--spacing-s);
}

.state_filter {
  min-width: 260px;
  @media (width <= 960px) {
    width: calc(100vw - 2em);
  }
}

.agent-name__dropdown {
  & .custom-select-wrapper input {
    width: 400px;
  }
  @media (width <= 960px) {
    width: calc(100vw - 2em);
  }
}

.contract_dropdown {
  color: var(--text-grey);
  margin-left: var(--spacing-s);
  @media (width <= 960px) {
    margin-left: 0;
    margin-top: 1em;
    min-width: calc(100vw - 2em);
  }
}

.align-bottom {
  align-self: flex-end;
}

.clear-link {
  vertical-align: super;
  margin-left: var(--spacing-s);
  text-decoration: underline;
  cursor: pointer;
  font-size: 1rem;
}
</style>

<style lang="pcss">
@import "@/Shared/styles/filter-styles.pcss";
</style>

<style>
.hierarchy_appointment-table {
  margin-top: var(--spacing);
  margin-bottom: var(--spacing);

  & td {
    height: 3.5em;
  }
}

.mobile-data-table__item .mobile-data-table__cell:first-of-type {
  flex: 1 1 calc(50% - var(--list-column-gap));
}

.agent-name__dropdown .custom-select-wrapper input {
  width: 400px;
}

.font-bold {
  font-weight: bold;
}
</style>
