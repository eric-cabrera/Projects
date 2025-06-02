<template>
  <div class="case-management__table__insured">
    <table>
      <tbody>
        <tr v-if="relationship">
          <th>{{ relationship }}</th>
        </tr>
        <tr v-if="relatedPerson?.lastName">
          <td class="case-management__table__insured__name">
            {{ relatedPerson?.firstName + " " + relatedPerson?.lastName }}
          </td>
        </tr>
        <tr v-if="relatedPerson?.address?.line1">
          <td>{{ relatedPerson?.address?.line1 }}</td>
        </tr>
        <tr v-if="relatedPerson?.address?.line2">
          <td>
            {{ relatedPerson.address.line2 }}
          </td>
        </tr>
        <tr v-if="relatedPerson?.address?.line3">
          <td>
            {{ relatedPerson.address.line3 }}
          </td>
        </tr>
        <tr v-if="relatedPerson?.address?.line4">
          <td>
            {{ relatedPerson.address.line4 }}
          </td>
        </tr>
        <tr v-if="relatedPerson?.address?.city">
          <td>
            {{
              relatedPerson?.address?.city +
              ", " +
              relatedPerson?.address?.stateAbbreviation +
              " " +
              relatedPerson?.address?.zipCode
            }}
          </td>
        </tr>
        <tr v-if="relatedPerson?.phoneNumber">
          <td>
            {{
              relatedPerson?.phoneAreaCode + "-" + relatedPerson?.phoneNumber
            }}
          </td>
        </tr>
        <tr v-if="relatedPerson?.emailAddress">
          <td>
            {{ relatedPerson?.emailAddress }}
          </td>
        </tr>
      </tbody>
    </table>
    <table class="case-management__table__insured-demograpics">
      <tbody>
        <tr v-if="relatedPerson?.gender">
          <td>Gender:</td>
          <td style="text-transform: capitalize">
            {{ relatedPerson?.gender }}
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>
<script setup lang="ts">
import { type Assurity_ApplicationTracker_Contracts_DataTransferObjects_RelatedPerson as RelatedPerson } from "@assurity/newassurelink-client";
defineProps<{
  relatedPerson?: RelatedPerson;
  product?: string | null;
  relationship?: string | null;
}>();
</script>
