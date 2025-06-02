<template>
  <table class="case-management__table__status-mobile">
    <tbody>
      <tr v-for="event in events" :key="event?.event">
        <td>{{ event?.eventName }}</td>
        <td>{{ dayjs(event.createdDateTime).utc().format("MM/DD/YYYY") }}</td>
      </tr>
    </tbody>
  </table>
</template>
<script setup lang="ts">
import dayjs from "dayjs";
import { type Assurity_AgentPortal_Contracts_CaseManagement_CaseManagementEvent as Event } from "@assurity/newassurelink-client";
defineProps<{
  events?: Event[] | null;
}>();
</script>
