<template>
  <AButton
    v-if="data && data.hasAccess"
    button-style="primary"
    size="standard"
    text="AGENT E-CONTRACTING"
    class="col-span-7 vertafore-redirect-button"
    :disabled="data === null"
    @click="redirect()"
  ></AButton>
  <ErrorComponent v-if="isError" variant="error" />
</template>
<script setup lang="ts">
import AButton from "@/components/forms/aButton.vue";
import { useQuery } from "@tanstack/vue-query";
import axios from "axios";
import ErrorComponent from "../../components/ErrorComponent.vue";

type VertaforeRedirectUrlResponse = {
  hasAccess: boolean;
  redirectUrl: string;
};
async function fetchUrl(): Promise<VertaforeRedirectUrlResponse | null> {
  const response = await axios.get<VertaforeRedirectUrlResponse>(
    "/API/UserData/VertaforeInformation",
    {
      responseType: "json",
    },
  );

  if (response.status == 200) {
    return response.data;
  }
  return null;
}

const { data, isError } = useQuery({
  queryKey: [],
  staleTime: 300000,
  queryFn: () => fetchUrl(),
});

const redirect = () => {
  const link = document.createElement("a");
  link.target = "_";
  link.href = data.value?.redirectUrl ?? "";
  link.click();
};
</script>
<style scoped lang="pcss">
button {
  &.assurity-button--standard {
    max-width: 300px;
    padding: 15px var(--spacing-xl);
  }
}
</style>
