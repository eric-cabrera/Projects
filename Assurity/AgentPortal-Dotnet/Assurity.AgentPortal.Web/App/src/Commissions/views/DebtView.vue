<template>
  <div class="view">
    <div class="view__header">
      <div class="view__heading">
        <h1 class="text-no-wrap view__title">Debt</h1>
      </div>
    </div>

    <div class="commission-details__container">
      <DebtDetails @on-tab-change="onTabChange" />
    </div>
  </div>
</template>
<script setup lang="ts">
import { onMounted, watch, ref } from "vue";
import DebtDetails from "@/commissions/views/DebtDetails.vue";
import { useCommissionDebtStore } from "@/stores/commissionDebtStore";

const commissionDebtStore = useCommissionDebtStore();

const isLoading = ref(false);

onMounted(async () => {
  await getUnsecuredAdvances(commissionDebtStore.unsecuredAdvancesQueryString);
});

let isUpdatingPage = false;
watch(
  () => [
    commissionDebtStore.debtUnsecuredAdvances.page,
    commissionDebtStore.debtUnsecuredAdvances.pageSize,
    commissionDebtStore.debtUnsecuredAdvances.orderBy,
    commissionDebtStore.debtUnsecuredAdvances.sortDirection,
  ],
  async () => {
    if (isUpdatingPage) {
      return;
    }
    await getUnsecuredAdvances(
      commissionDebtStore.unsecuredAdvancesQueryString,
    );
  },
);

watch(
  () => [
    commissionDebtStore.debtSecuredAdvances.page,
    commissionDebtStore.debtSecuredAdvances.pageSize,
    commissionDebtStore.debtSecuredAdvances.orderBy,
    commissionDebtStore.debtSecuredAdvances.sortDirection,
  ],
  async () => {
    if (isUpdatingPage) {
      return;
    }
    await getSecuredAdvances(commissionDebtStore.securedAdvancesQueryString);
  },
);

async function onTabChange(tabId: string) {
  switch (tabId) {
    case "debtUnsecuredAdvances":
      isUpdatingPage = true;
      commissionDebtStore.debtUnsecuredAdvances.page = 1;
      await getUnsecuredAdvances(
        commissionDebtStore.unsecuredAdvancesQueryString,
      );
      isUpdatingPage = false;
      break;
    case "debtSecuredAdvances":
      isUpdatingPage = true;
      commissionDebtStore.debtSecuredAdvances.page = 1;
      await getSecuredAdvances(commissionDebtStore.securedAdvancesQueryString);
      isUpdatingPage = false;
      break;
  }
}

async function getUnsecuredAdvances(
  filterQuery?: string,
  exportReport = false,
) {
  if (exportReport) {
    await commissionDebtStore.exportDebtUnsecuredAdvancesReport(
      commissionDebtStore.debtUnsecuredAdvances.page,
      commissionDebtStore.debtUnsecuredAdvances.pageSize,
      commissionDebtStore.debtUnsecuredAdvances.orderBy,
      filterQuery,
    );
  } else {
    await commissionDebtStore.getDebtUnsecuredAdvances(
      commissionDebtStore.debtUnsecuredAdvances.page,
      commissionDebtStore.debtUnsecuredAdvances.pageSize,
      commissionDebtStore.debtUnsecuredAdvances.orderBy,
      commissionDebtStore.debtUnsecuredAdvances.sortDirection,
      true,
      filterQuery,
    );
  }
}

async function getSecuredAdvances(filterQuery?: string, exportReport = false) {
  if (exportReport) {
    await commissionDebtStore.exportDebtSecuredAdvancesReport(
      commissionDebtStore.debtSecuredAdvances.page,
      commissionDebtStore.debtSecuredAdvances.pageSize,
      commissionDebtStore.debtSecuredAdvances.orderBy,
      filterQuery,
    );
  } else {
    await commissionDebtStore.getDebtSecuredAdvances(
      commissionDebtStore.debtSecuredAdvances.page,
      commissionDebtStore.debtSecuredAdvances.pageSize,
      commissionDebtStore.debtSecuredAdvances.orderBy,
      commissionDebtStore.debtSecuredAdvances.sortDirection,
      true,
      filterQuery,
    );
  }
}

watch(
  () => commissionDebtStore.unsecuredAdvancesQueryString,
  async (newValue) => {
    isLoading.value = true;
    isUpdatingPage = true;
    commissionDebtStore.debtUnsecuredAdvances.page = 1;
    await getUnsecuredAdvances(newValue);
    isLoading.value = false;
    isUpdatingPage = false;
  },
);

watch(
  () => commissionDebtStore.securedAdvancesQueryString,
  async (newValue) => {
    isLoading.value = true;
    isUpdatingPage = true;
    commissionDebtStore.debtSecuredAdvances.page = 1;
    await getSecuredAdvances(newValue);
    isLoading.value = false;
    isUpdatingPage = false;
  },
);
</script>
<style scoped lang="pcss">
@import "@/Shared/styles/status-summary.pcss";
.commission-details__container {
  margin-bottom: var(--spacing-xxl);
  position: relative;
}
</style>
<style lang="pcss">
.v-sheet.elevation-4 {
  box-shadow: var(--desktop-shadow) !important;
}
</style>
