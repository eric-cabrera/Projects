<template>
  <div class="view__filters">
    <aFilterDrawer
      ref="caseManagementFiltersDrawer"
      v-model:is-filter-tray-open="filterDrawerOpen"
      v-model:filters="filters"
      :date-compare="false"
      @created-date-change="createdDateChange"
      @filter-change="filterChange"
    />
  </div>
  <DynamicContent full-width>
    <template #header>
      <div class="case-management__header-wrapper col-span-12">
        <div class="case-management__header-left">
          <h1 class="heading--1">Case Management</h1>
        </div>
        <div
          ref="startNewAppRef"
          class="case-management__header__start-new-app"
        >
          <aButton
            id="start-new-app__button"
            button-style="primary"
            size="s"
            class="start-new-app"
            :text="`Start New App`"
            aria-haspopup="true"
            aria-controls="menu"
            @click="toggleDropdown"
          >
            <template #append>
              <AssurityIconDropdown class="start-new-app__icon" color="white" />
            </template>
          </aButton>
          <div
            v-if="startNewAppOpen"
            class="start-new-app__menu"
            aria-labelledby="start-new-app__button"
          >
            <ul class="start-new-app__links" role="menu">
              <li role="menuitem">
                <a
                  :href="startNewAppEnviron + 'Agent-Accident'"
                  target="_blank"
                  @click="startNewAppOpen = false"
                  >Accident</a
                >
              </li>
              <li role="menuitem">
                <a
                  :href="startNewAppEnviron + 'Agent-AccidentalDeath'"
                  target="_blank"
                  @click="startNewAppOpen = false"
                  >Accidental Death</a
                >
              </li>
              <li role="menuitem">
                <a
                  :href="startNewAppEnviron + 'Agent-CenturyDI'"
                  target="_blank"
                  @click="startNewAppOpen = false"
                  >Century+ DI</a
                >
              </li>

              <li role="menuitem">
                <a
                  :href="startNewAppEnviron + 'Agent-CriticalIllness'"
                  target="_blank"
                  @click="startNewAppOpen = false"
                  >Critical Illness</a
                >
              </li>

              <li role="menuitem">
                <a
                  :href="startNewAppEnviron + 'Agent-IncomeProtection'"
                  target="_blank"
                  @click="startNewAppOpen = false"
                  >Income Protection</a
                >
              </li>

              <li role="menuitem">
                <a
                  :href="startNewAppEnviron + 'Agent-TermLife'"
                  target="_blank"
                  @click="startNewAppOpen = false"
                  >Term Life</a
                >
              </li>
            </ul>
          </div>
        </div>
      </div>
      <div class="title-sub-text">
        Summary and details of applications started or submitted through
        Assurity’s E-application platform
      </div>
    </template>
    <div class="case-management__header-wrapper col-span-12">
      <div class="case-management__header-left">
        <aDropdownV2
          id="view-as"
          class="filters__header-view-as"
          label="View As"
          bg-color="white"
          :value="viewAsAgentId"
          :clearable="true"
          :options="filterItems.viewAsAgentValues"
          @update:model-value="viewAsChange"
        />
        <!-- v-if="!filtersQuery.isLoading.value" -->
        <aButton
          button-style="primary"
          :text="`FILTER ${filterCount ? `(${filterCount})` : ''}`"
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
          class="case-management__header-selected-filters"
        >
          <aSelectedFilters
            :selected-filters="selectedFilters"
            :selected-filter-count="selectedFilterCount"
            @reset-filters="resetFilters"
            @filter-value-changed="onRemoveSelection"
          />
        </div>
      </div>
    </div>

    <div class="content__container assurity-grid col-span-12">
      <CaseManagementTable />
    </div>
  </DynamicContent>
</template>

<script setup lang="ts">
import {
  ref,
  computed,
  reactive,
  onMounted,
  onUnmounted,
  watchEffect,
} from "vue";
import dayjs from "dayjs";
import DynamicContent from "@/layouts/DynamicContent.vue";
import aButton from "@/components/forms/aButton.vue";
import aDropdownV2 from "@/components/forms/aDropdownV2.vue";
import aFilterDrawer from "@/components/aFilterDrawer/aFilterDrawer.vue";
import aSelectedFilters from "@/components/aFilterDrawer/aSelectedFilters.vue";
import AssurityIconFilter from "@/components/icons/AssurityIconFilter.vue";
import CaseManagementTable from "@/CaseManagement/views/CaseManagementTable.vue";
import AssurityIconDropdown from "@/components/icons/AssurityIconDropdown.vue";
import type { aFilterItem } from "@/components/aFilterDrawer/models/aFilterItem";
import { aFilterType } from "@/components/aFilterDrawer/models/enums/aFilterType";
import { aFilterField } from "@/components/aFilterDrawer/models/enums/aFilterField";
import type { aFilterData } from "@/components/aFilterDrawer/models/aFilterData";
import { CaseManagementFilterData } from "../models/CaseManagementFilterData";
import EnvironHelpers from "@/Shared/utils/environHelpers";
import { useCaseManagementStore } from "../caseManagementStore";
import { useCaseManagementFilterOptions } from "@/CaseManagement/api/caseManagementApi";
import CaseManagementFilterHelpers from "@/CaseManagement/utils/caseManagementFilterHelpers";
import FormatHelpers from "@/Shared/utils/FormatHelpers";
import type { Assurity_AgentPortal_Contracts_CaseManagement_CaseManagementFilters as CaseManagementFilters } from "@assurity/newassurelink-client";

const caseManagementStore = useCaseManagementStore();

const environ = EnvironHelpers.getEnviron();

const startNewAppEnviron = ref(`https://${environ}quickstart.assurity.com/`);

const filterDrawerOpen = ref(false);

const caseManagementFiltersDrawer = ref<typeof aFilterDrawer | null>();

const filters = ref<aFilterData[]>([]);

const filterModel = new CaseManagementFilterData();

filters.value = filterModel.caseManagementFilters;

const filterItems = reactive({
  viewAsAgentValues: ["Loading..."] as string[],
});

const { data } = useCaseManagementFilterOptions();

const processFilterOptions = (data: CaseManagementFilters) => {
  const primaryInsuredNameFilter = filters.value.find(
    (item: aFilterData) => item.field === aFilterField.PrimaryInsuredName,
  );
  if (primaryInsuredNameFilter) {
    primaryInsuredNameFilter.items = data.primaryInsuredNames?.map((name) => {
      name = FormatHelpers.capitalCase(name);
      return {
        id: name,
        text: name,
      };
    });
  }
  filterItems.viewAsAgentValues = data.viewAsAgentIds ?? [];
};

watchEffect(() => {
  if (data.value) {
    processFilterOptions(data.value);
  }
});

const viewAsAgentId = computed(() => {
  const viewAsFilter = filters.value.find(
    (filter) => filter.field === aFilterField.ViewAsAgent,
  );
  return (
    viewAsFilter?.selection && viewAsFilter?.selection?.length > 0
      ? viewAsFilter?.selection[0].id
      : ""
  ) as string | number | undefined;
});

const filterCount = computed(() => {
  let count = 0;
  filters.value.forEach((filter) => {
    switch (filter.type) {
      case aFilterType.DateStaticItems:
        if (filter.toDate) {
          count++;
        }
        break;
      default:
        count += filter.selection?.length ?? 0;
    }
  });
  return count;
});

const startNewAppOpen = ref(false);
const startNewAppRef = ref<HTMLDivElement | null>();

const toggleDropdown = () => {
  startNewAppOpen.value = !startNewAppOpen.value;
};

const handleClickOutside = (event: MouseEvent) => {
  if (
    startNewAppRef.value &&
    !startNewAppRef.value.contains(event.target as Node) &&
    event.target !== document.getElementById("start-new-app__button")
  ) {
    startNewAppOpen.value = false;
  }
};

onMounted(() => {
  document.addEventListener("click", handleClickOutside);
});

onUnmounted(() => {
  document.removeEventListener("click", handleClickOutside);
});

const viewAsChange = (value: string | number | undefined) => {
  const viewAsFilter = filters.value.find(
    (item: aFilterData) => item.field === aFilterField.ViewAsAgent,
  );

  let newSelection: aFilterItem[] | null = [] as aFilterItem[];

  if (value) {
    const newValue: string = value.toString();
    newSelection = [
      {
        id: newValue,
        text: newValue,
        option: "",
        level: 0,
      },
    ];
    caseManagementStore.filterOptions.viewAsAgentId = newValue;
  } else {
    caseManagementStore.filterOptions.viewAsAgentId = "";
    newSelection = null;
  }

  if (viewAsFilter !== undefined) {
    viewAsFilter.selection = newSelection;
  }
};

const createdDateChange = (filters: aFilterData[]) => {
  let createdDatesFilter = filters.find(
    (filter) => filter.type === aFilterType.CreatedDates,
  );

  if (!createdDatesFilter || !createdDatesFilter.createdDateBegin) {
    createdDatesFilter = {
      display: true,
      text: "Created Date",
      type: aFilterType.DateStaticItems,
      field: aFilterField.CreatedDates,
      selection: null as aFilterItem[] | null,
      items: null as aFilterItem[] | null,
      loading: false,
      active: true,
      createdDateBegin: dayjs().startOf("year"),
      createdDateEnd: dayjs(),
    };
  }
  const newFilterOptions =
    CaseManagementFilterHelpers.mapCreatedDateSelections(createdDatesFilter);
  caseManagementStore.filterOptions = {
    ...caseManagementStore.filterOptions,
    ...newFilterOptions,
  };
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

  const newFilterOptions = CaseManagementFilterHelpers.mapSelections(
    selections.filter((item) => item.selection !== null),
  );

  caseManagementStore.filterOptions = {
    ...caseManagementStore.filterOptions,
    ...newFilterOptions,
  };
};

const selectedFilters = computed(() => {
  if (caseManagementFiltersDrawer.value) {
    return caseManagementFiltersDrawer.value.selectedFilters || [];
  }
  return [];
});

const selectedFilterCount = computed(() => {
  if (caseManagementFiltersDrawer.value) {
    return caseManagementFiltersDrawer.value.selectedFilterCount;
  }
  return 0;
});

function onRemoveSelection(value: {
  filterName: string;
  itemId: string | null | undefined;
}) {
  if (caseManagementFiltersDrawer.value) {
    caseManagementFiltersDrawer.value.onRemoveSelectionEmitted(value);
  }
  return [];
}

function resetFilters() {
  if (caseManagementFiltersDrawer.value) {
    caseManagementFiltersDrawer.value.resetFilters();
  }
  return [];
}
</script>

<style scoped lang="pcss">
@import "case-management-view.pcss";
</style>
<style>
.start-new-app .assurity-button__text {
  @media (width < 960px) {
    width: 100%;
  }
}
.case-management__header__start-new-app {
  align-self: flex-end;
  padding-bottom: 0.5em;
  width: 100%;

  @media (width >= 960px) {
    width: auto;
  }
}
.custom-select-wrapper {
  height: 52px;
}
</style>
