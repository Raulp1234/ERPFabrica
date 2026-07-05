<template>
  <div class="productos-view">
    <!-- Encabezado con título y botón de acción -->
    <div class="d-flex justify-content-between align-items-center mb-4">
      <h4 class="mb-0">Productos</h4>
      <button class="btn btn-primary" @click="abrirModalCrear">
        <i class="bi bi-plus-lg me-1"></i>
        Nuevo Producto
      </button>
    </div>

    <!-- Filtros y búsqueda -->
    <div class="card shadow-sm mb-4">
      <div class="card-body">
        <div class="row g-3">
          <div class="col-md-6">
            <div class="input-group">
              <span class="input-group-text bg-white">
                <i class="bi bi-search"></i>
              </span>
              <input
                type="text"
                class="form-control"
                placeholder="Buscar por nombre o SKU..."
                v-model="filtros.busqueda"
                @input="buscarProductos"
              />
            </div>
          </div>
          <div class="col-md-4">
            <select
              class="form-select"
              v-model="filtros.categoriaId"
              @change="buscarProductos"
            >
              <option value="">Todas las categorías</option>
              <option v-for="cat in categorias" :key="cat.id" :value="cat.id">
                {{ cat.nombre }}
              </option>
            </select>
          </div>
          <div class="col-md-2">
            <button
              class="btn btn-outline-secondary w-100"
              @click="resetFiltros"
            >
              <i class="bi bi-x-circle"></i>
              Limpiar
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Tabla de productos -->
    <div class="card shadow-sm">
      <div class="card-body p-0">
        <div class="table-responsive">
          <table class="table table-hover mb-0">
            <thead class="table-light">
              <tr>
                <th>SKU</th>
                <th>Nombre</th>
                <th>Categoría</th>
                <th class="text-end">Precio Costo</th>
                <th class="text-end">Precio Venta</th>
                <th class="text-center">Estado</th>
                <th class="text-end">Acciones</th>
              </tr>
            </thead>
            <tbody>
              <tr
                v-for="producto in inventarioStore.productos"
                :key="producto.id"
              >
                <td>
                  <code>{{ producto.codigoSKU }}</code>
                </td>
                <td>{{ producto.nombre }}</td>
                <td>{{ producto.categoriaNombre || "-" }}</td>
                <td class="text-end">
                  {{ formatCurrency(producto.precioCosto) }}
                </td>
                <td class="text-end fw-bold">
                  {{ formatCurrency(producto.precioVenta) }}
                </td>
                <td class="text-center">
                  <span
                    class="badge"
                    :class="producto.esActivo ? 'bg-success' : 'bg-secondary'"
                  >
                    {{ producto.esActivo ? "Activo" : "Inactivo" }}
                  </span>
                </td>
                <td class="text-end">
                  <button
                    class="btn btn-sm btn-outline-primary me-1"
                    @click="verProducto(producto.id)"
                    title="Ver detalle"
                  >
                    <i class="bi bi-eye"></i>
                  </button>
                  <button
                    class="btn btn-sm btn-outline-secondary me-1"
                    @click="editarProducto(producto)"
                    title="Editar"
                  >
                    <i class="bi bi-pencil"></i>
                  </button>
                  <button
                    class="btn btn-sm btn-outline-danger"
                    @click="confirmarEliminar(producto)"
                    title="Eliminar"
                  >
                    <i class="bi bi-trash"></i>
                  </button>
                </td>
              </tr>
              <tr
                v-if="
                  !inventarioStore.productos?.length && !inventarioStore.loading
                "
              >
                <td colspan="7" class="text-center py-5 text-muted">
                  <i class="bi bi-inbox display-6 d-block mb-2"></i>
                  No hay productos registrados
                </td>
              </tr>
            </tbody>
          </table>
        </div>

        <!-- Loading spinner -->
        <div v-if="inventarioStore.loading" class="text-center py-5">
          <div class="spinner-border text-primary" role="status">
            <span class="visually-hidden">Cargando...</span>
          </div>
        </div>
      </div>

      <!-- Paginación -->
      <div v-if="totalPaginas > 1" class="card-footer bg-white">
        <nav aria-label="Paginación">
          <ul class="pagination justify-content-center mb-0">
            <li class="page-item" :class="{ disabled: paginaActual === 1 }">
              <button
                class="page-link"
                @click="cambiarPagina(paginaActual - 1)"
              >
                <i class="bi bi-chevron-left"></i>
              </button>
            </li>
            <li
              v-for="pagina in totalPaginas"
              :key="pagina"
              class="page-item"
              :class="{ active: pagina === paginaActual }"
            >
              <button class="page-link" @click="cambiarPagina(pagina)">
                {{ pagina }}
              </button>
            </li>
            <li
              class="page-item"
              :class="{ disabled: paginaActual === totalPaginas }"
            >
              <button
                class="page-link"
                @click="cambiarPagina(paginaActual + 1)"
              >
                <i class="bi bi-chevron-right"></i>
              </button>
            </li>
          </ul>
        </nav>
      </div>
    </div>

    <!-- Modal Crear/Editar Producto -->
    <div
      class="modal fade"
      id="modalProducto"
      tabindex="-1"
      ref="modalProductoRef"
    >
      <div class="modal-dialog modal-lg">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">
              {{ modoEdicion ? "Editar Producto" : "Nuevo Producto" }}
            </h5>
            <button
              type="button"
              class="btn-close"
              @click="cerrarModal"
            ></button>
          </div>
          <div class="modal-body">
            <form @submit.prevent="guardarProducto">
              <div class="row g-3">
                <div class="col-md-6">
                  <label class="form-label">Código SKU *</label>
                  <input
                    type="text"
                    class="form-control"
                    v-model="formulario.codigoSKU"
                    required
                  />
                </div>
                <div class="col-md-6">
                  <label class="form-label">Nombre *</label>
                  <input
                    type="text"
                    class="form-control"
                    v-model="formulario.nombre"
                    required
                  />
                </div>
                <div class="col-12">
                  <label class="form-label">Descripción</label>
                  <textarea
                    class="form-control"
                    v-model="formulario.descripcion"
                    rows="3"
                  ></textarea>
                </div>
                <div class="col-md-4">
                  <label class="form-label">Precio Costo *</label>
                  <input
                    type="number"
                    class="form-control"
                    v-model.number="formulario.precioCosto"
                    step="0.01"
                    min="0"
                    required
                  />
                </div>
                <div class="col-md-4">
                  <label class="form-label">Precio Venta *</label>
                  <input
                    type="number"
                    class="form-control"
                    v-model.number="formulario.precioVenta"
                    step="0.01"
                    min="0"
                    required
                  />
                </div>
                <div class="col-md-4">
                  <label class="form-label">Unidad de Medida</label>
                  <select class="form-select" v-model="formulario.unidadMedida">
                    <option value="unidad">Unidad</option>
                    <option value="kg">Kilogramo</option>
                    <option value="g">Gramo</option>
                    <option value="l">Litro</option>
                    <option value="ml">Mililitro</option>
                    <option value="m">Metro</option>
                    <option value="cm">Centímetro</option>
                  </select>
                </div>
                <div class="col-md-6">
                  <label class="form-label">Tipo de Producto</label>
                  <select class="form-select" v-model="formulario.tipoProducto">
                    <option value="producto">Producto</option>
                    <option value="servicio">Servicio</option>
                    <option value="kit">Kit</option>
                  </select>
                </div>
                <div class="col-md-6">
                  <label class="form-label">Categoría</label>
                  <select
                    class="form-select"
                    v-model="formulario.categoriaProductoId"
                  >
                    <option value="">Sin categoría</option>
                    <option
                      v-for="cat in categorias"
                      :key="cat.id"
                      :value="cat.id"
                    >
                      {{ cat.nombre }}
                    </option>
                  </select>
                </div>

                <!-- Campos extra dinámicos -->
                <div v-if="camposExtra.length" class="col-12">
                  <hr />
                  <h6>Campos Personalizados</h6>
                  <div class="row g-3">
                    <div
                      v-for="campo in camposExtra"
                      :key="campo.id"
                      class="col-md-6"
                    >
                      <label class="form-label">
                        {{ campo.nombreCampo }}
                        <span v-if="campo.esRequerido" class="text-danger"
                          >*</span
                        >
                      </label>
                      <component
                        :is="getInputType(campo.tipoDato)"
                        class="form-control"
                        v-model="formulario.valoresExtra[campo.id]"
                        :required="campo.esRequerido"
                      />
                    </div>
                  </div>
                </div>
              </div>
            </form>
          </div>
          <div class="modal-footer">
            <button
              type="button"
              class="btn btn-secondary"
              @click="cerrarModal"
            >
              Cancelar
            </button>
            <button
              type="button"
              class="btn btn-primary"
              @click="guardarProducto"
              :disabled="guardando"
            >
              <span
                v-if="guardando"
                class="spinner-border spinner-border-sm me-2"
              ></span>
              {{ guardando ? "Guardando..." : "Guardar" }}
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted, computed } from "vue";
import { useRouter } from "vue-router";
import { useInventarioStore } from "../../stores/inventario";
import { useCamposExtraStore } from "../../stores/camposExtra";
import { useTenantConfigStore } from "../../stores/tenant";
import { toast } from "vue3-toastify";
import { Modal } from "bootstrap";

const router = useRouter();
const inventarioStore = useInventarioStore();
const camposExtraStore = useCamposExtraStore();
const tenantConfigStore = useTenantConfigStore();

const modalProductoRef = ref(null);
let modalInstance = null;

const filtros = reactive({
  busqueda: "",
  categoriaId: "",
  pagina: 1,
  limite: 10,
});

const formulario = reactive({
  id: null,
  codigoSKU: "",
  nombre: "",
  descripcion: "",
  precioCosto: 0,
  precioVenta: 0,
  tipoProducto: "producto",
  unidadMedida: "unidad",
  categoriaProductoId: "",
  valoresExtra: {},
});

const modoEdicion = ref(false);
const guardando = ref(false);
const categorias = ref([]);
const camposExtra = ref([]);

const paginaActual = computed(() => filtros.pagina);
const totalPaginas = computed(() =>
  Math.ceil(inventarioStore.pagination.total / filtros.limite),
);

const formatCurrency = (value) => {
  const moneda = tenantConfigStore.monedaPorDefecto || "USD";
  return new Intl.NumberFormat("es-MX", {
    style: "currency",
    currency: moneda,
  }).format(value);
};

const cargarProductos = async () => {
  await inventarioStore.obtenerProductos(filtros);
};

const cargarCategorias = async () => {
  try {
    categorias.value = await inventarioStore.obtenerCategorias("producto");
  } catch (error) {
    console.error("Error al cargar categorías:", error);
  }
};

const cargarCamposExtra = async () => {
  try {
    camposExtra.value = await camposExtraStore.obtenerDefiniciones("Producto");
  } catch (error) {
    console.error("Error al cargar campos extra:", error);
  }
};

const buscarProductos = () => {
  filtros.pagina = 1;
  cargarProductos();
};

const resetFiltros = () => {
  filtros.busqueda = "";
  filtros.categoriaId = "";
  filtros.pagina = 1;
  cargarProductos();
};

const cambiarPagina = (pagina) => {
  if (pagina < 1 || pagina > totalPaginas.value) return;
  filtros.pagina = pagina;
  cargarProductos();
};

const abrirModalCrear = () => {
  modoEdicion.value = false;
  resetearFormulario();
  modalInstance = new Modal(modalProductoRef.value);
  modalInstance.show();
};

const editarProducto = (producto) => {
  modoEdicion.value = true;
  Object.assign(formulario, {
    id: producto.id,
    codigoSKU: producto.codigoSKU,
    nombre: producto.nombre,
    descripcion: producto.descripcion || "",
    precioCosto: producto.precioCosto,
    precioVenta: producto.precioVenta,
    tipoProducto: producto.tipoProducto,
    unidadMedida: producto.unidadMedida,
    categoriaProductoId: producto.categoriaProductoId || "",
    valoresExtra: producto.valoresExtra || {},
  });
  modalInstance = new Modal(modalProductoRef.value);
  modalInstance.show();
};

const verProducto = (id) => {
  router.push(`/inventario/productos/${id}`);
};

const confirmarEliminar = async (producto) => {
  if (confirm(`¿Estás seguro de eliminar el producto "${producto.nombre}"?`)) {
    try {
      await inventarioStore.eliminarProducto(producto.id);
      toast.success("Producto eliminado correctamente");
      cargarProductos();
    } catch (error) {
      toast.error("Error al eliminar producto");
    }
  }
};

const guardarProducto = async () => {
  guardando.value = true;

  try {
    // Convertir valoresExtra al formato esperado por la API
    const valoresExtraArray = Object.entries(formulario.valoresExtra)
      .filter(([_, valor]) => valor !== "" && valor !== null)
      .map(([campoExtraDefinicionId, valor]) => ({
        campoExtraDefinicionId,
        valor,
      }));

    const productoData = {
      ...formulario,
      valoresExtra: valoresExtraArray,
    };

    if (modoEdicion.value) {
      await inventarioStore.actualizarProducto(formulario.id, productoData);
      toast.success("Producto actualizado correctamente");
    } else {
      await inventarioStore.crearProducto(productoData);
      toast.success("Producto creado correctamente");
    }

    cerrarModal();
    cargarProductos();
  } catch (error) {
    toast.error(error.response?.data?.message || "Error al guardar producto");
  } finally {
    guardando.value = false;
  }
};

const cerrarModal = () => {
  if (modalInstance) {
    modalInstance.hide();
  }
  resetearFormulario();
};

const resetearFormulario = () => {
  Object.assign(formulario, {
    id: null,
    codigoSKU: "",
    nombre: "",
    descripcion: "",
    precioCosto: 0,
    precioVenta: 0,
    tipoProducto: "producto",
    unidadMedida: "unidad",
    categoriaProductoId: "",
    valoresExtra: {},
  });
};

const getInputType = (tipoDato) => {
  const tipos = {
    texto: "input",
    numero: "input",
    fecha: "input",
    booleano: "input",
    seleccion: "select",
  };
  return tipos[tipoDato] || "input";
};

onMounted(() => {
  cargarProductos();
  cargarCategorias();
  cargarCamposExtra();
});
</script>

<style scoped>
.productos-view {
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

.btn-primary {
  background-color: var(--color-primario, #3498db);
  border-color: var(--color-primario, #3498db);
}

.badge {
  padding: 0.5em 0.75em;
  font-weight: 500;
}
</style>
