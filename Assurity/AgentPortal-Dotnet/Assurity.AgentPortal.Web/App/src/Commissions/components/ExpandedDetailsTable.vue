<template>
  <v-table class="details-extended-table d-none d-md-block">
    <tbody>
      <tr v-for="(item, index) in props.expandedItems" :key="index">
        <td
          v-for="(header, headerIndex) in props.expandedHeaders"
          :key="headerIndex"
        >
          <div class="details-extended-table__cell-content">
            <div class="details-extended-table__header">
              {{ header.title }}:
            </div>
            <div
              class="details-extended-table__value"
              :class="{
                'details-extended-table__value--bold': header.emphasizeValue,
              }"
            >
              {{ item[header.key] as string }}
            </div>
          </div>
        </td>
      </tr>
    </tbody>
  </v-table>
  <div class="details-extended-table_mobile d-block d-md-none">
    <div
      v-for="(item, index) in props.expandedItems"
      :key="index"
      class="details-extended-table_mobile-policy"
    >
      <div
        v-for="(header, headerIndex) in props.expandedHeaders"
        :key="headerIndex"
        class="details-extended-table_mobile__cell-content"
      >
        <div class="v-card__content__label">{{ header.title }}:</div>
        <div
          class="v-card__content__value"
          :class="`v-card__content__value--${header.key}`"
        >
          {{ item[header.key] as string }}
        </div>
      </div>

      <aHorizontalRule class="details-extended-table_mobile__hr" />
    </div>
  </div>
</template>

<script setup lang="ts">
import type { DebtUnsecuredAdvancesAgentPolicy } from "@/models/Responses/CommissionsDebtUnsecuredAdvancesResponse";
import type { DebtSecuredAdvancesAgentPolicy } from "@/models/Responses/CommissionsDebtSecuredAdvancesResponse";
import aHorizontalRule from "@/components/aHorizontalRule.vue";
interface Header {
  title: string;
  key: string;
  emphasizeValue?: boolean;
}
const props = defineProps<{
  expandedItems:
    | DebtSecuredAdvancesAgentPolicy[]
    | DebtUnsecuredAdvancesAgentPolicy[];
  expandedHeaders: Header[];
}>();
</script>

<style scoped lang="pcss">
.details-extended-table {
  background-color: transparent;
  width: 100%;
  color: var(--text-grey);
}
.details-extended-table tbody td:first-child {
  padding-left: 100px;
}
.details-extended-table__cell-content {
  display: flex;
  flex-direction: column;
}

.details-extended-table__header {
  font-weight: bold;
}

.details-extended-table__value {
  font-weight: normal;
}

.details-extended-table__value--bold {
  font-weight: 600;
}

.details-extended-table_mobile__cell-content {
  margin-bottom: var(--spacing-l);
  .v-card__content__label {
    font-size: 14px;
    font-weight: 300;
  }
  .v-card__content__value {
    font-weight: 600;
    font-size: 18px;
  }
  .v-card__content__value--policyNumber {
    color: var(--accent-color);
  }
}
.details-extended-table_mobile-policy {
  margin-bottom: var(--spacing-l);
}
</style>
