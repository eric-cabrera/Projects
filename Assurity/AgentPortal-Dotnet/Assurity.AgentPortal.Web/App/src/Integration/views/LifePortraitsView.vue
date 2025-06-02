<template>
  <div class="illustration-pro__loading">
    <v-progress-circular v-if="isGettingRedirectUrl" indeterminate />
    <AssurityIconDownload v-else color="white" />
  </div>
</template>

<script setup lang="ts">
import { onMounted, ref } from "vue";
import { useRoute } from "vue-router";
import { useUserStore } from "../../stores/userStore";
import { LifePortraitsService } from "@assurity/newassurelink-client";
const isGettingRedirectUrl = ref(true);
const userStore = useUserStore();

onMounted(async () => {
  if (!userStore.user.isAuthenticated) {
    userStore.login();
  }

  try {
    const route = useRoute();
    const illustrationURL =
      await LifePortraitsService.getApiLifePortraitsIntegrationRedirectUrl();
    const openInNew = route.params.new;
    const link = document.createElement("a");
    if (openInNew) link.target = "_";
    link.href = illustrationURL;
    link.click();
  } catch (error) {
    console.error(error);
    isGettingRedirectUrl.value = false;
  }
});
</script>
