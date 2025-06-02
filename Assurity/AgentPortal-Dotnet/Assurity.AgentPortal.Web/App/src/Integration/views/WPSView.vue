<template>
  <div class="wps-pro__loading">
    <v-progress-circular v-if="isGettingWPS" indeterminate />
    <AssurityIconDownload v-else color="white" />
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from "vue";
import { useRoute } from "vue-router";
import { getWPSUrl } from "../api";
import { useUserStore } from "../../stores/userStore";
const isGettingWPS = ref(true);
const userStore = useUserStore();

onMounted(async () => {
  if (!userStore.user.isAuthenticated) {
    userStore.login();
  }

  try {
    const route = useRoute();
    const openInNew = route.params.new;
    const link = document.createElement("a");
    if (openInNew) link.target = "_";
    link.href = await getWPSUrl();
    link.click();
    isGettingWPS.value = false;
  } catch (error) {
    console.error(error);
    isGettingWPS.value = false;
  }
});
</script>
