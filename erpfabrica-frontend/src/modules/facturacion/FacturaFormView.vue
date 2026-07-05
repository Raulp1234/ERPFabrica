<template>
  <div class="factura-form-view container-fluid py-4">
    <div class="row mb-4">
      <div class="col-12">
        <h2><i class="bi bi-file-earmark-plus me-2"></i>Nueva Factura</h2>
      </div>
    </div>

    <form @submit.prevent="guardarFactura">
      <!-- Datos principales -->
      <div class="card mb-4">
        <div class="card-header">
          <h5 class="mb-0">Información General</h5>
        </div>
        <div class="card-body">
          <div class="row g-3">
            <div class="col-md-3">
              <label class="form-label">Tipo de Factura</label>
              <select v-model="factura.tipoFactura" class="form-select" required>
                <option value="Venta">Venta</option>
                <option value="Compra">Compra</option>
                <option value="NotaCredito">Nota Crédito</option>
                <option value="NotaDebito">Nota Débito</option>
              </select>
            </div>
            <div class="col-md-4">
              <label class="form-label">Tercero</label>
              <div class="input-group">
                <select 
                  v-model="factura.terceroId" 
                  class="form-select" 
                  required
                  @change="obtenerSaldoPendiente"
                >
                  <option value="">Seleccione...</option>
                  <option v-for="tercero in terceros" :key="tercero.id" :value="tercero.id">
                    {{ tercero.nombre }} ({{ tercero.tipoDocumento }})
                  </option>
                </select>
                <router-link to="/terceros/nuevo" class="btn btn-outline-primary" title="Nuevo tercero">
                  <i class="bi bi-plus-lg"></i>
                </router-link>
              </div>
            </div>
            <div class="col-md-3">
              <label class="form-label">Fecha Vencimiento</label>
              <input v-model="factura.fechaVencimiento" type="date" class="form-control" />
            </div>
            <div class="col-md-2">
              <label class="form-label">Solicitud (Opcional)</label>
              <select v-model="factura.solicitudId" class="form-select">
                <option value="">Ninguna</option>
                <option v-for="sol in solicitudes" :key="sol.id" :value="sol.id">
                  {{ sol.numeroSolicitud }}
                </option>
              </select>
            </div>
          </div>
          <div class="row mt-3">
            <div class="col-12">
              <label class="form-label">Notas</label>
              <textarea v-model="factura.notas" class="form-control" rows="2"></textarea>
            </div>
          </div>
        </div>
      </div>

      <!-- Líneas de factura -->
      <div class="card mb-4">
        <div class="card-header d-flex justify-content-between align-items-center">
          <h5 class="mb-0">Ítems</h5>
          <button type="button" class="btn btn-sm btn-primary" @click="agregarLinea">
            <i class="bi bi-plus-lg me-1"></i> Agregar Ítem
          </button>
        </div>
        <div class="card-body p-0">
          <table class="table table-hover mb-0">
            <thead class="table-light">
              <tr>
                <th style="width: 35%">Producto/Servicio</th>
                <th style="width: 15%">Cantidad</th>
                <th style="width: 15%">Precio Unit.</th>
                <th style="width: 12%">Impuesto %</th>
                <th style="width: 13%">Total</th>
                <th style="width: 10%"></th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="(linea, index) in factura.lineas" :key="index">
                <td>
                  <select v-model="linea.productoId" class="form-select form-select-sm" required>
                    <option value="">Seleccione...</option>
                    <option v-for="prod in productos" :key="prod.id" :value="prod.id">
                      {{ prod.nombre }} - ${{ prod.precioVenta }}
                    </option>
                  </select>
                  <input 
                    v-if="!linea.productoId" 
                    v-model="linea.descripcion" 
                    class="form-control form-control-sm mt-1" 
                    placeholder="Descripción manual"
                  />
                </td>
                <td>
                  <input 
                    v-model.number="linea.cantidad" 
                    type="number" 
                    step="0.01" 
                    min="0"
                    class="form-control form-control-sm" 
                    required
                  />
                </td>
                <td>
                  <input 
                    v-model.number="linea.precioUnitario" 
                    type="number" 
                    step="0.01" 
                    min="0"
                    class="form-control form-control-sm" 
                    required
                  />
                </td>
                <td>
                  <input 
                    v-model.number="linea.impuesto" 
                    type="number" 
                    step="0.01" 
                    min="0"
                    max="100"
                    class="form-control form-control-sm" 
                  />
                </td>
                <td class="fw-bold">{{ formatCurrency(calcularTotalLinea(linea)) }}</td>
                <td>
                  <button 
                    type="button" 
                    class="btn btn-sm btn-outline-danger" 
                    @click="eliminarLinea(index)"
                  >
                    <i class="bi bi-trash"></i>
                  </button>
                </td>
              </tr>
            </tbody>
            <tfoot v-if="factura.lineas.length === 0">
              <tr>
                <td colspan="6" class="text-center py-4 text-muted">
                  No hay ítems agregados. Use el botón "Agregar Ítem" para comenzar.
                </td>
              </tr>
            </tfoot>
          </table>
        </div>
      </div>

      <!-- Totales -->
      <div class="card mb-4">
        <div class="card-body">
          <div class="row justify-content-end">
            <div class="col-md-4">
              <table class="table table-sm">
                <tr>
                  <td class="text-end">Subtotal:</td>
                  <td class="text-end fw-bold">{{ formatCurrency(totales.subtotal) }}</td>
                </tr>
                <tr>
                  <td class="text-end">Impuestos:</td>
                  <td class="text-end fw-bold">{{ formatCurrency(totales.impuestos) }}</td>
                </tr>
                <tr>
                  <td class="text-end">Descuento:</td>
                  <td class="text-end fw-bold">{{ formatCurrency(totales.descuento) }}</td>
                </tr>
                <tr class="table-primary">
                  <td class="text-end">TOTAL:</td>
                  <td class="text-end fw-bold fs-5">{{ formatCurrency(totales.total) }}</td>
                </tr>
              </table>
            </div>
          </div>
        </div>
      </div>

      <!-- Botones de acción -->
      <div class="d-flex justify-content-between">
        <router-link to="/facturacion/facturas" class="btn btn-secondary">
          <i class="bi bi-arrow-left me-1"></i> Cancelar
        </router-link>
        <button type="submit" class="btn btn-primary" :disabled="procesando || factura.lineas.length === 0">
          <span v-if="procesando" class="spinner-border spinner-border-sm me-2"></span>
          <i v-else class="bi bi-save me-1"></i> Guardar Factura
        </button>
      </div>
    </form>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/authStore'
import api from '@/services/api'

const router = useRouter()
const authStore = useAuthStore()
const tenantId = authStore.getTenantId

const procesando = ref(false)
const terceros = ref([])
const productos = ref([])
const solicitudes = ref([])

const factura = ref({
  tipoFactura: 'Venta',
  terceroId: '',
  solicitudId: '',
  fechaVencimiento: '',
  notas: '',
  lineas: []
})

const totales = computed(() => {
  const subtotal = factura.value.lineas.reduce((sum, l) => sum + (l.cantidad * l.precioUnitario), 0)
  const impuestos = factura.value.lineas.reduce((sum, l) => sum + (l.cantidad * l.precioUnitario * (l.impuesto / 100)), 0)
  const descuento = 0 // Se puede agregar campo de descuento global si es necesario
  return {
    subtotal,
    impuestos,
    descuento,
    total: subtotal + impuestos - descuento
  }
})

const calcularTotalLinea = (linea) => {
  const subtotal = linea.cantidad * linea.precioUnitario
  const impuesto = subtotal * (linea.impuesto / 100)
  return subtotal + impuesto
}

const agregarLinea = () => {
  factura.value.lineas.push({
    productoId: '',
    descripcion: '',
    cantidad: 1,
    precioUnitario: 0,
    impuesto: 0
  })
}

const eliminarLinea = (index) => {
  factura.value.lineas.splice(index, 1)
}

const cargarTerceros = async () => {
  try {
    const response = await api.get(`/${tenantId}/terceros`)
    terceros.value = response.data
  } catch (err) {
    console.error('Error al cargar terceros:', err)
  }
}

const cargarProductos = async () => {
  try {
    const response = await api.get(`/${tenantId}/productos`)
    productos.value = response.data
  } catch (err) {
    console.error('Error al cargar productos:', err)
  }
}

const cargarSolicitudes = async () => {
  try {
    const response = await api.get(`/${tenantId}/solicitudes`)
    solicitudes.value = response.data.filter(s => s.estado !== 'Anulada')
  } catch (err) {
    console.error('Error al cargar solicitudes:', err)
  }
}

const obtenerSaldoPendiente = async () => {
  if (!factura.value.terceroId) return
  try {
    const response = await api.get(`/${tenantId}/facturas/terceros/${factura.value.terceroId}/saldo-pendiente`)
    // Se podría mostrar el saldo pendiente en la UI si se desea
    console.log('Saldo pendiente:', response.data)
  } catch (err) {
    console.error('Error al obtener saldo pendiente:', err)
  }
}

const guardarFactura = async () => {
  if (factura.value.lineas.length === 0) {
    alert('Debe agregar al menos un ítem a la factura')
    return
  }

  procesando.value = true
  try {
    const dto = {
      tipoFactura: factura.value.tipoFactura,
      terceroId: parseInt(factura.value.terceroId),
      solicitudId: factura.value.solicitudId ? parseInt(factura.value.solicitudId) : null,
      fechaVencimiento: factura.value.fechaVencimiento || null,
      notas: factura.value.notas,
      lineas: factura.value.lineas.map(l => ({
        productoId: l.productoId ? parseInt(l.productoId) : null,
        descripcion: l.descripcion || '',
        cantidad: l.cantidad,
        precioUnitario: l.precioUnitario,
        impuesto: l.impuesto
      }))
    }

    const response = await api.post(`/${tenantId}/facturas`, dto)
    alert('Factura creada correctamente')
    router.push(`/facturacion/facturas/${response.data.id}`)
  } catch (err) {
    alert(err.response?.data?.error || 'Error al crear la factura')
  } finally {
    procesando.value = false
  }
}

onMounted(() => {
  cargarTerceros()
  cargarProductos()
  cargarSolicitudes()
  agregarLinea() // Agregar primera línea por defecto
})
</script>

<style scoped>
.factura-form-view {
  max-width: 1200px;
  margin: 0 auto;
}

.form-select-sm,
.form-control-sm {
  font-size: 0.875rem;
}

.table th {
  font-weight: 600;
  font-size: 0.9rem;
}
</style>
