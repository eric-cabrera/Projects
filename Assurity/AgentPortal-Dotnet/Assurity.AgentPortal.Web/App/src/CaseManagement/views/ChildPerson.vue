<template>
  <div class="case-management__table__insured">
    <table>
      <tbody>
        <tr>
          <th>Child</th>
        </tr>
        <tr v-if="child?.firstName">
          <td class="case-management__table__insured__name">
            {{ child?.firstName + " " + child?.lastName }}
          </td>
        </tr>
        <tr v-if="child?.address?.line1">
          <td>{{ child?.address?.line1 }}</td>
        </tr>
        <tr v-if="child?.address?.line2">
          <td>
            {{ child.address.line2 }}
          </td>
        </tr>
        <tr v-if="child?.address?.line3">
          <td>
            {{ child.address.line3 }}
          </td>
        </tr>
        <tr v-if="child?.address?.line4">
          <td>
            {{ child.address.line4 }}
          </td>
        </tr>
        <tr v-if="child?.address?.city">
          <td>
            {{
              child?.address?.city +
              ", " +
              child?.address?.stateAbbreviation +
              " " +
              child?.address?.zipCode
            }}
          </td>
        </tr>
        <tr v-if="child?.phoneNumber">
          <td>
            {{ child?.phoneAreaCode + "-" + child?.phoneNumber }}
          </td>
        </tr>
        <tr v-if="child?.emailAddress">
          <td>
            {{ child?.emailAddress }}
          </td>
        </tr>
      </tbody>
    </table>
    <table class="case-management__table__insured-demograpics">
      <tbody>
        <tr v-if="product !== 'cpdi'">
          <td>Age:</td>
          <td>
            {{ getAgeByDOB(child?.dateOfBirth) }}
          </td>
        </tr>
        <tr v-if="product === 'cpdi'">
          <td>Date of Birth:</td>
          <td>
            {{ child?.dateOfBirth }}
          </td>
        </tr>
        <tr v-if="child?.gender">
          <td>Gender:</td>
          <td style="text-transform: capitalize">{{ child?.gender }}</td>
        </tr>
      </tbody>
    </table>
  </div>
</template>
<script setup lang="ts">
import dayjs from "dayjs";
import { getAgeByDOB } from "@/CaseManagement/utils/dateUtils.js";
import relativeTime from "dayjs/plugin/relativeTime";
import { type Assurity_ApplicationTracker_Contracts_DataTransferObjects_Child as Child } from "@assurity/newassurelink-client";
defineProps<{
  child?: Child;
  product?: string | null;
}>();
dayjs.extend(relativeTime);
</script>
