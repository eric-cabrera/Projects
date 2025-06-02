<template>
  <div class="commission__details-tabs">
    <TabbedContent
      :initial-tab="0"
      :tabs="tabs"
      @onchange="$emit('onTabChange', $event)"
    ></TabbedContent>
  </div>
</template>
<script setup lang="ts">
import TabbedContent from "@/components/TabbedContent.vue";
import CommissionPolicyDetailsTab from "@/Commissions/views/CommissionPolicyDetailsTab.vue";
import CommissionWritingAgentTab from "@/Commissions/views/CommissionWritingAgentTab.vue";

defineEmits<{
  (e: "onTabChange", value: string): void;
}>();

const tabs = [
  {
    label: "With Policy Details",
    id: "policyDetails", // this id lines up with the data object in the store.
    component: CommissionPolicyDetailsTab,
  },
  {
    label: "By Writing Agent",
    id: "writingAgentDetails", // this id lines up with the data object in the store.
    component: CommissionWritingAgentTab,
  },
];
</script>
<style scoped lang="pcss">
.commission__details-tabs {
  @media (width < 600px) {
    margin-left: calc(var(--spacing) * -1);
    margin-right: calc(var(--spacing) * -1);
  }
}
</style>
