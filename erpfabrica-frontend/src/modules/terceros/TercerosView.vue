<template>
  <div class="terceros-view">
    <!-- Encabezado -->
    <div class="d-flex justify-content-between align-items-center mb-4">
      <h4 class="mb-0">Terceros</h4>
      <button class="btn btn-primary" @click="abrirModalCrear">
        <i class="bi bi-plus-lg me-1"></i>
        Nuevo Tercero
      </button>
    </div>

    <!-- Filtros -->
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
                placeholder="Buscar por nombre o documento..."
                v-model="filtros.busqueda"
                @input="buscarTerceros"
              />
            </div>
          </div>
          <div class="col-md-4">
            <select
              class="form-select"
              v-model="filtros.tipo"
              @change="buscarTerceros"
            >
              <option value="">Todos los tipos</option>
              <option value="Cliente">Cliente</option>
              <option value="Proveedor">Proveedor</option>
              <option value="Ambos">Ambos</option>
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

    <!-- Tabla de terceros -->
    <div class="card shadow-sm">
      <div class="card-body p-0">
        <div class="table-responsive">
          <table class="table table-hover mb-0">
            <thead class="table-light">
              <tr>
                <th>Código</th>
                <th>Nombre</th>
                <th>Tipo</th>
                <th>Documento</th>
                <th>Email</th>
                <th>Teléfono</th>
                <th class="text-center">Estado</th>
                <th class="text-end">Acciones</th>
              </tr>
            </thead>
            <tbody>
              <tr
                v-for="tercero in terceros"
                :key="tercero.id"
              >
                <td><code>{{ tercero.codigo }}</code></td>
                <td>
                  <router-link :to="`/terceros/${tercero.id}`">
                    {{ tercero.nombre }}
                  </router-link>
                </td>
                <td>
                  <span class="badge bg-info">{{ tercero.tipo }}</span>
                </td>
                <td>{{ tercero.documentoIdentidad || '-' }}</td>
                <td>{{ tercero.email || '-' }}</td>
                <td>{{ tercero.telefono || '-' }}</td>
                <td class="text-center">
                  <span
                    class="badge"
                    :class="tercero.esActivo ? 'bg-success' : 'bg-secondary'"
                  >
                    {{ tercero.esActivo ? "Activo" : "Inactivo" }}
                  </span>
                </td>
                <td class="text-end">
                  <button
                    class="btn btn-sm btn-outline-primary me-1"
                    @click="verDetalle(tercero.id)"
                    title="Ver detalle"
                  >
                    <i class="bi bi-eye"></i>
                  </button>
                  <button
                    class="btn btn-sm btn-outline-secondary me-1"
                    @click="editarTercero(tercero)"
                    title="Editar"
                  >
                    <i class="bi bi-pencil"></i>
                  </button>
                  <button
                    class="btn btn-sm btn-outline-danger"
                    @click="confirmarEliminar(tercero)"
                    title="Eliminar"
                  >
                    <i class="bi bi-trash"></i>
                  </button>
                </td>
              </tr>
              <tr
                v-if="!terceros.length && !loading"
              >
                <td colspan="8" class="text-center py-5 text-muted">
                  <i class="bi bi-inbox display-6 d-block mb-2"></i>
                  No hay terceros registrados
                </td>
              </tr>
            </tbody>
          </table>
        </div>

        <!-- Loading spinner -->
        <div v-if="loading" class="text-center py-5">
          <div class="spinner-border text-primary" role="status">
            <span class="visually-hidden">Cargando...</span>
          </div>
        </div>
      </div>
    </div>

    <!-- Modal Crear/Editar Tercero -->
    <div
      class="modal fade"
      id="modalTercero"
      tabindex="-1"
      ref="modalTerceroRef"
    >
      <div class="modal-dialog modal-lg">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">
              {{ modoEdicion ? "Editar Tercero" : "Nuevo Tercero" }}
            </h5>
            <button
              type="button"
              class="btn-close"
              @click="cerrarModal"
            ></button>
          </div>
          <div class="modal-body">
            <form @submit.prevent="guardarTercero">
              <div class="row g-3">
                <div class="col-md-6">
                  <label class="form-label">Código *</label>
                  <input
                    type="text"
                    class="form-control"
                    v-model="formulario.codigo"
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
                <div class="col-md-6">
                  <label class="form-label">Tipo *</label>
                  <select 
                    class="form-select" 
                    v-model="formulario.tipo"
                    required
                  >
                    <option value="Cliente">Cliente</option>
                    <option value="Proveedor">Proveedor</option>
                    <option value="Ambos">Ambos</option>
                  </select>
                </div>
                <div class="col-md-6">
                  <label class="form-label">Documento de Identidad</label>
                  <input
                    type="text"
                    class="form-control"
                    v-model="formulario.documentoIdentidad"
                  />
                </div>
                <div class="col-md-6">
                  <label class="form-label">Email</label>
                  <input
                    type="email"
                    class="form-control"
                    v-model="formulario.email"
                  />
                </div>
                <div class="col-md-6">
                  <label class="form-label">Teléfono</label>
                  <input
                    type="text"
                    class="form-control"
                    v-model="formulario.telefono"
                  />
                </div>
                <div class="col-12">
                  <label class="form-label">Dirección</label>
                  <input
                    type="text"
                    class="form-control"
                    v-model="formulario.direccion"
                  />
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
              @click="guardarTercero"
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
import axios from "axios";
import { useAuthStore } from "../../stores/auth";
import { toast } from "vue3-toastify";
import { Modal } from "bootstrap";

const router = useRouter();
const authStore = useAuthStore();

const terceros = ref([]);
const loading = ref(false);
const modalTerceroRef = ref(null);
let modalInstance = null;

const filtros = reactive({
  busqueda: "",
  tipo: "",
});

const formulario = reactive({
  id: null,
  codigo: "",
  nombre: "",
  tipo: "Cliente",
  email: "",
  telefono: "",
  direccion: "",
  documentoIdentidad: "",
});

const modoEdicion = ref(false);
const guardando = ref(false);

const tenantId = computed(() => authStore.tenantId || localStorage.getItem("tenantId"));

const cargarTerceros = async () => {
  loading.value = true;
  try {
    const response = await axios.get(
      `/api/${tenantId.value}/terceros`,
      { headers: { Authorization: `Bearer ${authStore.token}` } }
    );
    
    let data = response.data;
    
    // Aplicar filtros
    if (filtros.busqueda) {
      const busqueda = filtros.busqueda.toLowerCase();
      data = data.filter(t => 
        t.nombre.toLowerCase().includes(busqueda) ||
        (t.documentoIdentidad && t.documentoIdentidad.toLowerCase().includes(busqueda))
      );
    }
    
    if (filtros.tipo) {
      data = data.filter(t => t.tipo === filtros.tipo);
    }
    
    terceros.value = data;
  } catch (error) {
    console.error("Error al cargar terceros:", error);
    toast.error("Error al cargar los terceros");
  } finally {
    loading.value = false;
  }
};

const buscarTerceros = () => {
  cargarTerceros();
};

const resetFiltros = () => {
  filtros.busqueda = "";
  filtros.tipo = "";
  cargarTerceros();
};

const abrirModalCrear = () => {
  modoEdicion.value = false;
  resetearFormulario();
  modalInstance = new Modal(modalTerceroRef.value);
  modalInstance.show();
};

const editarTercero = (tercero) => {
  modoEdicion.value = true;
  Object.assign(formulario, {
    id: tercero.id,
    codigo: tercero.codigo,
    nombre: tercero.nombre,
    tipo: tercero.tipo,
    email: tercero.email || "",
    telefono: tercero.telefono || "",
    direccion: tercero.direccion || "",
    documentoIdentidad: tercero.documentoIdentidad || "",
  });
  modalInstance = new Modal(modalTerceroRef.value);
  modalInstance.show();
};

const verDetalle = (id) => {
  router.push(`/terceros/${id}`);
};

const confirmarEliminar = async (tercero) => {
  if (confirm(`¿Estás seguro de eliminar el tercero "${tercero.nombre}"?`)) {
    try {
      await axios.delete(
        `/api/${tenantId.value}/terceros/${tercero.id}`,
        { headers: { Authorization: `Bearer ${authStore.token}` } }
      );
      toast.success("Tercero eliminado correctamente");
      cargarTerceros();
    } catch (error) {
      console.error("Error al eliminar tercero:", error);
      toast.error(error.response?.data?.error || "Error al eliminar el tercero");
    }
  }
};

const guardarTercero = async () => {
  guardando.value = true;
  try {
    const data = { ...formulario };
    
    if (modoEdicion.value) {
      await axios.put(
        `/api/${tenantId.value}/terceros/${formulario.id}`,
        data,
        { headers: { Authorization: `Bearer ${authStore.token}` } }
      );
      toast.success("Tercero actualizado correctamente");
    } else {
      await axios.post(
        `/api/${tenantId.value}/terceros`,
        data,
        { headers: { Authorization: `Bearer ${authStore.token}` } }
      );
      toast.success("Tercero creado correctamente");
    }
    
    cerrarModal();
    cargarTerceros();
  } catch (error) {
    console.error("Error al guardar tercero:", error);
    toast.error(error.response?.data?.error || "Error al guardar el tercero");
  } finally {
    guardando.value = false;
  }
};

const cerrarModal = () => {
  modalInstance.hide();
};

const resetearFormulario = () => {
  Object.assign(formulario, {
    id: null,
    codigo: "",
    nombre: "",
    tipo: "Cliente",
    email: "",
    telefono: "",
    direccion: "",
    documentoIdentidad: "",
  });
};

onMounted(() => {
  cargarTerceros();
});
</script>
</template>
