<template>
  <div class="view__header hierarchy">
    <div class="view__header-left">
      <div class="view__heading">
        <h1 class="text-no-wrap view__title">Hierarchy</h1>
      </div>
      <aDropdownV2
        id="agentId"
        label="View As"
        bg-color="white"
        :value="viewAsAgentId"
        :options="viewAsAgentValues"
        @update:model-value="viewAsChange"
      />
    </div>
    <div class="view__header-right">
      <div>
        <strong>{{
          FormatHelpers.formatNumber(hierarchyStore.agentCounts.active)
        }}</strong>
        Active Agents
      </div>
      <div>
        <strong>{{
          FormatHelpers.formatNumber(hierarchyStore.agentCounts.jit)
        }}</strong>
        Just-In-Time Agents
        <AssurityIconTimer />
      </div>
    </div>
  </div>
  <div>
    <TabbedContent
      :initial-tab="0"
      :tabs="tabs"
      @onchange="$emit('onTabChange', $event)"
    ></TabbedContent>
  </div>
</template>

<script setup lang="ts">
import { computed, ref, watch } from "vue";
import aDropdownV2 from "@/components/forms/aDropdownV2.vue";
import TabbedContent from "@/components/TabbedContent.vue";
import HierarchyActiveTab from "@/Hierarchy/views/HierarchyActiveTab.vue";
import HierarchyPendingTab from "@/Hierarchy/views/HierarchyPendingTab.vue";
import AssurityIconTimer from "@/components/icons/AssurityIconTimer.vue";
import FormatHelpers from "@/Shared/utils/FormatHelpers";
import { useViewAsData } from "../api/hierarchyApi";
import type { Assurity_AgentPortal_Contracts_AgentContracts_DropdownOption as DropdownOption } from "@assurity/newassurelink-client";
import { useHierarchyStore } from "../hierarchyStore";

const hierarchyStore = useHierarchyStore();

defineEmits<{
  (e: "onTabChange", value: string): void;
}>();

const viewAsAgentId = ref("");

const viewAsAgents = ref<DropdownOption[]>([]);

const viewAsAgentValues = computed(() => {
  return viewAsAgents.value.map((agentContract) => {
    return {
      value: agentContract.displayValue ?? "",
      label: agentContract.displayValue ?? "",
    };
  });
});

const viewAsChange = (value: string | number | undefined) => {
  const viewAsAgent = viewAsAgents.value.find(
    (agent) => agent.displayValue === value,
  );
  hierarchyStore.filterOptions.agentLevel = viewAsAgent?.agentLevel ?? "";
  hierarchyStore.filterOptions.agentNumber = viewAsAgent?.agentNumber ?? "";
  hierarchyStore.filterOptions.companyCode = viewAsAgent?.companyCode ?? "";
  hierarchyStore.filterOptions.marketCode = viewAsAgent?.marketCode ?? "";
};

const tabs = [
  {
    label: "Active Hierarchy",
    id: "policyDetails", // this id lines up with the data object in the store.
    component: HierarchyActiveTab,
  },
  {
    label: "Pending Requirements",
    id: "writingAgentDetails", // this id lines up with the data object in the store.
    component: HierarchyPendingTab,
  },
];

const viewAsQuery = useViewAsData();

watch(
  viewAsQuery.data,
  (response: DropdownOption[]) => {
    if (response) {
      viewAsAgents.value = response;

      const viewAsAgent = response.at(0);

      viewAsAgentId.value = viewAsAgent?.displayValue ?? "";
      viewAsChange(viewAsAgentId.value);
    }
  },
  { immediate: true },
);
</script>

<style scoped lang="pcss">
@import "@/Shared/styles/status-summary.pcss";

.view_as_filter {
  background: #fff;
}

.view__header {
  display: flex;
  justify-content: space-between;
}

.view__header-right {
  color: var(--text-grey);

  & div:not(:last-child) {
    margin: 6px 0;
    border-bottom: 1px solid var(--bg-grey-dark);
  }

  & svg {
    margin: 0 0 -0.25em 0.25em;
    fill: var(--accent-color-lighter);
  }
  @media (width <= 960px) {
    display: none;
  }
}
</style>
<style>
.hierarchy {
  color: var(--text-grey);
}

.hierarchy__header {
  display: flex;
  justify-content: space-between;
}

.hierarchy__header__download {
  justify-content: end;
  display: flex;
  @media (width <= 960px) {
    display: none;
  }
}

.hierarchy__active .custom-select-wrapper {
  min-height: 52px;
  @media (width <= 960px) {
    min-height: 44px;
  }
}

.hierarchy__pending .custom-select-wrapper {
  min-height: 52px;
  @media (width <= 960px) {
    min-height: 44px;
  }
}

.hierarchy .custom-select-wrapper {
  min-height: 52px;
  width: 235px;
  @media (width <= 960px) {
    min-height: 44px;
    width: 50vw;
  }
}
.hierarchy .custom-select-options {
  z-index: 12;
}

.hierarchy__active .custom-select-wrapper input {
  @media (width <= 960px) {
    width: calc(100vw - 3em);
  }
}
.hierarchy__pending .custom-select-wrapper input {
  @media (width <= 960px) {
    width: calc(100vw - 5em);
  }
}
.hierarchy__footer__download {
  display: none;
  @media (width <= 960px) {
    display: block;
    padding: 1em 1em 0;
  }
}

.hierarchy__footer__download button {
  @media (width <= 960px) {
    width: 100%;
  }
}

.tabbed__content-container .tabs__nav-item {
  @media (width <= 960px) {
    font-size: 1em;
    margin-right: 0;
    padding: var(--spacing-l) 1em var(--spacing-s) 1em;
  }
}
</style>
