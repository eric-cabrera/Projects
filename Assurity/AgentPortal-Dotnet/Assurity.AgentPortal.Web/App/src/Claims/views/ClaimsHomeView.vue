<template>
  <div class="view">
    <div>
      <h1 class="claims-access__heading">Claims Access</h1>
    </div>
    <div class="claims-access__container">
      <div class="claims-search__container">
        <div class="claims-search">
          <div class="claims-search__header">
            <div>
              <AssurityIconSearchBlue
                class="claims-search__icon"
              ></AssurityIconSearchBlue>
            </div>
            <div class="claims-search__header-text-container">
              <div class="claims-search__header-text">
                Find an existing claim
              </div>
              <div class="claims-search__sub-header-text">
                How would you like to search for this claim?
              </div>
            </div>
          </div>
          <aHorizontalRule class="horizontal-rule" />
          <div class="claims-search__content">
            <div class="claims-search__input-container">
              <aTextInput
                id="policyNumberInput"
                placeholder="Policy/Certificate Number"
                class="claims-search__policy"
                :value="policyNumber"
                :disabled="
                  claimNumber.length > 0 ||
                  claimantFirstName.length > 0 ||
                  claimantLastName.length > 0
                "
                @input="searchPolicyCertificate($event)"
              />
            </div>
            <div class="claims-search__input-container">
              <div class="claims-search__field-divider">Or</div>
              <div>
                <aTextInput
                  id="policyClaimantFirst"
                  placeholder="Claimant First"
                  class="claims-search__claimant-first"
                  :value="claimantFirstName"
                  :disabled="policyNumber.length > 0 || claimNumber.length > 0"
                  @input="searchClaimantFirstName($event)"
                />
                <aTextInput
                  id="policyClaimantLast"
                  placeholder="Claimant Last"
                  class="claims-search__claimant-last"
                  :value="claimantLastName"
                  :disabled="policyNumber.length > 0 || claimNumber.length > 0"
                  @input="searchClaimantLastName($event)"
                />
              </div>
            </div>
            <div class="claims-search__input-container">
              <div class="claims-search__field-divider">Or</div>
              <aTextInput
                id="policyClaimNumber"
                placeholder="Claim Number"
                class="claims-search__claim"
                :value="claimNumber"
                :disabled="
                  policyNumber.length > 0 ||
                  claimantFirstName.length > 0 ||
                  claimantLastName.length > 0
                "
                @input="searchClaimNumber($event)"
              />
            </div>
          </div>
          <div class="claims-search__footer">
            <button
              :disabled="isFetching"
              @click="enableClaimsCall()"
              @keyup.enter="enableClaimsCall()"
              @keyup.space="enableClaimsCall()"
              @keypress="$event.preventDefault()"
            >
              <span v-if="!isFetching">SEARCH</span>
              <v-progress-circular
                v-if="isFetching"
                indeterminate
                size="40"
                width="8"
                color="#007b99"
                class="claims-search__loading-indicator"
                title="Loading..."
              >
              </v-progress-circular>
            </button>
          </div>
        </div>
      </div>
      <RadioButtons
        :header-text="radioButtonsConfig.headerText"
        :sub-header-text="radioButtonsConfig.subHeaderText"
        :radio-items="radioItems"
      />
    </div>
    <PopoverModal
      v-if="visibilityState.policy"
      unique-id="policyNotFoundModal"
      header-text="Policy/certificate number not found"
      :icon="AssurityIconStopSign"
      :close-btn="visibilityState.closeBtn"
    >
      No records were found that match the information entered. Please verify
      the policy/certificate number and try again. You can also search by the
      claimant's name or claim number.
    </PopoverModal>
    <PopoverModal
      v-if="visibilityState.claimant"
      unique-id="claimantNotFoundModal"
      header-text="Claimant's name not found"
      :icon="AssurityIconStopSign"
      :close-btn="visibilityState.closeBtn"
    >
      No records were found that match the information entered. Please verify
      the claimant's name and try again. You can also search by the
      Policy/Certificate Number or claim number.
    </PopoverModal>

    <PopoverModal
      v-if="visibilityState.claimNumber"
      unique-id="claimNumberNotFoundModal"
      header-text="Claim number not found"
      :icon="AssurityIconStopSign"
      :close-btn="visibilityState.closeBtn"
    >
      No records were found that match the information entered. Please verify
      the claim number and try again. You can also search by the
      policy/certificate number or the claimant’s name.
    </PopoverModal>
  </div>
</template>

<script setup lang="ts">
import { computed, watch, onMounted, ref, nextTick } from "vue";
import { useRouter } from "vue-router";
import axios from "axios";
import { useClaimsData } from "@/Claims/api";
import aHorizontalRule from "@/components/aHorizontalRule.vue";
import AssurityIconSearchBlue from "@/components/icons/AssurityIconSearchBlue.vue";
import AssurityIconStopSign from "@/components/icons/AssurityIconStopSign.vue";
import PopoverModal from "@/components/PopoverModal.vue";
import RadioButtons from "@/components/RadioButtons.vue";
import type { RadioItem } from "@/models/RadioItem.ts";
import aTextInput from "@/components/forms/aTextInput.vue";

const router = useRouter();

const claimNumber = ref("");
const claimantFirstName = ref("");
const claimantLastName = ref("");
const policyNumber = ref("");

const queryParams = computed(() => ({
  claimNumber: "",
  claimantFirstName: "",
  claimantLastName: "",
  policyNumber: "",
}));

type VisibilityState = {
  policy: boolean;
  claimant: boolean;
  claimNumber: boolean;
  closeBtn: boolean;
};

const visibilityState = ref<VisibilityState>({
  policy: policyNumber.value.length > 0,
  claimant:
    claimantFirstName.value.length > 0 || claimantLastName.value.length > 0,
  claimNumber: claimNumber.value.length > 0,
  closeBtn: true,
});

const modalMap = {
  policy: "policyNotFoundModal",
  claimant: "claimantNotFoundModal",
  claimNumber: "claimNumberNotFoundModal",
};

const { data, isFetching, refetch } = useClaimsData(queryParams, false);

watch([data, isFetching], async () => {
  if (!isFetching.value) {
    if ((data.value?.claims?.length ?? 0) > 0) {
      let routerParams;

      if (queryParams.value.policyNumber?.length) {
        routerParams = { policyNumber: queryParams.value.policyNumber };
      }

      if (queryParams.value.claimNumber?.length) {
        routerParams = { claimNumber: queryParams.value.claimNumber };
      }

      if (
        queryParams.value.claimantFirstName?.length &&
        queryParams.value.claimantLastName?.length
      ) {
        routerParams = {
          claimantFirstName: queryParams.value.claimantFirstName,
          claimantLastName: queryParams.value.claimantLastName,
        };
      }

      router.push({
        name: "claims-results",
        query: routerParams,
      });
    } else {
      showNotFoundModal();
    }
  }
});

const radioItems = ref<RadioItem[]>([]);

const radioButtonsConfig = {
  headerText: "Filing a claim",
  subHeaderText: "What type of claim is your client reporting?",
};

const searchPolicyCertificate = (newPolicyNumber: string) => {
  policyNumber.value = newPolicyNumber;
};

const searchClaimNumber = (newClaimNumber: string) => {
  claimNumber.value = newClaimNumber;
};

const searchClaimantFirstName = (newClaimantFirstName: string) => {
  claimantFirstName.value = newClaimantFirstName;
};

const searchClaimantLastName = (newClaimantLastName: string) => {
  claimantLastName.value = newClaimantLastName;
};

function enableClaimsCall() {
  queryParams.value.policyNumber = policyNumber.value;
  queryParams.value.claimNumber = claimNumber.value;
  queryParams.value.claimantFirstName = claimantFirstName.value;
  queryParams.value.claimantLastName = claimantLastName.value;
  refetch();
}

const showNotFoundModal = async () => {
  visibilityState.value = {
    policy: false,
    claimant: false,
    claimNumber: false,
    closeBtn: true,
  };

  const conditions: { condition: boolean; key: keyof VisibilityState }[] = [
    { condition: policyNumber.value.length > 0, key: "policy" },
    { condition: claimNumber.value.length > 0, key: "claimNumber" },
    {
      condition:
        claimantFirstName.value.length > 0 || claimantLastName.value.length > 0,
      key: "claimant",
    },
  ];

  for (const { condition, key } of conditions) {
    if (condition) {
      visibilityState.value[key] = true;
      break;
    }
  }

  await nextTick();

  for (const [key, modalId] of Object.entries(modalMap)) {
    if (visibilityState.value[key as keyof VisibilityState]) {
      document.getElementById(modalId)?.showPopover();
      break;
    }
  }
};

onMounted(async () => {
  const queryBody = `
    query {
        agent_center_claims_products( filter: {
          _and: [
            {status: {_eq: "published"}},
          ] 
        }) {
          name
          page {
            slug
          }
          date_updated
        }
    }
  `;

  const response = await axios({
    url: "/content/graphql",
    method: "post",
    data: {
      query: queryBody,
    },
  });

  if (response.status == 200) {
    const claimsPages = response.data.data.agent_center_claims_products;
    const items = claimsPages.map(
      (claimsPage: { name: string; page: { slug: string } }) => ({
        product: claimsPage.name,
        productUrl: claimsPage.page.slug,
      }),
    );

    radioItems.value = items;
  }
});
</script>

<style scoped lang="pcss">
.popover {
  max-width: 500px;
}

.view {
  position: relative;
  max-width: 560px;
  margin-left: auto;
  margin-right: auto;

  @media (width >= 960px) {
    max-width: none;
  }
}

.claims-access__heading {
  color: var(--primary-color);
  font-size: 1.75rem;
  font-weight: 700;
  font-family: var(--primary-font);
  line-height: 1.5;
  margin-top: 30px;
  margin-bottom: 19px;

  @media (width >= 960px) {
    font-size: 3.0625rem;
    margin-top: 80px;
  }
}

.claims-access__container {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-xl);
  margin: var(--spacing-xl) 0;

  @media (width >= 960px) {
    flex-direction: row;
    flex-wrap: wrap;
    gap: var(--spacing-xxl);
    margin: var(--spacing-xxl) 0;
  }
}

.claims-search__container {
  display: flex;
  justify-content: center;
  box-shadow: 1px 1px 20px rgba(0, 0, 0, 0.12);
  border-radius: 8px;
  @media (width >= 960px) {
    display: block;
  }
}

.claims-search {
  width: 100%;
  padding: 20px;
  background-color: var(--white);
  border-radius: 8px;

  @media (width >= 960px) {
    max-width: 572px;
    padding: 32px 48px 40px;
  }

  .claims-search__header {
    display: flex;
    align-items: center;
    padding-bottom: 16px;

    .claims-search__icon {
      width: 78px;
      height: 78px;
    }

    .claims-search__header-text {
      font-size: 1.3125rem;
      padding-left: 15px;
      color: var(--secondary-color);
      font-weight: bold;

      @media (width >= 960px) {
        font-size: 1.75rem;
        padding-left: 24px;
      }
    }

    .claims-search__sub-header-text {
      padding-left: 15px;
      color: var(--text-grey);
      font-weight: bold;

      @media (width >= 960px) {
        padding: 4px 0 8px 24px;
      }
    }
  }

  .claims-search__content {
    display: flex;
    justify-content: center;
    flex-wrap: wrap;
    color: var(--text-grey);
    padding-top: 20px;

    .claims-search__field-divider {
      padding-bottom: 30px;
      text-align: center;
    }

    .claims-search__input-container {
      width: 100%;
      padding-top: 10px;

      span {
        font-weight: bold;
      }

      input {
        border: 1px solid var(--text-grey);
        border-radius: 4px;
        height: 52px;
        padding: 12px;
      }

      input,
      input::placeholder {
        font-size: 1.3125rem;
      }

      .claims-search__policy,
      .claims-search__claim {
        width: 100%;
      }

      .claims-search__claimant-first {
        float: left;
        width: 42%;
      }

      .claims-search__claimant-last {
        float: right;
        width: 55%;
      }
    }
  }

  .claims-search__footer {
    padding-top: 20px;

    @media (width >= 600px) {
      display: flex;
      justify-content: center;
      padding-top: 39px;
    }

    button {
      color: var(--white);
      background-color: var(--secondary-color);
      width: 100%;
      height: 52px;
      border-radius: 2px;
      font-size: 1.125rem;
      font-weight: bold;

      @media (width >= 600px) {
        width: 220px;
      }
    }
  }
}
</style>
