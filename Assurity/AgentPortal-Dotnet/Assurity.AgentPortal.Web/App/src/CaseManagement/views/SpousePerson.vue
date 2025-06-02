<template>
  <div class="case-management__table__insured">
    <table>
      <tbody>
        <tr>
          <th>Spouse</th>
        </tr>
        <tr v-if="spouseInsured?.lastName">
          <td class="case-management__table__insured__name">
            {{ spouseInsured?.firstName + " " + spouseInsured?.lastName }}
          </td>
        </tr>
        <tr v-if="spouseInsured?.address?.line1">
          <td>{{ spouseInsured?.address?.line1 }}</td>
        </tr>
        <tr v-if="spouseInsured?.address?.line2">
          <td>
            {{ spouseInsured.address.line2 }}
          </td>
        </tr>
        <tr v-if="spouseInsured?.address?.line3">
          <td>
            {{ spouseInsured.address.line3 }}
          </td>
        </tr>
        <tr v-if="spouseInsured?.address?.line4">
          <td>
            {{ spouseInsured.address.line4 }}
          </td>
        </tr>
        <tr v-if="spouseInsured?.address?.city">
          <td>
            {{
              spouseInsured?.address?.city +
              ", " +
              spouseInsured?.address?.stateAbbreviation +
              " " +
              spouseInsured?.address?.zipCode
            }}
          </td>
        </tr>
        <tr v-if="spouseInsured?.phoneNumber">
          <td>
            {{
              spouseInsured?.phoneAreaCode + "-" + spouseInsured?.phoneNumber
            }}
          </td>
        </tr>
        <tr v-if="spouseInsured?.emailAddress">
          <td>
            {{ spouseInsured?.emailAddress }}
          </td>
        </tr>
      </tbody>
    </table>
    <table class="case-management__table__insured-demograpics">
      <tbody>
        <tr v-if="spouseInsured?.dateOfBirth && product !== 'cpdi'">
          <td>Age:</td>
          <td>
            {{ getAgeByDOB(spouseInsured?.dateOfBirth) }}
          </td>
        </tr>
        <tr v-if="spouseInsured?.dateOfBirth && product === 'cpdi'">
          <td>Date of Birth:</td>
          <td>
            {{ dayjs(spouseInsured?.dateOfBirth).format("MM/DD/YYYY") }}
          </td>
        </tr>
        <tr v-if="spouseInsured?.gender">
          <td>Gender:</td>
          <td>{{ spouseInsured?.gender }}</td>
        </tr>
        <tr v-if="['termlife', 'termde', 'ci', 'cpdi'].includes(product ?? '')">
          <td>Nicotine Use:</td>
          <td>{{ (spouseInsured?.usesTobacco ?? false) ? "Y" : "N" }}</td>
        </tr>
        <tr
          v-if="
            ['fiveyrt', 'ipas'].includes(product ?? '') &&
            spouseInsured?.riskClassApplicationValue
          "
        >
          <td>Nicotine Use:</td>
          <td v-if="spouseInsured?.riskClassApplicationValue === 'Tobacco'">
            Y
          </td>
          <td v-if="spouseInsured?.riskClassApplicationValue === 'NonTobacco'">
            N
          </td>
        </tr>
        <tr v-if="['termlife', 'termde'].includes(product ?? '')">
          <td>Health:</td>
          <td
            v-if="
              spouseInsured?.riskClassApplicationValue
                ?.toLowerCase()
                .startsWith('preferredplus')
            "
          >
            Excellent
          </td>
          <td
            v-else-if="
              spouseInsured?.riskClassApplicationValue
                ?.toLowerCase()
                .startsWith('preferred')
            "
          >
            Great
          </td>
          <td
            v-else-if="
              spouseInsured?.riskClassApplicationValue
                ?.toLowerCase()
                .startsWith('standard')
            "
          >
            Good
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>
<script setup lang="ts">
import dayjs from "dayjs";
import { getAgeByDOB } from "@/CaseManagement/utils/dateUtils.js";
import relativeTime from "dayjs/plugin/relativeTime";
import { type Assurity_ApplicationTracker_Contracts_DataTransferObjects_Insured as Insured } from "@assurity/newassurelink-client";
defineProps<{
  spouseInsured?: Insured;
  product?: string | null;
}>();
dayjs.extend(relativeTime);
</script>
