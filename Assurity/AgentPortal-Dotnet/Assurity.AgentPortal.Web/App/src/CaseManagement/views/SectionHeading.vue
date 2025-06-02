<template>
  <table>
    <tbody>
      <tr>
        <td :class="className">
          {{ heading }}
        </td>
      </tr>
    </tbody>
  </table>
</template>
<script setup lang="ts">
defineProps<{
  heading?: string;
  className?: string;
}>();
</script>
