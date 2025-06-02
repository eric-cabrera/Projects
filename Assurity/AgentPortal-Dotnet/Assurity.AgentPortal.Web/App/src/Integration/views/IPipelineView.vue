<template>
  <div v-if="isGettingSAMLResponse" class="ipipeline__loading">
    <v-progress-circular
      indeterminate
      size="95"
      width="8"
      color="#007b99"
      title="Loading..."
    >
    </v-progress-circular>
  </div>
  <ErrorComponent v-if="isUnauthorizedResponse" variant="error" />
  <div
    v-else-if="viewAsAgentValues.length > 1"
    class="viewAsDropdown_container"
  >
    <aDropdownV2
      id="agentId"
      label="View As"
      bg-color="white"
      class="viewAsDropdown"
      :options="viewAsAgentValues"
      @update:model-value="viewAsChange"
    />
  </div>
  <form
    id="samlpost"
    :target="openInNew ? '_blank' : undefined"
    method="post"
    :action="iPipelineConnection"
  >
    <input type="hidden" name="SAMLResponse" :value="iPipelineSAMLResponse" />
    <input type="hidden" name="TARGET" :value="iPipelineTarget" />
    <input type="hidden" name="RelayState" :value="iPipelineTarget" />
    <input
      :id="`submitBtn-${submitBtnUUID}`"
      style="display: none"
      type="submit"
      value="submit"
    />
  </form>
</template>
<script setup lang="ts">
import { computed, onMounted, ref } from "vue";
import { useRoute } from "vue-router";
import { useUserStore } from "@/stores/userStore";
import aDropdownV2 from "@/components/forms/aDropdownV2.vue";
import {
  type Assurity_AgentPortal_Contracts_AgentContracts_DropdownOption as DropdownOption,
  type Assurity_AgentPortal_Contracts_Integration_IPipelineResponse as IPipelineResponse,
  IpipelineService,
} from "@assurity/newassurelink-client";
import { getViewAsAgents } from "../api";

const userStore = useUserStore();
const submitBtnUUID = crypto.randomUUID();
const isGettingSAMLResponse = ref(false);
const isUnauthorizedResponse = ref(false);
const iPipelineSAMLResponse = ref("");
const iPipelineTarget = ref("");
const iPipelineConnection = ref("");
const openInNew = ref();
const agents = ref<DropdownOption[]>([]);
const route = useRoute();

onMounted(async () => {
  if (!userStore.user.isAuthenticated) {
    userStore.login();
  }
  if (userStore.user.agentId.length > 0) {
    agents.value = await getViewAsAgents();
  }

  if (viewAsAgentValues.value?.length === 1) {
    await viewAsChange(viewAsAgentValues.value[0].value);
  } else if (userStore.user.isHomeOffice && !userStore.user.agentId) {
    await getIPipelineXML("");
  } else if (
    userStore.user?.agentData?.additionalAgents?.length > 0 &&
    userStore.user?.agentData?.additionalAgents[0].agentIds?.length <= 1
  ) {
    await getIPipelineXML(
      userStore.user.agentData.additionalAgents[0].agentIds[0],
    );
  }
});

type AgentValue = { value: string; label: string };

const viewAsAgentValues = computed((): AgentValue[] => {
  const seen = new Set<string>();
  return agents.value.reduce<AgentValue[]>((unique, agentContract) => {
    const agentNumber = agentContract.agentNumber ?? "";
    if (!seen.has(agentNumber)) {
      seen.add(agentNumber);
      unique.push({ value: agentNumber, label: agentNumber });
    }
    return unique;
  }, []);
});

const viewAsChange = async (agent: string | number | undefined) => {
  await getIPipelineXML(agent as string);
};

async function getIPipelineXML(agent: string) {
  try {
    isGettingSAMLResponse.value = true;

    const ipipelineResponse: IPipelineResponse =
      await IpipelineService.getApiIpipelineIntegrationIpipelineResponse({
        agentId: agent,
      });
    iPipelineSAMLResponse.value = ipipelineResponse.samlResponse ?? "";
    iPipelineTarget.value = ipipelineResponse.iPipelineTargetString ?? "";
    iPipelineConnection.value =
      ipipelineResponse.iPipelineConnectionString ?? "";
    openInNew.value = route.params.new;
    setTimeout(function () {
      const submitButton = document.getElementById(
        `submitBtn-${submitBtnUUID}`,
      );
      submitButton?.click();
    }, 500);
    isGettingSAMLResponse.value = false;
  } catch (error) {
    console.error(error);
    isGettingSAMLResponse.value = false;
    isUnauthorizedResponse.value = true;
  }
}
</script>
<style lang="pcss" scoped>
.ipipeline__loading {
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
.viewAsDropdown_container {
  display: flex;
  justify-content: center;
  margin-top: 25vh;
}

.viewAsDropdown {
  width: 300px;
}
</style>
