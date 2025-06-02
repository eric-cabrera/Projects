<template>
  <div class="commission_debt__details-tabs">
    <TabbedContent
      :initial-tab="0"
      :tabs="tabs"
      @onchange="$emit('onTabChange', $event)"
    ></TabbedContent>
  </div>
</template>

<script setup lang="ts">
import TabbedContent from "@/components/TabbedContent.vue";
import DebtSecuredAdvancesDetailsTab from "@/commissions/views/DebtSecuredAdvancesDetailsTab.vue";
import DebtUnsecuredAdvancesDetailsTab from "@/commissions/views/DebtUnsecuredAdvancesDetailsTab.vue";

defineEmits<{
  (e: "onTabChange", value: string): void;
}>();

const tabs = [
  {
    id: "debtUnsecuredAdvances",
    label: "Unsecured Advances",
    component: DebtUnsecuredAdvancesDetailsTab,
  },
  {
    id: "debtSecuredAdvances",
    label: "Secured Advances",
    component: DebtSecuredAdvancesDetailsTab,
  },
];
</script>

<style scoped lang="pcss">
.commission_debt__details-tabs {
  @media (width < 600px) {
    margin-left: calc(var(--spacing) * -1);
    margin-right: calc(var(--spacing) * -1);
  }
}
</style>
