<template>
  <div class="case-management__table__product">
    <table
      v-if="isAccidentalDeath"
      class="case-management__table__product-table"
    >
      <tbody>
        <tr>
          <td colspan="2">Spouse {{ productName }}</td>
        </tr>
        <tr v-if="spouse?.benefitAmount">
          <td>Benefit Amount:</td>
          <td>{{ FormatHelpers.formatMoney(spouse?.benefitAmount ?? 0) }}</td>
        </tr>
      </tbody>
    </table>

    <table
      v-if="
        isIncomeProtectionAcccidentOnly ||
        isIncomeProtectionAcccidentAndSickness
      "
      class="case-management__table__product-table"
    >
      <tbody>
        <tr>
          <td colspan="2">Spouse {{ productName }}</td>
        </tr>
        <tr v-if="isIncomeProtectionAcccidentOnly">
          <td>Coverage: Accident Only</td>
          <td>&nbsp;</td>
        </tr>
        <tr v-if="isIncomeProtectionAcccidentAndSickness">
          <td>Coverage: Accident and Sickness</td>
          <td>&nbsp;</td>
        </tr>
        <tr>
          <td>Benefit Amount:</td>
          <td>{{ FormatHelpers.formatMoney(spouse?.benefitAmount ?? 0) }}</td>
        </tr>
        <tr>
          <td>Stay-at-Home Spouse Disability Income:</td>
          <td><AssurityIconCheckBox class="case-management__check-box" /></td>
        </tr>
      </tbody>
    </table>
  </div>
</template>
<script setup lang="ts">
import dayjs from "dayjs";
import relativeTime from "dayjs/plugin/relativeTime";
import { type Assurity_ApplicationTracker_Contracts_DataTransferObjects_Coverage as Coverage } from "@assurity/newassurelink-client";
import AssurityIconCheckBox from "@/components/icons/AssurityIconCheckBox.vue";
import FormatHelpers from "@/Shared/utils/FormatHelpers";
import { ProductType } from "../models/enums/ProductType";
const props = defineProps<{
  product?: string | null;
  productName?: string | null;
  coverages?: Coverage[] | null;
}>();
dayjs.extend(relativeTime);

const spouse = props.coverages?.find(
  (coverage) => coverage.coverageLookup === "Spouse",
);

const isAccidentalDeath = [
  ProductType.AccidentalDeath as string,
  ProductType.AccidentalDeathDismemberment as string,
].includes(props.product ?? "");

const isIncomeProtectionAcccidentOnly =
  props.product === ProductType.IncomeProtectionAccidentOnly;

const isIncomeProtectionAcccidentAndSickness =
  props.product === ProductType.IncomeProtectionAccidentSickness;
</script>
