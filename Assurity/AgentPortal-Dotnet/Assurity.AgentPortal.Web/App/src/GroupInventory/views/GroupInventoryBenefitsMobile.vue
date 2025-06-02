<template>
  <table class="group-inventory__table__benefits">
    <tbody>
      <tr>
        <td class="group-inventory__table__divider" colspan="2"><hr /></td>
      </tr>
      <tr>
        <td>
          <div>Primary Insured</div>
          <div class="group-inventory__table__data">
            {{ policy?.primaryInsured?.name ?? "" }}
          </div>
        </td>
        <td>&nbsp;</td>
      </tr>
      <tr>
        <td>
          <div>Current City</div>
          <div class="group-inventory__table__data">
            {{ policy?.primaryInsured?.currentCity ?? "" }}
          </div>
        </td>
        <td>
          <div>Date of Birth</div>
          <div class="group-inventory__table__data">
            {{
              dayjs(policy?.primaryInsured?.dateOfBirth).format("MM/DD/YYYY")
            }}
          </div>
        </td>
      </tr>
      <tr>
        <td class="group-inventory__table__divider" colspan="2"><hr /></td>
      </tr>
      <template
        v-for="(benefit, index) in policy?.benefits"
        :key="benefit.coverageType ?? '' + index"
      >
        <template v-if="index < 2">
          <tr>
            <td :class="{ base: benefit.coverageType === 'Base' }">
              <div>Benefits</div>
              <div class="group-inventory__table__data">
                <template v-if="benefit.coverageType === 'Base'">Base</template>
                <template
                  v-if="benefit.coverageType === 'Rider' && index === 1"
                >
                  Rider(s)
                </template>
              </div>
            </td>
            <td :class="{ base: benefit.coverageType === 'Base' }">&nbsp;</td>
          </tr>
        </template>
        <tr>
          <td :class="{ base: benefit.coverageType === 'Base' }">
            <div>Benefit Description</div>
            <div class="group-inventory__table__data">
              {{ benefit.description ?? "" }}
            </div>
          </td>
          <td :class="{ base: benefit.coverageType === 'Base' }">
            <div>Benefit Amount</div>
            <div class="group-inventory__table__data">
              {{ benefit.amount ?? "" }}
            </div>
          </td>
        </tr>
        <tr v-if="benefit.coverageType === 'Base'">
          <td :class="{ base: benefit.coverageType === 'Base' }" colspan="2">
            <div>Coverage Options</div>
            <div
              v-for="(coverageOption, optionIndex) in benefit?.coverageOptions"
              :key="coverageOption ?? '' + optionIndex"
              class="group-inventory__table__data"
            >
              {{ "• " + coverageOption }}
            </div>
          </td>
        </tr>
        <tr
          v-if="policy.benefits && index < (policy?.benefits.length ?? 0) - 1"
        >
          <td class="group-inventory__table__divider" colspan="2"><hr /></td>
        </tr>
      </template>
    </tbody>
  </table>
</template>
<script setup lang="ts">
import dayjs from "dayjs";
import type { Assurity_AgentPortal_Contracts_GroupInventory_Response_Policy as Policy } from "@assurity/newassurelink-client";

defineProps<{
  policy?: Policy | null;
}>();
</script>
<style lang="pcss">
.group-inventory__table__data {
  font-weight: bold;
}
.group-inventory__table__benefits {
  width: 100vw;
  border-collapse: collapse;
}

.group-inventory__table__benefits th {
  padding: 1em;
}
.group-inventory__table__benefits td.base {
  background: #fff;
  border: none;
}
.group-inventory__benefit-amount {
  text-align: right;
}
</style>
