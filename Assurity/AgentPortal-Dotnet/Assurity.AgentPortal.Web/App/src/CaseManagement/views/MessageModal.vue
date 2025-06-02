<template>
  <PopoverModal
    :unique-id="uniqueId"
    :header-text="headerText"
    :icon="AssurityIconMailBills"
    :close-btn="true"
  >
    <span>{{ bodyText }}</span>
  </PopoverModal>
</template>
<script setup lang="ts">
import PopoverModal from "@/components/PopoverModal.vue";
import AssurityIconMailBills from "@/components/icons/AssurityIconMailBills.vue";
defineProps<{
  uniqueId: string;
  headerText?: string;
  bodyText?: string;
}>();
</script>
<style>
.case-management {
  &__status-modal {
    width: 100%;
    @media (width >= 480px) {
      width: 880px;
    }
    th {
      text-align: left;
      &:nth-child(2) {
        display: none;
        @media (width >= 480px) {
          display: table-cell;
        }
      }
    }
    td {
      display: flex;
      padding: 5px 0;
      border-bottom: 1px solid var(--bg-grey-dark);
      &:nth-child(1) {
        font-weight: bold;
      }
      @media (width >= 480px) {
        display: table-cell;
      }
    }
  }
}
</style>
