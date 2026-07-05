<template>
  <div class="tercero-detalle-view">
    <div v-if="loading" class="text-center py-5">
      <div class="spinner-border text-primary" role="status">
        <span class="visually-hidden">Cargando...</span>
      </div>
      <p class="mt-2">Cargando información del tercero...</p>
    </div>

    <div v-else-if="error" class="alert alert-danger" role="alert">
      {{ error }}
    </div>

    <div v-else-if="tercero" class="container-fluid">
      <!-- Encabezado con acciones -->
      <div class="d-flex justify-content-between align-items-center mb-4">
        <h2 class="mb-0">
          <i class="bi bi-person-circle me-2"></i>
          {{ tercero.nombre }}
        </h2>
        <div class="btn-group">
          <router-link :to="{ name: 'Terceros' }" class="btn btn-outline-secondary">
            <i class="bi bi-arrow-left me-1"></i> Volver
          </router-link>
          <button @click="editarTercero" class="btn btn-primary ms-2">
            <i class="bi bi-pencil me-1"></i> Editar
          </button>
          <button @click="eliminarTercero" class="btn btn-danger ms-2" :disabled="!puedeEliminar">
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
                <div class="col-sm-4 text-muted">Tipo de Documento:</div>
                <div class="col-sm-8"><strong>{{ tercero.tipoDocumento }}</strong></div>
              </div>
              <div class="row mb-3">
                <div class="col-sm-4 text-muted">Número de Documento:</div>
                <div class="col-sm-8"><strong>{{ tercero.numeroDocumento }}</strong></div>
              </div>
              <div class="row mb-3">
                <div class="col-sm-4 text-muted">Nombre Completo:</div>
                <div class="col-sm-8"><strong>{{ tercero.nombre }}</strong></div>
              </div>
              <div class="row mb-3" v-if="tercero.nombreComercial">
                <div class="col-sm-4 text-muted">Nombre Comercial:</div>
                <div class="col-sm-8"><strong>{{ tercero.nombreComercial }}</strong></div>
              </div>
              <div class="row mb-3">
                <div class="col-sm-4 text-muted">Estado:</div>
                <div class="col-sm-8">
                  <span :class="['badge', tercero.activo ? 'bg-success' : 'bg-danger']">
                    {{ tercero.activo ? 'Activo' : 'Inactivo' }}
                  </span>
                </div>
              </div>
              <div class="row mb-3" v-if="tercero.email">
                <div class="col-sm-4 text-muted">Email:</div>
                <div class="col-sm-8">
                  <a :href="`mailto:${tercero.email}`">{{ tercero.email }}</a>
                </div>
              </div>
              <div class="row mb-3" v-if="tercero.telefono">
                <div class="col-sm-4 text-muted">Teléfono:</div>
                <div class="col-sm-8">{{ tercero.telefono }}</div>
              </div>
              <div class="row mb-3" v-if="tercero.direccion">
                <div class="col-sm-4 text-muted">Dirección:</div>
                <div class="col-sm-8">{{ tercero.direccion }}</div>
              </div>
            </div>
          </div>
        </div>

        <!-- Columna derecha: Información adicional -->
        <div class="col-md-6 mb-4">
          <div class="card h-100">
            <div class="card-header">
              <h5 class="mb-0"><i class="bi bi-building me-2"></i>Información Fiscal</h5>
            </div>
            <div class="card-body">
              <div class="row mb-3" v-if="tercero.condicionIVA">
                <div class="col-sm-4 text-muted">Condición IVA:</div>
                <div class="col-sm-8"><strong>{{ tercero.condicionIVA }}</strong></div>
              </div>
              <div class="row mb-3" v-if="tercero.codigoPostal">
                <div class="col-sm-4 text-muted">Código Postal:</div>
                <div class="col-sm-8">{{ tercero.codigoPostal }}</div>
              </div>
              <div class="row mb-3" v-if="tercero.provincia">
                <div class="col-sm-4 text-muted">Provincia:</div>
                <div class="col-sm-8">{{ tercero.provincia }}</div>
              </div>
              <div class="row mb-3" v-if="tercero.ciudad">
                <div class="col-sm-4 text-muted">Ciudad:</div>
                <div class="col-sm-8">{{ tercero.ciudad }}</div>
              </div>
              <div class="row mb-3" v-if="tercero.pais">
                <div class="col-sm-4 text-muted">País:</div>
                <div class="col-sm-8">{{ tercero.pais }}</div>
              </div>
              <div class="row mb-3" v-if="tercero.notas">
                <div class="col-sm-4 text-muted">Notas:</div>
                <div class="col-sm-8">{{ tercero.notas }}</div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Pestañas con información relacionada -->
      <div class="row mt-4">
        <div class="col-12">
          <ul class="nav nav-tabs" id="terceroTabs" role="tablist">
            <li class="nav-item" role="presentation">
              <button class="nav-link active" id="facturas-tab" data-bs-toggle="tab" data-bs-target="#facturas" type="button">
                <i class="bi bi-receipt me-1"></i> Facturas
              </button>
            </li>
            <li class="nav-item" role="presentation">
              <button class="nav-link" id="solicitudes-tab" data-bs-toggle="tab" data-bs-target="#solicitudes" type="button">
                <i class="bi bi-clipboard-check me-1"></i> Solicitudes
              </button>
            </li>
          </ul>
          <div class="tab-content p-3 border border-top-0 bg-white" id="terceroTabsContent">
            <div class="tab-pane fade show active" id="facturas" role="tabpanel">
              <div class="text-center py-4 text-muted">
                <i class="bi bi-receipt display-4 mb-2"></i>
                <p>No hay facturas asociadas a este tercero.</p>
              </div>
            </div>
            <div class="tab-pane fade" id="solicitudes" role="tabpanel">
              <div class="text-center py-4 text-muted">
                <i class="bi bi-clipboard-check display-4 mb-2"></i>
                <p>No hay solicitudes asociadas a este tercero.</p>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Modal de edición -->
    <div v-if="mostrarModalEdicion" class="modal fade show d-block" tabindex="-1" style="background-color: rgba(0,0,0,0.5);">
      <div class="modal-dialog modal-lg">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">Editar Tercero</h5>
            <button type="button" class="btn-close" @click="cerrarModalEdicion"></button>
          </div>
          <div class="modal-body">
            <form @submit.prevent="guardarCambios">
              <div class="row">
                <div class="col-md-6 mb-3">
                  <label class="form-label">Tipo de Documento</label>
                  <select v-model="formulario.tipoDocumento" class="form-select" required>
                    <option value="CC">Cédula de Ciudadanía</option>
                    <option value="CE">Cédula de Extranjería</option>
                    <option value="NIT">NIT</option>
                    <option value="PAS">Pasaporte</option>
                  </select>
                </div>
                <div class="col-md-6 mb-3">
                  <label class="form-label">Número de Documento</label>
                  <input v-model="formulario.numeroDocumento" type="text" class="form-control" required />
                </div>
              </div>
              <div class="mb-3">
                <label class="form-label">Nombre Completo</label>
                <input v-model="formulario.nombre" type="text" class="form-control" required />
              </div>
              <div class="mb-3">
                <label class="form-label">Nombre Comercial</label>
                <input v-model="formulario.nombreComercial" type="text" class="form-control" />
              </div>
              <div class="mb-3">
                <label class="form-label">Email</label>
                <input v-model="formulario.email" type="email" class="form-control" />
              </div>
              <div class="mb-3">
                <label class="form-label">Teléfono</label>
                <input v-model="formulario.telefono" type="text" class="form-control" />
              </div>
              <div class="mb-3">
                <label class="form-label">Dirección</label>
                <input v-model="formulario.direccion" type="text" class="form-control" />
              </div>
              <div class="row">
                <div class="col-md-6 mb-3">
                  <label class="form-label">Ciudad</label>
                  <input v-model="formulario.ciudad" type="text" class="form-control" />
                </div>
                <div class="col-md-6 mb-3">
                  <label class="form-label">Provincia</label>
                  <input v-model="formulario.provincia" type="text" class="form-control" />
                </div>
              </div>
              <div class="row">
                <div class="col-md-6 mb-3">
                  <label class="form-label">Código Postal</label>
                  <input v-model="formulario.codigoPostal" type="text" class="form-control" />
                </div>
                <div class="col-md-6 mb-3">
                  <label class="form-label">País</label>
                  <input v-model="formulario.pais" type="text" class="form-control" />
                </div>
              </div>
              <div class="mb-3">
                <label class="form-label">Condición IVA</label>
                <select v-model="formulario.condicionIVA" class="form-select">
                  <option value="">Seleccione...</option>
                  <option value="Responsable Inscrito">Responsable Inscrito</option>
                  <option value="Monotributista">Monotributista</option>
                  <option value="Exento">Exento</option>
                  <option value="Consumidor Final">Consumidor Final</option>
                </select>
              </div>
              <div class="mb-3">
                <label class="form-label">Notas</label>
                <textarea v-model="formulario.notas" class="form-control" rows="3"></textarea>
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
const tercero = ref(null);
const mostrarModalEdicion = ref(false);
const guardando = ref(false);

const formulario = ref({
  tipoDocumento: '',
  numeroDocumento: '',
  nombre: '',
  nombreComercial: '',
  email: '',
  telefono: '',
  direccion: '',
  ciudad: '',
  provincia: '',
  codigoPostal: '',
  pais: '',
  condicionIVA: '',
  notas: '',
  activo: true
});

const tenantId = computed(() => route.params.tenantId || localStorage.getItem('tenantId'));
const puedeEliminar = computed(() => tercero.value?.activo !== false);

const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:5000';

const cargarTercero = async () => {
  loading.value = true;
  error.value = null;
  
  try {
    const response = await axios.get(`${API_URL}/api/${tenantId.value}/terceros/${route.params.id}`, {
      headers: {
        'Authorization': `Bearer ${localStorage.getItem('token')}`
      }
    });
    tercero.value = response.data;
  } catch (err) {
    console.error('Error al cargar tercero:', err);
    if (err.response?.status === 404) {
      error.value = 'El tercero no fue encontrado.';
    } else {
      error.value = err.response?.data?.message || 'Error al cargar la información del tercero.';
    }
  } finally {
    loading.value = false;
  }
};

const editarTercero = () => {
  if (!tercero.value) return;
  
  formulario.value = {
    tipoDocumento: tercero.value.tipoDocumento || 'CC',
    numeroDocumento: tercero.value.numeroDocumento || '',
    nombre: tercero.value.nombre || '',
    nombreComercial: tercero.value.nombreComercial || '',
    email: tercero.value.email || '',
    telefono: tercero.value.telefono || '',
    direccion: tercero.value.direccion || '',
    ciudad: tercero.value.ciudad || '',
    provincia: tercero.value.provincia || '',
    codigoPostal: tercero.value.codigoPostal || '',
    pais: tercero.value.pais || '',
    condicionIVA: tercero.value.condicionIVA || '',
    notas: tercero.value.notas || '',
    activo: tercero.value.activo !== false
  };
  
  mostrarModalEdicion.value = true;
};

const cerrarModalEdicion = () => {
  mostrarModalEdicion.value = false;
};

const guardarCambios = async () => {
  guardando.value = true;
  
  try {
    await axios.put(`${API_URL}/api/${tenantId.value}/terceros/${route.params.id}`, formulario.value, {
      headers: {
        'Authorization': `Bearer ${localStorage.getItem('token')}`,
        'Content-Type': 'application/json'
      }
    });
    
    // Recargar datos
    await cargarTercero();
    mostrarModalEdicion.value = false;
    
    // Mostrar notificación de éxito
    alert('Tercero actualizado correctamente');
  } catch (err) {
    console.error('Error al actualizar tercero:', err);
    alert(err.response?.data?.error || 'Error al actualizar el tercero.');
  } finally {
    guardando.value = false;
  }
};

const eliminarTercero = async () => {
  if (!confirm('¿Está seguro de que desea eliminar este tercero? Esta acción no se puede deshacer.')) {
    return;
  }
  
  try {
    await axios.delete(`${API_URL}/api/${tenantId.value}/terceros/${route.params.id}`, {
      headers: {
        'Authorization': `Bearer ${localStorage.getItem('token')}`
      }
    });
    
    // Redirigir a la lista de terceros
    router.push({ name: 'Terceros', params: { tenantId: tenantId.value } });
  } catch (err) {
    console.error('Error al eliminar tercero:', err);
    alert(err.response?.data?.error || 'Error al eliminar el tercero.');
  }
};

onMounted(() => {
  cargarTercero();
});
</script>

<style scoped>
.tercero-detalle-view {
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
