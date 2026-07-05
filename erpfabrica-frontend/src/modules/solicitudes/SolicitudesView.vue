<template>
  <div class="solicitudes-view">
    <!-- Encabezado -->
    <div class="d-flex justify-content-between align-items-center mb-4">
      <h4 class="mb-0">Solicitudes</h4>
      <button class="btn btn-primary" @click="nuevaSolicitud">
        <i class="bi bi-plus-lg me-1"></i>
        Nueva Solicitud
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
                placeholder="Buscar por número o tercero..."
                v-model="filtros.busqueda"
                @input="buscarSolicitudes"
              />
            </div>
          </div>
          <div class="col-md-4">
            <select
              class="form-select"
              v-model="filtros.estado"
              @change="buscarSolicitudes"
            >
              <option value="">Todos los estados</option>
              <option value="Pendiente">Pendiente</option>
              <option value="Aprobada">Aprobada</option>
              <option value="EnProceso">En Proceso</option>
              <option value="Completada">Completada</option>
              <option value="Cancelada">Cancelada</option>
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

    <!-- Tabla de solicitudes -->
    <div class="card shadow-sm">
      <div class="card-body p-0">
        <div class="table-responsive">
          <table class="table table-hover mb-0">
            <thead class="table-light">
              <tr>
                <th>Número</th>
                <th>Tipo</th>
                <th>Tercero</th>
                <th>Estado</th>
                <th>Fecha Creación</th>
                <th>Fecha Límite</th>
                <th class="text-end">Acciones</th>
              </tr>
            </thead>
            <tbody>
              <tr
                v-for="solicitud in solicitudes"
                :key="solicitud.id"
              >
                <td>
                  <code>{{ solicitud.numeroSolicitud }}</code>
                </td>
                <td>{{ solicitud.tipoSolicitud }}</td>
                <td>{{ solicitud.terceroNombre }}</td>
                <td>
                  <span
                    class="badge"
                    :class="getEstadoClass(solicitud.estado)"
                  >
                    {{ solicitud.estado }}
                  </span>
                </td>
                <td>{{ formatDate(solicitud.fechaCreacion) }}</td>
                <td>{{ solicitud.fechaLimite ? formatDate(solicitud.fechaLimite) : '-' }}</td>
                <td class="text-end">
                  <button
                    class="btn btn-sm btn-outline-primary me-1"
                    @click="verDetalle(solicitud.id)"
                    title="Ver detalle"
                  >
                    <i class="bi bi-eye"></i>
                  </button>
                  <button
                    v-if="solicitud.estado === 'Pendiente'"
                    class="btn btn-sm btn-outline-success me-1"
                    @click="cambiarEstado(solicitud, 'Aprobada')"
                    title="Aprobar"
                  >
                    <i class="bi bi-check-lg"></i>
                  </button>
                  <button
                    v-if="solicitud.estado === 'Aprobada'"
                    class="btn btn-sm btn-outline-info me-1"
                    @click="generarFactura(solicitud.id)"
                    title="Generar Factura"
                  >
                    <i class="bi bi-file-earmark-text"></i>
                  </button>
                </td>
              </tr>
              <tr
                v-if="!solicitudes.length && !loading"
              >
                <td colspan="7" class="text-center py-5 text-muted">
                  <i class="bi bi-inbox display-6 d-block mb-2"></i>
                  No hay solicitudes registradas
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
  </div>
</template>

<script setup>
import { ref, reactive, onMounted, computed } from "vue";
import { useRouter } from "vue-router";
import axios from "axios";
import { useAuthStore } from "../../stores/auth";
import { toast } from "vue3-toastify";

const router = useRouter();
const authStore = useAuthStore();

const solicitudes = ref([]);
const loading = ref(false);

const filtros = reactive({
  busqueda: "",
  estado: "",
});

const tenantId = computed(() => authStore.tenantId || localStorage.getItem("tenantId"));

const cargarSolicitudes = async () => {
  loading.value = true;
  try {
    const params = {};
    if (filtros.estado) params.estado = filtros.estado;
    
    const response = await axios.get(
      `/api/${tenantId.value}/solicitudes`,
      {
        params,
        headers: { Authorization: `Bearer ${authStore.token}` }
      }
    );
    
    let data = response.data;
    
    // Aplicar filtro de búsqueda si existe
    if (filtros.busqueda) {
      const busqueda = filtros.busqueda.toLowerCase();
      data = data.filter(s => 
        s.numeroSolicitud.toLowerCase().includes(busqueda) ||
        s.terceroNombre.toLowerCase().includes(busqueda)
      );
    }
    
    solicitudes.value = data;
  } catch (error) {
    console.error("Error al cargar solicitudes:", error);
    toast.error("Error al cargar las solicitudes");
  } finally {
    loading.value = false;
  }
};

const buscarSolicitudes = () => {
  cargarSolicitudes();
};

const resetFiltros = () => {
  filtros.busqueda = "";
  filtros.estado = "";
  cargarSolicitudes();
};

const getEstadoClass = (estado) => {
  const clases = {
    Pendiente: "bg-warning",
    Aprobada: "bg-success",
    EnProceso: "bg-info",
    Completada: "bg-primary",
    Cancelada: "bg-danger"
  };
  return clases[estado] || "bg-secondary";
};

const formatDate = (dateString) => {
  if (!dateString) return "-";
  return new Date(dateString).toLocaleDateString("es-MX");
};

const nuevaSolicitud = () => {
  router.push("/solicitudes/nueva");
};

const verDetalle = (id) => {
  router.push(`/solicitudes/${id}`);
};

const cambiarEstado = async (solicitud, nuevoEstado) => {
  try {
    await axios.put(
      `/api/${tenantId.value}/solicitudes/${solicitud.id}/estado`,
      { estadoNuevo: nuevoEstado },
      { headers: { Authorization: `Bearer ${authStore.token}` } }
    );
    toast.success(`Solicitud ${nuevoEstado.toLowerCase()} correctamente`);
    cargarSolicitudes();
  } catch (error) {
    console.error("Error al cambiar estado:", error);
    toast.error(error.response?.data?.error || "Error al cambiar el estado");
  }
};

const generarFactura = async (id) => {
  try {
    const response = await axios.post(
      `/api/${tenantId.value}/solicitudes/${id}/generar-factura`,
      {},
      { headers: { Authorization: `Bearer ${authStore.token}` } }
    );
    toast.success("Factura generada correctamente");
    router.push(`/facturacion/facturas/${response.data.id}`);
  } catch (error) {
    console.error("Error al generar factura:", error);
    toast.error(error.response?.data?.error || "Error al generar la factura");
  }
};

onMounted(() => {
  cargarSolicitudes();
});
</script>
