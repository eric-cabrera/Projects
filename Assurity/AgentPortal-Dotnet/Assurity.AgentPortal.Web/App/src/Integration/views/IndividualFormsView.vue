<template>
  <div v-if="isGettingIndividualForms" class="individualForms__loading">
    <v-progress-circular
      indeterminate
      size="95"
      width="8"
      color="#007b99"
      title="Loading..."
    >
    </v-progress-circular>
  </div>
  <form
    id="individualForms"
    method="POST"
    action="https://nettrac.ipipeline.com/sso/applicationstart.asp?FormXML=Y"
  >
    <input
      id="ApplicationDataXML"
      type="hidden"
      name="ApplicationDataXML"
      value=""
    />
    <input
      :id="`submitBtn-${submitBtnUUID}`"
      style="display: none"
      type="submit"
      value="submit"
    />
  </form>
</template>
<script setup lang="ts">
import { onMounted, ref } from "vue";
import { useRoute } from "vue-router";
import { individualForms } from "../api";
import { useUserStore } from "../../stores/userStore";

const submitBtnUUID = crypto.randomUUID();
const isGettingIndividualForms = ref(true);
const openInNew = ref();
const userStore = useUserStore();

onMounted(async () => {
  await getIndividualFormsXML();
});

async function getIndividualFormsXML() {
  if (!userStore.user.isAuthenticated) {
    userStore.login();
  }

  try {
    const route = useRoute();
    const result = await individualForms();
    document.forms[0].ApplicationDataXML.value = result;
    openInNew.value = route.params.new;
    setTimeout(function () {
      const submitButton = document.getElementById(
        `submitBtn-${submitBtnUUID}`,
      );
      submitButton?.click();
    }, 500);
    isGettingIndividualForms.value = false;
  } catch (error) {
    console.error(error);
    isGettingIndividualForms.value = false;
  }
}
</script>
<style lang="pcss" scoped>
.individualForms__loading {
  display: flex;
  justify-content: center;
  align-items: center;
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  padding: var(--spacing-xxxl) var(--spacing-xl) !important;
  z-index: var(--zindex-higher);
  backdrop-filter: none;
}
</style>
