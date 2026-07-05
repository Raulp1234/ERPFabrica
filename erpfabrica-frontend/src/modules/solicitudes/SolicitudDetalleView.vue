<template>
  <div class="solicitud-detalle-view">
    <div class="d-flex justify-content-between align-items-center mb-4">
      <h4 class="mb-0">Detalle de Solicitud</h4>
      <button class="btn btn-secondary" @click="volver">
        <i class="bi bi-arrow-left me-1"></i>
        Volver
      </button>
    </div>

    <div v-if="loading" class="text-center py-5">
      <div class="spinner-border text-primary" role="status">
        <span class="visually-hidden">Cargando...</span>
      </div>
    </div>

    <div v-else-if="solicitud" class="row">
      <!-- Información general -->
      <div class="col-md-8">
        <div class="card shadow-sm mb-4">
          <div class="card-header d-flex justify-content-between align-items-center">
            <h5 class="mb-0">{{ solicitud.numeroSolicitud }}</h5>
            <span class="badge" :class="getEstadoClass(solicitud.estado)">
              {{ solicitud.estado }}
            </span>
          </div>
          <div class="card-body">
            <div class="row mb-3">
              <div class="col-md-6">
                <label class="text-muted small">Tipo de Solicitud</label>
                <p class="mb-0 fw-bold">{{ solicitud.tipoSolicitud }}</p>
              </div>
              <div class="col-md-6">
                <label class="text-muted small">Tercero</label>
                <p class="mb-0 fw-bold">{{ solicitud.terceroNombre }}</p>
              </div>
            </div>
            <div class="row mb-3">
              <div class="col-md-6">
                <label class="text-muted small">Fecha de Creación</label>
                <p class="mb-0">{{ formatDate(solicitud.fechaCreacion) }}</p>
              </div>
              <div class="col-md-6">
                <label class="text-muted small">Fecha Límite</label>
                <p class="mb-0">{{ solicitud.fechaLimite ? formatDate(solicitud.fechaLimite) : '-' }}</p>
              </div>
            </div>
            <div v-if="solicitud.notas" class="mb-3">
              <label class="text-muted small">Notas</label>
              <p class="mb-0">{{ solicitud.notas }}</p>
            </div>
          </div>
        </div>

        <!-- Líneas de la solicitud -->
        <div class="card shadow-sm mb-4">
          <div class="card-header">
            <h5 class="mb-0">Productos / Servicios</h5>
          </div>
          <div class="card-body p-0">
            <div class="table-responsive">
              <table class="table table-hover mb-0">
                <thead class="table-light">
                  <tr>
                    <th>Producto</th>
                    <th class="text-center">Cantidad</th>
                    <th class="text-end">Precio Unit.</th>
                    <th class="text-end">Impuesto</th>
                    <th class="text-end">Total</th>
                    <th class="text-center">Estado</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="linea in solicitud.lineas" :key="linea.id">
                    <td>{{ linea.productoNombre || linea.descripcion }}</td>
                    <td class="text-center">{{ linea.cantidadSolicitada }}</td>
                    <td class="text-end">{{ formatCurrency(linea.precioUnitario) }}</td>
                    <td class="text-end">{{ linea.impuesto }}%</td>
                    <td class="text-end fw-bold">{{ formatCurrency(linea.totalLinea) }}</td>
                    <td class="text-center">
                      <span 
                        v-if="linea.facturada" 
                        class="badge bg-success"
                      >
                        Facturado
                      </span>
                      <span v-else class="badge bg-warning">
                        Pendiente
                      </span>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>
        </div>
      </div>

      <!-- Acciones -->
      <div class="col-md-4">
        <div class="card shadow-sm mb-4">
          <div class="card-header">
            <h5 class="mb-0">Acciones</h5>
          </div>
          <div class="card-body">
            <div class="d-grid gap-2">
              <button
                v-if="solicitud.estado === 'Pendiente'"
                class="btn btn-success"
                @click="cambiarEstado('Aprobada')"
              >
                <i class="bi bi-check-lg me-1"></i>
                Aprobar Solicitud
              </button>
              <button
                v-if="solicitud.estado === 'Aprobada'"
                class="btn btn-info text-white"
                @click="cambiarEstado('EnProceso')"
              >
                <i class="bi bi-play-fill me-1"></i>
                Iniciar Proceso
              </button>
              <button
                v-if="solicitud.estado === 'EnProceso'"
                class="btn btn-primary"
                @click="cambiarEstado('Completada')"
              >
                <i class="bi bi-check-all me-1"></i>
                Completar
              </button>
              <button
                v-if="['Aprobada', 'EnProceso'].includes(solicitud.estado)"
                class="btn btn-outline-primary"
                @click="generarFactura"
              >
                <i class="bi bi-file-earmark-text me-1"></i>
                Generar Factura
              </button>
              <button
                v-if="!['Cancelada', 'Completada'].includes(solicitud.estado)"
                class="btn btn-outline-danger"
                @click="cambiarEstado('Cancelada')"
              >
                <i class="bi bi-x-lg me-1"></i>
                Cancelar Solicitud
              </button>
            </div>
          </div>
        </div>

        <!-- Resumen total -->
        <div class="card shadow-sm bg-light">
          <div class="card-body text-center">
            <label class="text-muted small">Total General</label>
            <h3 class="mb-0 text-primary">{{ formatCurrency(totalGeneral) }}</h3>
          </div>
        </div>
      </div>
    </div>

    <div v-else class="text-center py-5">
      <i class="bi bi-exclamation-triangle display-1 text-warning"></i>
      <p class="mt-3 text-muted">No se encontró la solicitud</p>
      <button class="btn btn-primary" @click="volver">Volver al listado</button>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from "vue";
import { useRouter, useRoute } from "vue-router";
import axios from "axios";
import { useAuthStore } from "../../stores/auth";
import { toast } from "vue3-toastify";

const router = useRouter();
const route = useRoute();
const authStore = useAuthStore();

const solicitud = ref(null);
const loading = ref(false);

const tenantId = computed(() => authStore.tenantId || localStorage.getItem("tenantId"));

const totalGeneral = computed(() => {
  if (!solicitud.value?.lineas) return 0;
  return solicitud.value.lineas.reduce((sum, linea) => sum + linea.totalLinea, 0);
});

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

const formatCurrency = (value) => {
  return new Intl.NumberFormat("es-MX", {
    style: "currency",
    currency: "USD",
  }).format(value);
};

const cargarSolicitud = async () => {
  loading.value = true;
  try {
    const response = await axios.get(
      `/api/${tenantId.value}/solicitudes/${route.params.id}`,
      { headers: { Authorization: `Bearer ${authStore.token}` } }
    );
    solicitud.value = response.data;
  } catch (error) {
    console.error("Error al cargar solicitud:", error);
    toast.error("Error al cargar los detalles de la solicitud");
  } finally {
    loading.value = false;
  }
};

const cambiarEstado = async (nuevoEstado) => {
  try {
    await axios.put(
      `/api/${tenantId.value}/solicitudes/${solicitud.value.id}/estado`,
      { estadoNuevo: nuevoEstado },
      { headers: { Authorization: `Bearer ${authStore.token}` } }
    );
    toast.success(`Solicitud ${nuevoEstado.toLowerCase()} correctamente`);
    cargarSolicitud();
  } catch (error) {
    console.error("Error al cambiar estado:", error);
    toast.error(error.response?.data?.error || "Error al cambiar el estado");
  }
};

const generarFactura = async () => {
  try {
    const response = await axios.post(
      `/api/${tenantId.value}/solicitudes/${solicitud.value.id}/generar-factura`,
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

const volver = () => {
  router.push("/solicitudes");
};

onMounted(() => {
  cargarSolicitud();
});
</script>
</template>
