<template>
  <div class="AgentStatement-container">
    <v-sheet :class="{ 'elevation-4 rounded-lg': !isMobile }">
      <div class="agent-statement__content">
        <span v-if="errorMessage" class="AgentStatement-error">{{
          errorMessage
        }}</span>
        <h2 class="AgentStatement-header">Agent Statements</h2>
        <AHorizontalRule />
        <aForm
          id="AgentStatement-Form"
          :loading="loading"
          @submit.prevent="submitForm()"
        >
          <aDropdown
            id="AgentStatement-dropdown__agentid"
            v-model="agentId"
            class="AgentStatement-dropdown"
            :options="agentData"
            label="Agent Name or ID"
            :required="true"
            full-width
            :empty-first-option="true"
          ></aDropdown>
          <aDropdown
            id="AgentStatement-dropdown__cycledate"
            v-model="cycleDate"
            class="AgentStatement-dropdown"
            :options="cycleDateValues"
            label="Cycle Date"
            :required="true"
            full-width
            :empty-first-option="true"
          ></aDropdown>
          <div
            v-for="report in reportTypes"
            :key="report.value"
            class="AgentStatement-radio"
          >
            <label class="AgentStatement-radio-control">
              <input
                :id="`${report.value}-Radio`"
                v-model="reportType"
                type="radio"
                :value="report.value"
                name="AgentStatementRadioGroup"
                :checked="report.value === reportType"
                required
              />
              {{ report.label }}
            </label>
          </div>
          <aButton
            class="AgentStatement-SubmitButton"
            button-style="primary"
            type="submit"
            text="Run Report"
            size="standard"
            @keypress="$event.preventDefault()"
          />
        </aForm>
      </div>
    </v-sheet>
  </div>
</template>
<script setup lang="ts">
import { computed, ref, onMounted, onUnmounted } from "vue";
import aButton from "@/components/forms/aButton.vue";
import aForm from "@/components/forms/aForm.vue";
import aDropdown from "@/components/forms/aDropdown.vue";
import AHorizontalRule from "@/components/aHorizontalRule.vue";

const props = defineProps<{
  loading: boolean;
  agentData: { value: string; label: string }[];
  cycleDates: string[];
  reportTypes: { value: string; label: string }[];
  errorMessage: string;
}>();

const cycleDateValues = computed(() => {
  return props.cycleDates.map((date) => {
    return { value: date };
  });
});

const emits = defineEmits<{
  (
    e: "submit",
    value: { agent: string; cycleDate: string; reportType: string },
  ): void;
}>();

const agentId = ref("");
const cycleDate = ref("");
const reportType = ref(props.reportTypes[0].value);

const isMobile = ref(window.innerWidth <= 960);
const updateIsMobile = () => {
  isMobile.value = window.innerWidth <= 960;
};

onMounted(() => {
  window.addEventListener("resize", updateIsMobile);
});

onUnmounted(() => {
  window.removeEventListener("resize", updateIsMobile);
});

async function submitForm() {
  emits("submit", {
    agent: agentId.value,
    cycleDate: cycleDate.value,
    reportType: reportType.value,
  });
}
</script>
<style scoped lang="pcss">
.AgentStatement-container {
  margin: 15px 0;
}
.agent-statement__content {
  padding: 24px 40px;
}

.AgentStatement-error {
  color: var(--primary-color);
  font-weight: bold;
}

.AgentStatement-radio {
  margin: 10px 0px 10px 0px;
  display: flex;
}

.AgentStatement-radio-control input {
  margin-right: 8px;
  /* The native appearance is hidden */
  appearance: none;
  -webkit-appearance: none;

  width: 18px;
  height: 18px;

  /* For a circular appearance we need a border-radius. */
  border-radius: 50%;
  border: 1px solid var(--text-grey);

  /* The background will be the radio dot's color. */
  background: #fff;
  cursor: pointer;
}

.AgentStatement-radio-control input[type="radio"]:before {
  content: "";
  display: block;
  width: 80%;
  height: 80%;
  margin: 12% auto;
  border-radius: 50%;
}

.AgentStatement-radio-control input[type="radio"]:checked:before {
  background: var(--secondary-color);
}

.AgentStatement-dropdown {
  margin-top: 10px;
}

.AgentStatement-radio-control {
  cursor: pointer;
  font-size: 18px;
}

.AgentStatement-header {
  padding-bottom: 8px;
  color: var(--text-grey);
}
.v-sheet.elevation-4 {
  box-shadow: var(--desktop-shadow) !important;
}
</style>
