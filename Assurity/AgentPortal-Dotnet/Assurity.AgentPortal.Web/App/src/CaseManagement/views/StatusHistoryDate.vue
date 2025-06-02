<template>
  <table class="case-management__table__status">
    <tbody>
      <tr>
        <th>&nbsp;</th>
      </tr>
      <tr v-for="event in events" :key="event?.event">
        <template v-if="event?.eventName?.startsWith('Signature')">
          <td>
            <template v-for="signer in signers" :key="signer?.recipientId">
              <template v-if="signer.recipientId === event.recipientId">
                <div class="case-management__signer-date">
                  {{ dayjs(event.createdDateTime).utc().format("MM/DD/YYYY") }}
                </div>
              </template>
            </template>
          </td>
        </template>
        <template v-else>
          <td>{{ dayjs(event.createdDateTime).utc().format("MM/DD/YYYY") }}</td>
        </template>
      </tr>
    </tbody>
  </table>
</template>
<script setup lang="ts">
import dayjs from "dayjs";
import { type Assurity_AgentPortal_Contracts_CaseManagement_CaseManagementEvent as Event } from "@assurity/newassurelink-client";
import { type Assurity_ApplicationTracker_Contracts_DataTransferObjects_Signer as Signer } from "@assurity/newassurelink-client";

defineProps<{
  events?: Event[] | null;
  signers?: Signer[] | null;
}>();
</script>
<style>
.case-management__signer-date:first-child {
  margin-top: 24px;
}
</style>
