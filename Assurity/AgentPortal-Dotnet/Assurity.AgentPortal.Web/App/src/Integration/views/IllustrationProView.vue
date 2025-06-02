<template>
  <div class="illustration-pro__loading">
    <v-progress-circular v-if="isGettingAccountId" indeterminate />
    <AssurityIconDownload v-else color="white" />
  </div>
</template>

<script setup lang="ts">
import { onMounted, ref } from "vue";
import { useRoute } from "vue-router";
import { useUserStore } from "../../stores/userStore";
import { IllustrationProService } from "@assurity/newassurelink-client";
import { getIllustrationProUrl } from "../api";
const isGettingAccountId = ref(true);
const userStore = useUserStore();

onMounted(async () => {
  if (!userStore.user.isAuthenticated) {
    userStore.login();
  }

  try {
    const route = useRoute();
    const accountId =
      await IllustrationProService.getApiIllustrationProIntegrationAccountId();
    const illustrationProURL = await getIllustrationProUrl();
    const openInNew = route.params.new;
    const link = document.createElement("a");
    if (openInNew) link.target = "_";
    link.href = illustrationProURL + "api?TempAccountId=" + accountId;
    link.click();
    isGettingAccountId.value = false;
  } catch (error) {
    console.error(error);
    isGettingAccountId.value = false;
  }
});
</script>

<style lang="pcss" scoped></style>
