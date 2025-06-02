<template>
  <v-app v-if="featureStore.hideNav" class="app__container">
    <AppBanner />
    <v-main>
      <div class="page-content page-content">
        <RouterView />
      </div>
    </v-main>
  </v-app>
  <v-app
    v-else
    :class="
      featureStore.feature.left_nav
        ? 'app__container--left-nav'
        : 'app__container'
    "
  >
    <component
      :is="featureStore.feature.left_nav ? AppNavigationDynamic : AppNavigation"
    />
    <v-main>
      <div
        class="main-content"
        :class="
          featureStore.feature.left_nav
            ? 'page-content page-content--left-nav'
            : 'page-content'
        "
      >
        <div class="content-container">
          <div v-if="utilityStore.httpStatus.loading" class="error_container">
            Loading
            <div class="loading-container">
              <aLoadingSpinner size="95" width="8"></aLoadingSpinner>
            </div>
          </div>
          <ErrorComponent
            v-else-if="utilityStore.httpStatus.unauthorized"
            variant="unauthorized"
          />
          <ErrorComponent
            v-else-if="utilityStore.httpStatus.error"
            variant="error"
          />
          <div v-else>
            <RouterView />
          </div>
        </div>
        <div v-if="!utilityStore.httpStatus.loading">
          <AppFooter />
        </div>
      </div>
    </v-main>
  </v-app>
</template>
<script setup lang="ts">
import { RouterView } from "vue-router";
import { useFeatureStore } from "@/stores/featureStore";
import { useUtilityStore } from "@/stores/utilityStore";
import aLoadingSpinner from "@/components/aLoadingSpinner.vue";
import AppNavigation from "./components/AppNavigation.vue";
import AppNavigationDynamic from "./components/AppNavigationDynamic.vue";
import AppBanner from "./components/AppBanner.vue";
import AppFooter from "./components/AppFooter.vue";
import ErrorComponent from "./components/ErrorComponent.vue";

const featureStore = useFeatureStore();
const utilityStore = useUtilityStore();

utilityStore.httpStatus.loading = true;
</script>
<style lang="pcss" scoped>
.loading-container {
  position: relative;
  top: 25px;
  left: -42px;
  width: calc(100% + 4%);
  height: calc(100% + 2%);
  margin-left: -2%;
  margin-top: -1%;
  z-index: 5;
  border-radius: 1%;
}

.main-content {
  display: flex;
  flex-direction: column;
  min-height: 100vh;
  padding-bottom: 0;
  max-width: 1920px;
}
.content-container {
  flex: 1;
}
</style>
<style lang="pcss">
html {
  scroll-behavior: smooth;
}
body {
  background: var(--bg-grey);
  font-family: var(--primary-font);
  color: var(--text-grey);
  --v-theme-on-background: var(--text-grey);
}

/* Hide visually but not from screen readers or when focused */
.visually-hidden:not(:focus):not(:active) {
  clip: rect(0 0 0 0);
  clip-path: inset(50%);
  height: 1px;
  overflow: hidden;
  position: absolute;
  white-space: nowrap;
  width: 1px;
}

a {
  color: var(--accent-color);
  font-weight: bold;
  text-decoration-skip-ink: none;
}
a[href^="mailto:"] {
  text-decoration: none;
}
a[href^="tel:"] {
  text-decoration: none;
}

.page-content {
  max-width: 100%;

  margin: 0 auto;
  padding: 0 var(--spacing);

  @media (width >= 960px) {
    padding: 0 calc(var(--spacing) * 6);
  }

  @media (width >= 1280px) {
    max-width: 1920px;
    margin-left: 0;
    margin-right: auto;
  }

  &--left-nav {
    padding-bottom: 42px;

    @media (width >= 960px) {
      flex: 1 1 auto;
      padding-left: 32px;
      padding-right: 32px;
    }

    @media (width >= 1440px) {
      padding-left: 48px;
      padding-right: 48px;
    }
  }
}

.money__container {
  text-align: right;
}

.money__dollar {
  font-size: 1.3125rem;
  font-weight: bold;
  color: var(--text-grey);

  @media (width >= 960px) {
    font-size: 1rem;
  }
}

.money__cents {
  font-size: 0.65625rem;
  font-weight: bold;
  color: var(--text-grey);

  @media (width >= 960px) {
    font-size: 0.5rem;
  }
}
</style>
