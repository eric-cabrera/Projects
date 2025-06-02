<template>
  <template v-for="x in depth" :key="x">
    <div class="agent-agency-table__agent-level">&nbsp;</div>
  </template>
  <div class="agent-agency-table__details-container">
    <div class="agent-agency-table__details">
      <div
        v-for="(detail, detailsIndex) in details"
        :key="'agent-agency-details-' + detailsIndex"
      >
        <span class="agent-agency-table__details_title">
          {{ detail.title }}:
        </span>
        {{ detail.value }}
      </div>
    </div>
    <div v-if="contactDetails" class="agent-agency-table__contact-details">
      <div
        v-for="(contactDetail, contactDetailsIndex) in contactDetails"
        :key="'agent-agency-contact-details-' + contactDetailsIndex"
      >
        <span class="agent-agency-table__details_title">
          {{ contactDetail.title }}:
        </span>
        {{ contactDetail.value }}
      </div>
    </div>
    <div
      v-if="requirementDetails"
      class="agent-agency-table__requirement-details"
    >
      <div
        v-for="(
          requirementDetail, requirementDetailsIndex
        ) in requirementDetails"
        :key="'agent-agency-requirement-details-' + requirementDetailsIndex"
      >
        <span class="agent-agency-table__details_title">
          {{ requirementDetail.title }}:
        </span>
        {{ requirementDetail.value }}
      </div>
    </div>
  </div>
  <div class="agent-agency-table__container__mobile">
    <div class="agent-agency-table__pending-details__mobile">
      <div class="agent-agency-table__pending-details-column__mobile">
        <template
          v-for="(mobileDetails, mobileDetailsIndex) in details"
          :key="'agent-agency-details-' + mobileDetailsIndex"
        >
          <div>
            <div class="agent-agency-table__details_title__mobile">
              {{ mobileDetails.title }}
            </div>
            <div class="agent-agency-table__details_field__mobile">
              {{ mobileDetails.value }}
            </div>
          </div>
        </template>
      </div>
      <div class="agent-agency-table__pending-details-column__mobile">
        <template
          v-for="(
            mobileContactDetails, mobileContactDetailsIndex
          ) in contactDetails"
          :key="'agent-agency-contact-details-' + mobileContactDetailsIndex"
        >
          <div>
            <div class="agent-agency-table__details_title__mobile">
              {{ mobileContactDetails.title }}
            </div>
            <div class="agent-agency-table__details_field__mobile">
              {{ mobileContactDetails.value }}
            </div>
          </div>
        </template>
      </div>
    </div>
    <template
      v-for="(
        mobileRequirementDetails, mobileRequirementDetailsIndex
      ) in requirementDetails"
      :key="'agent-agency-contact-details-' + mobileRequirementDetailsIndex"
    >
      <div class="agent-agency-table__details_title__mobile">
        {{ mobileRequirementDetails.title }}
      </div>
      <div class="agent-agency-table__details_field__mobile-wide">
        {{ mobileRequirementDetails.value }}
      </div>
    </template>
  </div>
</template>
<script setup lang="ts">
import { ref } from "vue";
import type { AgentAgencyDetails } from "../models/AgentAgencyDetails";
import type { Assurity_AgentPortal_Contracts_AgentContracts_Requirement as Requirement } from "@assurity/newassurelink-client";

const props = defineProps<{
  depth: number;
  marketCode: string;
  agentLevel: string;
  phoneNumber: string;
  emailAddress: string;
  pendingRequirements: Requirement[];
}>();

const details = ref<AgentAgencyDetails[]>([]);
const contactDetails = ref<AgentAgencyDetails[]>([]);
const requirementDetails = ref<AgentAgencyDetails[]>([]);

details.value = [];
details.value.push({ title: "Market Code", value: props.marketCode });
details.value.push({ title: "Agent Level", value: props.agentLevel });

contactDetails.value = [];
contactDetails.value.push({
  title: "Email",
  value: props?.emailAddress ?? "",
});
contactDetails.value.push({
  title: "Phone",
  value: props?.phoneNumber ?? "",
});

requirementDetails.value = [];
props.pendingRequirements.forEach((requirement) => {
  requirementDetails.value.push({
    title: "Requirement",
    value: requirement.description ?? "",
  });
  if (requirement.note) {
    requirementDetails.value.push({
      title: "Notes",
      value: requirement.note ?? "",
    });
  }
});
</script>
<style scoped>
@import "agent-agency-details.pcss";
</style>
