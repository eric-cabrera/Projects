<template>
  <table class="case-management__table__status">
    <tbody>
      <tr>
        <th>Status History</th>
      </tr>
      <tr v-for="event in events" :key="event?.event">
        <td>
          <div>{{ event?.eventName }}</div>
          <template v-if="event?.eventName?.startsWith('Signature')">
            <template v-for="signer in signers" :key="signer?.recipientId">
              <template v-if="signer.recipientId === event.recipientId">
                <div class="case-management__table__status-signers">
                  {{ signer.name }}
                </div>
              </template>
            </template>
          </template>
        </td>
      </tr>
    </tbody>
  </table>
</template>
<script setup lang="ts">
import { type Assurity_AgentPortal_Contracts_CaseManagement_CaseManagementEvent as Event } from "@assurity/newassurelink-client";
import { type Assurity_ApplicationTracker_Contracts_DataTransferObjects_Signer as Signer } from "@assurity/newassurelink-client";

defineProps<{
  events?: Event[] | null;
  signers?: Signer[] | null;
}>();
</script>
<style>
.case-management__table__status-signers {
  padding-left: 1em;
}
</style>
