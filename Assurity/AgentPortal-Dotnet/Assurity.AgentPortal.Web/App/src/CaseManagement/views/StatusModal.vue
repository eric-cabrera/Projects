<template>
  <PopoverModal
    :unique-id="uniqueId"
    :header-text="headerText"
    :icon="AssurityIconInfoModal"
  >
    <table class="case-management__status-modal">
      <tbody>
        <tr>
          <th>Case Status</th>
          <th>Description</th>
        </tr>
        <tr>
          <th colspan="2">
            <aHorizontalRule class="horizontal-rule" />
          </th>
        </tr>
        <tr>
          <td>Quote Sent</td>
          <td>Quote sent, application is not complete</td>
        </tr>
        <tr>
          <td>Case Started</td>
          <td>Application started, but is not complete</td>
        </tr>
        <tr>
          <td>Interview Started</td>
          <td>Interview has started, but is not complete.</td>
        </tr>
        <tr>
          <td>Interview Complete</td>
          <td>
            Interview completed, but pending acceptance of approved or approved
            with changes.
          </td>
        </tr>
        <tr>
          <td>Signature Requested</td>
          <td>
            One or more consumer electronic signatures are outstanding and
            required.
          </td>
        </tr>
        <tr>
          <td>Signature Declined</td>
          <td>One or more consumers declined to sign electronically.</td>
        </tr>
        <tr>
          <td>Signature Expired</td>
          <td>
            One or more consumers did not sign or decline electronically within
            a timeframe of
            <span style="font-weight: bold; display: contents">14 days</span>.
          </td>
        </tr>
        <tr>
          <td>Signature Completed</td>
          <td>One consumer completed e-signature.</td>
        </tr>
        <tr>
          <td>Application Submitted</td>
          <td>
            The application has been electronically submitted for processing.
          </td>
        </tr>
        <tr>
          <td>Expired</td>
          <td>
            Incomplete cases that are older than
            <span style="font-weight: bold; display: contents">14 days</span>
            from initial case create effective date.
          </td>
        </tr>
      </tbody>
    </table>
  </PopoverModal>
</template>
<script setup lang="ts">
import PopoverModal from "@/components/PopoverModal.vue";
import AssurityIconInfoModal from "@/components/icons/AssurityIconInfoModal.vue";
import aHorizontalRule from "@/components/aHorizontalRule.vue";
defineProps<{
  uniqueId: string;
  headerText?: string;
}>();
</script>
<style scoped>
.case-management {
  &__status-modal {
    width: 100%;
    @media (width >= 480px) {
      width: 880px;
    }
    th {
      text-align: left;
      &:nth-child(2) {
        display: none;
        @media (width >= 480px) {
          display: table-cell;
        }
      }
    }
    td {
      display: flex;
      padding: 5px 0 0 0;
      border-bottom: 1px solid var(--bg-grey-dark);
      &:nth-child(1) {
        font-weight: bold;
        border-bottom: none;
        @media (width >= 480px) {
          border-bottom: 1px solid var(--bg-grey-dark);
        }
      }
      @media (width >= 480px) {
        display: table-cell;
        padding: 8px 0;
      }
    }
  }
}
</style>
