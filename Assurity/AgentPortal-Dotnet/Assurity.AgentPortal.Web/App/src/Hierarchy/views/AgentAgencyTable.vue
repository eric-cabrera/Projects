<template>
  <div v-if="!isLoading" class="agent-agency-table__container">
    <table class="agent-agency-table__table">
      <thead>
        <tr>
          <th class="agent-agency-table__heading-left">Agent/Agency</th>
          <th class="agent-agency-table__heading-right" colspan="2">Details</th>
        </tr>
      </thead>
      <tbody>
        <template v-if="!agentAgencies || agentAgencies.length === 0">
          <tr>
            <td colspan="2" class="agent-agency-table__not-found">
              <span class="mobile-table__error">No data</span>
              <span class="mobile-table__error-message"
                >There is no data available for display.</span
              >
            </td>
          </tr>
        </template>
        <template
          v-for="(agentAgency, index) in agentAgencies"
          :key="'agent-agency-' + index"
        >
          <tr>
            <td
              :id="'agent-' + agentAgency.agentNumber"
              colspan="2"
              class="agent-agency-table__cell"
            >
              <div class="agent-agency-table__agent-cell">
                <template v-for="x in agentAgency.depth" :key="x">
                  <div class="agent-agency-table__agent-level">&nbsp;</div>
                </template>
                <div class="agent-agency-table__agent-info">
                  <AssurityIconHome
                    v-if="agentAgency.depth === 0"
                    class="agent-agency-table__agent-icon"
                  />
                  <AssurityIconAccountMultiple
                    v-if="agentAgency.hasBranches"
                    class="agent-agency-table__agent-icon"
                  />
                  <span class="agent-agency-table__agent-number">
                    {{ agentAgency.agentNumber }}</span
                  >
                  <span :class="agentAgencySelected(agentAgency)">
                    -
                    <span class="agent-agency-table__name">{{
                      agentAgency.name
                    }}</span>
                    (<span
                      >Agent{{
                        agentAgency.agentLevel
                          ? " " + agentAgency.agentLevel
                          : ""
                      }}</span
                    >)
                  </span>
                  <span
                    v-if="agentAgency.contractStatus === JIT"
                    v-atooltip.top="{
                      text: 'Just-In-Time Agent',
                    }"
                    class="tooltip-icon__container"
                  >
                    &nbsp;<AssurityIconTimer class="jit_icon" />
                  </span>
                </div>
              </div>
            </td>
            <td
              :class="
                'agent-agency-table__details-icon' +
                (agentAgency.detailsOpen === true ? ' open' : '')
              "
            >
              <AssurityIconChevronUp
                v-if="
                  !props.isPending ||
                  (props.isPending &&
                    (agentAgency.pendingRequirements?.length ?? 0) > 0)
                "
                class="agent-agency-table__icon_desktop"
                @click="toggleAgentDetails(agentAgency)"
              />
            </td>
          </tr>
          <tr
            v-if="agentAgency.detailsOpen === true"
            class="agent-agency-table__details"
          >
            <td colspan="3">
              <AgentAgencyActiveDetails
                v-if="!props.isPending"
                :depth="agentAgency.depth ?? 0"
                :agent-number="agentAgency.agentNumber ?? ''"
                :agent-level="agentAgency.agentLevel ?? ''"
                :company-code="agentAgency.companyCode ?? ''"
                :market-code="agentAgency.marketCode ?? ''"
              />
              <AgentAgencyPendingDetails
                v-if="props.isPending"
                :depth="agentAgency.depth ?? 0"
                :agent-level="agentAgency.agentLevel ?? ''"
                :market-code="agentAgency.marketCode ?? ''"
                :phone-number="agentAgency.phoneNumber ?? ''"
                :email-address="agentAgency.emailAddress ?? ''"
                :pending-requirements="agentAgency.pendingRequirements ?? []"
              />
            </td>
            <td>&nbsp;</td>
          </tr>
        </template>
      </tbody>
    </table>
  </div>
  <div v-if="isLoading" class="agent-agency-table__loading-container">
    <v-progress-circular
      indeterminate
      size="95"
      width="8"
      color="#007b99"
      class="agent-agency-table__loading-indicator"
      title="Loading..."
    >
    </v-progress-circular>
  </div>
</template>
<script setup lang="ts">
import AssurityIconChevronUp from "@/components/icons/AssurityIconChevronUp.vue";
import AssurityIconHome from "@/components/icons/AssurityIconHome.vue";
import AssurityIconAccountMultiple from "@/components/icons/AssurityIconAccountMultiple.vue";
import AssurityIconTimer from "@/components/icons/AssurityIconTimer.vue";
import type { AgentAgencyRow } from "../models/AgentAgencyRow";
import AgentAgencyActiveDetails from "./AgentAgencyActiveDetails.vue";
import AgentAgencyPendingDetails from "./AgentAgencyPendingDetails.vue";

const props = defineProps<{
  agentAgencies: AgentAgencyRow[];
  isLoading: boolean;
  isPending: boolean;
}>();

const JIT = "JIT";
const agentAgencySelected = (agentAgency: AgentAgencyRow) => {
  agentAgency.selected ? "agent-agency-table__agent-selected" : undefined;
};

const toggleAgentDetails = (agentAgency: AgentAgencyRow) => {
  agentAgency.detailsOpen = !agentAgency.detailsOpen;
  props.agentAgencies.forEach((agency) => {
    if (agency != agentAgency && agency.detailsOpen) {
      agency.detailsOpen = false;
    }
  });
};
</script>

<style scoped>
.mobile-table__error {
  display: block;
  text-align: center;
  font-size: 1.5rem;
  font-weight: bold;
  color: var(--primary-color);
  margin: 0 0 0.67em;
}

.mobile-table__error-message {
  display: block;
  text-align: center;
  font-size: 1.125rem;
  color: var(--text-grey);
}

.jit_icon {
  fill: var(--accent-color-lighter);
}

.agent-agency-table {
  &__not-found {
    padding: 3em;
    text-align: center;
  }
  &__loading-container {
    padding: 1em;
    text-align: center;
  }
  &__heading-left {
    @media (width <= 960px) {
      color: var(--primary-color);
      font-size: 1.25em;
    }
  }
  &__agent-cell {
    display: flex;
  }
  &__agent-level {
    display: inline-block;
    width: 3em;
    min-width: 3em;
    @media (width <= 600px) {
      min-width: 1.3em;
      width: 1.3em;
    }
  }
  &__agent-info {
    display: inline;
    padding-left: 1em;
  }
  &__agent-icon {
    @media (width <= 960px) {
      display: none;
    }
  }
  &__container {
    height: 60vh;
    overflow-y: scroll;
    margin: 0 0.5em 1em 0;
    color: var(--text-grey);
    @media (width <= 960px) {
      margin: 0 0 1em 0;
    }
  }
  &__table {
    width: 100%;
    border-collapse: collapse;
    @media (width <= 960px) {
      width: 100vw;
    }
    & > thead > tr > th {
      height: var(--spacing-xxxl);
      text-align: left;
      border-bottom: 0;
      padding: var(--spacing) 0 var(--spacing-xs) var(--spacing);
      &:last-of-type {
        @media (width <= 960px) {
          text-align: right;
        }
      }
      @media (width <= 960px) {
        padding: var(--spacing-xs) 1em 20px 16px;
      }
    }
    & > thead {
      position: relative;
    }
    & > thead:after {
      content: "";
      display: block;
      position: absolute;
      bottom: 0;
      left: 0;
      border: none;
      width: calc(100% - 2em);
      max-width: none;
      height: 2px;
      background-image: radial-gradient(
        circle at 1px 1px,
        var(--border-grey) 1px,
        rgba(0, 0, 0, 0) 0
      );
      background-repeat: repeat-x;
      background-size: 6px 4px;
      margin: 0 1em;
    }
  }
  &__cell {
    height: 3.5em;
    text-align: left;
    border-bottom: 1px solid var(--bg-grey-dark);
    & svg {
      position: relative;
      top: 5px;
    }
  }
  &__agent-number {
    font-weight: bold;
  }
  &__details {
    background-color: #f1f7f9;
    color: var(--label-grey);
  }
  &__details-icon {
    text-align: center;
    border-bottom: 1px solid var(--bg-grey-dark);
    width: 80px;
    & svg {
      color: var(--accent-color);
      width: 24px;
      height: 24px;
      transform: rotate(180deg);
      transition: transform 0.3s;
      @media (width <= 960px) {
        width: 35px;
        height: 35px;
      }
    }
    @media (width <= 960px) {
      padding-right: 1em;
    }
  }
  &__details-icon.open svg {
    transform: rotate(0deg);
    transition: transform 0.3s;
  }
  &__agent-selected {
    font-weight: bold;
  }
}
</style>
