<template>
  <div class="action">
    <h2 class="action__title">My Action Items</h2>
    <DataTable
      v-model:header-rows="headers"
      :data-rows="agentRequirements"
      :more-pages="false"
      :less-pages="false"
      :error="false"
      :loading="false"
      page-indicator=""
      class="policy-actions-table"
      @column-selected="selectAction($event)"
    ></DataTable>
    <h2 class="action__title">Home Office</h2>
    <DataTable
      v-model:header-rows="headersHomeOffice"
      :data-rows="homeOfficeRequirements"
      :more-pages="false"
      :less-pages="false"
      :error="false"
      :loading="false"
      page-indicator=""
      class="policy-actions-table"
    ></DataTable>
  </div>
  <AssurityDrawer
    v-model="isDrawerOpen"
    :heading="drawerHeading"
    :open-event="openEvent"
  >
    <aForm
      :id="utilityStore.getNewId()"
      :heading="`Re: ${selectedRequirement?.name || ''}`"
      :loading="formLoading"
      @submit.prevent="submitForm()"
    >
      <aTextArea
        v-if="selectedRequirement?.actionType !== 'Upload File'"
        :id="utilityStore.getNewId()"
        v-model="message"
        label="Message"
        :error="isMessageInvalid"
        instructions="Max 1000 characters."
        placeholder="Type message here"
      />
      <aFileUpload
        v-if="selectedRequirement?.actionType !== 'Send Message'"
        :id="utilityStore.getNewId()"
        v-model="filesSelected"
        v-model:error="fileUploadError"
        :required="true"
        :max-file-size-in-mb="10"
        :valid-file-types="validFileTypes"
        :valid-file-types-display="validFileTypesDisplay"
        :disable-drag-and-drop="
          selectedRequirement?.actionType === 'Upload File Or Send Message'
            ? true
            : false
        "
      />
      <div class="form-actions">
        <aButton
          button-style="tertiary"
          type="button"
          text="Cancel"
          size="standard"
          @click="
            isDrawerOpen = false;
            $event.preventDefault();
          "
          @keyup.enter="isDrawerOpen = false"
          @keyup.space="isDrawerOpen = false"
          @keypress="$event.preventDefault()"
        />
        <aButton
          button-style="primary"
          type="submit"
          text="Submit"
          size="standard"
          :disabled="isFormDisabled"
          @keypress="$event.preventDefault()"
        />
      </div>
      <template v-if="actionSuccess" #success>
        <h4 class="assurity-form__response-heading">Thank you!</h4>
        <p class="assurity-form__response-message">
          Please allow time for our dedicated underwriting team to review your
          submission.
        </p>
        <p>
          If you have any urgent questions or concerns, please don’t hesitate to
          contact our Customer Connections team at 800-276-7619.
        </p>
        <div class="close-drawer-button">
          <aButton
            button-style="primary"
            text="Close"
            size="standard"
            @click="
              isDrawerOpen = false;
              $event.preventDefault();
            "
            @keyup.enter="isDrawerOpen = false"
            @keyup.space="isDrawerOpen = false"
            @keypress="$event.preventDefault()"
          />
        </div>
      </template>
    </aForm>
  </AssurityDrawer>
</template>
<script setup lang="ts">
import { computed, ref, watch } from "vue";

import { usePolicyStore } from "@/stores/policystore";
import { useUtilityStore } from "@/stores/utilityStore";

import {
  ValidFileUploadTypes,
  ValidFileUploadTypesDisplay,
} from "@/models/enums/FileTypes";
import type { Requirement } from "@/models/Requirement";

import AssurityDrawer from "@/components/AssurityDrawer.vue";
import aForm from "@/components/forms/aForm.vue";
import aButton from "@/components/forms/aButton.vue";
import aFileUpload from "@/components/forms/aFileUpload.vue";
import aTextArea from "@/components/forms/aTextArea.vue";
import type { Ref } from "vue";
import type { TableColumn } from "@/models/components/TableColumn";
import FormatHelpers from "@/Shared/utils/FormatHelpers";
import type { Participant } from "@/models/Participant";
import { RequirementStatus } from "@/models/enums/RequirementStatus";

import DataTable from "@/components/DataTable.vue";
import type { TableRow } from "@/models/components/TableRow";

const utilityStore = useUtilityStore();
const policyStore = usePolicyStore();

const message = ref("");
const filesSelected = ref([]);
const fileUploadError = ref(true);
const validFileTypes = Object.values(ValidFileUploadTypes);
const validFileTypesDisplay = Object.values(ValidFileUploadTypesDisplay);

const selectedRequirement: Ref<Requirement | null> = ref(null);
const isDrawerOpen = ref(false);
const drawerHeading = ref("Agent Message");
const formLoading = ref(false);
const actionSuccess = ref(false);
// TODO: Update to be the click event that opens the drawer
// Once we have actions being supplied by the API
const openEvent = ref(new MouseEvent("click"));

watch(
  () => isDrawerOpen.value,
  async (newValue: boolean) => {
    if (!newValue) {
      resetActionForm();
    }
  },
);

type TableRowData = {
  [key: string]: { value: string; subValue: string } | string;
};

const agentRequirements = computed(() => {
  if (!policyStore.selectedPolicy.policy?.requirements) {
    return [];
  }

  const requirements = policyStore.selectedPolicy.policy?.requirements.filter(
    (req) => req.display === true && req.fulfillingParty === "Agent",
  );

  return requirements.map((requirement) => mapRequirements(requirement));
});

const homeOfficeRequirements = computed(() => {
  if (!policyStore.selectedPolicy.policy?.requirements) {
    return [];
  }

  const requirements = policyStore.selectedPolicy.policy.requirements.filter(
    (req) => req.display === true && req.fulfillingParty === "Home Office",
  );

  return requirements.map((requirement) => mapRequirements(requirement));
});

function mapRequirements(req: Requirement) {
  const requirements = [
    {
      id: "date",
      heading: "Date",
      value: FormatHelpers.formatDate(req.addedDate || new Date()),
    },
    {
      id: "reqComments",
      heading: "Requirements/Comments",
      value: FormatHelpers.capitalizeFirstLetter(req.name || ""),
      subValue: `
        ${req.globalComment ?? ""} 
        ${req.lifeProComment ?? ""} 
        ${req.phoneNumberComment ?? ""}
      `,
      sortable: false,
    },
    {
      id: "appliesTo",
      heading: "Requirement Applies to",
      value: getName(req.appliesTo),
    },
    {
      id: "met",
      heading: "Met",
      value: getStatusText(req.status),
    },
    {
      id: "actionNeeded",
      heading: "Actions",
      value: getBtnText(req.actionType),
      isButton: true,
      display: showButton(req),
      data: req.id,
    },
  ] as TableColumn[];

  return requirements;
}

function getName(participant: Participant) {
  if (participant?.person?.name) {
    return `${participant.person.name.individualFirst.toLowerCase()} ${participant.person.name.individualLast.toLowerCase()}`;
  } else {
    return;
  }
}

function getStatusText(status: string) {
  switch (status) {
    case RequirementStatus.Met:
      return "Yes";
    case RequirementStatus.Unmet:
      return "No";
    case RequirementStatus.Waived:
      return "Waived";
    case RequirementStatus.None:
    default:
      return "";
  }
}

function getBtnText(action?: string) {
  switch (action) {
    case "Send Message":
      return "Send Info";
    case "Upload File":
      return "Send File";
    case "Upload File Or Send Message":
      return "Send Info/File";
    default:
      return "";
  }
}

function showButton(req: Requirement) {
  if (
    req.status === RequirementStatus.Unmet &&
    req.actionType !== "" &&
    req.fulfillingParty === "Agent"
  ) {
    return true;
  }

  return false;
}

const headers = computed(() => {
  return [
    {
      id: "date",
      heading: "Date",
      display: true,
      sortable: true,
    },
    {
      id: "reqComments",
      heading: "Requirements/Comments",
      sortable: false,
      display: true,
    },
    {
      id: "appliesTo",
      heading: "Requirement Applies to",
      sortable: true,
      display: true,
    },
    {
      id: "met",
      heading: "Met",
      display: true,
      sortable: true,
    },
    {
      id: "actionNeeded",
      heading: "Actions",
      display: true,
      sortable: false,
    },
  ] as TableColumn[];
});

const headersHomeOffice = computed(() => {
  return [
    {
      id: "date",
      heading: "Date",
      display: true,
      sortable: true,
    },
    {
      id: "reqComments",
      heading: "Requirements/Comments",
      sortable: false,
      display: true,
    },
    {
      id: "appliesTo",
      heading: "Requirement Applies to",
      sortable: true,
      display: true,
    },
    {
      id: "met",
      heading: "Met",
      display: true,
      sortable: true,
    },
    {
      id: "actionNeeded",
      heading: "", // Left blank to align columns between tables
      display: true,
      sortable: false,
    },
  ] as TableColumn[];
});

const isMessageInvalid = computed(() => {
  if (message.value.length > 1000) {
    return true;
  }
  if (selectedRequirement.value?.actionType === "Send Message") {
    return isMessageEmpty.value;
  } else {
    return null;
  }
});

const isMessageEmpty = computed(() => {
  return message.value.length === 0;
});

const isFormDisabled = computed(() => {
  const isMessageOnlyAndMessageIsInvalid =
    selectedRequirement.value?.actionType === "Send Message" &&
    isMessageInvalid.value;

  if (isMessageOnlyAndMessageIsInvalid) {
    return true;
  }

  const isFileUploadOnlyAndFileHasError =
    fileUploadError.value &&
    selectedRequirement.value?.actionType === "Upload File";

  if (isFileUploadOnlyAndFileHasError) {
    return true;
  }

  const isFileUploadOrMessage =
    selectedRequirement.value?.actionType === "Upload File Or Send Message";

  const hasAtLeastMessageOrFile =
    !isMessageEmpty.value || filesSelected.value.length > 0;
  const hasFileUploadedAndFileHasError =
    filesSelected.value.length > 0 && fileUploadError.value;
  const hasMessageEnteredAndMessageHasError =
    !isMessageEmpty.value && isMessageInvalid.value;

  if (isFileUploadOrMessage) {
    // has to have one or the other
    if (hasAtLeastMessageOrFile) {
      // check validity
      if (
        hasFileUploadedAndFileHasError ||
        hasMessageEnteredAndMessageHasError
      ) {
        return true;
      }
    } else {
      return true;
    }
  }
  return false;
});

function resetActionForm() {
  window.setTimeout(() => {
    filesSelected.value = [];
    message.value = "";
    actionSuccess.value = false;
    formLoading.value = false;
    // Delay form reset 250ms for animation of closing drawer to complete.
  }, 250);
}
const selectedRowId = ref<string | null | undefined>(null);
function selectAction({ event, data }: { event: Event; data: TableColumn }) {
  const selectedRowReqId = data.data;
  const requirement = policyStore.selectedPolicy.policy?.requirements.find(
    (req) => req.id === selectedRowReqId,
  );

  if (!requirement?.actionType) return;
  openEvent.value = event as MouseEvent;
  selectedRequirement.value = requirement as Requirement;
  selectedRowId.value = selectedRowReqId ?? null;
  isDrawerOpen.value = true;
}

function getUploadMessage(message: string): string {
  const tableRows: TableRow[] = [...agentRequirements.value];
  const result: TableRowData = {
    message: { value: message, subValue: "" },
    title: "Agent Reply",
    headTitle: "Requirement Information",
  };

  let rowMatched = false;

  if (tableRows.length > 0) {
    tableRows.forEach((row) => {
      const actionCell = row.find(
        (cell: TableColumn) => cell.heading === "Actions",
      );

      const rowData = actionCell?.data;
      if (rowData === selectedRowId.value) {
        rowMatched = true;

        Object.keys(row).forEach((key) => {
          const cell = row[key] as TableColumn;

          if (cell && typeof cell === "object") {
            const heading = cell.heading || "";
            const value = String(cell.value) || "";
            const subValue = String(cell.subValue ?? "") || "";

            if (heading && heading !== "Actions") {
              result[heading] = { value, subValue };
            }
          }
        });
      }
    });

    if (!rowMatched) {
      return JSON.stringify({ message: "", ...result });
    }
  } else {
    return JSON.stringify({
      message: "No data found in tableRows.",
      ...result,
    });
  }

  return JSON.stringify(result);
}

async function submitForm() {
  if (!policyStore.selectedPolicy.policyNumber) return;
  if (!message.value && !filesSelected.value.length) return;
  formLoading.value = true;
  const formData = new FormData();
  formData.append(
    "UploadData",
    JSON.stringify({
      Message: getUploadMessage(message.value),
      PolicyNumber: policyStore.selectedPolicy.policyNumber,
    }),
  );
  if (filesSelected.value.length) {
    filesSelected.value.forEach((file) => {
      formData.append("FilesToUpload", file);
    });
  }
  const response = await policyStore.submitActionItem(formData);
  if (response) {
    drawerHeading.value = "File Submitted";
    if (response.status === 200) actionSuccess.value = true;
  }
  formLoading.value = false;
}
</script>
<style scoped lang="pcss">
.form-actions {
  display: flex;
  column-gap: var(--spacing-xxl);
  justify-content: flex-end;
  margin: var(--spacing-xxxl) 0 0;
}
.action {
  &__title {
    color: var(--primary-color);
    font-size: 1.3125rem;
    font-weight: 700;
    font-family: var(--primary-font);
    margin: var(--spacing-l) auto 0;
    line-height: 1;
    max-width: 560px;

    @media (width >= 960px) {
      max-width: none;
      font-size: 2.0625rem;
      margin: var(--spacing-xxl) 0 var(--spacing-l);
    }
  }
}

:deep(.policy-actions-table) {
  border-radius: 8px;
  background: rgb(var(--v-theme-surface));
  box-shadow:
    0px 2px 4px -1px var(--v-shadow-key-umbra-opacity, rgba(0, 0, 0, 0.2)),
    0px 4px 5px 0px var(--v-shadow-key-penumbra-opacity, rgba(0, 0, 0, 0.14)),
    0px 1px 10px 0px var(--v-shadow-key-penumbra-opacity, rgba(0, 0, 0, 0.12));
}

:deep(.policy-actions-table .mobile-table) {
  max-width: none;
}

/** Force columns to be the same size across both tables */
@media (width >= 960px) {
  :deep(.data-table__table-heading) {
    &--date {
      width: 15%;
    }
    &--reqComments {
      width: 33%;
    }
    &--appliesTo {
      width: 23%;
    }
    &--met {
      width: 11%;
    }
  }
}

.close-drawer-button {
  display: flex;
  justify-content: flex-end;
  margin: var(--spacing-xxxl) 0 0;
}

:deep() {
  .data-table__value--appliesTo {
    text-transform: capitalize;
  }
  .mobile-table .mobile-table__column--actionNeeded {
    position: absolute;
    top: var(--spacing-l);
    right: var(--spacing-l);
    margin: 0;
  }
  .mobile-table .mobile-table__column--actionNeeded:before {
    display: none;
  }
}
</style>
