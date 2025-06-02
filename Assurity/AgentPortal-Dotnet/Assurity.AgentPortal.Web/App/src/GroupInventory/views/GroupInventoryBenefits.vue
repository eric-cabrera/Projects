<template>
  <table class="group-inventory__table__benefits">
    <tbody>
      <tr>
        <th>Primary Insured</th>
        <th>Insured DOB</th>
        <th>Current City</th>
        <th>Benefits</th>
        <th>Benefit Description</th>
        <th>Benefit Amount</th>
        <th>Coverage Options</th>
      </tr>
      <tr
        v-for="(benefit, index) in policy?.benefits"
        :key="benefit.coverageType ?? '' + index"
      >
        <td>
          <div class="group-inventory__primary-insured">
            {{ index === 0 ? (props.policy?.primaryInsured?.name ?? "") : "" }}
          </div>
        </td>
        <td>
          <div class="group-inventory__date-of-birth">
            {{
              index === 0
                ? dayjs(props.policy?.primaryInsured?.dateOfBirth).format(
                    "MM/DD/YYYY",
                  )
                : ""
            }}
          </div>
        </td>
        <td>
          <div class="group-inventory__primary-insured-current-city">
            {{ index === 0 ? (policy?.primaryInsured?.currentCity ?? "") : "" }}
          </div>
        </td>
        <td :class="{ base: benefit.coverageType === 'Base' }">
          <div class="group-inventory__benefit-coverageType">
            <template v-if="benefit.coverageType === 'Base'">Base</template>
            <template v-if="benefit.coverageType === 'Rider' && index === 1">
              Rider(s)
            </template>
          </div>
        </td>
        <td :class="{ base: benefit.coverageType === 'Base' }">
          <div class="group-inventory__benefit-description">
            {{ benefit.description ?? "" }}
          </div>
        </td>
        <td :class="{ base: benefit.coverageType === 'Base' }">
          <div class="group-inventory__benefit-amount">
            {{ benefit.amount ?? "" }}
          </div>
        </td>
        <td :class="{ base: benefit.coverageType === 'Base' }">
          <template v-if="benefit.coverageType === 'Base'">
            <div
              v-for="(coverageOption, optionIndex) in benefit?.coverageOptions"
              :key="coverageOption ?? '' + optionIndex"
              class="group-inventory__benefit-coverage-options"
            >
              {{ "• " + coverageOption }}
            </div></template
          >
        </td>
      </tr>
    </tbody>
  </table>
</template>
<script setup lang="ts">
import dayjs from "dayjs";
import type { Assurity_AgentPortal_Contracts_GroupInventory_Response_Policy as Policy } from "@assurity/newassurelink-client";

const props = defineProps<{
  policy?: Policy | null;
}>();
</script>
<style lang="pcss">
.group-inventory__table__benefits th {
  padding: 1em;
}
.group-inventory__table__benefits td {
  vertical-align: top;
  padding: 1em;
}
.group-inventory__table__benefits td.base {
  background: #fff;
}
.group-inventory__benefit-amount {
  text-align: right;
}
.group-inventory__table__benefits td.group-inventory__table__divider {
  padding: 0 1em;

  & hr {
    border: none;
    border-top: 1px solid var(--disabled-grey);
  }
}
</style>
