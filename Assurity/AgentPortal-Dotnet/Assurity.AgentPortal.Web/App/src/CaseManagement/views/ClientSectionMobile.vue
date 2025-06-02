<template>
  <div class="case-management__table__mobile-section">
    <PrimaryInsuredPerson
      :primary-insured="primaryInsured"
      :product="product"
    />
    <SpousePerson v-if="spouse" :spouse-insured="spouse" />
    <div v-for="child in children" :key="child.firstName ?? ''">
      <ChildPerson :child="child" :product="product" />
    </div>
    <template
      v-if="
        primaryPayor &&
        primaryPayor.relationshipToPrimaryInsured !== null &&
        primaryPayor.relationshipToPrimaryInsured !== 'Self' &&
        !(
          primaryPayor?.firstName === primaryInsured?.firstName &&
          primaryPayor?.lastName === primaryInsured?.lastName &&
          primaryPayor?.phoneNumber === primaryInsured?.phoneNumber &&
          primaryPayor?.emailAddress === primaryInsured?.emailAddress
        )
      "
    >
      <hr />
      <RelatedPerson
        :related-person="primaryPayor"
        :product="product"
        :relationship="'Payor'"
      />
    </template>
    <template
      v-if="
        primaryOwner &&
        primaryOwner.relationshipToPrimaryInsured !== null &&
        primaryOwner.relationshipToPrimaryInsured !== 'Self' &&
        !(
          primaryOwner?.firstName === primaryInsured?.firstName &&
          primaryOwner?.lastName === primaryInsured?.lastName &&
          primaryOwner?.phoneNumber === primaryInsured?.phoneNumber &&
          primaryOwner?.emailAddress === primaryInsured?.emailAddress
        )
      "
    >
      <hr />
      <RelatedPerson
        :related-person="primaryOwner"
        :product="product"
        :relationship="'Owner'"
      />
    </template>
  </div>
</template>
<script setup lang="ts">
import {
  type Assurity_ApplicationTracker_Contracts_DataTransferObjects_PrimaryInsured as PrimaryInsured,
  type Assurity_ApplicationTracker_Contracts_DataTransferObjects_Insured as Insured,
  type Assurity_ApplicationTracker_Contracts_DataTransferObjects_Child as Child,
  type Assurity_ApplicationTracker_Contracts_DataTransferObjects_RelatedPerson as RelatedPersonObj,
} from "@assurity/newassurelink-client";
import PrimaryInsuredPerson from "./PrimaryInsuredPerson.vue";
import SpousePerson from "./SpousePerson.vue";
import ChildPerson from "./ChildPerson.vue";
import RelatedPerson from "./RelatedPerson.vue";

defineProps<{
  primaryInsured?: PrimaryInsured;
  spouse?: Insured;
  children?: Child[] | null;
  primaryPayor?: RelatedPersonObj;
  primaryOwner?: RelatedPersonObj;
  product?: string | null;
}>();
</script>
