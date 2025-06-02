<template>
  <div v-if="child" class="case-management__table__product">
    <table class="case-management__table__product-table">
      <tbody>
        <tr>
          <td colspan="2">Children's {{ productName }}</td>
        </tr>
        <tr v-if="isAccidentalDeath">
          <td>Benefit Amount:</td>
          <td>{{ FormatHelpers.formatMoney(child?.benefitAmount ?? 0) }}</td>
        </tr>
        <tr v-if="isTermLife">
          <td>Term Rider:</td>
          <td>{{ FormatHelpers.formatMoney(child?.benefitAmount ?? 0) }}</td>
        </tr>
      </tbody>
    </table>
  </div>
</template>
<script setup lang="ts">
import dayjs from "dayjs";
import relativeTime from "dayjs/plugin/relativeTime";
import { type Assurity_ApplicationTracker_Contracts_DataTransferObjects_Coverage as Coverage } from "@assurity/newassurelink-client";
import FormatHelpers from "@/Shared/utils/FormatHelpers";
const props = defineProps<{
  product?: string | null;
  productName?: string | null;
  coverages?: Coverage[] | null;
  isAccidentalDeath?: boolean;
  isTermLife?: boolean;
}>();
dayjs.extend(relativeTime);

const child = props.coverages?.find(
  (coverage) => coverage.coverageLookup === "Child",
);
</script>
