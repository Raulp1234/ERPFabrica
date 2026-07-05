<template>
  <div class="almacenes-view">
    <div class="d-flex justify-content-between align-items-center mb-4">
      <h2><i class="bi bi-box-seam me-2"></i>Gestión de Almacenes</h2>
      <button class="btn btn-primary" @click="abrirModalCrear()">
        <i class="bi bi-plus-lg me-1"></i>Nuevo Almacén
      </button>
    </div>

    <!-- Tabla de almacenes -->
    <div class="card shadow-sm">
      <div class="card-body p-0">
        <div v-if="loading" class="text-center py-5">
          <div class="spinner-border text-primary" role="status">
            <span class="visually-hidden">Cargando...</span>
          </div>
        </div>
        <div v-else-if="error" class="alert alert-danger m-3">{{ error }}</div>
        <div v-else-if="almacenes.length === 0" class="text-center py-5 text-muted">
          <i class="bi bi-inbox display-4"></i>
          <p class="mt-2">No hay almacenes registrados</p>
        </div>
        <table v-else class="table table-hover mb-0">
          <thead class="table-light">
            <tr>
              <th>ID</th>
              <th>Nombre</th>
              <th>Ubicación</th>
              <th>Es Principal</th>
              <th>Acciones</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="almacen in almacenes" :key="almacen.id">
              <td>{{ almacen.id }}</td>
              <td>{{ almacen.nombre }}</td>
              <td>{{ almacen.ubicacion }}</td>
              <td>
                <span v-if="almacen.esPrincipal" class="badge bg-success">Sí</span>
                <span v-else class="badge bg-secondary">No</span>
              </td>
              <td>
                <button class="btn btn-sm btn-outline-primary me-1" @click="editarAlmacen(almacen)">
                  <i class="bi bi-pencil"></i>
                </button>
                <button class="btn btn-sm btn-outline-danger" @click="eliminarAlmacen(almacen.id)">
                  <i class="bi bi-trash"></i>
                </button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Modal Crear/Editar -->
    <div v-if="mostrarModal" class="modal fade show d-block" tabindex="-1" style="background: rgba(0,0,0,0.5)">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">{{ esEdicion ? 'Editar Almacén' : 'Nuevo Almacén' }}</h5>
            <button type="button" class="btn-close" @click="cerrarModal()"></button>
          </div>
          <div class="modal-body">
            <form @submit.prevent="guardarAlmacen">
              <div class="mb-3">
                <label class="form-label">Nombre *</label>
                <input v-model="formulario.nombre" type="text" class="form-control" required />
              </div>
              <div class="mb-3">
                <label class="form-label">Ubicación</label>
                <input v-model="formulario.ubicacion" type="text" class="form-control" />
              </div>
              <div class="form-check mb-3">
                <input v-model="formulario.esPrincipal" type="checkbox" class="form-check-input" id="esPrincipal" />
                <label class="form-check-label" for="esPrincipal">Marcar como almacén principal</label>
              </div>
              <div class="d-flex justify-content-end gap-2">
                <button type="button" class="btn btn-secondary" @click="cerrarModal()">Cancelar</button>
                <button type="submit" class="btn btn-primary" :disabled="guardando">
                  {{ guardando ? 'Guardando...' : 'Guardar' }}
                </button>
              </div>
            </form>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import apiClient from '@/api';

const almacenes = ref([]);
const loading = ref(false);
const error = ref(null);
const mostrarModal = ref(false);
const esEdicion = ref(false);
const guardando = ref(false);
const formulario = ref({
  nombre: '',
  ubicacion: '',
  esPrincipal: false
});
const almacenEditando = ref(null);

const getTenantId = () => localStorage.getItem('tenantId');

const cargarAlmacenes = async () => {
  loading.value = true;
  error.value = null;
  try {
    const tenantId = getTenantId();
    // El backend no tiene endpoint directo para almacenes, usamos la configuración del tenant
    // o podemos crear un endpoint en TenantConfigController para listar almacenes
    // Por ahora, simulamos la llamada - el backend debería tener un AlmacenController
    const response = await apiClient.get(`/${tenantId}/almacenes`);
    almacenes.value = response.data;
  } catch (err) {
    error.value = err.response?.data?.error || err.message || 'Error al cargar almacenes';
    console.error('Error cargando almacenes:', err);
  } finally {
    loading.value = false;
  }
};

const abrirModalCrear = () => {
  esEdicion.value = false;
  formulario.value = { nombre: '', ubicacion: '', esPrincipal: false };
  almacenEditando.value = null;
  mostrarModal.value = true;
};

const editarAlmacen = (almacen) => {
  esEdicion.value = true;
  formulario.value = { ...almacen };
  almacenEditando.value = almacen;
  mostrarModal.value = true;
};

const cerrarModal = () => {
  mostrarModal.value = false;
  almacenEditando.value = null;
};

const guardarAlmacen = async () => {
  guardando.value = true;
  try {
    const tenantId = getTenantId();
    if (esEdicion.value && almacenEditando.value) {
      await apiClient.put(`/${tenantId}/almacenes/${almacenEditando.value.id}`, formulario.value);
    } else {
      await apiClient.post(`/${tenantId}/almacenes`, formulario.value);
    }
    await cargarAlmacenes();
    cerrarModal();
  } catch (err) {
    error.value = err.response?.data?.error || err.message || 'Error al guardar almacén';
    console.error('Error guardando almacén:', err);
  } finally {
    guardando.value = false;
  }
};

const eliminarAlmacen = async (id) => {
  if (!confirm('¿Está seguro de eliminar este almacén?')) return;
  try {
    const tenantId = getTenantId();
    await apiClient.delete(`/${tenantId}/almacenes/${id}`);
    await cargarAlmacenes();
  } catch (err) {
    error.value = err.response?.data?.error || err.message || 'Error al eliminar almacén';
    console.error('Error eliminando almacén:', err);
  }
};

onMounted(() => {
  cargarAlmacenes();
});
</script>

<style scoped>
.almacenes-view {
  padding: 20px;
}
</style>
