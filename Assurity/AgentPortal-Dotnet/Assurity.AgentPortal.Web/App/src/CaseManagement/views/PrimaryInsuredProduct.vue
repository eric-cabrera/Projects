<template>
  <div class="case-management__table__product">
    <table
      v-if="isAccidentalDeath"
      class="case-management__table__product-table"
    >
      <tbody>
        <tr>
          <td colspan="2">{{ productName }}</td>
        </tr>
        <tr v-if="base?.benefitAmount">
          <td>Benefit Amount:</td>
          <td>
            {{ FormatHelpers.formatMoney(base?.benefitAmount ?? 0) }}
          </td>
        </tr>
        <tr v-if="base?.coveragePeriod">
          <td>Benefit Period:</td>
          <td>
            {{ FindAccidentalDeathCoveragePeriod(base?.coveragePeriod ?? "") }}
          </td>
        </tr>
        <tr v-if="accidentOnlyDisabilityIncome">
          <td>Accident-Only Disability Income:</td>
          <td>
            {{
              FormatHelpers.formatMoney(
                accidentOnlyDisabilityIncome?.benefitAmount ?? 0,
              )
            }}
          </td>
        </tr>
        <tr v-if="criticalAccident">
          <td>Critical Accident:</td>
          <td><AssurityIconCheckBox class="case-management__check-box" /></td>
        </tr>
        <tr v-if="waiverOfPremium">
          <td>Disability Waiver of Premium:</td>
          <td><AssurityIconCheckBox class="case-management__check-box" /></td>
        </tr>
        <tr v-if="returnOfPremium">
          <td>Return of Premium:</td>
          <td><AssurityIconCheckBox class="case-management__check-box" /></td>
        </tr>
      </tbody>
    </table>

    <table
      v-if="isAcccidentalInsurance"
      class="case-management__table__product-table"
    >
      <tbody>
        <tr>
          <td colspan="2">{{ productName }}</td>
        </tr>
        <tr v-if="base?.insuredOption">
          <td>Insured Option:</td>
          <td>
            {{ base?.insuredOption ?? "" }}
          </td>
        </tr>
        <tr v-if="base?.benefitPackage">
          <td>Benefit Option:</td>
          <td>
            {{ base?.benefitPackage ?? "" }}
          </td>
        </tr>
        <tr v-if="base?.coverageType">
          <td>Coverage Option:</td>
          <td>
            {{ FindAITCoverageType(base?.coverageType ?? "") }}
          </td>
        </tr>
        <tr v-if="accidentOnlyDisabilityIncome">
          <td>Accidental-Only Disability Income:</td>
          <td>
            {{
              FormatHelpers.formatMoney(
                accidentOnlyDisabilityIncome?.benefitAmount ?? 0,
              )
            }}
          </td>
        </tr>
        <tr v-if="preventative">
          <td>Preventative:</td>
          <td><AssurityIconCheckBox class="case-management__check-box" /></td>
        </tr>
      </tbody>
    </table>

    <table v-if="isTermedLife" class="case-management__table__product-table">
      <tbody>
        <tr>
          <td colspan="2">{{ productName }}</td>
        </tr>
        <tr v-if="base?.benefitAmount">
          <td>Benefit Amount:</td>
          <td>
            {{ FormatHelpers.formatMoney(base?.benefitAmount ?? 0) }}
          </td>
        </tr>
        <tr v-if="base?.termPeriod">
          <td>Benefit Period:</td>
          <td>
            {{ FindTermLifeTermPeriod(base?.termPeriod ?? "") }}
          </td>
        </tr>
        <tr v-if="supplementalDisabilityIncome">
          <td>Supplemental Disability Rider:</td>
          <td>
            {{
              FormatHelpers.formatMoney(
                supplementalDisabilityIncome?.benefitAmount ?? 0,
              )
            }}
          </td>
        </tr>
        <tr v-if="criticalIllnessBenefit">
          <td>Critical Illness Benefit:</td>
          <td>
            {{
              FormatHelpers.formatMoney(
                criticalIllnessBenefit?.benefitAmount ?? 0,
              )
            }}
          </td>
        </tr>
        <tr v-if="waiverOfPremium">
          <td>Disability Waiver of Premium:</td>
          <td><AssurityIconCheckBox class="case-management__check-box" /></td>
        </tr>
        <tr v-if="returnOfPremium">
          <td>Return of Premium:</td>
          <td><AssurityIconCheckBox class="case-management__check-box" /></td>
        </tr>
        <tr v-if="monthlyDisabilityIncome">
          <td>Monthly Disability Income:</td>
          <td>
            {{
              FormatHelpers.formatMoney(
                monthlyDisabilityIncome?.benefitAmount ?? 0,
              )
            }}
          </td>
        </tr>
      </tbody>
    </table>

    <table
      v-if="is5YearRenewableTerm"
      class="case-management__table__product-table"
    >
      <tbody>
        <tr>
          <td colspan="2">{{ productName }}</td>
        </tr>
        <tr v-if="base?.benefitAmount">
          <td>Benefit Amount:</td>
          <td>
            {{ FormatHelpers.formatMoney(base?.benefitAmount ?? 0) }}
          </td>
        </tr>
      </tbody>
    </table>

    <table
      v-if="
        isIncomeProtectionAcccidentOnly ||
        isIncomeProtectionAcccidentAndSickness
      "
      class="case-management__table__product-table"
    >
      <tbody>
        <tr>
          <td colspan="2">{{ productName }}</td>
        </tr>
        <tr v-if="isIncomeProtectionAcccidentOnly">
          <td>Coverage:</td>
          <td>Accident Only</td>
        </tr>
        <tr v-if="isIncomeProtectionAcccidentAndSickness">
          <td>Coverage:</td>
          <td>Accident and Sickness</td>
        </tr>

        <tr v-if="base?.benefitAmount">
          <td>Weekly Benefit Amount:</td>
          <td>
            {{ FormatHelpers.formatMoney(base?.benefitAmount ?? 0) }}
          </td>
        </tr>
        <tr v-if="base?.benefitPeriod">
          <td>Benefit Period:</td>
          <td>
            {{ FindIncomeProtectionBenefitPeriod(base?.benefitPeriod ?? "") }}
          </td>
        </tr>
        <tr v-if="base?.eliminationPeriod">
          <td>Elimination Period:</td>
          <td>
            {{
              FindIncomeProtectionEliminationPeriod(
                base?.eliminationPeriod ?? "",
              )
            }}
          </td>
        </tr>
        <tr v-if="catastrophicDisabilityBenefit?.benefitAmount">
          <td>Catastrophic Disability:</td>
          <td>
            {{
              FormatHelpers.formatMoney(
                catastrophicDisabilityBenefit?.benefitAmount ?? 0,
              )
            }}
          </td>
        </tr>

        <tr v-if="accidentOnlyDisabilityIncome">
          <td>Accident-Only Disability Income:</td>
          <td>
            {{
              FormatHelpers.formatMoney(
                accidentOnlyDisabilityIncome?.benefitAmount ?? 0,
              )
            }}
          </td>
        </tr>
        <tr v-if="criticalIllnessBenefit">
          <td>Critical Illness Benefit:</td>
          <td>
            {{
              FormatHelpers.formatMoney(
                criticalIllnessBenefit?.benefitAmount ?? 0,
              )
            }}
          </td>
        </tr>
        <tr v-if="waiverOfPremium">
          <td>Disability Waiver of Premium:</td>
          <td><AssurityIconCheckBox class="case-management__check-box" /></td>
        </tr>
        <tr v-if="returnOfPremium">
          <td>Return of Premium:</td>
          <td><AssurityIconCheckBox class="case-management__check-box" /></td>
        </tr>
        <tr v-if="monthlyDisabilityIncome">
          <td>Monthly Disability Income:</td>
          <td>
            {{
              FormatHelpers.formatMoney(
                monthlyDisabilityIncome?.benefitAmount ?? 0,
              )
            }}
          </td>
        </tr>
        <tr v-if="familyCare">
          <td>Family Care:</td>
          <td><AssurityIconCheckBox class="case-management__check-box" /></td>
        </tr>
        <tr v-if="guaranteedInsurability">
          <td>Guaranteed Insurability:</td>
          <td><AssurityIconCheckBox class="case-management__check-box" /></td>
        </tr>
        <tr v-if="ownOccupation">
          <td>Own Occupation:</td>
          <td><AssurityIconCheckBox class="case-management__check-box" /></td>
        </tr>
        <tr v-if="ownOccupation">
          <td>Benefit Period:</td>
          <td>
            <div style="margin-left: 30px">
              {{
                FindIncomeProtectionBenefitPeriod(
                  ownOccupation?.benefitPeriod ?? "",
                )
              }}
            </div>
          </td>
        </tr>
        <tr v-if="retroactiveInjuryBenefit">
          <td>Retroactive Injury:</td>
          <td><AssurityIconCheckBox class="case-management__check-box" /></td>
        </tr>
        <tr v-if="socialInsuranceOffset">
          <td>Social Insurance Offset:</td>
          <td><AssurityIconCheckBox class="case-management__check-box" /></td>
        </tr>
      </tbody>
    </table>

    <table
      v-if="isCriticalIllness"
      class="case-management__table__product-table"
    >
      <tbody>
        <tr>
          <td colspan="2">{{ productName }}</td>
        </tr>
        <tr v-if="base?.benefitAmount">
          <td>Benefit Amount:</td>
          <td>
            {{ FormatHelpers.formatMoney(base?.benefitAmount ?? 0) }}
          </td>
        </tr>
        <tr v-if="accidentalDeathBenefit?.benefitAmount">
          <td>Accidental Death Benefit Rider:</td>
          <td>
            {{
              FormatHelpers.formatMoney(
                accidentalDeathBenefit?.benefitAmount ?? 0,
              )
            }}
          </td>
        </tr>
        <tr v-if="additionalCriticalIllness">
          <td>Additional Critical Illness Rider:</td>
          <td>
            {{ FormatHelpers.formatMoney(base?.benefitAmount ?? 0) }}
          </td>
        </tr>
        <tr v-if="criticalAccident">
          <td>Critical Accident Rider:</td>
          <td><AssurityIconCheckBox class="case-management__check-box" /></td>
        </tr>
        <tr v-if="waiverOfPremium">
          <td>Disability Waiver of Premium Rider:</td>
          <td><AssurityIconCheckBox class="case-management__check-box" /></td>
        </tr>
        <tr v-if="increasingBenefit">
          <td>Increasing Benefit Rider:</td>
          <td><AssurityIconCheckBox class="case-management__check-box" /></td>
        </tr>
        <tr v-if="lossOfIndependentLiving">
          <td>Loss of Independent Living Rider:</td>
          <td>
            {{
              FormatHelpers.formatMoney(
                lossOfIndependentLiving?.benefitAmount ?? 0,
              )
            }}
          </td>
        </tr>
        <tr v-if="reoccurrence">
          <td>Reoccurrence Rider:</td>
          <td><AssurityIconCheckBox class="case-management__check-box" /></td>
        </tr>
        <tr v-if="returnOfPremium">
          <td>Return of Premium Rider:</td>
          <td><AssurityIconCheckBox class="case-management__check-box" /></td>
        </tr>
      </tbody>
    </table>

    <table v-if="isCenturyPlus" class="case-management__table__product-table">
      <tbody>
        <tr>
          <td colspan="2">{{ productName }}</td>
        </tr>
        <tr v-if="base?.benefitAmount">
          <td>Monthly Benefit Amount:</td>
          <td>
            {{ FormatHelpers.formatMoney(base?.benefitAmount ?? 0) }}
          </td>
        </tr>
        <tr v-if="base?.benefitPeriod">
          <td>Benefit Period:</td>
          <td>
            {{ FindCenturyPlusTermPeriod(base?.benefitPeriod ?? "") }}
          </td>
        </tr>
        <tr v-if="base?.eliminationPeriod">
          <td>Elimination Period:</td>
          <td>
            {{
              FindCenturyPlusEliminationPeriod(base?.eliminationPeriod ?? "")
            }}
          </td>
        </tr>
        <tr v-if="accidentalDeathBenefit?.benefitAmount">
          <td>Accidental Death Benefit Rider:</td>
          <td>
            {{ FormatHelpers.formatMoney(base?.benefitAmount ?? 0) }}
          </td>
        </tr>
        <tr v-if="supplementalDisabilityIncome">
          <td>Supplemental Disability Rider:</td>
          <td>
            {{
              FormatHelpers.formatMoney(
                supplementalDisabilityIncome?.benefitAmount ?? 0,
              )
            }}
          </td>
        </tr>
        <tr v-if="criticalAccident">
          <td>Critical Accident Rider:</td>
          <td><AssurityIconCheckBox class="case-management__check-box" /></td>
        </tr>
        <tr v-if="waiverOfPremium">
          <td>Disability Waiver of Premium Rider:</td>
          <td><AssurityIconCheckBox class="case-management__check-box" /></td>
        </tr>
        <tr v-if="increasingBenefit">
          <td>Increasing Benefit Rider:</td>
          <td><AssurityIconCheckBox class="case-management__check-box" /></td>
        </tr>
        <tr v-if="lossOfIndependentLiving">
          <td>Loss of Independent Living Rider:</td>
          <td>
            {{
              FormatHelpers.formatMoney(
                lossOfIndependentLiving?.benefitAmount ?? 0,
              )
            }}
          </td>
        </tr>
        <tr v-if="automaticBenefitIncrease">
          <td>Automatic Benefit Increase Rider:</td>
          <td><AssurityIconCheckBox class="case-management__check-box" /></td>
        </tr>
        <tr v-if="guaranteedInsurability">
          <td>Guaranteed Insurability Rider:</td>
          <td><AssurityIconCheckBox class="case-management__check-box" /></td>
        </tr>
        <tr v-if="nonCancelable">
          <td>Non-Cancelable Rider:</td>
          <td><AssurityIconCheckBox class="case-management__check-box" /></td>
        </tr>
        <tr v-if="ownOccupation">
          <td>Own Occupation Rider:</td>
          <td>
            {{ FindCenturyPlusTermPeriod(ownOccupation?.benefitPeriod ?? "") }}
          </td>
        </tr>
        <tr v-if="residualDisabilityBenefit">
          <td>Own Occupation Rider:</td>
          <td>
            {{
              FindCenturyPlusTermPeriod(
                residualDisabilityBenefit?.benefitPeriod ?? "",
              )
            }}
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>
<script setup lang="ts">
import dayjs from "dayjs";
import relativeTime from "dayjs/plugin/relativeTime";
import { type Assurity_ApplicationTracker_Contracts_DataTransferObjects_Coverage as Coverage } from "@assurity/newassurelink-client";
import AssurityIconCheckBox from "@/components/icons/AssurityIconCheckBox.vue";
import {
  FindAITCoverageType,
  FindAccidentalDeathCoveragePeriod,
  FindTermLifeTermPeriod,
  FindIncomeProtectionBenefitPeriod,
  FindIncomeProtectionEliminationPeriod,
  FindCenturyPlusTermPeriod,
  FindCenturyPlusEliminationPeriod,
} from "@/CaseManagement/utils/caseUtils";
import FormatHelpers from "@/Shared/utils/FormatHelpers";
import { ProductType } from "../models/enums/ProductType";
const props = defineProps<{
  product?: string | null;
  productName?: string | null;
  coverages?: Coverage[] | null;
}>();
dayjs.extend(relativeTime);

const base = props.coverages?.find(
  (coverage) => coverage.coverageLookup === "Base",
);

const accidentOnlyDisabilityIncome = props.coverages?.find(
  (coverage) => coverage.coverageLookup === "AccidentOnlyDisabilityIncome",
);

const criticalAccident = props.coverages?.find(
  (coverage) => coverage.coverageLookup === "CriticalAccident",
);

const criticalIllnessBenefit = props.coverages?.find(
  (coverage) => coverage.coverageLookup === "CriticalIllnessBenefit",
);

const monthlyDisabilityIncome = props.coverages?.find(
  (coverage) => coverage.coverageLookup === "MonthlyDisabilityIncome",
);

const waiverOfPremium = props.coverages?.find(
  (coverage) => coverage.coverageLookup === "WaiverOfPremium",
);

const returnOfPremium = props.coverages?.find(
  (coverage) => coverage.coverageLookup === "ReturnOfPremium",
);

const preventative = props.coverages?.find(
  (coverage) => coverage.coverageLookup === "Preventive",
);

const catastrophicDisabilityBenefit = props.coverages?.find(
  (coverage) => coverage.coverageLookup === "CatastrophicDisabilityBenefit",
);

const familyCare = props.coverages?.find(
  (coverage) => coverage.coverageLookup === "FamilyCare",
);

const guaranteedInsurability = props.coverages?.find(
  (coverage) => coverage.coverageLookup === "GuaranteedInsurability",
);

const ownOccupation = props.coverages?.find(
  (coverage) => coverage.coverageLookup === "OwnOccupation",
);

const retroactiveInjuryBenefit = props.coverages?.find(
  (coverage) => coverage.coverageLookup === "RetroactiveInjuryBenefit",
);

const socialInsuranceOffset = props.coverages?.find(
  (coverage) => coverage.coverageLookup === "SocialInsuranceOffset",
);

const accidentalDeathBenefit = props.coverages?.find(
  (coverage) => coverage.coverageLookup === "AccidentalDeathBenefit",
);

const additionalCriticalIllness = props.coverages?.find(
  (coverage) => coverage.coverageLookup === "AdditionalCriticalIllness",
);

const increasingBenefit = props.coverages?.find(
  (coverage) => coverage.coverageLookup === "IncreasingBenefit",
);

const lossOfIndependentLiving = props.coverages?.find(
  (coverage) => coverage.coverageLookup === "LossOfIndependentLiving",
);

const reoccurrence = props.coverages?.find(
  (coverage) => coverage.coverageLookup === "Reoccurrence",
);

const supplementalDisabilityIncome = props.coverages?.find(
  (coverage) => coverage.coverageLookup === "SupplementalDisabilityIncome",
);

const automaticBenefitIncrease = props.coverages?.find(
  (coverage) => coverage.coverageLookup === "AutomaticBenefitIncrease",
);

const nonCancelable = props.coverages?.find(
  (coverage) => coverage.coverageLookup === "NonCancelable",
);

const residualDisabilityBenefit = props.coverages?.find(
  (coverage) => coverage.coverageLookup === "residualDisabilityBenefit",
);

const isAccidentalDeath = [
  ProductType.AccidentalDeath as string,
  ProductType.AccidentalDeathDismemberment as string,
].includes(props.product ?? "");

const isAcccidentalInsurance =
  props.product === ProductType.AccidentInsuranceTiered;

const isTermedLife = [
  ProductType.TermLife as string,
  ProductType.TermDeveloperEdition as string,
].includes(props.product ?? "");

const isIncomeProtectionAcccidentOnly =
  props.product === ProductType.IncomeProtectionAccidentOnly;

const isIncomeProtectionAcccidentAndSickness =
  props.product === ProductType.IncomeProtectionAccidentSickness;

const isCriticalIllness = props.product === ProductType.CriticalIllness;

const isCenturyPlus =
  props.product === ProductType.CentryPlusDisabilityInsurance;

const is5YearRenewableTerm = props.product === ProductType.FiveYearTerm;
</script>
