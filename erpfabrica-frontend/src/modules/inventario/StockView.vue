<template>
  <div class="stock-view">
    <!-- Encabezado -->
    <div class="d-flex justify-content-between align-items-center mb-4">
      <h4 class="mb-0">Control de Stock</h4>
      <button class="btn btn-primary" @click="ajustarStock">
        <i class="bi bi-plus-lg me-1"></i>
        Ajuste de Stock
      </button>
    </div>

    <!-- Filtros -->
    <div class="card shadow-sm mb-4">
      <div class="card-body">
        <div class="row g-3">
          <div class="col-md-4">
            <label class="form-label">Producto</label>
            <select class="form-select" v-model="filtros.productoId" @change="cargarStock">
              <option value="">Todos los productos</option>
              <option v-for="prod in inventarioStore.productos" :key="prod.id" :value="prod.id">
                {{ prod.nombre }}
              </option>
            </select>
          </div>
          <div class="col-md-4">
            <label class="form-label">Almacén</label>
            <select class="form-select" v-model="filtros.almacenId" @change="cargarStock">
              <option value="">Todos los almacenes</option>
              <option v-for="alm in almacenes" :key="alm.id" :value="alm.id">
                {{ alm.nombre }}
              </option>
            </select>
          </div>
          <div class="col-md-4">
            <label class="form-label">Estado</label>
            <select class="form-select" v-model="filtros.estado" @change="cargarStock">
              <option value="">Todos</option>
              <option value="ok">Stock OK</option>
              <option value="bajo">Stock Bajo</option>
              <option value="agotado">Agotado</option>
            </select>
          </div>
        </div>
      </div>
    </div>

    <!-- Tabla de Stock -->
    <div class="card shadow-sm">
      <div class="card-body p-0">
        <div class="table-responsive">
          <table class="table table-hover mb-0">
            <thead class="table-light">
              <tr>
                <th>Producto</th>
                <th>SKU</th>
                <th>Almacén</th>
                <th class="text-end">Stock Actual</th>
                <th class="text-end">Mínimo</th>
                <th class="text-end">Máximo</th>
                <th class="text-center">Estado</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="item in stockData" :key="`${item.productoId}-${item.almacenId}`">
                <td>{{ item.productoNombre }}</td>
                <td><code>{{ item.productoSku }}</code></td>
                <td>{{ item.almacenNombre }}</td>
                <td class="text-end fw-bold">{{ item.cantidadDisponible }}</td>
                <td class="text-end">{{ item.stockMinimo }}</td>
                <td class="text-end">{{ item.stockMaximo }}</td>
                <td class="text-center">
                  <span
                    class="badge"
                    :class="{
                      'bg-success': item.estado === 'OK',
                      'bg-warning': item.estado === 'Bajo',
                      'bg-danger': item.estado === 'Agotado'
                    }"
                  >
                    {{ item.estado }}
                  </span>
                </td>
              </tr>
              <tr v-if="!stockData?.length && !loading">
                <td colspan="7" class="text-center py-5 text-muted">
                  No hay datos de stock disponibles
                </td>
              </tr>
            </tbody>
          </table>
        </div>

        <div v-if="loading" class="text-center py-5">
          <div class="spinner-border text-primary" role="status">
            <span class="visually-hidden">Cargando...</span>
          </div>
        </div>
      </div>
    </div>

    <!-- Modal Ajuste de Stock -->
    <div class="modal fade" id="modalAjuste" tabindex="-1" ref="modalAjusteRef">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">Ajuste de Stock</h5>
            <button type="button" class="btn-close" @click="cerrarModalAjuste"></button>
          </div>
          <div class="modal-body">
            <form @submit.prevent="guardarAjuste">
              <div class="mb-3">
                <label class="form-label">Producto *</label>
                <select class="form-select" v-model="ajusteForm.productoId" required>
                  <option value="">Seleccione un producto</option>
                  <option v-for="prod in inventarioStore.productos" :key="prod.id" :value="prod.id">
                    {{ prod.nombre }}
                  </option>
                </select>
              </div>
              <div class="mb-3">
                <label class="form-label">Almacén *</label>
                <select class="form-select" v-model="ajusteForm.almacenId" required>
                  <option value="">Seleccione un almacén</option>
                  <option v-for="alm in almacenes" :key="alm.id" :value="alm.id">
                    {{ alm.nombre }}
                  </option>
                </select>
              </div>
              <div class="mb-3">
                <label class="form-label">Tipo de Movimiento *</label>
                <select class="form-select" v-model="ajusteForm.tipoMovimiento" required>
                  <option value="Entrada">Entrada</option>
                  <option value="Salida">Salida</option>
                  <option value="Ajuste">Ajuste</option>
                </select>
              </div>
              <div class="mb-3">
                <label class="form-label">Cantidad *</label>
                <input type="number" class="form-control" v-model.number="ajusteForm.cantidad" 
                       step="0.01" min="0.01" required />
              </div>
              <div class="mb-3">
                <label class="form-label">Motivo</label>
                <textarea class="form-control" v-model="ajusteForm.motivo" rows="3"></textarea>
              </div>
            </form>
          </div>
          <div class="modal-footer">
            <button type="button" class="btn btn-secondary" @click="cerrarModalAjuste">Cancelar</button>
            <button type="button" class="btn btn-primary" @click="guardarAjuste" :disabled="guardando">
              <span v-if="guardando" class="spinner-border spinner-border-sm me-2"></span>
              {{ guardando ? 'Guardando...' : 'Guardar' }}
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue';
import { useInventarioStore } from '../../stores/inventario';
import { toast } from 'vue3-toastify';
import { Modal } from 'bootstrap';

const inventarioStore = useInventarioStore();

const modalAjusteRef = ref(null);
let modalAjusteInstance = null;

const loading = ref(false);
const guardando = ref(false);
const almacenes = ref([]);
const stockData = ref([]);

const filtros = reactive({
  productoId: '',
  almacenId: '',
  estado: ''
});

const ajusteForm = reactive({
  productoId: '',
  almacenId: '',
  tipoMovimiento: 'Entrada',
  cantidad: 0,
  motivo: ''
});

const cargarStock = async () => {
  loading.value = true;
  try {
    const params = {};
    if (filtros.productoId) params.productoId = filtros.productoId;
    if (filtros.almacenId) params.almacenId = filtros.almacenId;
    
    await inventarioStore.obtenerStock(params);
    stockData.value = inventarioStore.stock;
  } catch (error) {
    toast.error('Error al cargar stock');
  } finally {
    loading.value = false;
  }
};

const cargarAlmacenes = async () => {
  try {
    await inventarioStore.obtenerAlmacenes();
    almacenes.value = inventarioStore.almacenes;
  } catch (error) {
    console.error('Error al cargar almacenes:', error);
  }
};

const ajustarStock = () => {
  Object.assign(ajusteForm, {
    productoId: '',
    almacenId: '',
    tipoMovimiento: 'Entrada',
    cantidad: 0,
    motivo: ''
  });
  modalAjusteInstance = new Modal(modalAjusteRef.value);
  modalAjusteInstance.show();
};

const cerrarModalAjuste = () => {
  if (modalAjusteInstance) {
    modalAjusteInstance.hide();
  }
};

const guardarAjuste = async () => {
  guardando.value = true;
  try {
    const movimientoData = {
      productoId: ajusteForm.productoId,
      almacenId: ajusteForm.almacenId,
      tipoMovimiento: ajusteForm.tipoMovimiento,
      cantidad: ajusteForm.cantidad,
      motivo: ajusteForm.motivo
    };
    
    await inventarioStore.registrarMovimiento(movimientoData);
    toast.success('Movimiento registrado correctamente');
    cerrarModalAjuste();
    cargarStock();
  } catch (error) {
    toast.error(error.response?.data?.message || 'Error al registrar movimiento');
  } finally {
    guardando.value = false;
  }
};

onMounted(() => {
  inventarioStore.obtenerProductos().then(() => {
    cargarStock();
    cargarAlmacenes();
  });
});
</script>

<style scoped>
.stock-view {
  animation: fadeIn 0.3s ease;
}

@keyframes fadeIn {
  from { opacity: 0; }
  to { opacity: 1; }
}

.card {
  border: none;
  border-radius: 10px;
}

.btn-primary {
  background-color: var(--color-primario, #3498db);
  border-color: var(--color-primario, #3498db);
}
</style>
