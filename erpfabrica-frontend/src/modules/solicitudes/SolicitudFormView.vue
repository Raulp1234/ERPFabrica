<template>
  <div class="solicitud-form-view">
    <div class="d-flex justify-content-between align-items-center mb-4">
      <h4 class="mb-0">{{ esEdicion ? 'Editar Solicitud' : 'Nueva Solicitud' }}</h4>
      <button class="btn btn-secondary" @click="cancelar">
        <i class="bi bi-arrow-left me-1"></i>
        Volver
      </button>
    </div>

    <div class="card shadow-sm">
      <div class="card-body">
        <form @submit.prevent="guardarSolicitud">
          <!-- Datos generales -->
          <h5 class="mb-3">Datos Generales</h5>
          <div class="row g-3 mb-4">
            <div class="col-md-6">
              <label class="form-label">Tipo de Solicitud *</label>
              <select 
                class="form-select" 
                v-model="formulario.tipoSolicitud"
                required
              >
                <option value="PedidoCliente">Pedido de Cliente</option>
                <option value="OrdenCompra">Orden de Compra</option>
                <option value="Traslado">Traslado</option>
              </select>
            </div>
            <div class="col-md-6">
              <label class="form-label">Tercero *</label>
              <select 
                class="form-select" 
                v-model.number="formulario.terceroId"
                required
              >
                <option value="">Seleccione un tercero</option>
                <option 
                  v-for="tercero in terceros" 
                  :key="tercero.id" 
                  :value="tercero.id"
                >
                  {{ tercero.nombre }} ({{ tercero.tipo }})
                </option>
              </select>
            </div>
            <div class="col-md-6">
              <label class="form-label">Fecha Límite</label>
              <input 
                type="date" 
                class="form-control" 
                v-model="formulario.fechaLimite"
              />
            </div>
            <div class="col-12">
              <label class="form-label">Notas</label>
              <textarea 
                class="form-control" 
                v-model="formulario.notas"
                rows="2"
              ></textarea>
            </div>
          </div>

          <!-- Líneas de solicitud -->
          <h5 class="mb-3">Productos / Servicios</h5>
          <div class="table-responsive mb-3">
            <table class="table table-bordered">
              <thead class="table-light">
                <tr>
                  <th>Producto</th>
                  <th style="width: 120px;">Cantidad</th>
                  <th style="width: 150px;">Precio Unit.</th>
                  <th style="width: 120px;">Impuesto %</th>
                  <th style="width: 120px;" class="text-end">Total</th>
                  <th style="width: 50px;"></th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="(linea, index) in formulario.lineas" :key="index">
                  <td>
                    <select 
                      class="form-select" 
                      v-model.number="linea.productoId"
                      @change="seleccionarProducto(index)"
                    >
                      <option value="">Seleccione producto</option>
                      <option 
                        v-for="prod in productos" 
                        :key="prod.id" 
                        :value="prod.id"
                      >
                        {{ prod.nombre }}
                      </option>
                    </select>
                  </td>
                  <td>
                    <input 
                      type="number" 
                      class="form-control" 
                      v-model.number="linea.cantidad"
                      min="1"
                      step="1"
                      required
                    />
                  </td>
                  <td>
                    <input 
                      type="number" 
                      class="form-control" 
                      v-model.number="linea.precioUnitario"
                      min="0"
                      step="0.01"
                      required
                    />
                  </td>
                  <td>
                    <input 
                      type="number" 
                      class="form-control" 
                      v-model.number="linea.impuesto"
                      min="0"
                      max="100"
                      step="0.01"
                    />
                  </td>
                  <td class="text-end">
                    {{ formatCurrency(calcularTotalLinea(linea)) }}
                  </td>
                  <td>
                    <button 
                      type="button"
                      class="btn btn-outline-danger btn-sm"
                      @click="eliminarLinea(index)"
                    >
                      <i class="bi bi-trash"></i>
                    </button>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>

          <button 
            type="button" 
            class="btn btn-outline-primary mb-4"
            @click="agregarLinea"
          >
            <i class="bi bi-plus-lg me-1"></i>
            Agregar Producto
          </button>

          <!-- Total general -->
          <div class="row justify-content-end">
            <div class="col-md-4">
              <div class="card bg-light">
                <div class="card-body text-end">
                  <h5 class="mb-0">Total: {{ formatCurrency(totalGeneral) }}</h5>
                </div>
              </div>
            </div>
          </div>

          <!-- Botones de acción -->
          <div class="d-flex justify-content-end mt-4 gap-2">
            <button 
              type="button" 
              class="btn btn-secondary"
              @click="cancelar"
            >
              Cancelar
            </button>
            <button 
              type="submit" 
              class="btn btn-primary"
              :disabled="guardando"
            >
              <span 
                v-if="guardando" 
                class="spinner-border spinner-border-sm me-2"
              ></span>
              {{ guardando ? 'Guardando...' : 'Guardar Solicitud' }}
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive, computed, onMounted } from "vue";
import { useRouter, useRoute } from "vue-router";
import axios from "axios";
import { useAuthStore } from "../../stores/auth";
import { toast } from "vue3-toastify";

const router = useRouter();
const route = useRoute();
const authStore = useAuthStore();

const tenantId = computed(() => authStore.tenantId || localStorage.getItem("tenantId"));

const formulario = reactive({
  tipoSolicitud: "PedidoCliente",
  terceroId: null,
  fechaLimite: "",
  notas: "",
  lineas: []
});

const terceros = ref([]);
const productos = ref([]);
const guardando = ref(false);
const esEdicion = ref(false);

const totalGeneral = computed(() => {
  return formulario.lineas.reduce((sum, linea) => sum + calcularTotalLinea(linea), 0);
});

const calcularTotalLinea = (linea) => {
  const subtotal = linea.cantidad * linea.precioUnitario;
  const impuesto = subtotal * (linea.impuesto / 100);
  return subtotal + impuesto;
};

const formatCurrency = (value) => {
  const moneda = "USD";
  return new Intl.NumberFormat("es-MX", {
    style: "currency",
    currency: moneda,
  }).format(value);
};

const cargarTerceros = async () => {
  try {
    const response = await axios.get(
      `/api/${tenantId.value}/terceros`,
      { headers: { Authorization: `Bearer ${authStore.token}` } }
    );
    terceros.value = response.data;
  } catch (error) {
    console.error("Error al cargar terceros:", error);
  }
};

const cargarProductos = async () => {
  try {
    const response = await axios.get(
      `/api/${tenantId.value}/producto`,
      { headers: { Authorization: `Bearer ${authStore.token}` } }
    );
    productos.value = response.data;
  } catch (error) {
    console.error("Error al cargar productos:", error);
  }
};

const agregarLinea = () => {
  formulario.lineas.push({
    productoId: null,
    descripcion: "",
    cantidad: 1,
    precioUnitario: 0,
    impuesto: 0
  });
};

const eliminarLinea = (index) => {
  formulario.lineas.splice(index, 1);
};

const seleccionarProducto = (index) => {
  const linea = formulario.lineas[index];
  const producto = productos.value.find(p => p.id === linea.productoId);
  if (producto) {
    linea.descripcion = producto.nombre;
    linea.precioUnitario = producto.precioVenta || 0;
  }
};

const guardarSolicitud = async () => {
  if (formulario.lineas.length === 0) {
    toast.error("Debe agregar al menos un producto");
    return;
  }

  guardando.value = true;
  try {
    const data = {
      tipoSolicitud: formulario.tipoSolicitud,
      terceroId: formulario.terceroId,
      fechaLimite: formulario.fechaLimite || null,
      notas: formulario.notas,
      lineas: formulario.lineas.map(l => ({
        productoId: l.productoId,
        descripcion: l.descripcion,
        cantidad: l.cantidad,
        precioUnitario: l.precioUnitario,
        impuesto: l.impuesto
      }))
    };

    await axios.post(
      `/api/${tenantId.value}/solicitudes`,
      data,
      { headers: { Authorization: `Bearer ${authStore.token}` } }
    );

    toast.success("Solicitud creada correctamente");
    router.push("/solicitudes");
  } catch (error) {
    console.error("Error al guardar solicitud:", error);
    toast.error(error.response?.data?.error || "Error al guardar la solicitud");
  } finally {
    guardando.value = false;
  }
};

const cancelar = () => {
  router.push("/solicitudes");
};

onMounted(async () => {
  await Promise.all([cargarTerceros(), cargarProductos()]);
  agregarLinea(); // Agregar primera línea vacía
  
  // Si hay ID en la ruta, cargar datos para edición
  if (route.params.id) {
    esEdicion.value = true;
    // TODO: Implementar carga de datos para edición
    toast.info("La edición se implementará próximamente");
  }
});
</script>
