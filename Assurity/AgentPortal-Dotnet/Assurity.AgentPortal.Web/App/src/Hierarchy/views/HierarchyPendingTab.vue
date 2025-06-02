<template>
  <div class="hierarchy__pending hierarchy__header">
    <div class="filterIcon">
      <AssurityIconFilter class="filters__filter-icon" />
      <span>FILTER</span>
      <a v-if="agentQuery" class="clear-link" @click="clearAgent()">
        CLEAR ALL
      </a>
    </div>
    <div class="hierarchy__pending__top-controls">
      <aSearch
        id="agent"
        placeholder="Agent Name or ID"
        class="top-contols__agent-select"
        bg-color="white"
        :clearable="true"
        :value="agentQuery"
        :options="agentOptions"
        :min-length="2"
        :search-query="agentQuery"
        :suggestions="agentSuggestions ?? []"
        @update:model-value="agentChange"
        @update:clear="() => clearAgent()"
        @update:search-query:input="
          (newValue: string) => {
            agentQuery = newValue;
            if (newValue === '') {
              agentQueryId = '';
            }
          }
        "
        @update:search-query:suggestion="
          (newValue: string) => {
            agentQuery = newValue;
            agentQueryId = newValue;
          }
        "
        @update:search-query:clear="clearAgent()"
        @focus="() => (agentQuery = '')"
      >
      </aSearch>
      <div class="hierarchy__header__download">
        <aButton
          v-if="canPendingDownload"
          button-style="primary"
          text="DOWNLOAD XLSX"
          size="s"
          class="top-contols__download"
          @click="callDownloadPendingDocument()"
          @keyup.enter="callDownloadPendingDocument()"
          @keyup.space="callDownloadPendingDocument()"
          @keypress="$event.preventDefault()"
        >
          <template #prepend>
            <v-progress-circular v-if="isDownloading" indeterminate />
            <AssurityIconDownload v-else color="white" />
          </template>
        </aButton>
      </div>
    </div>
    <div ref="hierarchyTable" class="hierarchy__pending__hierarchy-table">
      <ContainerBlock nosidepadding>
        <AgentAgencyTable
          :agent-agencies="agentAgencies"
          :is-loading="requirementsQuery.isLoading.value"
          :is-pending="true"
        >
        </AgentAgencyTable>
        <div class="hierarchy__footer__download">
          <aButton
            v-if="canPendingDownload"
            button-style="primary"
            text="DOWNLOAD XLSX"
            size="s"
            class="top-contols__download"
            @click="callDownloadPendingDocument()"
            @keyup.enter="callDownloadPendingDocument()"
            @keyup.space="callDownloadPendingDocument()"
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
import { computed, ref, watch } from "vue";
import axios from "axios";
import { FileHelper } from "@/Shared/utils/FileHelper";
import AgentAgencyTable from "@/Hierarchy/views/AgentAgencyTable.vue";
import ContainerBlock from "@/components/content/ContainerBlock.vue";
import aSearch from "@/components/aSearch/aSearch.vue";
import type { Option } from "@/components/aSearch/aSearch.vue";
import aButton from "@/components/forms/aButton.vue";
import AssurityIconFilter from "@/components/icons/AssurityIconFilter.vue";
import AssurityIconDownload from "@/components/icons/AssurityIconDownload.vue";
import { useHierarchyStore } from "../hierarchyStore";
import { useHierarchyWithPendingRequirementsData } from "../api/hierarchyApi";
import type { AgentAgencyRow } from "../models/AgentAgencyRow";
import type {
  $OpenApiTs,
  Assurity_AgentPortal_Contracts_AgentContracts_Agent as Agent,
  Assurity_AgentPortal_Contracts_AgentContracts_PendingRequirementsHierarchyBranch as PendingRequirementsHierarchyBranch,
} from "@assurity/newassurelink-client";
import type { HierarchyFilterOptions } from "../models/HierarchyFilterOptions";

const agentAgencies = ref<AgentAgencyRow[]>([]);
const agentSuggestions = ref<string[] | undefined>([]);
const agentOptions = ref<Option[] | undefined>([]);
const agentQueryId = ref("");
const hierarchyTable = ref<HTMLDivElement>();
const hierarchyStore = useHierarchyStore();
const queryParams = ref<HierarchyFilterOptions>({});
let currentAgent: HTMLElement;

watch(
  hierarchyStore.filterOptions,
  (newFilterOptions) => {
    queryParams.value = { ...newFilterOptions };
  },
  { immediate: true },
);

const requirementsQuery = useHierarchyWithPendingRequirementsData(queryParams);

const processAgentNames = (agentNames: Agent[] | undefined) => {
  agentSuggestions.value = agentNames?.map((agent) => {
    return agent.displayValue;
  });
  agentOptions.value = agentNames?.map((agent) => {
    return {
      label: agent.displayValue,
      value: agent.agentNumber,
    } as Option;
  });
};

const processAgentAgencies = (
  hierarchy: PendingRequirementsHierarchyBranch | undefined,
) => {
  agentAgencies.value.push({
    agentNumber: hierarchy?.agentNumber,
    marketCode: hierarchy?.marketCode,
    agentLevel: hierarchy?.agentLevel,
    companyCode: hierarchy?.companyCode,
    name: hierarchy?.name,
    emailAddress: hierarchy?.emailAddress,
    phoneNumber: hierarchy?.phoneNumber,
    contractStatus: hierarchy?.contractStatus,
    pendingRequirements: hierarchy?.pendingRequirements ?? [],
    depth: 0,
    selected: false,
  });
  if (hierarchy?.branches) {
    getBranchRows(hierarchy?.branches, 1);
  }
};

const getBranchRows = (
  branches: PendingRequirementsHierarchyBranch[],
  depth: number,
) => {
  branches.forEach((branch) => {
    agentAgencies.value.push({
      agentNumber: branch.agentNumber,
      marketCode: branch?.marketCode,
      agentLevel: branch?.agentLevel,
      companyCode: branch?.companyCode,
      emailAddress: branch?.emailAddress,
      phoneNumber: branch?.phoneNumber,
      name: branch.name,
      contractStatus: branch.contractStatus,
      pendingRequirements: branch.pendingRequirements ?? [],
      depth: depth,
      hasBranches: (branch.branches?.length ?? 0) > 0,
      selected: false,
    });
    if (branch.branches) {
      getBranchRows(branch.branches, depth + 1);
    }
  });
};

const agentChange = (value: string | number | undefined) => {
  if (value) {
    const newValue: string = value.toString();
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
          currentAgent.scrollIntoView({ behavior: "smooth", block: "center" });
        }, 100);
      }
    }
  }
};

const clearAgent = () => {
  agentQuery.value = "";

  agentAgencies.value.forEach((agency) => {
    if (agency.detailsOpen) {
      agency.detailsOpen = false;
    }
  });

  if (currentAgent) {
    currentAgent.classList.remove("font-bold");
  }
};

const agentQuery = ref("");

const isDownloading = ref(false);

async function callDownloadPendingDocument() {
  isDownloading.value = true;
  const params: $OpenApiTs["/API/AgentHierarchy/PendingRequirements/Export"]["get"]["req"] =
    {
      agentLevel: queryParams.value.agentLevel ?? "",
      agentNumber: queryParams.value.agentNumber ?? "",
      marketCode: queryParams.value.marketCode ?? "",
      companyCode: queryParams.value.companyCode ?? "",
    };
  await callWithAxios(
    params,
    "/API/AgentHierarchy/PendingRequirements/Export",
    "Pending_Hierarchy_Details",
  );

  isDownloading.value = false;
}

async function callWithAxios(
  params: $OpenApiTs["/API/AgentHierarchy/PendingRequirements/Export"]["get"]["req"],
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

const canPendingDownload = computed(() => {
  return !requirementsQuery.isLoading.value && agentAgencies.value.length > 0;
});

watch(
  requirementsQuery.data,
  (response, error) => {
    if (error) {
      agentSuggestions.value = [];
      agentAgencies.value = [];
    }
    if (response?.hierarchy) {
      agentAgencies.value = [];
      processAgentNames(response?.filters?.agentNames ?? []);
      processAgentAgencies(response?.hierarchy);
    }
  },
  { immediate: true },
);
</script>

<style lang="pcss">
.hierarchy__pending {
  &__top-controls {
    & .custom-select-container {
      float: left;
    }
    & .top-contols__download {
      margin-left: auto;
      @media (width <= 600px) {
        display: none;
      }
    }
    & .bottom-contols__download {
      margin: 1em auto 0;
      @media (width >= 600px) {
        display: none;
      }
    }
  }
  & .container-block__container {
    margin: 1rem 0;
  }
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

.clear-link {
  vertical-align: super;
  margin-left: var(--spacing-s);
  text-decoration: underline;
  cursor: pointer;
  font-size: 1rem;
}

.hierarchy__pending__hierarchy-table {
  @media (width <= 960px) {
    display: inline-block;
    width: calc(100vw - 2em);
  }
}

.top-contols__agent-select {
  & .custom-select-wrapper input {
    width: 400px;
  }
  @media (width <= 960px) {
    width: calc(100vw - 2em);
  }
}
</style>
