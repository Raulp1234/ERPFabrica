<template>
  <div class="facturas-view container-fluid py-4">
    <div class="row mb-4 align-items-center">
      <div class="col-md-6">
        <h2 class="mb-0"><i class="bi bi-receipt me-2"></i>Facturas</h2>
      </div>
      <div class="col-md-6 text-md-end mt-3 mt-md-0">
        <router-link to="/facturacion/facturas/nueva" class="btn btn-primary">
          <i class="bi bi-plus-lg me-1"></i> Nueva Factura
        </router-link>
      </div>
    </div>

    <!-- Filtros -->
    <div class="card mb-4">
      <div class="card-body">
        <div class="row g-3">
          <div class="col-md-4">
            <label class="form-label">Estado</label>
            <select v-model="filtros.estado" class="form-select" @change="cargarFacturas">
              <option value="">Todos</option>
              <option value="Borrador">Borrador</option>
              <option value="Emitida">Emitida</option>
              <option value="Pagada">Pagada</option>
              <option value="Parcial">Parcial</option>
              <option value="Anulada">Anulada</option>
            </select>
          </div>
          <div class="col-md-4">
            <label class="form-label">Buscar</label>
            <input 
              v-model="filtros.busqueda" 
              type="text" 
              class="form-control" 
              placeholder="Número, tercero..."
              @keyup.enter="cargarFacturas"
            />
          </div>
          <div class="col-md-4 d-flex align-items-end">
            <button class="btn btn-outline-secondary me-2" @click="limpiarFiltros">
              <i class="bi bi-x-lg me-1"></i> Limpiar
            </button>
            <button class="btn btn-primary" @click="cargarFacturas">
              <i class="bi bi-search me-1"></i> Buscar
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Tabla de facturas -->
    <div class="card">
      <div class="card-body p-0">
        <div v-if="cargando" class="text-center py-5">
          <div class="spinner-border text-primary" role="status">
            <span class="visually-hidden">Cargando...</span>
          </div>
          <p class="mt-2 text-muted">Cargando facturas...</p>
        </div>
        <div v-else-if="error" class="alert alert-danger m-3">
          <i class="bi bi-exclamation-triangle me-2"></i>{{ error }}
        </div>
        <div v-else-if="facturas.length === 0" class="text-center py-5">
          <i class="bi bi-inbox display-4 text-muted"></i>
          <p class="text-muted mt-2">No hay facturas registradas</p>
        </div>
        <table v-else class="table table-hover mb-0">
          <thead class="table-light">
            <tr>
              <th>Número</th>
              <th>Tipo</th>
              <th>Tercero</th>
              <th>Fecha Emisión</th>
              <th>Vencimiento</th>
              <th>Total</th>
              <th>Estado</th>
              <th>Acciones</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="factura in facturas" :key="factura.id">
              <td>
                <router-link :to="`/facturacion/facturas/${factura.id}`" class="text-decoration-none">
                  {{ factura.numeroFactura }}
                </router-link>
              </td>
              <td>{{ factura.tipoFactura }}</td>
              <td>{{ factura.terceroNombre }}</td>
              <td>{{ formatDate(factura.fechaEmision) }}</td>
              <td>{{ factura.fechaVencimiento ? formatDate(factura.fechaVencimiento) : '-' }}</td>
              <td class="fw-bold">{{ formatCurrency(factura.total) }}</td>
              <td>
                <span :class="getEstadoClass(factura.estado)" class="badge px-3 py-2">
                  {{ factura.estado }}
                </span>
              </td>
              <td>
                <div class="btn-group btn-group-sm">
                  <router-link 
                    :to="`/facturacion/facturas/${factura.id}`" 
                    class="btn btn-outline-primary"
                    title="Ver detalle"
                  >
                    <i class="bi bi-eye"></i>
                  </router-link>
                  <button 
                    v-if="factura.estado === 'Borrador'" 
                    class="btn btn-outline-success" 
                    @click="emitirFactura(factura)"
                    title="Emitir"
                  >
                    <i class="bi bi-check-lg"></i>
                  </button>
                  <button 
                    v-if="factura.estado === 'Emitida' || factura.estado === 'Parcial'" 
                    class="btn btn-outline-warning" 
                    @click="abrirModalPago(factura)"
                    title="Registrar pago"
                  >
                    <i class="bi bi-cash-coin"></i>
                  </button>
                  <button 
                    v-if="factura.estado !== 'Anulada'" 
                    class="btn btn-outline-danger" 
                    @click="anularFactura(factura)"
                    title="Anular"
                  >
                    <i class="bi bi-x-lg"></i>
                  </button>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Modal de Pago -->
    <div 
      v-if="modalPagoVisible" 
      class="modal fade show d-block" 
      tabindex="-1" 
      style="background: rgba(0,0,0,0.5)"
    >
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">Registrar Pago - {{ facturaSeleccionada?.numeroFactura }}</h5>
            <button type="button" class="btn-close" @click="cerrarModalPago"></button>
          </div>
          <div class="modal-body">
            <div v-if="facturaSeleccionada" class="mb-3">
              <p><strong>Saldo Pendiente:</strong> {{ formatCurrency(facturaSeleccionada.total - (facturaSeleccionada.pagos?.reduce((sum, p) => sum + p.monto, 0) || 0)) }}</p>
            </div>
            <form @submit.prevent="registrarPago">
              <div class="mb-3">
                <label class="form-label">Monto</label>
                <input 
                  v-model.number="pagoForm.monto" 
                  type="number" 
                  step="0.01" 
                  class="form-control" 
                  required
                />
              </div>
              <div class="mb-3">
                <label class="form-label">Método de Pago</label>
                <select v-model="pagoForm.metodoPago" class="form-select" required>
                  <option value="">Seleccione...</option>
                  <option value="Efectivo">Efectivo</option>
                  <option value="Transferencia">Transferencia</option>
                  <option value="Tarjeta">Tarjeta</option>
                  <option value="Cheque">Cheque</option>
                </select>
              </div>
              <div class="mb-3">
                <label class="form-label">Referencia (Opcional)</label>
                <input 
                  v-model="pagoForm.referencia" 
                  type="text" 
                  class="form-control" 
                  placeholder="N° de transacción, cheque, etc."
                />
              </div>
            </form>
          </div>
          <div class="modal-footer">
            <button type="button" class="btn btn-secondary" @click="cerrarModalPago">Cancelar</button>
            <button type="button" class="btn btn-primary" @click="registrarPago" :disabled="procesando">
              <span v-if="procesando" class="spinner-border spinner-border-sm me-2"></span>
              Registrar Pago
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useAuthStore } from '@/stores/authStore'
import api from '@/services/api'

const authStore = useAuthStore()
const tenantId = authStore.getTenantId

const facturas = ref([])
const cargando = ref(false)
const error = ref('')
const filtros = ref({
  estado: '',
  busqueda: ''
})

const modalPagoVisible = ref(false)
const facturaSeleccionada = ref(null)
const procesando = ref(false)
const pagoForm = ref({
  monto: 0,
  metodoPago: '',
  referencia: ''
})

const cargarFacturas = async () => {
  cargando.value = true
  error.value = ''
  
  try {
    const params = {}
    if (filtros.value.estado) params.estado = filtros.value.estado
    
    const response = await api.get(`/${tenantId}/facturas`, { params })
    facturas.value = response.data
    
    // Filtrado local por búsqueda
    if (filtros.value.busqueda) {
      const termino = filtros.value.busqueda.toLowerCase()
      facturas.value = facturas.value.filter(f => 
        f.numeroFactura?.toLowerCase().includes(termino) ||
        f.terceroNombre?.toLowerCase().includes(termino)
      )
    }
  } catch (err) {
    error.value = err.response?.data?.message || 'Error al cargar las facturas'
    console.error(err)
  } finally {
    cargando.value = false
  }
}

const limpiarFiltros = () => {
  filtros.value = { estado: '', busqueda: '' }
  cargarFacturas()
}

const formatDate = (dateString) => {
  if (!dateString) return '-'
  return new Date(dateString).toLocaleDateString('es-ES')
}

const formatCurrency = (amount) => {
  return new Intl.NumberFormat('es-CO', {
    style: 'currency',
    currency: 'COP',
    minimumFractionDigits: 0
  }).format(amount)
}

const getEstadoClass = (estado) => {
  const classes = {
    'Borrador': 'bg-secondary',
    'Emitida': 'bg-primary',
    'Pagada': 'bg-success',
    'Parcial': 'bg-warning text-dark',
    'Anulada': 'bg-danger'
  }
  return classes[estado] || 'bg-secondary'
}

const emitirFactura = async (factura) => {
  if (!confirm(`¿Está seguro de emitir la factura ${factura.numeroFactura}?`)) return
  
  try {
    await api.put(`/${tenantId}/facturas/${factura.id}/emitir`)
    alert('Factura emitida correctamente')
    cargarFacturas()
  } catch (err) {
    alert(err.response?.data?.error || 'Error al emitir la factura')
  }
}

const abrirModalPago = (factura) => {
  facturaSeleccionada.value = factura
  pagoForm.value = { monto: 0, metodoPago: '', referencia: '' }
  modalPagoVisible.value = true
}

const cerrarModalPago = () => {
  modalPagoVisible.value = false
  facturaSeleccionada.value = null
}

const registrarPago = async () => {
  if (!facturaSeleccionada.value) return
  
  procesando.value = true
  try {
    await api.post(`/${tenantId}/facturas/${facturaSeleccionada.value.id}/pagos`, pagoForm.value)
    alert('Pago registrado correctamente')
    cerrarModalPago()
    cargarFacturas()
  } catch (err) {
    alert(err.response?.data?.error || 'Error al registrar el pago')
  } finally {
    procesando.value = false
  }
}

const anularFactura = async (factura) => {
  if (!confirm(`¿Está seguro de ANULAR la factura ${factura.numeroFactura}? Esta acción no se puede deshacer.`)) return
  
  try {
    await api.put(`/${tenantId}/facturas/${factura.id}/anular`)
    alert('Factura anulada correctamente')
    cargarFacturas()
  } catch (err) {
    alert(err.response?.data?.error || 'Error al anular la factura')
  }
}

onMounted(() => {
  cargarFacturas()
})
</script>

<style scoped>
.facturas-view {
  max-width: 1400px;
  margin: 0 auto;
}

.badge {
  font-size: 0.85rem;
}

.btn-group-sm .btn {
  padding: 0.25rem 0.5rem;
  font-size: 0.875rem;
}
</style>
