<template>
  <div class="case-management__table__insured">
    <table>
      <tbody>
        <tr>
          <th>Primary Insured</th>
        </tr>
        <tr v-if="primaryInsured?.firstName">
          <td class="case-management__table__insured__name">
            {{ primaryInsured?.firstName + " " + primaryInsured?.lastName }}
          </td>
        </tr>
        <tr v-if="primaryInsured?.address?.line1">
          <td>{{ primaryInsured?.address?.line1 }}</td>
        </tr>
        <tr v-if="primaryInsured?.address?.line2">
          <td>
            {{ primaryInsured.address.line2 }}
          </td>
        </tr>
        <tr v-if="primaryInsured?.address?.line3">
          <td>
            {{ primaryInsured.address.line3 }}
          </td>
        </tr>
        <tr v-if="primaryInsured?.address?.line4">
          <td>
            {{ primaryInsured.address.line4 }}
          </td>
        </tr>
        <tr v-if="primaryInsured?.address?.city">
          <td>
            {{
              primaryInsured?.address?.city +
              ", " +
              primaryInsured?.address?.stateAbbreviation +
              " " +
              primaryInsured?.address?.zipCode
            }}
          </td>
        </tr>
        <tr v-if="primaryInsured?.phoneNumber">
          <td>
            {{
              primaryInsured?.phoneAreaCode + "-" + primaryInsured?.phoneNumber
            }}
          </td>
        </tr>
        <tr v-if="primaryInsured?.emailAddress">
          <td>
            {{ primaryInsured?.emailAddress }}
          </td>
        </tr>
      </tbody>
    </table>
    <table class="case-management__table__insured-demograpics">
      <tbody>
        <tr v-if="primaryInsured?.dateOfBirth && product !== 'cpdi'">
          <td>Age:</td>
          <td>
            {{ getAgeByDOB(primaryInsured?.dateOfBirth) }}
          </td>
        </tr>
        <tr v-if="primaryInsured?.dateOfBirth && product === 'cpdi'">
          <td>Date of Birth:</td>
          <td>
            {{ dayjs(primaryInsured?.dateOfBirth).format("MM/DD/YYYY") }}
          </td>
        </tr>
        <tr v-if="primaryInsured?.gender">
          <td>Gender:</td>
          <td>{{ primaryInsured?.gender }}</td>
        </tr>
        <tr v-if="['termlife', 'termde', 'ci', 'cpdi'].includes(product ?? '')">
          <td>Nicotine Use:</td>
          <td>{{ (primaryInsured?.usesTobacco ?? false) ? "Y" : "N" }}</td>
        </tr>
        <tr
          v-else-if="
            ['fiveyrt', 'ipas'].includes(product ?? '') &&
            primaryInsured?.riskClassApplicationValue
          "
        >
          <td>Nicotine Use:</td>
          <td v-if="primaryInsured?.riskClassApplicationValue === 'Tobacco'">
            Y
          </td>
          <td v-if="primaryInsured?.riskClassApplicationValue === 'NonTobacco'">
            N
          </td>
        </tr>
        <tr
          v-if="showOccupation && primaryInsured?.occupation?.occupationClass"
        >
          <td>Occupation Class:</td>
          <td>{{ primaryInsured?.occupation?.occupationClass }}</td>
        </tr>
        <tr v-if="showOccupation && primaryInsured?.occupation?.annualIncome">
          <td>Annual Income:</td>
          <td>{{ primaryInsured?.occupation?.annualIncome }}</td>
        </tr>
        <tr v-if="product === 'cpdi'">
          <td>Government Employee:</td>
          <td>
            {{ primaryInsured?.occupation?.isGovernmentEmployee ? "Y" : "N" }}
          </td>
        </tr>
        <tr v-if="['termlife', 'termde'].includes(product ?? '')">
          <td>Health:</td>
          <td
            v-if="
              primaryInsured?.riskClassApplicationValue
                ?.toLowerCase()
                .startsWith('preferredplus')
            "
          >
            Excellent
          </td>
          <td
            v-else-if="
              primaryInsured?.riskClassApplicationValue
                ?.toLowerCase()
                .startsWith('preferred')
            "
          >
            Great
          </td>
          <td
            v-else-if="
              primaryInsured?.riskClassApplicationValue
                ?.toLowerCase()
                .startsWith('standard')
            "
          >
            Good
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>
<script setup lang="ts">
import dayjs from "dayjs";
import { getAgeByDOB } from "@/CaseManagement/utils/dateUtils.js";
import relativeTime from "dayjs/plugin/relativeTime";
import { type Assurity_ApplicationTracker_Contracts_DataTransferObjects_PrimaryInsured as PrimaryInsured } from "@assurity/newassurelink-client";
import { ProductType } from "../models/enums/ProductType";

const props = defineProps<{
  primaryInsured?: PrimaryInsured;
  product?: string | null;
}>();

dayjs.extend(relativeTime);

const showOccupation =
  props.primaryInsured?.occupation &&
  [
    ProductType.IncomeProtectionAccidentSickness as string,
    ProductType.IncomeProtectionAccidentOnly as string,
    ProductType.CentryPlusDisabilityInsurance as string,
  ].includes(props.product ?? "");
</script>
