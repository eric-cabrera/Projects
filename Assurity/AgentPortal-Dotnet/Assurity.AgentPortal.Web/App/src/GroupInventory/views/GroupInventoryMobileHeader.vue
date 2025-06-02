<template>
  <div>
    <div class="group-inventory__mobile-header" @click="navigateToDetails">
      {{ groupName }}
    </div>
    <button
      class="group-inventory__mobile-header-chevron"
      @click="navigateToDetails"
    >
      <AssurityIconChevronRight style="width: 40px" />
    </button>
  </div>
</template>
<script setup lang="ts">
import AssurityIconChevronRight from "@/components/icons/AssurityIconChevronRight.vue";
import { useRouter } from "vue-router";
const props = defineProps<{
  groupName?: string | null;
  groupNumber?: string | null;
}>();
const router = useRouter();

const navigateToDetails = () => {
  router.push({
    path: `/group-inventory/${props.groupNumber}`,
  });
};
</script>
<style>
.group-inventory__mobile-header {
  font-size: 1.2em;
  font-weight: bold;
  float: left;
  cursor: pointer;

  &-chevron {
    float: right;
    margin-top: -8px;
  }
}
</style>
