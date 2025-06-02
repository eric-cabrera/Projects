<template>
  <div class="contracts-form col-span-12">
    <aHorizontalRule />
    <div class="form">
      <aDropdownV2
        id="agent-number"
        label="Agent Number"
        :always-visible="true"
        :value="agentNumberQuery"
        :clearable="true"
        :options="agentNumberOptions"
        @update:model-value="(newValue) => (agentNumberQuery = `${newValue}`)"
        @update:clear="() => clearArgs()"
      />
      <aDropdownV2
        id="market-code"
        label="Market Code"
        :disabled="!agentNumberQuery"
        :always-visible="true"
        :value="marketCodeQuery"
        :clearable="true"
        :options="marketCodeOptions"
        @update:model-value="(newValue) => (marketCodeQuery = `${newValue}`)"
        @update:clear="() => clearArgs()"
      />
      <AButton
        button-style="primary"
        size="standard"
        text="SUBMIT"
        @click="querySubmitted"
      ></AButton>
    </div>
    <div v-if="listData" class="document-list">
      <AButton
        v-for="document in listData"
        :key="document.id"
        size="standard"
        button-style="tertiary"
        :text="document.displayName"
        @click="() => downloadDocument(document)"
      ></AButton>
      <div
        v-if="
          !listDataFetching && listDataQuerySubmitted && listData.length < 1
        "
      >
        <p>There is no data available for display.</p>
      </div>
    </div>
    <ErrorComponent v-if="contractsError || listError" variant="error" />
    <div v-if="isDownloadingFile" class="download-message">
      <v-progress-circular class="progress-circle" indeterminate />
      <h4>Downloading Contract</h4>
    </div>
  </div>
</template>
<script setup lang="ts">
import { useUserStore } from "@/stores/userStore";
import axios from "axios";

import { useQuery } from "@tanstack/vue-query";

import { computed, ref } from "vue";

import { UserDataService } from "@assurity/newassurelink-client";
import type { Assurity_AgentPortal_Contracts_AgentContracts_AgentContractsResponse as AgentContractsResponse } from "@assurity/newassurelink-client";

import AButton from "@/components/forms/aButton.vue";
import aDropdownV2 from "@/components/forms/aDropdownV2.vue";
import aHorizontalRule from "@/components/aHorizontalRule.vue";
import { FileHelper } from "@/Shared/utils/FileHelper";
import ErrorComponent from "../../components/ErrorComponent.vue";

const userStore = useUserStore();

const agentNumberQuery = ref("");
const marketCodeQuery = ref("");
const queryArgs = ref({ AgentNumber: "", MarketCode: "" });
const isDownloadingFile = ref(false);
const listDataQuerySubmitted = ref(false);

const clearArgs = () => {
  agentNumberQuery.value = "";
  marketCodeQuery.value = "";
  queryArgs.value = { AgentNumber: "", MarketCode: "" };
  listDataQuerySubmitted.value = false;
};

const querySubmitted = () => {
  queryArgs.value.AgentNumber = agentNumberQuery.value;
  queryArgs.value.MarketCode = marketCodeQuery.value;
  listDataQuerySubmitted.value = true;
};

const { data: agentData, error: contractsError } =
  useQuery<AgentContractsResponse>({
    queryKey: [userStore.user.agentId],
    staleTime: 300000,
    queryFn: () =>
      UserDataService.getAgentContracts({
        includeAssociatedAgentNumbers: true,
        marketCodeFilter: "AgentCenter",
      }),
  });

type Contract = { id: string; displayName: string; downloadName: string };

async function fetchList(
  agentNumber: string,
  marketCode: string,
): Promise<Contract[] | null> {
  const agentLevels =
    agentData.value?.agentContracts?.[agentNumber]?.[marketCode];
  if (!agentLevels) return null;

  const response = await axios({
    url: "/Content/Contracts",
    method: "post",
    data: {
      marketCode: marketCode,
      agentLevels: agentLevels,
    },
  });

  if (response.status == 200) {
    return response.data;
  }
  return null;
}

const {
  data: listData,
  isFetching: listDataFetching,
  error: listError,
} = useQuery({
  queryKey: [queryArgs],
  staleTime: 300000,
  queryFn: () =>
    fetchList(queryArgs.value.AgentNumber, queryArgs.value.MarketCode),
});

const agentNumberOptions = computed(() => {
  return Object.keys(agentData.value?.agentContracts ?? {});
});

const marketCodeOptions = computed(() => {
  return Object.keys(
    agentData.value?.agentContracts?.[agentNumberQuery.value] ?? {},
  );
});

const downloadDocument = async (item: {
  displayName: string;
  id: string;
  downloadName: string;
}) => {
  try {
    isDownloadingFile.value = true;
    const response = await axios.get<{ fileContents: string }>(
      `/Content/File/${item.id}`,
      {
        responseType: "json",
      },
    );

    const byteChars = atob(response.data.fileContents);
    const byteArray = new Uint8Array(byteChars.length);
    for (let i = 0; i < byteChars.length; i++) {
      byteArray[i] = byteChars.charCodeAt(i);
    }
    const blob = new Blob([byteArray]);

    const file = new FileHelper(item.downloadName, blob);
    file.downloadFile();
  } catch (error) {
    console.error(error);
  }
  isDownloadingFile.value = false;
};
</script>
<style scoped lang="pcss">
.form {
  display: flex;
  column-gap: var(--spacing-s);
  justify-content: stretch;

  @media (width < 960px) {
    flex-direction: column;
    row-gap: var(--spacing-s);
  }

  @media (width < 1560px) {
    .assurity-button {
      min-width: 100px;
    }
  }
}

hr {
  margin: var(--spacing-s) 0 var(--spacing-xl);
}

select {
  appearance: none;
  -webkit-appearance: none;
  width: 100%;
  font-size: 1.125rem;
  line-height: 1;
  padding: 15px 42px 15px var(--spacing);
  background-color: #fff;
  border: 1px solid var(--text-grey);
  border-radius: 0.25rem;
  color: #000;
  cursor: pointer;
}

.form {
  margin: 0 0 var(--spacing-xl);
}

.form .custom-select-container {
  min-width: 200px;
}

:deep(.custom-select-wrapper) {
  padding: 15px var(--spacing-l);
}

.form button {
  padding: 15px var(--spacing-xl);
}

.document-list button {
  margin: 0 0 var(--spacing);
}

:deep(.document-list button) {
  &::before {
    display: inline-block;
    content: "";
    background: url(/src/assets/Agent-Portal-Icon_PDF-Blue 1.svg);
    background-size: 22px 22px;
    width: 22px;
    height: 22px;
    margin: 0 var(--spacing) 0 0;
  }
}

:deep(.document-list .assurity-button) {
  display: flex;
  align-items: start;
}

.download-message {
  display: flex;
  text-wrap: nowrap;
}

:deep(.assurity-button__text) {
  text-align: left;
  font-size: 18px;
  text-transform: capitalize;
}
</style>
