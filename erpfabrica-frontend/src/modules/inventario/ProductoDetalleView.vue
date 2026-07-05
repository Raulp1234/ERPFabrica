<template>
  <div class="producto-detalle-view">
    <div v-if="loading" class="text-center py-5">
      <div class="spinner-border text-primary" role="status">
        <span class="visually-hidden">Cargando...</span>
      </div>
      <p class="mt-2">Cargando información del producto...</p>
    </div>

    <div v-else-if="error" class="alert alert-danger" role="alert">
      {{ error }}
    </div>

    <div v-else-if="producto" class="container-fluid">
      <!-- Encabezado con acciones -->
      <div class="d-flex justify-content-between align-items-center mb-4">
        <h2 class="mb-0">
          <i class="bi bi-box-seam me-2"></i>
          {{ producto.nombre }}
        </h2>
        <div class="btn-group">
          <router-link :to="{ name: 'Productos' }" class="btn btn-outline-secondary">
            <i class="bi bi-arrow-left me-1"></i> Volver
          </router-link>
          <button @click="editarProducto" class="btn btn-primary ms-2">
            <i class="bi bi-pencil me-1"></i> Editar
          </button>
          <button @click="eliminarProducto" class="btn btn-danger ms-2">
            <i class="bi bi-trash me-1"></i> Eliminar
          </button>
        </div>
      </div>

      <!-- Información principal -->
      <div class="row">
        <!-- Columna izquierda: Datos básicos -->
        <div class="col-md-6 mb-4">
          <div class="card h-100">
            <div class="card-header">
              <h5 class="mb-0"><i class="bi bi-info-circle me-2"></i>Información Básica</h5>
            </div>
            <div class="card-body">
              <div class="row mb-3">
                <div class="col-sm-4 text-muted">Código:</div>
                <div class="col-sm-8"><strong>{{ producto.codigo }}</strong></div>
              </div>
              <div class="row mb-3">
                <div class="col-sm-4 text-muted">Nombre:</div>
                <div class="col-sm-8"><strong>{{ producto.nombre }}</strong></div>
              </div>
              <div class="row mb-3" v-if="producto.descripcion">
                <div class="col-sm-4 text-muted">Descripción:</div>
                <div class="col-sm-8">{{ producto.descripcion }}</div>
              </div>
              <div class="row mb-3">
                <div class="col-sm-4 text-muted">Precio Unitario:</div>
                <div class="col-sm-8"><strong>${{ producto.precioUnitario?.toFixed(2) || '0.00' }}</strong></div>
              </div>
              <div class="row mb-3" v-if="producto.categoria">
                <div class="col-sm-4 text-muted">Categoría:</div>
                <div class="col-sm-8"><span class="badge bg-info">{{ producto.categoria }}</span></div>
              </div>
              <div class="row mb-3">
                <div class="col-sm-4 text-muted">Estado:</div>
                <div class="col-sm-8">
                  <span :class="['badge', producto.activo ? 'bg-success' : 'bg-danger']">
                    {{ producto.activo ? 'Activo' : 'Inactivo' }}
                  </span>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Columna derecha: Stock por almacén -->
        <div class="col-md-6 mb-4">
          <div class="card h-100">
            <div class="card-header d-flex justify-content-between align-items-center">
              <h5 class="mb-0"><i class="bi bi-building me-2"></i>Stock por Almacén</h5>
              <button @click="cargarStock" class="btn btn-sm btn-outline-primary">
                <i class="bi bi-arrow-clockwise"></i>
              </button>
            </div>
            <div class="card-body">
              <div v-if="loadingStock" class="text-center py-3">
                <div class="spinner-border spinner-border-sm text-primary"></div>
              </div>
              <div v-else-if="stock.length === 0" class="text-center text-muted py-3">
                <i class="bi bi-inbox display-6"></i>
                <p class="mb-0 mt-2">No hay stock registrado</p>
              </div>
              <div v-else class="table-responsive">
                <table class="table table-sm table-hover">
                  <thead>
                    <tr>
                      <th>Almacén</th>
                      <th class="text-end">Cantidad</th>
                      <th class="text-end">Mínimo</th>
                    </tr>
                  </thead>
                  <tbody>
                    <tr v-for="item in stock" :key="item.almacenId">
                      <td>{{ item.almacenNombre }}</td>
                      <td class="text-end">
                        <span :class="{'text-warning fw-bold': item.cantidad <= item.stockMinimo}">
                          {{ item.cantidad }}
                        </span>
                      </td>
                      <td class="text-end text-muted">{{ item.stockMinimo }}</td>
                    </tr>
                  </tbody>
                </table>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Pestañas con información relacionada -->
      <div class="row mt-4">
        <div class="col-12">
          <ul class="nav nav-tabs" id="productoTabs" role="tablist">
            <li class="nav-item" role="presentation">
              <button class="nav-link active" id="movimientos-tab" data-bs-toggle="tab" data-bs-target="#movimientos" type="button">
                <i class="bi bi-arrow-left-right me-1"></i> Movimientos
              </button>
            </li>
            <li class="nav-item" role="presentation">
              <button class="nav-link" id="campos-tab" data-bs-toggle="tab" data-bs-target="#campos" type="button">
                <i class="bi bi-card-text me-1"></i> Campos Extra
              </button>
            </li>
          </ul>
          <div class="tab-content p-3 border border-top-0 bg-white" id="productoTabsContent">
            <div class="tab-pane fade show active" id="movimientos" role="tabpanel">
              <div v-if="loadingMovimientos" class="text-center py-4">
                <div class="spinner-border text-primary"></div>
              </div>
              <div v-else-if="movimientos.length === 0" class="text-center py-4 text-muted">
                <i class="bi bi-arrow-left-right display-4 mb-2"></i>
                <p>No hay movimientos registrados para este producto.</p>
              </div>
              <div v-else class="table-responsive">
                <table class="table table-sm table-hover">
                  <thead>
                    <tr>
                      <th>Fecha</th>
                      <th>Tipo</th>
                      <th>Almacén</th>
                      <th class="text-end">Cantidad</th>
                      <th>Referencia</th>
                    </tr>
                  </thead>
                  <tbody>
                    <tr v-for="mov in movimientos" :key="mov.id">
                      <td>{{ formatDate(mov.fecha) }}</td>
                      <td>
                        <span :class="['badge', mov.tipo === 'Entrada' ? 'bg-success' : 'bg-danger']">
                          {{ mov.tipo }}
                        </span>
                      </td>
                      <td>{{ mov.almacenNombre }}</td>
                      <td class="text-end fw-bold">{{ mov.cantidad }}</td>
                      <td>{{ mov.referencia || '-' }}</td>
                    </tr>
                  </tbody>
                </table>
              </div>
            </div>
            <div class="tab-pane fade" id="campos" role="tabpanel">
              <div class="text-center py-4 text-muted">
                <i class="bi bi-card-text display-4 mb-2"></i>
                <p>No hay campos extra configurados para este producto.</p>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Modal de edición -->
    <div v-if="mostrarModalEdicion" class="modal fade show d-block" tabindex="-1" style="background-color: rgba(0,0,0,0.5);">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">Editar Producto</h5>
            <button type="button" class="btn-close" @click="cerrarModalEdicion"></button>
          </div>
          <div class="modal-body">
            <form @submit.prevent="guardarCambios">
              <div class="mb-3">
                <label class="form-label">Código</label>
                <input v-model="formulario.codigo" type="text" class="form-control" required />
              </div>
              <div class="mb-3">
                <label class="form-label">Nombre</label>
                <input v-model="formulario.nombre" type="text" class="form-control" required />
              </div>
              <div class="mb-3">
                <label class="form-label">Descripción</label>
                <textarea v-model="formulario.descripcion" class="form-control" rows="3"></textarea>
              </div>
              <div class="mb-3">
                <label class="form-label">Precio Unitario</label>
                <input v-model.number="formulario.precioUnitario" type="number" step="0.01" min="0" class="form-control" required />
              </div>
              <div class="mb-3">
                <label class="form-label">Categoría</label>
                <input v-model="formulario.categoria" type="text" class="form-control" />
              </div>
              <div class="form-check mb-3">
                <input v-model="formulario.activo" type="checkbox" class="form-check-input" id="activoCheck" />
                <label class="form-check-label" for="activoCheck">Activo</label>
              </div>
            </form>
          </div>
          <div class="modal-footer">
            <button type="button" class="btn btn-secondary" @click="cerrarModalEdicion">Cancelar</button>
            <button type="button" class="btn btn-primary" @click="guardarCambios" :disabled="guardando">
              <span v-if="guardando" class="spinner-border spinner-border-sm me-2"></span>
              Guardar Cambios
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import axios from 'axios';

const route = useRoute();
const router = useRouter();

const loading = ref(true);
const error = ref(null);
const producto = ref(null);
const stock = ref([]);
const movimientos = ref([]);
const loadingStock = ref(false);
const loadingMovimientos = ref(false);
const mostrarModalEdicion = ref(false);
const guardando = ref(false);

const formulario = ref({
  codigo: '',
  nombre: '',
  descripcion: '',
  precioUnitario: 0,
  categoria: '',
  activo: true
});

const tenantId = computed(() => route.params.tenantId || localStorage.getItem('tenantId'));
const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:5000';

const cargarProducto = async () => {
  loading.value = true;
  error.value = null;
  
  try {
    const response = await axios.get(`${API_URL}/api/${tenantId.value}/producto/${route.params.id}`, {
      headers: {
        'Authorization': `Bearer ${localStorage.getItem('token')}`
      }
    });
    producto.value = response.data;
  } catch (err) {
    console.error('Error al cargar producto:', err);
    if (err.response?.status === 404) {
      error.value = 'El producto no fue encontrado.';
    } else {
      error.value = err.response?.data?.message || 'Error al cargar la información del producto.';
    }
  } finally {
    loading.value = false;
  }
};

const cargarStock = async () => {
  loadingStock.value = true;
  
  try {
    const response = await axios.get(`${API_URL}/api/${tenantId.value}/stock/producto/${route.params.id}`, {
      headers: {
        'Authorization': `Bearer ${localStorage.getItem('token')}`
      }
    });
    stock.value = response.data || [];
  } catch (err) {
    console.error('Error al cargar stock:', err);
    stock.value = [];
  } finally {
    loadingStock.value = false;
  }
};

const cargarMovimientos = async () => {
  loadingMovimientos.value = true;
  
  try {
    const response = await axios.get(`${API_URL}/api/${tenantId.value}/movimientoinventario/historial/${route.params.id}`, {
      headers: {
        'Authorization': `Bearer ${localStorage.getItem('token')}`
      }
    });
    movimientos.value = response.data || [];
  } catch (err) {
    console.error('Error al cargar movimientos:', err);
    movimientos.value = [];
  } finally {
    loadingMovimientos.value = false;
  }
};

const editarProducto = () => {
  if (!producto.value) return;
  
  formulario.value = {
    codigo: producto.value.codigo || '',
    nombre: producto.value.nombre || '',
    descripcion: producto.value.descripcion || '',
    precioUnitario: producto.value.precioUnitario || 0,
    categoria: producto.value.categoria || '',
    activo: producto.value.activo !== false
  };
  
  mostrarModalEdicion.value = true;
};

const cerrarModalEdicion = () => {
  mostrarModalEdicion.value = false;
};

const guardarCambios = async () => {
  guardando.value = true;
  
  try {
    await axios.put(`${API_URL}/api/${tenantId.value}/producto/${route.params.id}`, formulario.value, {
      headers: {
        'Authorization': `Bearer ${localStorage.getItem('token')}`,
        'Content-Type': 'application/json'
      }
    });
    
    await cargarProducto();
    mostrarModalEdicion.value = false;
    alert('Producto actualizado correctamente');
  } catch (err) {
    console.error('Error al actualizar producto:', err);
    alert(err.response?.data?.message || 'Error al actualizar el producto.');
  } finally {
    guardando.value = false;
  }
};

const eliminarProducto = async () => {
  if (!confirm('¿Está seguro de que desea eliminar este producto? Esta acción no se puede deshacer.')) {
    return;
  }
  
  try {
    await axios.delete(`${API_URL}/api/${tenantId.value}/producto/${route.params.id}`, {
      headers: {
        'Authorization': `Bearer ${localStorage.getItem('token')}`
      }
    });
    
    router.push({ name: 'Productos', params: { tenantId: tenantId.value } });
  } catch (err) {
    console.error('Error al eliminar producto:', err);
    alert(err.response?.data?.message || 'Error al eliminar el producto.');
  }
};

const formatDate = (dateString) => {
  if (!dateString) return '-';
  const date = new Date(dateString);
  return date.toLocaleDateString() + ' ' + date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
};

onMounted(() => {
  cargarProducto();
  cargarStock();
  cargarMovimientos();
});
</script>

<style scoped>
.producto-detalle-view {
  padding: 20px;
}

.card {
  box-shadow: 0 0.125rem 0.25rem rgba(0, 0, 0, 0.075);
}

.nav-tabs .nav-link {
  color: #495057;
}

.nav-tabs .nav-link.active {
  font-weight: 600;
}
</style>
