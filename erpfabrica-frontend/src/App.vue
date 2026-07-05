<script setup>
import { onMounted, computed } from 'vue'
import { useRoute, RouterView } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import MainLayout from '@/components/layout/MainLayout.vue'

const authStore = useAuthStore()
const route = useRoute()

// Cargar datos de autenticación desde localStorage al iniciar
onMounted(() => {
  if (!authStore.token && localStorage.getItem('token')) {
    authStore.loadFromStorage()
  }
})

// Determinar si debemos usar el layout principal o no (para login)
const shouldUseLayout = computed(() => {
  return route.name !== 'Login'
})
</script>

<template>
  <div id="app">
    <RouterView v-slot="{ Component, route: currentRoute }">
      <Transition name="fade" mode="out-in">
        <component 
          :is="shouldUseLayout ? MainLayout : 'div'" 
          :key="currentRoute.fullPath"
        >
          <Component :is="Component" />
        </component>
      </Transition>
    </RouterView>
  </div>
</template>

<style>
/* Variables CSS globales para personalización multi-tenant */
:root {
  --color-primario: #3498db;
  --color-secundario: #2c3e50;
}

* {
  margin: 0;
  padding: 0;
  box-sizing: border-box;
}

body {
  font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue', Arial, sans-serif;
  font-size: 0.95rem;
  line-height: 1.6;
  color: #333;
  background-color: #f8f9fa;
}

#app {
  min-height: 100vh;
}

/* Utilidades */
.text-primary-custom {
  color: var(--color-primario) !important;
}

.bg-primary-custom {
  background-color: var(--color-primario) !important;
}

/* Scrollbar personalizado */
::-webkit-scrollbar {
  width: 8px;
  height: 8px;
}

::-webkit-scrollbar-track {
  background: #f1f1f1;
  border-radius: 4px;
}

::-webkit-scrollbar-thumb {
  background: #ccc;
  border-radius: 4px;
}

::-webkit-scrollbar-thumb:hover {
  background: #aaa;
}

/* Animaciones */
.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.2s ease;
}

.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}

/* Tablas */
.table th {
  font-weight: 600;
  font-size: 0.85rem;
  text-transform: uppercase;
  color: #6c757d;
  letter-spacing: 0.5px;
}

/* Cards */
.card {
  border: none;
  border-radius: 10px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.04);
}

/* Botones */
.btn {
  border-radius: 6px;
  font-weight: 500;
  transition: all 0.2s ease;
}

.btn-primary {
  background-color: var(--color-primario);
  border-color: var(--color-primario);
}

.btn-primary:hover {
  background-color: var(--color-secundario);
  border-color: var(--color-secundario);
}

/* Formularios */
.form-control:focus,
.form-select:focus {
  border-color: var(--color-primario);
  box-shadow: 0 0 0 0.2rem rgba(52, 152, 219, 0.25);
}

/* Badges */
.badge {
  padding: 0.5em 0.75em;
  font-weight: 500;
  border-radius: 6px;
}

/* Loading spinner */
.spinner-border {
  width: 1.5rem;
  height: 1.5rem;
}
</style>
