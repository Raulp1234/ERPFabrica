<template>
  <div class="configuracion-view">
    <h4 class="mb-4">Configuración del Sistema</h4>

    <!-- Tabs de navegación -->
    <ul class="nav nav-tabs mb-4" role="tablist">
      <li class="nav-item" role="presentation">
        <button
          class="nav-link active"
          data-bs-toggle="tab"
          data-bs-target="#tab-apariencia"
          type="button"
        >
          <i class="bi bi-palette me-2"></i>Apariencia
        </button>
      </li>
      <li class="nav-item" role="presentation">
        <button
          class="nav-link"
          data-bs-toggle="tab"
          data-bs-target="#tab-parametros"
          type="button"
        >
          <i class="bi bi-sliders me-2"></i>Parámetros
        </button>
      </li>
      <li class="nav-item" role="presentation">
        <button
          class="nav-link"
          data-bs-toggle="tab"
          data-bs-target="#tab-modulos"
          type="button"
        >
          <i class="bi bi-grid-3x3-gap me-2"></i>Módulos
        </button>
      </li>
      <li class="nav-item" role="presentation">
        <button
          class="nav-link"
          data-bs-toggle="tab"
          data-bs-target="#tab-campos-extra"
          type="button"
        >
          <i class="bi bi-plus-circle me-2"></i>Campos Extra
        </button>
      </li>
    </ul>

    <!-- Contenido de los tabs -->
    <div class="tab-content">
      <!-- Tab Apariencia -->
      <div class="tab-pane fade show active" id="tab-apariencia">
        <div class="card shadow-sm">
          <div class="card-body">
            <form @submit.prevent="guardarConfiguracion">
              <div class="row g-4">
                <div class="col-md-6">
                  <label class="form-label">Nombre del Sistema</label>
                  <input
                    type="text"
                    class="form-control"
                    v-model="configForm.nombreSistema"
                    placeholder="Ej: Mi ERP"
                  />
                </div>
                <div class="col-md-6">
                  <label class="form-label">URL del Logo</label>
                  <input
                    type="url"
                    class="form-control"
                    v-model="configForm.logoUrl"
                    placeholder="https://ejemplo.com/logo.png"
                  />
                  <div v-if="configForm.logoUrl" class="mt-2">
                    <img 
                      :src="configForm.logoUrl" 
                      alt="Vista previa" 
                      class="img-thumbnail"
                      style="max-height: 80px;"
                    />
                  </div>
                </div>
                <div class="col-md-6">
                  <label class="form-label">Color Primario</label>
                  <div class="input-group">
                    <input
                      type="color"
                      class="form-control form-control-color"
                      v-model="configForm.colorPrimario"
                      style="width: 60px;"
                    />
                    <input
                      type="text"
                      class="form-control"
                      v-model="configForm.colorPrimario"
                      placeholder="#3498db"
                    />
                  </div>
                </div>
                <div class="col-md-6">
                  <label class="form-label">Color Secundario</label>
                  <div class="input-group">
                    <input
                      type="color"
                      class="form-control form-control-color"
                      v-model="configForm.colorSecundario"
                      style="width: 60px;"
                    />
                    <input
                      type="text"
                      class="form-control"
                      v-model="configForm.colorSecundario"
                      placeholder="#2c3e50"
                    />
                  </div>
                </div>
              </div>
            </form>
          </div>
        </div>
      </div>

      <!-- Tab Parámetros -->
      <div class="tab-pane fade" id="tab-parametros">
        <div class="card shadow-sm">
          <div class="card-body">
            <form @submit.prevent="guardarConfiguracion">
              <div class="row g-4">
                <div class="col-md-4">
                  <label class="form-label">Moneda por Defecto</label>
                  <select class="form-select" v-model="configForm.monedaPorDefecto">
                    <option value="USD">USD - Dólar Americano</option>
                    <option value="EUR">EUR - Euro</option>
                    <option value="MXN">MXN - Peso Mexicano</option>
                    <option value="COP">COP - Peso Colombiano</option>
                    <option value="ARS">ARS - Peso Argentino</option>
                    <option value="CLP">CLP - Peso Chileno</option>
                  </select>
                </div>
                <div class="col-md-4">
                  <label class="form-label">Impuesto por Defecto (%)</label>
                  <input
                    type="number"
                    class="form-control"
                    v-model.number="configForm.impuestoPorDefecto"
                    step="0.01"
                    min="0"
                    max="100"
                  />
                  <small class="text-muted">Ej: 0.16 para 16%</small>
                </div>
                <div class="col-md-4">
                  <label class="form-label">Formato de Factura</label>
                  <select class="form-select" v-model="configForm.formatoFactura">
                    <option value="standard">Estándar</option>
                    <option value="simplificado">Simplificado</option>
                    <option value="electronico">Electrónico</option>
                  </select>
                </div>
                <div class="col-md-6">
                  <div class="form-check form-switch">
                    <input
                      class="form-check-input"
                      type="checkbox"
                      v-model="configForm.manejaMultiplesAlmacenes"
                    />
                    <label class="form-check-label">
                      Manejar Múltiples Almacenes
                    </label>
                  </div>
                </div>
                <div class="col-md-6">
                  <div class="form-check form-switch">
                    <input
                      class="form-check-input"
                      type="checkbox"
                      v-model="configForm.controlStockEstricto"
                    />
                    <label class="form-check-label">
                      Control de Stock Estricto
                    </label>
                  </div>
                </div>
                <div class="col-md-6">
                  <div class="form-check form-switch">
                    <input
                      class="form-check-input"
                      type="checkbox"
                      v-model="configForm.permiteVentaSinStock"
                    />
                    <label class="form-check-label">
                      Permitir Venta Sin Stock
                    </label>
                  </div>
                </div>
              </div>
            </form>
          </div>
        </div>
      </div>

      <!-- Tab Módulos -->
      <div class="tab-pane fade" id="tab-modulos">
        <div class="card shadow-sm">
          <div class="card-body">
            <p class="text-muted mb-4">
              Activa o desactiva los módulos disponibles en el sistema.
            </p>
            <div class="row g-3">
              <div 
                v-for="modulo in modulosDisponibles" 
                :key="modulo.id"
                class="col-md-6 col-lg-4"
              >
                <div class="card h-100">
                  <div class="card-body">
                    <div class="d-flex justify-content-between align-items-center">
                      <div>
                        <h6 class="card-title mb-1">{{ modulo.nombre }}</h6>
                        <small class="text-muted">{{ modulo.descripcion }}</small>
                      </div>
                      <div class="form-check form-switch">
                        <input
                          class="form-check-input"
                          type="checkbox"
                          :checked="configForm.moduloActivos?.includes(modulo.id)"
                          @change="toggleModulo(modulo.id, $event.target.checked)"
                        />
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Tab Campos Extra -->
      <div class="tab-pane fade" id="tab-campos-extra">
        <div class="card shadow-sm">
          <div class="card-body">
            <div class="d-flex justify-content-between align-items-center mb-4">
              <h5 class="mb-0">Definiciones de Campos Extra</h5>
              <button class="btn btn-primary" @click="abrirModalCampoExtra">
                <i class="bi bi-plus-lg me-1"></i>
                Nuevo Campo
              </button>
            </div>

            <!-- Lista de campos por entidad -->
            <div class="accordion" id="accordionCamposExtra">
              <div 
                v-for="entidad in ['Producto', 'Tercero', 'Factura']" 
                :key="entidad"
                class="accordion-item"
              >
                <h2 class="accordion-header">
                  <button
                    class="accordion-button"
                    type="button"
                    data-bs-toggle="collapse"
                    :data-bs-target="`#collapse${entidad}`"
                  >
                    {{ entidad }}s
                  </button>
                </h2>
                <div
                  :id="`collapse${entidad}`"
                  class="accordion-collapse collapse show"
                >
                  <div class="accordion-body">
                    <div class="table-responsive">
                      <table class="table table-hover">
                        <thead>
                          <tr>
                            <th>Nombre</th>
                            <th>Tipo</th>
                            <th>Requerido</th>
                            <th class="text-end">Acciones</th>
                          </tr>
                        </thead>
                        <tbody>
                          <tr 
                            v-for="campo in camposExtraStore.definiciones[entidad]" 
                            :key="campo.id"
                          >
                            <td>{{ campo.nombreCampo }}</td>
                            <td>
                              <span class="badge bg-info">{{ campo.tipoDato }}</span>
                            </td>
                            <td>
                              <i 
                                v-if="campo.esRequerido" 
                                class="bi bi-check-circle-fill text-success"
                              ></i>
                              <i v-else class="bi bi-dash-circle-fill text-muted"></i>
                            </td>
                            <td class="text-end">
                              <button
                                class="btn btn-sm btn-outline-secondary me-1"
                                @click="editarCampoExtra(campo)"
                              >
                                <i class="bi bi-pencil"></i>
                              </button>
                              <button
                                class="btn btn-sm btn-outline-danger"
                                @click="eliminarCampoExtra(campo.id)"
                              >
                                <i class="bi bi-trash"></i>
                              </button>
                            </td>
                          </tr>
                          <tr v-if="!camposExtraStore.definiciones[entidad]?.length">
                            <td colspan="4" class="text-center text-muted py-3">
                              No hay campos personalizados definidos
                            </td>
                          </tr>
                        </tbody>
                      </table>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Botones de acción -->
    <div class="mt-4 d-flex justify-content-end gap-2">
      <button class="btn btn-secondary" @click="cancelarCambios">
        <i class="bi bi-x-circle me-1"></i>
        Cancelar
      </button>
      <button 
        class="btn btn-primary" 
        @click="guardarConfiguracion"
        :disabled="guardando"
      >
        <span v-if="guardando" class="spinner-border spinner-border-sm me-2"></span>
        <i v-else class="bi bi-save me-1"></i>
        {{ guardando ? 'Guardando...' : 'Guardar Cambios' }}
      </button>
    </div>

    <!-- Modal para crear/editar campo extra -->
    <div 
      class="modal fade" 
      id="modalCampoExtra" 
      tabindex="-1"
      ref="modalCampoExtraRef"
    >
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">
              {{ modoEdicionCampo ? 'Editar Campo' : 'Nuevo Campo Extra' }}
            </h5>
            <button type="button" class="btn-close" @click="cerrarModalCampo"></button>
          </div>
          <div class="modal-body">
            <form @submit.prevent="guardarCampoExtra">
              <div class="mb-3">
                <label class="form-label">Entidad</label>
                <select 
                  class="form-select" 
                  v-model="campoForm.entidad"
                  :disabled="modoEdicionCampo"
                >
                  <option value="Producto">Producto</option>
                  <option value="Tercero">Tercero</option>
                  <option value="Factura">Factura</option>
                </select>
              </div>
              <div class="mb-3">
                <label class="form-label">Nombre del Campo</label>
                <input 
                  type="text" 
                  class="form-control" 
                  v-model="campoForm.nombreCampo"
                  required
                />
              </div>
              <div class="mb-3">
                <label class="form-label">Tipo de Dato</label>
                <select class="form-select" v-model="campoForm.tipoDato" required>
                  <option value="texto">Texto</option>
                  <option value="numero">Número</option>
                  <option value="fecha">Fecha</option>
                  <option value="booleano">Booleano (Sí/No)</option>
                  <option value="seleccion">Selección (Dropdown)</option>
                </select>
              </div>
              <div class="mb-3">
                <label class="form-label">Orden de Visualización</label>
                <input 
                  type="number" 
                  class="form-control" 
                  v-model.number="campoForm.orden"
                  min="1"
                />
              </div>
              <div class="form-check mb-3">
                <input
                  class="form-check-input"
                  type="checkbox"
                  v-model="campoForm.esRequerido"
                />
                <label class="form-check-label">
                  Campo Requerido
                </label>
              </div>
              <div v-if="campoForm.tipoDato === 'seleccion'" class="mb-3">
                <label class="form-label">Opciones (separadas por coma)</label>
                <textarea 
                  class="form-control" 
                  v-model="campoForm.opcionesJson"
                  rows="3"
                  placeholder="Opción 1, Opción 2, Opción 3"
                ></textarea>
              </div>
            </form>
          </div>
          <div class="modal-footer">
            <button type="button" class="btn btn-secondary" @click="cerrarModalCampo">
              Cancelar
            </button>
            <button 
              type="button" 
              class="btn btn-primary"
              @click="guardarCampoExtra"
              :disabled="guardandoCampo"
            >
              <span v-if="guardandoCampo" class="spinner-border spinner-border-sm me-2"></span>
              {{ guardandoCampo ? 'Guardando...' : 'Guardar' }}
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue';
import { useRoute } from 'vue-router';
import { useTenantConfigStore } from '@/stores/tenant';
import { useCamposExtraStore } from '@/stores/camposExtra';
import { toast } from 'vue3-toastify';
import { Modal } from 'bootstrap';

const route = useRoute();
const tenantConfigStore = useTenantConfigStore();
const camposExtraStore = useCamposExtraStore();

const modalCampoExtraRef = ref(null);
let modalCampoInstance = null;

const guardando = ref(false);
const guardandoCampo = ref(false);
const modoEdicionCampo = ref(false);

const configForm = reactive({
  nombreSistema: '',
  logoUrl: '',
  colorPrimario: '#3498db',
  colorSecundario: '#2c3e50',
  monedaPorDefecto: 'USD',
  impuestoPorDefecto: 0.16,
  formatoFactura: 'standard',
  manejaMultiplesAlmacenes: false,
  controlStockEstricto: false,
  permiteVentaSinStock: false,
  moduloActivos: [],
});

const campoForm = reactive({
  id: null,
  entidad: 'Producto',
  nombreCampo: '',
  tipoDato: 'texto',
  esRequerido: false,
  opcionesJson: '',
  orden: 1,
});

const modulosDisponibles = [
  { id: 'Inventario', nombre: 'Inventario', descripcion: 'Gestión de productos y stock' },
  { id: 'Pedidos', nombre: 'Pedidos', descripcion: 'Solicitudes y órdenes' },
  { id: 'Facturacion', nombre: 'Facturación', descripcion: 'Facturas y cobros' },
  { id: 'Terceros', nombre: 'Terceros', descripcion: 'Clientes y proveedores' },
];

const cargarConfiguracion = () => {
  const config = tenantConfigStore.config;
  if (config) {
    Object.assign(configForm, {
      nombreSistema: config.nombreSistema || '',
      logoUrl: config.logoUrl || '',
      colorPrimario: config.colorPrimario || '#3498db',
      colorSecundario: config.colorSecundario || '#2c3e50',
      monedaPorDefecto: config.monedaPorDefecto || 'USD',
      impuestoPorDefecto: config.impuestoPorDefecto || 0.16,
      formatoFactura: config.formatoFactura || 'standard',
      manejaMultiplesAlmacenes: config.manejaMultiplesAlmacenes || false,
      controlStockEstricto: config.controlStockEstricto || false,
      permiteVentaSinStock: config.permiteVentaSinStock || false,
      moduloActivos: config.moduloActivos || [],
    });
  }
};

const cargarCamposExtra = async () => {
  try {
    await Promise.all([
      camposExtraStore.obtenerDefiniciones('Producto'),
      camposExtraStore.obtenerDefiniciones('Tercero'),
      camposExtraStore.obtenerDefiniciones('Factura'),
    ]);
  } catch (error) {
    console.error('Error al cargar campos extra:', error);
  }
};

const toggleModulo = (moduloId, activado) => {
  if (activado) {
    if (!configForm.moduloActivos.includes(moduloId)) {
      configForm.moduloActivos.push(moduloId);
    }
  } else {
    configForm.moduloActivos = configForm.moduloActivos.filter(m => m !== moduloId);
  }
};

const guardarConfiguracion = async () => {
  guardando.value = true;
  
  try {
    await tenantConfigStore.actualizarConfig(configForm);
    toast.success('Configuración guardada correctamente');
  } catch (error) {
    toast.error('Error al guardar configuración');
  } finally {
    guardando.value = false;
  }
};

const cancelarCambios = () => {
  cargarConfiguracion();
  toast.info('Cambios cancelados');
};

// Métodos para campos extra
const abrirModalCampoExtra = () => {
  modoEdicionCampo.value = false;
  Object.assign(campoForm, {
    id: null,
    entidad: 'Producto',
    nombreCampo: '',
    tipoDato: 'texto',
    esRequerido: false,
    opcionesJson: '',
    orden: 1,
  });
  modalCampoInstance = new Modal(modalCampoExtraRef.value);
  modalCampoInstance.show();
};

const editarCampoExtra = (campo) => {
  modoEdicionCampo.value = true;
  Object.assign(campoForm, {
    id: campo.id,
    entidad: campo.entidad,
    nombreCampo: campo.nombreCampo,
    tipoDato: campo.tipoDato,
    esRequerido: campo.esRequerido,
    opcionesJson: campo.opcionesJson ? JSON.parse(campo.opcionesJson).join(', ') : '',
    orden: campo.orden,
  });
  modalCampoInstance = new Modal(modalCampoExtraRef.value);
  modalCampoInstance.show();
};

const cerrarModalCampo = () => {
  if (modalCampoInstance) {
    modalCampoInstance.hide();
  }
};

const guardarCampoExtra = async () => {
  guardandoCampo.value = true;
  
  try {
    // Convertir opciones a JSON
    const opcionesArray = campoForm.opcionesJson
      .split(',')
      .map(o => o.trim())
      .filter(o => o);
    
    const campoData = {
      ...campoForm,
      opcionesJson: opcionesArray.length > 0 ? JSON.stringify(opcionesArray) : null,
    };

    if (modoEdicionCampo.value) {
      await camposExtraStore.actualizarDefinicion(campoForm.id, campoData);
      toast.success('Campo actualizado correctamente');
    } else {
      await camposExtraStore.crearDefinicion(campoForm.entidad, campoData);
      toast.success('Campo creado correctamente');
    }
    
    cerrarModalCampo();
    cargarCamposExtra();
  } catch (error) {
    toast.error('Error al guardar campo');
  } finally {
    guardandoCampo.value = false;
  }
};

const eliminarCampoExtra = async (id) => {
  if (confirm('¿Estás seguro de eliminar este campo?')) {
    try {
      await camposExtraStore.eliminarDefinicion(id);
      toast.success('Campo eliminado correctamente');
      cargarCamposExtra();
    } catch (error) {
      toast.error('Error al eliminar campo');
    }
  }
};

onMounted(() => {
  cargarConfiguracion();
  cargarCamposExtra();
});
</script>

<style scoped>
.configuracion-view {
  animation: fadeIn 0.3s ease;
}

@keyframes fadeIn {
  from {
    opacity: 0;
  }
  to {
    opacity: 1;
  }
}

.card {
  border: none;
  border-radius: 10px;
}

.nav-tabs .nav-link {
  color: #6c757d;
  border: none;
  border-bottom: 2px solid transparent;
}

.nav-tabs .nav-link.active {
  color: var(--color-primario, #3498db);
  border-color: var(--color-primario, #3498db);
  background: none;
}

.btn-primary {
  background-color: var(--color-primario, #3498db);
  border-color: var(--color-primario, #3498db);
}
</style>
