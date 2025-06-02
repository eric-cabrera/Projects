<template>
  <div class="awps_loading">
    <v-progress-circular v-if="loading" indeterminate />
    <AssurityIconDownload v-else color="white" />
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from "vue";
import { getViewAsAgents, setCookiesAndGetAwpsUrl } from "../api";
import { useUtilityStore } from "../../stores/utilityStore";
import { useUserStore } from "../../stores/userStore";
import { type Assurity_AgentPortal_Contracts_AgentContracts_DropdownOption as DropdownOption } from "@assurity/newassurelink-client";

interface Agent {
  displayValue: string;
  agentNumber: string;
  marketCode: string;
  companyCode: string;
  agentLevel: string;
}

const viewAsAgents = ref<Agent[]>([]);
const errorMessage = ref<string | null>(null);
const wsAgentId = ref<string | null>(null);
const wsAgentIdEncrypted = ref<string | null>(null);
const utilityStore = useUtilityStore();
const userStore = useUserStore();
const loading = ref<boolean>(true);

const fetchData = async () => {
  if (!userStore.user.isAuthenticated) {
    userStore.login();
  }

  try {
    if (userStore.user.isHomeOffice && userStore.user.agentId === "") {
      await setCookiesAndGetAwpsUrl("HO9999");
      return;
    }

    const apiData = await getViewAsAgents();
    viewAsAgents.value = apiData.map((agent: DropdownOption) => ({
      displayValue: agent.displayValue ?? "",
      agentNumber: agent.agentNumber ?? "",
      marketCode: agent.marketCode ?? "",
      companyCode: agent.companyCode ?? "",
      agentLevel: agent.agentLevel ?? "",
    }));

    const result = getWSAgentId(viewAsAgents.value);
    wsAgentId.value = result.wsAgentId;
    if (wsAgentId.value) {
      await setCookiesAndGetAwpsUrl(wsAgentId.value);
    } else {
      wsAgentIdEncrypted.value = null;
    }
    errorMessage.value = result.errorMessage;
  } catch (error) {
    console.error("Error fetching data:", error);
    errorMessage.value = "Error fetching agent data.";
  } finally {
    loading.value = false;
  }
};

onMounted(() => {
  fetchData();
});

const getWSAgentId = (agents: Agent[]) => {
  const wsAgents = agents.filter((agent) => agent.marketCode.includes("WS"));
  if (wsAgents.length === 0) {
    utilityStore.httpStatus.unauthorized = true;
    return {
      errorMessage: "No agent with MarketCode 'WS' found.",
      wsAgentId: null,
    };
  } else {
    const sortedWsAgents = wsAgents.sort((a, b) =>
      String(a.agentNumber).localeCompare(String(b.agentNumber)),
    );
    return {
      errorMessage: null,
      wsAgentId: sortedWsAgents[0]?.agentNumber ?? null,
    };
  }
};
</script>
