<template><div></div></template>
<script setup lang="ts">
import { useRoute, useRouter } from "vue-router";

const router = useRouter();
const route = useRoute();

let routeName = "forms-launcher-individual";
if (route.fullPath.includes("individual")) {
  routeName = "forms-launcher-individual";
} else {
  routeName = "forms-launcher-worksite";
}

const newRoute = router.resolve({
  name: routeName,
});

import { onMounted } from "vue";

onMounted(async () => {
  window.open(newRoute.href, "_blank");
});
</script>
<style lang="pcss" scoped></style>
