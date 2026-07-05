<template>
  <div class="dashboard-view">
    <!-- Tarjetas de métricas -->
    <div class="row g-4 mb-4">
      <!-- Productos bajo stock mínimo -->
      <div class="col-md-6 col-xl-3">
        <div class="card metric-card shadow-sm border-start border-4" :style="{ borderColor: '#e74c3c' }">
          <div class="card-body">
            <div class="d-flex justify-content-between align-items-center">
              <div>
                <h6 class="text-muted mb-2">Stock Bajo</h6>
                <h3 class="mb-0">{{ dashboardData.productosBajoStockMinimo?.length || 0 }}</h3>
              </div>
              <div class="metric-icon bg-danger bg-opacity-10 text-danger">
                <i class="bi bi-exclamation-triangle"></i>
              </div>
            </div>
            <small class="text-muted">Productos con stock mínimo</small>
          </div>
        </div>
      </div>

      <!-- Facturas pendientes de cobro -->
      <div class="col-md-6 col-xl-3">
        <div class="card metric-card shadow-sm border-start border-4" :style="{ borderColor: '#f39c12' }">
          <div class="card-body">
            <div class="d-flex justify-content-between align-items-center">
              <div>
                <h6 class="text-muted mb-2">Por Cobrar</h6>
                <h3 class="mb-0">{{ formatCurrency(dashboardData.ventasDelMes) }}</h3>
              </div>
              <div class="metric-icon bg-warning bg-opacity-10 text-warning">
                <i class="bi bi-currency-dollar"></i>
              </div>
            </div>
            <small class="text-muted">Ventas del mes</small>
          </div>
        </div>
      </div>

      <!-- Solicitudes pendientes -->
      <div class="col-md-6 col-xl-3">
        <div class="card metric-card shadow-sm border-start border-4" :style="{ borderColor: '#3498db' }">
          <div class="card-body">
            <div class="d-flex justify-content-between align-items-center">
              <div>
                <h6 class="text-muted mb-2">Pendientes</h6>
                <h3 class="mb-0">{{ dashboardData.solicitudesPendientes || 0 }}</h3>
              </div>
              <div class="metric-icon bg-primary bg-opacity-10 text-primary">
                <i class="bi bi-clipboard-check"></i>
              </div>
            </div>
            <small class="text-muted">Solicitudes pendientes</small>
          </div>
        </div>
      </div>

      <!-- Últimos movimientos -->
      <div class="col-md-6 col-xl-3">
        <div class="card metric-card shadow-sm border-start border-4" :style="{ borderColor: '#27ae60' }">
          <div class="card-body">
            <div class="d-flex justify-content-between align-items-center">
              <div>
                <h6 class="text-muted mb-2">Movimientos</h6>
                <h3 class="mb-0">{{ dashboardData.ultimosMovimientos?.length || 0 }}</h3>
              </div>
              <div class="metric-icon bg-success bg-opacity-10 text-success">
                <i class="bi bi-arrow-left-right"></i>
              </div>
            </div>
            <small class="text-muted">Últimos movimientos</small>
          </div>
        </div>
      </div>
    </div>

    <!-- Gráficos y tablas -->
    <div class="row g-4">
      <!-- Tabla: Productos bajo stock mínimo -->
      <div class="col-lg-6">
        <div class="card shadow-sm h-100">
          <div class="card-header bg-white">
            <h5 class="mb-0">
              <i class="bi bi-exclamation-circle me-2"></i>
              Productos Bajo Stock Mínimo
            </h5>
          </div>
          <div class="card-body p-0">
            <div class="table-responsive">
              <table class="table table-hover mb-0">
                <thead class="table-light">
                  <tr>
                    <th>Producto</th>
                    <th class="text-end">Stock Actual</th>
                    <th class="text-end">Mínimo</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="producto in dashboardData.productosBajoStockMinimo" :key="producto.productoId">
                    <td>{{ producto.nombre }}</td>
                    <td class="text-end text-danger fw-bold">{{ producto.stockActual }}</td>
                    <td class="text-end">{{ producto.stockMinimo }}</td>
                  </tr>
                  <tr v-if="!dashboardData.productosBajoStockMinimo?.length">
                    <td colspan="3" class="text-center py-4 text-muted">
                      No hay productos con stock bajo
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>
        </div>
      </div>

      <!-- Tabla: Facturas pendientes de cobro -->
      <div class="col-lg-6">
        <div class="card shadow-sm h-100">
          <div class="card-header bg-white">
            <h5 class="mb-0">
              <i class="bi bi-receipt me-2"></i>
              Facturas Pendientes de Cobro
            </h5>
          </div>
          <div class="card-body p-0">
            <div class="table-responsive">
              <table class="table table-hover mb-0">
                <thead class="table-light">
                  <tr>
                    <th>Factura</th>
                    <th>Cliente</th>
                    <th class="text-end">Saldo</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="factura in dashboardData.facturasPendientesCobro" :key="factura.facturaId">
                    <td>{{ factura.numeroFactura }}</td>
                    <td>{{ factura.terceroNombre }}</td>
                    <td class="text-end text-warning fw-bold">{{ formatCurrency(factura.saldoPendiente) }}</td>
                  </tr>
                  <tr v-if="!dashboardData.facturasPendientesCobro?.length">
                    <td colspan="3" class="text-center py-4 text-muted">
                      No hay facturas pendientes
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Últimos movimientos -->
    <div class="row g-4 mt-1">
      <div class="col-12">
        <div class="card shadow-sm">
          <div class="card-header bg-white">
            <h5 class="mb-0">
              <i class="bi bi-clock-history me-2"></i>
              Últimos Movimientos de Inventario
            </h5>
          </div>
          <div class="card-body p-0">
            <div class="table-responsive">
              <table class="table table-hover mb-0">
                <thead class="table-light">
                  <tr>
                    <th>Fecha</th>
                    <th>Producto</th>
                    <th>Tipo</th>
                    <th class="text-end">Cantidad</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="movimiento in dashboardData.ultimosMovimientos" :key="movimiento.id">
                    <td>{{ formatDate(movimiento.fecha) }}</td>
                    <td>{{ movimiento.productoNombre }}</td>
                    <td>
                      <span 
                        class="badge"
                        :class="{
                          'bg-success': movimiento.tipoMovimiento === 'Entrada',
                          'bg-danger': movimiento.tipoMovimiento === 'Salida',
                          'bg-info': movimiento.tipoMovimiento === 'Ajuste',
                        }"
                      >
                        {{ movimiento.tipoMovimiento }}
                      </span>
                    </td>
                    <td class="text-end fw-bold">{{ movimiento.cantidad }}</td>
                  </tr>
                  <tr v-if="!dashboardData.ultimosMovimientos?.length">
                    <td colspan="4" class="text-center py-4 text-muted">
                      No hay movimientos recientes
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
</template>

<script setup>
import { ref, onMounted, computed } from 'vue';
import { useRoute } from 'vue-router';
import apiClient from '@/api';
import { useTenantConfigStore } from '@/stores/tenant';
import { toast } from 'vue3-toastify';

const route = useRoute();
const tenantConfigStore = useTenantConfigStore();

const loading = ref(true);
const dashboardData = ref({
  productosBajoStockMinimo: [],
  facturasPendientesCobro: [],
  ventasDelMes: 0,
  solicitudesPendientes: 0,
  ultimosMovimientos: [],
});

const formatCurrency = (value) => {
  const moneda = tenantConfigStore.monedaPorDefecto || 'USD';
  return new Intl.NumberFormat('es-MX', {
    style: 'currency',
    currency: moneda,
  }).format(value);
};

const formatDate = (dateString) => {
  if (!dateString) return '';
  const date = new Date(dateString);
  return date.toLocaleDateString('es-MX', {
    year: 'numeric',
    month: 'short',
    day: 'numeric',
  });
};

const cargarDashboard = async () => {
  loading.value = true;
  
  try {
    const response = await apiClient.get('/dashboard');
    dashboardData.value = response.data;
  } catch (error) {
    console.error('Error al cargar dashboard:', error);
    toast.error('No se pudo cargar la información del dashboard');
  } finally {
    loading.value = false;
  }
};

onMounted(() => {
  cargarDashboard();
});
</script>

<style scoped>
.dashboard-view {
  animation: fadeIn 0.3s ease;
}

@keyframes fadeIn {
  from {
    opacity: 0;
    transform: translateY(10px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.metric-card {
  transition: transform 0.2s ease, box-shadow 0.2s ease;
}

.metric-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 0.5rem 1rem rgba(0, 0, 0, 0.15) !important;
}

.metric-icon {
  width: 50px;
  height: 50px;
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: 10px;
  font-size: 1.5rem;
}

.card {
  border: none;
  border-radius: 10px;
}

.card-header {
  border-bottom: 1px solid #eee;
  padding: 1rem 1.25rem;
}

.table th {
  font-weight: 600;
  font-size: 0.85rem;
  text-transform: uppercase;
  color: #6c757d;
}

.badge {
  padding: 0.5em 0.75em;
  font-weight: 500;
}
</style>
