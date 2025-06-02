<template>
  <div class="direct-deposit__container">
    <v-sheet :class="{ 'elevation-4 rounded-lg': !isMobile }">
      <div class="direct-deposit__content">
        <h2 class="direct-deposit__header" data-test="header">
          {{ props.title }}
        </h2>
        <AHorizontalRule />
        <div
          v-for="link in props.links"
          :key="link.link"
          class="direct-deposit__link-row"
        >
          <a :href="link.link" target="_blank" data-test="link">{{
            link.text
          }}</a>
        </div>
      </div>
    </v-sheet>
  </div>
</template>
<script setup lang="ts">
import AHorizontalRule from "@/components/aHorizontalRule.vue";

interface LinkItem {
  text: string;
  link: string;
}

const props = defineProps<{
  title: string;
  links: LinkItem[];
  isMobile: boolean;
}>();
</script>
<style scoped>
.direct-deposit__container {
  margin-top: 15px;
}
.direct-deposit__content {
  padding: 24px 40px;
}
.direct-deposit__link-row {
  padding-top: 8px;
}
.direct-deposit__header {
  padding-bottom: 8px;
  color: var(--text-grey);
}
.v-sheet.elevation-4 {
  box-shadow: var(--desktop-shadow) !important;
}
</style>
