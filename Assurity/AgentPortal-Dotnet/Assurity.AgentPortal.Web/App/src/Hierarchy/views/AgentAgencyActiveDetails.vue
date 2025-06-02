<template>
  <div class="agent-agency-table__active-details__desktop">
    <template v-for="x in depth" :key="x">
      <div class="agent-agency-table__agent-level">&nbsp;</div>
    </template>
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
  </div>
  <div class="agent-agency-table__container__mobile">
    <div class="agent-agency-table__container__mobile_title">
      Agent/Agency Details
    </div>
    <aHorizontalRule
      class="mobile-data-table__dotted-divider"
    ></aHorizontalRule>
    <div class="agent-agency-table__active-details__mobile">
      <div class="agent-agency-table__contact-details__mobile-column">
        <template
          v-for="(mobileDetails, mobileDetailsIndex) in detailsLeftMobile"
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
      <div class="agent-agency-table__contact-details__mobile-column">
        <template
          v-for="(mobileDetails, mobileDetailsIndex) in detailsRightMobile"
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
    </div>
    <aHorizontalRule
      class="mobile-data-table__dotted-divider"
    ></aHorizontalRule>
    <div class="agent-agency-table__contact-details__mobile">
      <div class="agent-agency-table__contact-details__mobile-column">
        <template
          v-for="(
            mobileContactDetails, mobileContactDetailsIndex
          ) in contactDetailsLeft"
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
      <div class="agent-agency-table__contact-details__mobile-column">
        <template
          v-for="(
            mobileContactDetails, mobileContactDetailsIndex
          ) in contactDetailsRight"
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
  </div>
</template>
<script setup lang="ts">
import { ref, watchEffect } from "vue";
import type { AgentAgencyDetails } from "../models/AgentAgencyDetails";
import { useAgentInformationData } from "../api/hierarchyApi";
import type { Assurity_AgentPortal_Contracts_AgentContracts_AgentContractInformation as AgentContractInformation } from "@assurity/newassurelink-client";
import FormatHelpers from "@/Shared/utils/FormatHelpers";
import { useHierarchyStore } from "../hierarchyStore";
import aHorizontalRule from "@/components/aHorizontalRule.vue";

const props = defineProps<{
  depth: number;
  agentNumber: string;
  marketCode: string;
  agentLevel: string;
  companyCode: string;
}>();

const downlineParams = ref({
  agentLevel: props.agentLevel,
  agentNumber: props.agentNumber,
  marketCode: props.marketCode,
  companyCode: props.companyCode,
});

const hierarchyStore = useHierarchyStore();

const viewAsParams = ref({
  agentLevel: hierarchyStore.filterOptions.agentLevel,
  agentNumber: hierarchyStore.filterOptions.agentNumber,
  marketCode: hierarchyStore.filterOptions.marketCode,
  companyCode: hierarchyStore.filterOptions.companyCode,
});

const details = ref<AgentAgencyDetails[]>([]);
const detailsLeftMobile = ref<AgentAgencyDetails[]>([]);
const detailsRightMobile = ref<AgentAgencyDetails[]>([]);
const contactDetails = ref<AgentAgencyDetails[]>([]);
const contactDetailsLeft = ref<AgentAgencyDetails[]>([]);
const contactDetailsRight = ref<AgentAgencyDetails[]>([]);

const { data } = useAgentInformationData(downlineParams, viewAsParams);

const processDetailsData = (agentAgency: AgentContractInformation) => {
  processDetails(agentAgency);
  processContactDetails(agentAgency);

  processDetailsMobile(agentAgency);
  processContactDetailsMobile(agentAgency);
};

const processDetails = (agentAgency: AgentContractInformation) => {
  details.value = [];
  details.value.push({ title: "Agent ID", value: props.agentNumber });
  details.value.push({ title: "Market Code", value: props.marketCode });
  details.value.push({ title: "Agent Level", value: props.agentLevel });
  if (agentAgency.startDate) {
    details.value.push({
      title: "Start Date",
      value: agentAgency.startDate,
    });
  }

  if (agentAgency.advanceRate) {
    details.value.push({
      title: "Advance Rate",
      value: agentAgency.advanceRate,
    });
  }

  if (agentAgency.advanceFrequency) {
    details.value.push({
      title: "Advance Frequency",
      value: agentAgency.advanceFrequency,
    });
  }

  details.value.push({
    title: "Direct Deposit",
    value: agentAgency.directDeposit ?? "",
  });
  details.value.push({
    title: "Contract Status",
    value: agentAgency.contractStatus ?? "",
  });
  details.value.push({
    title: "AML",
    value: agentAgency.antiMoneyLaundering ?? "",
  });
};

const processDetailsMobile = (agentAgency: AgentContractInformation) => {
  detailsLeftMobile.value = [];
  detailsRightMobile.value = [];
  detailsLeftMobile.value.push({
    title: "Agent ID",
    value: props.agentNumber,
  });
  detailsLeftMobile.value.push({
    title: "Market Code",
    value: props.marketCode,
  });
  detailsLeftMobile.value.push({
    title: "Agent Level",
    value: props.agentLevel,
  });

  if (agentAgency.startDate) {
    detailsLeftMobile.value.push({
      title: "Start Date",
      value: agentAgency.startDate ?? "",
    });
  }

  if (agentAgency.advanceRate) {
    detailsLeftMobile.value.push({
      title: "Advance Rate",
      value: agentAgency.advanceRate,
    });
  }
  detailsRightMobile.value.push({
    title: "Direct Deposit",
    value: agentAgency.directDeposit ?? "",
  });
  detailsRightMobile.value.push({
    title: "Contract Status",
    value: agentAgency.contractStatus ?? "",
  });
  detailsRightMobile.value.push({
    title: "AML",
    value: agentAgency.antiMoneyLaundering ?? "",
  });

  if (agentAgency.advanceFrequency) {
    detailsRightMobile.value.push({
      title: "Advance Frequency",
      value: agentAgency.advanceFrequency,
    });
  }
};

const processContactDetails = (agentAgency: AgentContractInformation) => {
  contactDetails.value = [];

  contactDetails.value.push({
    title: "Address Line 1",
    value: agentAgency.address?.line1 ?? "",
  });
  if (agentAgency.address?.line2)
    contactDetails.value.push({
      title: "Address Line 2",
      value: agentAgency.address?.line2,
    });
  contactDetails.value.push({
    title: "City",
    value: agentAgency.address?.city ?? "",
  });
  contactDetails.value.push({
    title: "State",
    value: agentAgency.address?.stateAbbreviation ?? "",
  });
  contactDetails.value.push({
    title: "Zip Code",
    value: agentAgency.address?.zip ?? "",
  });
  contactDetails.value.push({
    title: "Phone",
    value: FormatHelpers.formatPhoneNumber(
      agentAgency.address?.phoneNumber ?? "",
    ),
  });
  if (agentAgency.address?.faxNumber)
    contactDetails.value.push({
      title: "Fax",
      value: FormatHelpers.formatPhoneNumber(
        agentAgency.address?.faxNumber ?? "",
      ),
    });
  contactDetails.value.push({
    title: "Email",
    value: agentAgency.address?.emailAddress ?? "",
  });
};

const processContactDetailsMobile = (agentAgency: AgentContractInformation) => {
  contactDetailsLeft.value = [];
  contactDetailsRight.value = [];

  contactDetailsLeft.value.push({
    title: "Address Line 1",
    value: agentAgency.address?.line1 ?? "",
  });
  if (agentAgency.address?.line2)
    contactDetailsLeft.value.push({
      title: "Address Line 2",
      value: agentAgency.address?.line2,
    });
  contactDetailsLeft.value.push({
    title: "City",
    value: agentAgency.address?.city ?? "",
  });
  contactDetailsLeft.value.push({
    title: "State",
    value: agentAgency.address?.stateAbbreviation ?? "",
  });
  contactDetailsLeft.value.push({
    title: "Zip Code",
    value: agentAgency.address?.zip ?? "",
  });
  contactDetailsRight.value.push({
    title: "Phone",
    value: FormatHelpers.formatPhoneNumber(
      agentAgency.address?.phoneNumber ?? "",
    ),
  });
  if (agentAgency.address?.faxNumber)
    contactDetailsRight.value.push({
      title: "Fax",
      value: FormatHelpers.formatPhoneNumber(
        agentAgency.address?.faxNumber ?? "",
      ),
    });
  contactDetailsRight.value.push({
    title: "Email",
    value: agentAgency.address?.emailAddress ?? "",
  });
};

watchEffect(() => {
  if (data.value) {
    processDetailsData(data.value);
  }
});
</script>
<style scoped>
@import "agent-agency-details.pcss";
</style>
