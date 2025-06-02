import { defineStore } from "pinia";
import { ref } from "vue";
import type { CaseManagementFilterOptions } from "./models/CaseManagementFilterOptions";

export const useCaseManagementStore = defineStore("caseManagement", () => {
  const filterOptions = ref<CaseManagementFilterOptions>({});
  const resetCaseManagementStore = () => {
    filterOptions.value = {};
  };
  return {
    filterOptions,
    resetCaseManagementStore,
  };
});
