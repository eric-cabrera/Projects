<template>
  <div class="case-management__table__mobile-section">
    <PrimaryInsuredProduct
      :product="product"
      :product-name="productName"
      :coverages="baseCoverages"
    />
    <SpouseProduct
      v-if="spouseCoverages"
      :product="product"
      :product-name="productName"
      :coverages="spouseCoverages"
    />
    <ChildProduct
      v-if="hasChildren && (isAccidentalDeath || isTermLife)"
      :product="product"
      :product-name="productName"
      :coverages="baseCoverages"
      :is-accidental-death="isAccidentalDeath"
      :is-term-life="isTermLife"
    />
  </div>
</template>
<script setup lang="ts">
import { type Assurity_ApplicationTracker_Contracts_DataTransferObjects_Coverage as Coverage } from "@assurity/newassurelink-client";

import PrimaryInsuredProduct from "./PrimaryInsuredProduct.vue";
import SpouseProduct from "./SpouseProduct.vue";
import ChildProduct from "./ChildProduct.vue";
import { ProductType } from "../models/enums/ProductType";

const props = defineProps<{
  baseCoverages?: Coverage[] | null;
  spouseCoverages?: Coverage[] | null;
  product?: string | null;
  productName?: string | null;
  hasChildren: boolean;
}>();

const isAccidentalDeath = [
  ProductType.AccidentalDeath as string,
  ProductType.AccidentalDeathDismemberment as string,
].includes(props.product ?? "");

const isTermLife = [
  ProductType.TermLife as string,
  ProductType.TermDeveloperEdition as string,
].includes(props.product ?? "");
</script>
<style>
.case-management__table__insured th {
  padding-top: 0.8em;
}
.case-management__table__mobile-section hr {
  margin: 1em 0 0.5em 0;
}
</style>
