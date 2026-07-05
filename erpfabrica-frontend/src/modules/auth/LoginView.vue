<template>
  <div class="login-container">
    <div class="login-card card shadow">
      <div class="card-body p-5">
        <!-- Logo y título -->
        <div class="text-center mb-4">
          <h1 class="h3 mb-2">ERPFabrica</h1>
          <p class="text-muted">Inicia sesión para continuar</p>
        </div>

        <!-- Formulario de login -->
        <form @submit.prevent="handleLogin">
          <div class="mb-3">
            <label for="email" class="form-label">Email</label>
            <input
              type="email"
              id="email"
              class="form-control"
              v-model="formData.email"
              placeholder="tu@email.com"
              required
              :disabled="loading"
            />
          </div>

          <div class="mb-3">
            <label for="password" class="form-label">Contraseña</label>
            <input
              type="password"
              id="password"
              class="form-control"
              v-model="formData.password"
              placeholder="••••••••"
              required
              :disabled="loading"
            />
          </div>

          <!-- Mensaje de error -->
          <div v-if="error" class="alert alert-danger" role="alert">
            {{ error }}
          </div>

          <!-- Botón de submit -->
          <button
            type="submit"
            class="btn btn-primary w-100"
            :disabled="loading"
          >
            <span v-if="loading" class="spinner-border spinner-border-sm me-2" role="status"></span>
            {{ loading ? 'Iniciando sesión...' : 'Iniciar Sesión' }}
          </button>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive } from 'vue';
import { useRouter } from 'vue-router';
import { useAuthStore } from '@/stores/auth';
import { useTenantConfigStore } from '@/stores/tenant';
import { toast } from 'vue3-toastify';

const router = useRouter();
const authStore = useAuthStore();
const tenantConfigStore = useTenantConfigStore();

const loading = ref(false);
const error = ref('');

const formData = reactive({
  email: '',
  password: '',
});

const handleLogin = async () => {
  loading.value = true;
  error.value = '';

  try {
    // Realizar login
    const response = await authStore.login(formData.email, formData.password);
    
    // Cargar configuración del tenant
    await tenantConfigStore.cargarConfig(response.tenantId);
    
    // Mostrar mensaje de éxito
    toast.success(`¡Bienvenido, ${response.nombreCompleto}!`);
    
    // Redirigir al dashboard del tenant
    router.push(`/${response.tenantId}/dashboard`);
  } catch (err) {
    error.value = err.response?.data?.message || 'Error al iniciar sesión. Verifica tus credenciales.';
    toast.error(error.value);
  } finally {
    loading.value = false;
  }
};
</script>

<style scoped>
.login-container {
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  background: linear-gradient(135deg, var(--color-primario, #3498db) 0%, var(--color-secundario, #2c3e50) 100%);
}

.login-card {
  width: 100%;
  max-width: 400px;
  border-radius: 10px;
}

.btn-primary {
  background-color: var(--color-primario, #3498db);
  border-color: var(--color-primario, #3498db);
}

.btn-primary:hover {
  background-color: var(--color-secundario, #2c3e50);
  border-color: var(--color-secundario, #2c3e50);
}
</style>
