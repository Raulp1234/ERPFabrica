<template>
  <div class="factura-detalle-view container-fluid py-4">
    <div v-if="cargando" class="text-center py-5">
      <div class="spinner-border text-primary" role="status">
        <span class="visually-hidden">Cargando...</span>
      </div>
      <p class="mt-2 text-muted">Cargando factura...</p>
    </div>

    <div v-else-if="error" class="alert alert-danger">
      <i class="bi bi-exclamation-triangle me-2"></i>{{ error }}
    </div>

    <div v-else-if="factura" class="row">
      <!-- Encabezado -->
      <div class="col-12 mb-4">
        <div class="d-flex justify-content-between align-items-start">
          <div>
            <h2 class="mb-1"><i class="bi bi-receipt me-2"></i>Factura {{ factura.numeroFactura }}</h2>
            <p class="text-muted mb-0">{{ factura.tipoFactura }} - {{ formatDate(factura.fechaEmision) }}</p>
          </div>
          <div class="text-end">
            <span :class="getEstadoClass(factura.estado)" class="badge px-3 py-2 fs-6">
              {{ factura.estado }}
            </span>
            <div class="mt-2">
              <router-link to="/facturacion/facturas" class="btn btn-sm btn-outline-secondary me-2">
                <i class="bi bi-arrow-left me-1"></i> Volver
              </router-link>
              <button 
                v-if="factura.estado === 'Borrador'" 
                class="btn btn-sm btn-success" 
                @click="emitirFactura"
              >
                <i class="bi bi-check-lg me-1"></i> Emitir
              </button>
              <button 
                v-if="factura.estado === 'Emitida' || factura.estado === 'Parcial'" 
                class="btn btn-sm btn-warning" 
                @click="abrirModalPago"
              >
                <i class="bi bi-cash-coin me-1"></i> Pagar
              </button>
              <button 
                v-if="factura.estado !== 'Anulada'" 
                class="btn btn-sm btn-danger" 
                @click="anularFactura"
              >
                <i class="bi bi-x-lg me-1"></i> Anular
              </button>
            </div>
          </div>
        </div>
      </div>

      <!-- Información principal -->
      <div class="col-md-8 mb-4">
        <div class="card h-100">
          <div class="card-header">
            <h5 class="mb-0">Información de la Factura</h5>
          </div>
          <div class="card-body">
            <div class="row g-3">
              <div class="col-md-6">
                <label class="text-muted small">Tercero</label>
                <p class="mb-0 fw-medium">{{ factura.terceroNombre }}</p>
              </div>
              <div class="col-md-6">
                <label class="text-muted small">Fecha de Emisión</label>
                <p class="mb-0">{{ formatDate(factura.fechaEmision) }}</p>
              </div>
              <div class="col-md-6">
                <label class="text-muted small">Fecha de Vencimiento</label>
                <p class="mb-0">{{ factura.fechaVencimiento ? formatDate(factura.fechaVencimiento) : 'No definida' }}</p>
              </div>
              <div class="col-md-6">
                <label class="text-muted small">Solicitud Relacionada</label>
                <p class="mb-0">
                  <router-link v-if="factura.solicitudId" :to="`/solicitudes/${factura.solicitudId}`" class="text-decoration-none">
                    Ver solicitud
                  </router-link>
                  <span v-else class="text-muted">Ninguna</span>
                </p>
              </div>
              <div class="col-12" v-if="factura.notas">
                <label class="text-muted small">Notas</label>
                <p class="mb-0">{{ factura.notas }}</p>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Totales -->
      <div class="col-md-4 mb-4">
        <div class="card h-100">
          <div class="card-header bg-primary text-white">
            <h5 class="mb-0">Totales</h5>
          </div>
          <div class="card-body">
            <table class="table table-sm">
              <tr>
                <td>Subtotal:</td>
                <td class="text-end fw-bold">{{ formatCurrency(factura.subTotal) }}</td>
              </tr>
              <tr>
                <td>Impuestos:</td>
                <td class="text-end fw-bold">{{ formatCurrency(factura.totalImpuestos) }}</td>
              </tr>
              <tr>
                <td>Descuento:</td>
                <td class="text-end fw-bold">{{ formatCurrency(factura.totalDescuento) }}</td>
              </tr>
              <tr class="table-primary">
                <td class="fs-5">TOTAL:</td>
                <td class="text-end fw-bold fs-4">{{ formatCurrency(factura.total) }}</td>
              </tr>
            </table>
            <hr>
            <div class="d-flex justify-content-between">
              <span>Pagado:</span>
              <span class="fw-bold text-success">{{ formatCurrency(totalPagado) }}</span>
            </div>
            <div class="d-flex justify-content-between">
              <span>Saldo Pendiente:</span>
              <span class="fw-bold" :class="saldoPendiente > 0 ? 'text-danger' : 'text-success'">
                {{ formatCurrency(saldoPendiente) }}
              </span>
            </div>
          </div>
        </div>
      </div>

      <!-- Ítems -->
      <div class="col-12 mb-4">
        <div class="card">
          <div class="card-header">
            <h5 class="mb-0">Ítems de la Factura</h5>
          </div>
          <div class="card-body p-0">
            <table class="table table-hover mb-0">
              <thead class="table-light">
                <tr>
                  <th>Producto/Servicio</th>
                  <th class="text-center">Cantidad</th>
                  <th class="text-end">Precio Unit.</th>
                  <th class="text-end">Impuesto %</th>
                  <th class="text-end">Total</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="linea in factura.lineas" :key="linea.id">
                  <td>
                    <strong>{{ linea.productoNombre || linea.descripcion }}</strong>
                    <div v-if="linea.descripcion && linea.productoNombre" class="small text-muted">
                      {{ linea.descripcion }}
                    </div>
                  </td>
                  <td class="text-center">{{ linea.cantidad }}</td>
                  <td class="text-end">{{ formatCurrency(linea.precioUnitario) }}</td>
                  <td class="text-end">{{ linea.impuesto }}%</td>
                  <td class="text-end fw-bold">{{ formatCurrency(linea.totalLinea) }}</td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      </div>

      <!-- Pagos -->
      <div class="col-12 mb-4" v-if="factura.pagos && factura.pagos.length > 0">
        <div class="card">
          <div class="card-header">
            <h5 class="mb-0">Historial de Pagos</h5>
          </div>
          <div class="card-body p-0">
            <table class="table table-hover mb-0">
              <thead class="table-light">
                <tr>
                  <th>Fecha</th>
                  <th>Método</th>
                  <th>Referencia</th>
                  <th class="text-end">Monto</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="pago in factura.pagos" :key="pago.id">
                  <td>{{ formatDate(pago.fechaPago) }}</td>
                  <td>{{ pago.metodoPago }}</td>
                  <td>{{ pago.referencia || '-' }}</td>
                  <td class="text-end fw-bold text-success">{{ formatCurrency(pago.monto) }}</td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
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
            <h5 class="modal-title">Registrar Pago - {{ factura?.numeroFactura }}</h5>
            <button type="button" class="btn-close" @click="cerrarModalPago"></button>
          </div>
          <div class="modal-body">
            <div v-if="factura" class="mb-3">
              <p><strong>Saldo Pendiente:</strong> {{ formatCurrency(saldoPendiente) }}</p>
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
                  :max="saldoPendiente"
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
import { ref, computed, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/authStore'
import api from '@/services/api'

const route = useRoute()
const router = useRouter()
const authStore = useAuthStore()
const tenantId = authStore.getTenantId

const factura = ref(null)
const cargando = ref(true)
const error = ref('')
const modalPagoVisible = ref(false)
const procesando = ref(false)
const pagoForm = ref({
  monto: 0,
  metodoPago: '',
  referencia: ''
})

const totalPagado = computed(() => {
  if (!factura.value?.pagos) return 0
  return factura.value.pagos.reduce((sum, p) => sum + p.monto, 0)
})

const saldoPendiente = computed(() => {
  if (!factura.value) return 0
  return factura.value.total - totalPagado.value
})

const cargarFactura = async () => {
  cargando.value = true
  error.value = ''
  
  try {
    const response = await api.get(`/${tenantId}/facturas/${route.params.id}`)
    factura.value = response.data
  } catch (err) {
    error.value = err.response?.data?.message || 'Error al cargar la factura'
    console.error(err)
  } finally {
    cargando.value = false
  }
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

const emitirFactura = async () => {
  if (!confirm('¿Está seguro de emitir esta factura?')) return
  
  try {
    await api.put(`/${tenantId}/facturas/${factura.value.id}/emitir`)
    alert('Factura emitida correctamente')
    cargarFactura()
  } catch (err) {
    alert(err.response?.data?.error || 'Error al emitir la factura')
  }
}

const anularFactura = async () => {
  if (!confirm('¿Está seguro de ANULAR esta factura? Esta acción no se puede deshacer.')) return
  
  try {
    await api.put(`/${tenantId}/facturas/${factura.value.id}/anular`)
    alert('Factura anulada correctamente')
    router.push('/facturacion/facturas')
  } catch (err) {
    alert(err.response?.data?.error || 'Error al anular la factura')
  }
}

const abrirModalPago = () => {
  pagoForm.value = { monto: Math.min(saldoPendiente.value, 0), metodoPago: '', referencia: '' }
  modalPagoVisible.value = true
}

const cerrarModalPago = () => {
  modalPagoVisible.value = false
}

const registrarPago = async () => {
  procesando.value = true
  try {
    await api.post(`/${tenantId}/facturas/${factura.value.id}/pagos`, pagoForm.value)
    alert('Pago registrado correctamente')
    cerrarModalPago()
    cargarFactura()
  } catch (err) {
    alert(err.response?.data?.error || 'Error al registrar el pago')
  } finally {
    procesando.value = false
  }
}

onMounted(() => {
  cargarFactura()
})
</script>

<style scoped>
.factura-detalle-view {
  max-width: 1400px;
  margin: 0 auto;
}

.badge {
  font-size: 0.9rem;
}

.table th {
  font-weight: 600;
  font-size: 0.9rem;
}
</style>
