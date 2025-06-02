<template>
  <table
    class="case-management__table__status"
    :class="[`data-quote-id-${quoteId}`]"
  >
    <tbody>
      <tr>
        <th>&nbsp;</th>
      </tr>
      <tr v-for="event in events" :key="event?.event">
        <template v-if="event?.eventName?.startsWith('Signature')">
          <td class="case-management__event-action">
            <template v-for="signer in signers" :key="signer?.recipientId">
              <template v-if="showResendEmailButton(event, signer)">
                <aButton
                  class="table-desktop__button case-management__event-button case-management__signer-button"
                  button-style="primary"
                  size="s"
                  text="Resend Email"
                  @click="
                    emit('linkClick', event?.event, {
                      eventType: event.event,
                      product: product,
                      envelopeId: event?.envelopeId,
                      experienceKey: experienceKey,
                      cacheId: cacheId,
                      quoteId: quoteId,
                    })
                  "
                >
                </aButton>
              </template>
            </template>
          </td>
        </template>
        <template v-else-if="showResumeButton(event)">
          <aButton
            class="table-desktop__button case-management__event-button"
            button-style="primary"
            size="s"
            text="Resume"
            @click="
              emit('linkClick', event?.event, {
                eventType: event.event,
                product: product,
                envelopeId: event?.envelopeId,
                experienceKey: experienceKey,
                cacheId: cacheId,
                quoteId: quoteId,
              })
            "
          >
          </aButton>
        </template>
      </tr>
    </tbody>
  </table>
</template>
<script setup lang="ts">
import aButton from "@/components/forms/aButton.vue";
import { type Assurity_AgentPortal_Contracts_CaseManagement_CaseManagementEvent as Event } from "@assurity/newassurelink-client";
import { type Assurity_ApplicationTracker_Contracts_DataTransferObjects_Signer as Signer } from "@assurity/newassurelink-client";
import { ref, type Ref } from "vue";
import { AudienceType } from "../models/enums/AudienceType";
import { EventType } from "../models/enums/EventType";
import { useUserStore } from "@/stores/userStore";

const userStore = useUserStore();

const props = defineProps<{
  events?: Event[];
  signers?: Signer[] | null;
  audience?: string;
  product?: string;
  experienceKey?: string;
  cacheId?: string;
  quoteId?: string;
}>();

const emit = defineEmits<{
  (e: "linkClick", key: string, value: object): void;
}>();

const expiredCase = ref(false);
const appSubmitted = ref(false);

const completedDeclinedExpiredSigners: Ref<(number | null | undefined)[]> = ref(
  [],
);

const caseCreated: Ref<boolean | undefined> = ref(false);
const signatureRequested: Ref<boolean | undefined> = ref(false);

// get completed signers
props.events?.forEach((event) => {
  if (
    event?.eventName == "Signature Completed" ||
    event?.eventName == "Signature Declined" ||
    event?.eventName == "Signature Expired"
  ) {
    completedDeclinedExpiredSigners.value.push(event.recipientId ?? undefined);
  } else if (event?.eventName == "Expired") {
    expiredCase.value = true;
  } else if (event?.eventName == "Application Submitted") {
    appSubmitted.value = true;
  }
});

// If a case has been created, don't show Received Quote or Case Started resume options
caseCreated.value = props.events?.some(
  (x) => x.eventName === "Interview Started",
);
signatureRequested.value = props.events?.some(
  (x) => x.eventName === "Signature Requested",
);

const showResendEmailButton = (event: Event, signer: Signer) => {
  return (
    signer.recipientId === event?.recipientId &&
    event.eventName === "Signature Requested" &&
    !completedDeclinedExpiredSigners.value.includes(signer.recipientId) &&
    !expiredCase.value &&
    !appSubmitted.value &&
    props.audience !== AudienceType.ConsumerFixedAgent &&
    !userStore.user.isSubaccount
  );
};

const showResumeButton = (event: Event) => {
  return (
    (event?.event === EventType.ReceivedQuote ||
      event?.event === EventType.CaseStarted) &&
    !caseCreated.value &&
    !signatureRequested.value &&
    props.audience !== AudienceType.ConsumerFixedAgent &&
    !userStore.user.isSubaccount &&
    !userStore.user.isHomeOfficeImpersonating
  );
};
</script>
<style scoped>
.assurity-button.case-management__event-button {
  padding: 8px;
  margin: 4px 0;
}
.assurity-button.case-management__signer-button:first-child {
  margin-top: 24px;
}
.case-management__table
  .table-desktop__wrapper
  table
  tbody
  > tr.table-desktop__row--nested
  td
  div
  .case-management__table__status
  td {
  padding: 0;
}
</style>
