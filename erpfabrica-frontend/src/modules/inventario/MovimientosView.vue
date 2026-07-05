<template>
  <div class="movimientos-view">
    <div class="container-fluid">
      <!-- Encabezado -->
      <div class="d-flex justify-content-between align-items-center mb-4">
        <h2 class="mb-0">
          <i class="bi bi-arrow-left-right me-2"></i>
          Movimientos de Inventario
        </h2>
      </div>

      <!-- Filtros -->
      <div class="card mb-4">
        <div class="card-body">
          <form @submit.prevent="cargarMovimientos" class="row g-3">
            <div class="col-md-3">
              <label class="form-label">Producto</label>
              <input v-model="filtros.productoId" type="number" class="form-control" placeholder="ID Producto" />
            </div>
            <div class="col-md-3">
              <label class="form-label">Almacén</label>
              <select v-model="filtros.almacenId" class="form-select">
                <option value="">Todos los almacenes</option>
                <option v-for="almacen in almacenes" :key="almacen.id" :value="almacen.id">
                  {{ almacen.nombre }}
                </option>
              </select>
            </div>
            <div class="col-md-3">
              <label class="form-label">Tipo</label>
              <select v-model="filtros.tipo" class="form-select">
                <option value="">Todos los tipos</option>
                <option value="Entrada">Entrada</option>
                <option value="Salida">Salida</option>
              </select>
            </div>
            <div class="col-md-3 d-flex align-items-end">
              <button type="submit" class="btn btn-primary me-2">
                <i class="bi bi-search me-1"></i> Filtrar
              </button>
              <button type="button" class="btn btn-outline-secondary" @click="limpiarFiltros">
                <i class="bi bi-x-circle me-1"></i> Limpiar
              </button>
            </div>
          </form>
        </div>
      </div>

      <!-- Tabla de movimientos -->
      <div class="card">
        <div class="card-header d-flex justify-content-between align-items-center">
          <h5 class="mb-0"><i class="bi bi-list-ul me-2"></i>Listado de Movimientos</h5>
          <button @click="cargarMovimientos" class="btn btn-sm btn-outline-primary">
            <i class="bi bi-arrow-clockwise"></i> Actualizar
          </button>
        </div>
        <div class="card-body">
          <div v-if="loading" class="text-center py-5">
            <div class="spinner-border text-primary" role="status">
              <span class="visually-hidden">Cargando...</span>
            </div>
            <p class="mt-2">Cargando movimientos...</p>
          </div>

          <div v-else-if="error" class="alert alert-danger" role="alert">
            {{ error }}
          </div>

          <div v-else-if="movimientos.length === 0" class="text-center py-5 text-muted">
            <i class="bi bi-inbox display-4 mb-2"></i>
            <p>No hay movimientos registrados.</p>
          </div>

          <div v-else class="table-responsive">
            <table class="table table-hover">
              <thead>
                <tr>
                  <th>ID</th>
                  <th>Fecha</th>
                  <th>Tipo</th>
                  <th>Producto</th>
                  <th>Almacén</th>
                  <th class="text-end">Cantidad</th>
                  <th>Referencia</th>
                  <th>Usuario</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="mov in movimientos" :key="mov.id">
                  <td>{{ mov.id }}</td>
                  <td>{{ formatDate(mov.fecha) }}</td>
                  <td>
                    <span :class="['badge', mov.tipo === 'Entrada' ? 'bg-success' : 'bg-danger']">
                      {{ mov.tipo }}
                    </span>
                  </td>
                  <td>{{ mov.productoNombre }}</td>
                  <td>{{ mov.almacenNombre }}</td>
                  <td class="text-end fw-bold">{{ mov.cantidad }}</td>
                  <td>{{ mov.referencia || '-' }}</td>
                  <td>{{ mov.usuarioNombre || '-' }}</td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue';
import axios from 'axios';

const loading = ref(true);
const error = ref(null);
const movimientos = ref([]);
const almacenes = ref([]);

const filtros = ref({
  productoId: '',
  almacenId: '',
  tipo: ''
});

const tenantId = computed(() => localStorage.getItem('tenantId'));
const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:5000';

const cargarMovimientos = async () => {
  loading.value = true;
  error.value = null;
  
  try {
    // Por ahora cargamos todos los movimientos sin filtros específicos
    // En una implementación más completa, se pasarían los filtros como query params
    const response = await axios.get(`${API_URL}/api/${tenantId.value}/movimientoinventario/historial/0`, {
      headers: {
        'Authorization': `Bearer ${localStorage.getItem('token')}`
      },
      params: {
        almacenId: filtros.value.almacenId || undefined
      }
    });
    
    // Si el endpoint devuelve un solo producto, lo convertimos a array
    movimientos.value = Array.isArray(response.data) ? response.data : [response.data];
  } catch (err) {
    console.error('Error al cargar movimientos:', err);
    if (err.response?.status === 404) {
      movimientos.value = [];
    } else {
      error.value = err.response?.data?.message || 'Error al cargar los movimientos.';
    }
  } finally {
    loading.value = false;
  }
};

const cargarAlmacenes = async () => {
  try {
    // Intentamos obtener la configuración del tenant para ver los almacenes
    const response = await axios.get(`${API_URL}/api/${tenantId.value}/config`, {
      headers: {
        'Authorization': `Bearer ${localStorage.getItem('token')}`
      }
    });
    
    // Simulamos almacenes basados en la configuración
    // En una implementación real, habría un endpoint específico para almacenes
    if (response.data) {
      almacenes.value = [
        { id: 1, nombre: 'Almacén Principal', esPrincipal: true }
      ];
    }
  } catch (err) {
    console.error('Error al cargar almacenes:', err);
    // Almacenes por defecto
    almacenes.value = [
      { id: 1, nombre: 'Almacén Principal', esPrincipal: true }
    ];
  }
};

const limpiarFiltros = () => {
  filtros.value = {
    productoId: '',
    almacenId: '',
    tipo: ''
  };
  cargarMovimientos();
};

const formatDate = (dateString) => {
  if (!dateString) return '-';
  const date = new Date(dateString);
  return date.toLocaleDateString() + ' ' + date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
};

onMounted(() => {
  cargarMovimientos();
  cargarAlmacenes();
});
</script>

<style scoped>
.movimientos-view {
  padding: 20px;
}

.card {
  box-shadow: 0 0.125rem 0.25rem rgba(0, 0, 0, 0.075);
}
</style>
