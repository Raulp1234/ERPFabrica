<template>
  <div class="usuarios-view">
    <div class="container-fluid">
      <!-- Encabezado con acciones -->
      <div class="d-flex justify-content-between align-items-center mb-4">
        <h2 class="mb-0">
          <i class="bi bi-people me-2"></i>
          Gestión de Usuarios
        </h2>
        <button @click="abrirModalCrear" class="btn btn-primary">
          <i class="bi bi-plus-circle me-1"></i> Nuevo Usuario
        </button>
      </div>

      <!-- Tabla de usuarios -->
      <div class="card">
        <div class="card-header d-flex justify-content-between align-items-center">
          <h5 class="mb-0"><i class="bi bi-list-ul me-2"></i>Listado de Usuarios</h5>
          <button @click="cargarUsuarios" class="btn btn-sm btn-outline-primary">
            <i class="bi bi-arrow-clockwise"></i> Actualizar
          </button>
        </div>
        <div class="card-body">
          <div v-if="loading" class="text-center py-5">
            <div class="spinner-border text-primary" role="status">
              <span class="visually-hidden">Cargando...</span>
            </div>
            <p class="mt-2">Cargando usuarios...</p>
          </div>

          <div v-else-if="error" class="alert alert-danger" role="alert">
            {{ error }}
          </div>

          <div v-else-if="usuarios.length === 0" class="text-center py-5 text-muted">
            <i class="bi bi-inbox display-4 mb-2"></i>
            <p>No hay usuarios registrados.</p>
          </div>

          <div v-else class="table-responsive">
            <table class="table table-hover">
              <thead>
                <tr>
                  <th>ID</th>
                  <th>Nombre de Usuario</th>
                  <th>Email</th>
                  <th>Rol</th>
                  <th>Estado</th>
                  <th>Último Acceso</th>
                  <th class="text-end">Acciones</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="usuario in usuarios" :key="usuario.id">
                  <td>{{ usuario.id }}</td>
                  <td><strong>{{ usuario.username }}</strong></td>
                  <td>{{ usuario.email }}</td>
                  <td>
                    <span :class="['badge', getRolBadgeClass(usuario.rol)]">
                      {{ usuario.rol || 'Sin rol' }}
                    </span>
                  </td>
                  <td>
                    <span :class="['badge', usuario.activo ? 'bg-success' : 'bg-danger']">
                      {{ usuario.activo ? 'Activo' : 'Inactivo' }}
                    </span>
                  </td>
                  <td>{{ formatDate(usuario.ultimoAcceso) }}</td>
                  <td class="text-end">
                    <button @click="verDetalle(usuario)" class="btn btn-sm btn-outline-info me-1" title="Ver detalle">
                      <i class="bi bi-eye"></i>
                    </button>
                    <button @click="editarUsuario(usuario)" class="btn btn-sm btn-outline-primary me-1" title="Editar">
                      <i class="bi bi-pencil"></i>
                    </button>
                    <button @click="eliminarUsuario(usuario)" class="btn btn-sm btn-outline-danger" title="Eliminar" :disabled="!puedeEliminar(usuario)">
                      <i class="bi bi-trash"></i>
                    </button>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      </div>
    </div>

    <!-- Modal Crear/Editar Usuario -->
    <div v-if="mostrarModal" class="modal fade show d-block" tabindex="-1" style="background-color: rgba(0,0,0,0.5);">
      <div class="modal-dialog">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">{{ esEdicion ? 'Editar Usuario' : 'Nuevo Usuario' }}</h5>
            <button type="button" class="btn-close" @click="cerrarModal"></button>
          </div>
          <div class="modal-body">
            <form @submit.prevent="guardarUsuario">
              <div class="mb-3">
                <label class="form-label">Nombre de Usuario</label>
                <input v-model="formulario.username" type="text" class="form-control" required :disabled="esEdicion" />
              </div>
              <div class="mb-3" v-if="!esEdicion">
                <label class="form-label">Contraseña</label>
                <input v-model="formulario.password" type="password" class="form-control" required />
              </div>
              <div class="mb-3">
                <label class="form-label">Email</label>
                <input v-model="formulario.email" type="email" class="form-control" required />
              </div>
              <div class="mb-3">
                <label class="form-label">Rol</label>
                <select v-model="formulario.rol" class="form-select" required>
                  <option value="">Seleccione un rol...</option>
                  <option value="Administrador">Administrador</option>
                  <option value="Vendedor">Vendedor</option>
                  <option value="Almacenista">Almacenista</option>
                  <option value="Contador">Contador</option>
                </select>
              </div>
              <div class="form-check mb-3">
                <input v-model="formulario.activo" type="checkbox" class="form-check-input" id="activoCheck" />
                <label class="form-check-label" for="activoCheck">Activo</label>
              </div>
            </form>
          </div>
          <div class="modal-footer">
            <button type="button" class="btn btn-secondary" @click="cerrarModal">Cancelar</button>
            <button type="button" class="btn btn-primary" @click="guardarUsuario" :disabled="guardando">
              <span v-if="guardando" class="spinner-border spinner-border-sm me-2"></span>
              {{ esEdicion ? 'Actualizar' : 'Crear' }}
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue';
import axios from 'axios';

const loading = ref(true);
const error = ref(null);
const usuarios = ref([]);
const mostrarModal = ref(false);
const esEdicion = ref(false);
const guardando = ref(false);

const formulario = ref({
  id: null,
  username: '',
  password: '',
  email: '',
  rol: '',
  activo: true
});

const tenantId = computed(() => localStorage.getItem('tenantId'));
const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:5000';

const puedeEliminar = (usuario) => {
  // No permitir eliminar el usuario actual o administradores
  const usuarioActual = JSON.parse(localStorage.getItem('usuario') || '{}');
  return usuario.id !== usuarioActual.id;
};

const cargarUsuarios = async () => {
  loading.value = true;
  error.value = null;
  
  try {
    const response = await axios.get(`${API_URL}/api/${tenantId.value}/usuarios`, {
      headers: {
        'Authorization': `Bearer ${localStorage.getItem('token')}`
      }
    });
    usuarios.value = response.data || [];
  } catch (err) {
    console.error('Error al cargar usuarios:', err);
    if (err.response?.status === 403) {
      error.value = 'No tiene permisos para ver los usuarios. Se requiere rol de administrador.';
    } else {
      error.value = err.response?.data?.error || 'Error al cargar los usuarios.';
    }
  } finally {
    loading.value = false;
  }
};

const abrirModalCrear = () => {
  esEdicion.value = false;
  formulario.value = {
    id: null,
    username: '',
    password: '',
    email: '',
    rol: '',
    activo: true
  };
  mostrarModal.value = true;
};

const editarUsuario = (usuario) => {
  esEdicion.value = true;
  formulario.value = {
    id: usuario.id,
    username: usuario.username,
    password: '',
    email: usuario.email,
    rol: usuario.rol || '',
    activo: usuario.activo !== false
  };
  mostrarModal.value = true;
};

const cerrarModal = () => {
  mostrarModal.value = false;
};

const guardarUsuario = async () => {
  guardando.value = true;
  
  try {
    if (esEdicion.value) {
      await axios.put(`${API_URL}/api/${tenantId.value}/usuarios/${formulario.value.id}`, {
        email: formulario.value.email,
        rol: formulario.value.rol,
        activo: formulario.value.activo
      }, {
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('token')}`,
          'Content-Type': 'application/json'
        }
      });
      alert('Usuario actualizado correctamente');
    } else {
      await axios.post(`${API_URL}/api/${tenantId.value}/usuarios`, formulario.value, {
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('token')}`,
          'Content-Type': 'application/json'
        }
      });
      alert('Usuario creado correctamente');
    }
    
    cerrarModal();
    await cargarUsuarios();
  } catch (err) {
    console.error('Error al guardar usuario:', err);
    alert(err.response?.data?.error || 'Error al guardar el usuario.');
  } finally {
    guardando.value = false;
  }
};

const eliminarUsuario = async (usuario) => {
  if (!confirm(`¿Está seguro de que desea eliminar al usuario "${usuario.username}"? Esta acción no se puede deshacer.`)) {
    return;
  }
  
  try {
    await axios.delete(`${API_URL}/api/${tenantId.value}/usuarios/${usuario.id}`, {
      headers: {
        'Authorization': `Bearer ${localStorage.getItem('token')}`
      }
    });
    
    alert('Usuario eliminado correctamente');
    await cargarUsuarios();
  } catch (err) {
    console.error('Error al eliminar usuario:', err);
    alert(err.response?.data?.error || 'Error al eliminar el usuario.');
  }
};

const verDetalle = (usuario) => {
  // Navegar a la vista de detalle si existe
  // router.push({ name: 'UsuarioDetalle', params: { id: usuario.id } });
  alert(`Detalle del usuario: ${usuario.username}\nEmail: ${usuario.email}\nRol: ${usuario.rol || 'Sin rol'}`);
};

const getRolBadgeClass = (rol) => {
  switch (rol) {
    case 'Administrador': return 'bg-danger';
    case 'Vendedor': return 'bg-primary';
    case 'Almacenista': return 'bg-success';
    case 'Contador': return 'bg-info';
    default: return 'bg-secondary';
  }
};

const formatDate = (dateString) => {
  if (!dateString) return '-';
  const date = new Date(dateString);
  return date.toLocaleDateString() + ' ' + date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
};

onMounted(() => {
  cargarUsuarios();
});
</script>

<style scoped>
.usuarios-view {
  padding: 20px;
}

.card {
  box-shadow: 0 0.125rem 0.25rem rgba(0, 0, 0, 0.075);
}
</style>
