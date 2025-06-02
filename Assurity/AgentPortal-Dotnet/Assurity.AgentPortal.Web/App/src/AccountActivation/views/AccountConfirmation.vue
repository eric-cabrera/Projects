<template>
  <DynamicContent>
    <ContainerBlock class="col-span-5">
      <div class="col-span-12 settings-section-header">
        <div class="settings-section-main">
          <h3 class="heading--3 profile-sub-heading">Create Password</h3>
        </div>
        <AssuritySettingsProfileLockIcon class="settings-icon" />
      </div>
      <div class="create-password-section col-span-12">
        <aAlertBox
          ref="createPasswordAlert"
          class="create-password-alert-box"
          :alert-type="createPasswordAlertType"
        />
        <aTextInput
          ref="confirmEmailTextbox"
          class="confirm-email-input"
          placeholder="Confirm Email"
          :validators="[
            { function: ValidationHelpers.required },
            { function: ValidationHelpers.emailAddress },
          ]"
          :error-text="'Email Required'"
          :disable-auto-complete="true"
        />
        <aTextInput
          ref="newPasswordTextbox"
          class="new-password-input"
          type="password"
          placeholder="New Password"
          :validators="[{ function: ValidationHelpers.required }]"
          :disable-auto-complete="true"
          @input="newPasswordInput()"
        />
        <div class="new-password-validators">
          <div class="validator-column">
            <div class="validator">
              <span
                :class="{
                  'validator-success-indicator': passwordValidation.length,
                  'validator-failure-indicator': !passwordValidation.length,
                }"
              >
              </span>
              <span
                class="validator-text"
                :class="passwordValidation.length && 'validator-text-success'"
                >8 characters minimum
              </span>
            </div>
            <div class="validator">
              <span
                :class="{
                  'validator-success-indicator': passwordValidation.noSpaces,
                  'validator-failure-indicator': !passwordValidation.noSpaces,
                }"
              >
              </span>
              <span
                class="validator-text"
                :class="passwordValidation.noSpaces && 'validator-text-success'"
                >No spaces</span
              >
            </div>
            <div class="validator">
              <span
                :class="{
                  'validator-success-indicator':
                    passwordValidation.oneUpperCase,
                  'validator-failure-indicator':
                    !passwordValidation.oneUpperCase,
                }"
              >
              </span>
              <span
                class="validator-text"
                :class="
                  passwordValidation.oneUpperCase && 'validator-text-success'
                "
                >One uppercase character</span
              >
            </div>
          </div>
          <div class="validator-column">
            <div class="validator">
              <span
                :class="{
                  'validator-success-indicator':
                    passwordValidation.oneLowerCase,
                  'validator-failure-indicator':
                    !passwordValidation.oneLowerCase,
                }"
              >
              </span>
              <span
                class="validator-text"
                :class="
                  passwordValidation.oneLowerCase && 'validator-text-success'
                "
                >One lowercase character</span
              >
            </div>
            <div class="validator">
              <span
                :class="{
                  'validator-success-indicator': passwordValidation.oneNumber,
                  'validator-failure-indicator': !passwordValidation.oneNumber,
                }"
              >
              </span>
              <span
                class="validator-text"
                :class="
                  passwordValidation.oneNumber && 'validator-text-success'
                "
                >One number</span
              >
            </div>
          </div>
        </div>
        <aTextInput
          ref="confirmPasswordTextbox"
          class="confirm-password-input"
          type="password"
          placeholder="Confirm New Password"
          :validators="[{ function: ValidationHelpers.required }]"
          :error-text="'Passwords do not match. Please try again'"
          :disable-auto-complete="true"
          @input="confirmPasswordInput()"
        />
        <aButton
          button-style="primary"
          text="Save"
          size="s"
          class="create-password-save-button"
          :disabled="!createPasswordValid()"
          @click="saveCreatePassword()"
        >
        </aButton>
      </div>
    </ContainerBlock>
  </DynamicContent>
</template>

<script setup lang="ts">
import ContainerBlock from "@/components/content/ContainerBlock.vue";
import aButton from "@/components/forms/aButton.vue";
import aTextInput from "@/components/forms/aTextInput.vue";
import DynamicContent from "@/layouts/DynamicContent.vue";
import AssuritySettingsProfileLockIcon from "@/Settings/icons/AssuritySettingsProfileLockIcon.vue";
import ValidationHelpers from "@/Shared/utils/ValidationHelpers";
import { onMounted, ref, useTemplateRef } from "vue";
import { createPassword } from "../api";
import aAlertBox from "@/components/aAlertBox/aAlertBox.vue";
import { AlertType } from "@/components/aAlertBox/definitions";
import { useRoute, useRouter } from "vue-router";
import { useFeatureStore } from "@/stores/featureStore";

const route = useRoute();
const router = useRouter();

const confirmEmailTextbox = useTemplateRef<typeof aTextInput>(
  "confirmEmailTextbox",
);
const newPasswordTextbox =
  useTemplateRef<typeof aTextInput>("newPasswordTextbox");
const confirmPasswordTextbox = useTemplateRef<typeof aTextInput>(
  "confirmPasswordTextbox",
);
const createPasswordAlert = ref<typeof aAlertBox>();
const createPasswordAlertType = ref<AlertType>(AlertType.Success);
const passwordValidation = ref({
  length: false,
  noSpaces: false,
  oneUpperCase: false,
  oneLowerCase: false,
  oneNumber: false,
});
const activationid = ref<string>("");

onMounted(() => {
  activationid.value = route.query.id?.toLocaleString() ?? "";
});

function newPasswordInput() {
  const newPassword = newPasswordTextbox.value?.value;
  passwordValidation.value.length = ValidationHelpers.length(newPassword, [
    "8",
  ]);
  passwordValidation.value.noSpaces = ValidationHelpers.noSpaces(newPassword);
  passwordValidation.value.oneLowerCase =
    ValidationHelpers.atLeastOneLowerCase(newPassword);
  passwordValidation.value.oneUpperCase =
    ValidationHelpers.atLeastOneUpperCase(newPassword);
  passwordValidation.value.oneNumber =
    ValidationHelpers.atLeastOneNumber(newPassword);
  checkPasswordMatch();
}

function confirmPasswordInput() {
  checkPasswordMatch();
}

function checkPasswordMatch() {
  if (confirmPasswordTextbox.value) {
    confirmPasswordTextbox.value.setValidity(true);

    if (
      confirmPasswordTextbox.value?.value !== newPasswordTextbox.value?.value
    ) {
      confirmPasswordTextbox.value.setValidity(false);
    }
  }
}

function createPasswordValid(): boolean {
  if (confirmPasswordTextbox.value && confirmEmailTextbox.value) {
    const validCheck =
      (confirmPasswordTextbox.value.isValid() as boolean) &&
      (confirmEmailTextbox.value.isValid() as boolean) &&
      passwordValidation.value.length &&
      passwordValidation.value.noSpaces &&
      passwordValidation.value.oneLowerCase &&
      passwordValidation.value.oneUpperCase &&
      passwordValidation.value.oneNumber;

    return validCheck;
  }
  return false;
}

async function saveCreatePassword() {
  let alertMessage = "";
  let focusMessage = "Success!";
  const result = await createPassword(
    confirmEmailTextbox.value?.value,
    activationid.value,
    confirmPasswordTextbox.value?.value,
  );
  if (result.valid) {
    //TODO reroute to home, login?
    alertMessage = "Your account activation has been completed";
    createPasswordAlertType.value = AlertType.Success;
    if (
      confirmEmailTextbox.value &&
      newPasswordTextbox.value &&
      confirmPasswordTextbox.value
    ) {
      confirmEmailTextbox.value.value = "";
      newPasswordTextbox.value.value = "";
      confirmPasswordTextbox.value.value = "";
      confirmEmailTextbox.value.isDirty = false;
      newPasswordTextbox.value.isDirty = false;
      confirmPasswordTextbox.value.isDirty = false;
      passwordValidation.value.length = false;
      passwordValidation.value.noSpaces = false;
      passwordValidation.value.oneLowerCase = false;
      passwordValidation.value.oneUpperCase = false;
      passwordValidation.value.oneNumber = false;
      useFeatureStore().hideNav = false;
      router.push("/");
    }
  } else if (result.message) {
    focusMessage = "";
    alertMessage = result.message;
    createPasswordAlertType.value = AlertType.Error;
  } else if (result.activationAttempts) {
    focusMessage = `Unable to activate account, you have ${3 - result.activationAttempts} remaining. please try again or contact your administrator`;
    alertMessage = "";
    createPasswordAlertType.value = AlertType.Error;
  } else {
    focusMessage = `Unable to activate account, please try again or contact your administrator`;
    alertMessage = "";
    createPasswordAlertType.value = AlertType.Error;
  }
  if (createPasswordAlert.value && createPasswordAlert.value) {
    createPasswordAlert.value.focusText = focusMessage;
    createPasswordAlert.value.text = alertMessage;
    createPasswordAlert.value.showAlert();
  }
}
</script>

<style scoped lang="pcss">
@import "../../Settings/views/settings-view.pcss";

.create-password-section {
  width: 100%;
}

.confirm-email-input {
  width: 100%;
  margin-bottom: 26px;
}

.new-password-input {
  width: 100%;
}

.confirm-password-input {
  width: 100%;
  margin-bottom: 20px;
}

.new-password-validators {
  display: inline-flex;
  margin-top: 2px;
  margin-bottom: 44px;
  width: 100%;
}

.validator-column {
  width: 50%;
}

.validator {
  height: 18px;
}

.validator-success-indicator {
  display: inline-flex;
  margin-right: 8px;
  background: var(--accent-color-lighter) 0% 0% no-repeat padding-box;
  border-radius: 50%;
  opacity: 1;
  width: 7px;
  height: 7px;
  align-self: top;
}

.validator-failure-indicator {
  display: inline-flex;
  margin-right: 8px;
  background: var(--disabled-grey) 0% 0% no-repeat padding-box;
  border-radius: 50%;
  opacity: 1;
  width: 7px;
  height: 7px;
  align-self: top;
}

.validator-text {
  font-size: 0.75rem;
  color: var(--label-grey);
}

.validator-text-success {
  font-weight: bold;
}

.create-password-save-button {
  margin-left: auto;
  width: 146px;
  height: 48px;
}

.create-password-alert-box {
  width: 100%;
  margin-bottom: 46px;
}

@media (width < 960px) {
  .create-password-section {
    margin-top: -4px;
  }

  .confirm-email-input {
    margin-bottom: 9px;
  }

  .new-password-validators {
    margin-bottom: 31px;
  }

  .confirm-password-input {
    margin-bottom: 6px;
  }

  .create-password-save-button {
    width: 100%;
  }
}
</style>
