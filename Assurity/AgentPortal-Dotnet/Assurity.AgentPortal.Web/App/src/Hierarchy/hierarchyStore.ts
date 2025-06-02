import { defineStore } from "pinia";
import type { HierarchyFilterOptions } from "./models/HierarchyFilterOptions";
import type { HierarchyAgentCounts } from "./models/HierarchyAgentCounts";
import { reactive } from "vue";

export const useHierarchyStore = defineStore("hierarchy", () => {
  const defaultFilterOptions: HierarchyFilterOptions = {
    agentLevel: "",
    agentNumber: "",
    marketCode: "",
    companyCode: "",
    contractStatus: "",
  };

  const defaultAgentCounts: HierarchyAgentCounts = {
    active: 0,
    jit: 0,
    pending: 0,
  };

  const filterOptions = reactive({
    ...defaultFilterOptions,
  });
  const agentCounts: HierarchyAgentCounts = reactive({
    ...defaultAgentCounts,
  });

  function resetHierarchyStore() {
    Object.assign(filterOptions, defaultFilterOptions);
    Object.assign(agentCounts, defaultAgentCounts);
  }
  return {
    filterOptions,
    agentCounts,
    resetHierarchyStore,
  };
});
