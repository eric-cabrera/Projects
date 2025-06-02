<template>
  <v-row>
    <v-col v-for="(item, index) in props.items" :key="index" cols="3">
      <div class="v-card__content__label">{{ item.label }}</div>
      <div class="v-card__content__value">{{ item.value }}</div>
    </v-col>
  </v-row>
</template>

<script setup lang="ts">
const props = defineProps({
  items: {
    type: Array as () => Array<{ label: string; value: string }>,
    required: true,
  },
});
</script>
<style scoped lang="pcss">
.v-card__content__label {
  font-size: 16px;
  font-weight: 600;
  color: #4a4a4a;
  margin-bottom: 0;
}
.v-card__content__value {
  font-size: 16px;
  font-weight: 300;
  color: #4a4a4a;
}
</style>
