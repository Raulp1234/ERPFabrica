<template>
  <div class="main-layout">
    <!-- Sidebar -->
    <aside 
      class="sidebar" 
      :class="{ 'sidebar-collapsed': sidebarCollapsed }"
      :style="{ '--sidebar-bg': tenantConfigStore.colorSecundario }"
    >
      <!-- Logo y nombre del sistema -->
      <div class="sidebar-header">
        <img 
          v-if="tenantConfigStore.logoUrl" 
          :src="tenantConfigStore.logoUrl" 
          alt="Logo" 
          class="sidebar-logo"
        />
        <h3 class="sidebar-title">{{ tenantConfigStore.nombreSistema }}</h3>
        <button 
          class="btn btn-link sidebar-toggle d-lg-none"
          @click="toggleSidebar"
        >
          <i class="bi bi-x-lg"></i>
        </button>
      </div>

      <!-- Menú de navegación -->
      <nav class="sidebar-nav">
        <ul class="nav flex-column">
          <!-- Dashboard (siempre visible) -->
          <li class="nav-item">
            <router-link 
              :to="`/${tenantId}/dashboard`" 
              class="nav-link"
              :class="{ active: $route.name === 'Dashboard' }"
            >
              <i class="bi bi-speedometer2"></i>
              <span>Dashboard</span>
            </router-link>
          </li>

          <!-- Módulo Inventario -->
          <li 
            v-if="tenantConfigStore.isModuloActivo('Inventario')"
            class="nav-item"
          >
            <a 
              class="nav-link" 
              href="#"
              @click.prevent="toggleSubmenu('inventario')"
              :class="{ active: isInventarioActive }"
            >
              <i class="bi bi-box-seam"></i>
              <span>Inventario</span>
              <i class="bi bi-chevron-down ms-auto" :class="{ 'rotate-180': submenus.inventario }"></i>
            </a>
            <ul v-show="submenus.inventario" class="nav flex-column submenu">
              <li class="nav-item">
                <router-link 
                  :to="`/${tenantId}/inventario/productos`" 
                  class="nav-link"
                >
                  Productos
                </router-link>
              </li>
              <li class="nav-item">
                <router-link 
                  :to="`/${tenantId}/inventario/stock`" 
                  class="nav-link"
                >
                  Stock
                </router-link>
              </li>
              <li class="nav-item">
                <router-link 
                  :to="`/${tenantId}/inventario/movimientos`" 
                  class="nav-link"
                >
                  Movimientos
                </router-link>
              </li>
              <li 
                v-if="tenantConfigStore.manejaMultiplesAlmacenes"
                class="nav-item"
              >
                <router-link 
                  :to="`/${tenantId}/inventario/almacenes`" 
                  class="nav-link"
                >
                  Almacenes
                </router-link>
              </li>
            </ul>
          </li>

          <!-- Módulo Solicitudes/Pedidos -->
          <li 
            v-if="tenantConfigStore.isModuloActivo('Pedidos')"
            class="nav-item"
          >
            <router-link 
              :to="`/${tenantId}/solicitudes`" 
              class="nav-link"
              :class="{ active: isSolicitudesActive }"
            >
              <i class="bi bi-cart3"></i>
              <span>Solicitudes</span>
            </router-link>
          </li>

          <!-- Módulo Facturación -->
          <li 
            v-if="tenantConfigStore.isModuloActivo('Facturacion')"
            class="nav-item"
          >
            <router-link 
              :to="`/${tenantId}/facturas`" 
              class="nav-link"
              :class="{ active: isFacturacionActive }"
            >
              <i class="bi bi-receipt"></i>
              <span>Facturación</span>
            </router-link>
          </li>

          <!-- Módulo Terceros -->
          <li 
            v-if="tenantConfigStore.isModuloActivo('Terceros') || authStore.isAdmin"
            class="nav-item"
          >
            <router-link 
              :to="`/${tenantId}/terceros`" 
              class="nav-link"
              :class="{ active: isTercerosActive }"
            >
              <i class="bi bi-people"></i>
              <span>Terceros</span>
            </router-link>
          </li>

          <!-- Configuración (solo admin) -->
          <li v-if="authStore.isAdmin" class="nav-item mt-auto">
            <router-link 
              :to="`/${tenantId}/configuracion`" 
              class="nav-link"
              :class="{ active: $route.name === 'Configuracion' }"
            >
              <i class="bi bi-gear"></i>
              <span>Configuración</span>
            </router-link>
          </li>

          <!-- Usuarios (solo admin) -->
          <li v-if="authStore.isAdmin" class="nav-item">
            <router-link 
              :to="`/${tenantId}/usuarios`" 
              class="nav-link"
              :class="{ active: $route.name === 'Usuarios' }"
            >
              <i class="bi bi-person-badge"></i>
              <span>Usuarios</span>
            </router-link>
          </li>
        </ul>
      </nav>
    </aside>

    <!-- Overlay para móvil -->
    <div 
      v-if="!sidebarCollapsed" 
      class="sidebar-overlay d-lg-none"
      @click="toggleSidebar"
    ></div>

    <!-- Contenido principal -->
    <div class="main-content">
      <!-- Navbar superior -->
      <header class="navbar">
        <div class="navbar-start">
          <button 
            class="btn btn-link sidebar-toggle d-none d-lg-block"
            @click="toggleSidebar"
          >
            <i class="bi bi-list"></i>
          </button>
          <h4 class="mb-0">{{ pageTitle }}</h4>
        </div>
        
        <div class="navbar-end">
          <!-- Información del usuario -->
          <div class="dropdown">
            <button 
              class="btn btn-link dropdown-toggle user-dropdown"
              type="button"
              data-bs-toggle="dropdown"
            >
              <i class="bi bi-person-circle me-1"></i>
              {{ authStore.nombreCompleto }}
            </button>
            <ul class="dropdown-menu dropdown-menu-end">
              <li class="dropdown-header">
                {{ authStore.roles.join(', ') }}
              </li>
              <li><hr class="dropdown-divider"></li>
              <li>
                <button class="dropdown-item" @click="handleLogout">
                  <i class="bi bi-box-arrow-right me-2"></i>
                  Cerrar Sesión
                </button>
              </li>
            </ul>
          </div>
        </div>
      </header>

      <!-- Área de contenido -->
      <main class="content-area">
        <slot />
      </main>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted, watch } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { useAuthStore } from '../../stores/auth';
import { useTenantConfigStore } from '../../stores/tenant';
import { toast } from 'vue3-toastify';

const route = useRoute();
const router = useRouter();
const authStore = useAuthStore();
const tenantConfigStore = useTenantConfigStore();

const sidebarCollapsed = ref(false);
const submenus = ref({
  inventario: false,
});

const tenantId = computed(() => authStore.tenantId);

// Verificar si algún submenu está activo
const isInventarioActive = computed(() => {
  return route.path.includes('/inventario');
});

const isSolicitudesActive = computed(() => {
  return route.path.includes('/solicitudes');
});

const isFacturacionActive = computed(() => {
  return route.path.includes('/facturas');
});

const isTercerosActive = computed(() => {
  return route.path.includes('/terceros');
});

// Título de la página basado en la ruta
const pageTitle = computed(() => {
  const routeNames = {
    Dashboard: 'Dashboard',
    Productos: 'Productos',
    ProductoDetalle: 'Detalle de Producto',
    Stock: 'Stock',
    Movimientos: 'Movimientos de Inventario',
    Almacenes: 'Almacenes',
    Solicitudes: 'Solicitudes',
    SolicitudDetalle: 'Detalle de Solicitud',
    SolicitudNueva: 'Nueva Solicitud',
    Facturas: 'Facturas',
    FacturaDetalle: 'Detalle de Factura',
    FacturaNueva: 'Nueva Factura',
    Terceros: 'Terceros',
    TerceroDetalle: 'Detalle de Tercero',
    Configuracion: 'Configuración',
    Usuarios: 'Usuarios',
  };
  return routeNames[route.name] || 'ERPFabrica';
});

const toggleSidebar = () => {
  sidebarCollapsed.value = !sidebarCollapsed.value;
};

const toggleSubmenu = (menu) => {
  submenus.value[menu] = !submenus.value[menu];
};

const handleLogout = () => {
  authStore.logout();
  toast.info('Sesión cerrada correctamente');
  router.push('/login');
};

// Cargar configuración del tenant al montar el componente
onMounted(async () => {
  if (tenantId.value && !tenantConfigStore.config) {
    try {
      await tenantConfigStore.cargarConfig(tenantId.value);
    } catch (error) {
      console.error('Error al cargar configuración del tenant:', error);
    }
  }
});

// Watch para cambios en la ruta (cerrar sidebar en móvil)
watch(route, () => {
  if (window.innerWidth < 992) {
    sidebarCollapsed.value = true;
  }
});
</script>

<style scoped>
.main-layout {
  display: flex;
  min-height: 100vh;
}

/* Sidebar */
.sidebar {
  width: 260px;
  background-color: var(--sidebar-bg, #2c3e50);
  color: white;
  transition: all 0.3s ease;
  position: fixed;
  height: 100vh;
  overflow-y: auto;
  z-index: 1000;
}

.sidebar-collapsed {
  margin-left: -260px;
}

.sidebar-header {
  padding: 1.5rem;
  border-bottom: 1px solid rgba(255, 255, 255, 0.1);
  display: flex;
  align-items: center;
  gap: 0.75rem;
}

.sidebar-logo {
  height: 40px;
  width: auto;
}

.sidebar-title {
  margin: 0;
  font-size: 1.25rem;
  font-weight: 600;
}

.sidebar-toggle {
  color: white;
  padding: 0.25rem;
}

.sidebar-toggle:hover {
  color: rgba(255, 255, 255, 0.75);
}

/* Navegación */
.sidebar-nav {
  padding: 1rem 0;
}

.nav-link {
  color: rgba(255, 255, 255, 0.8);
  padding: 0.75rem 1.5rem;
  display: flex;
  align-items: center;
  gap: 0.75rem;
  text-decoration: none;
  transition: all 0.2s ease;
}

.nav-link:hover {
  color: white;
  background-color: rgba(255, 255, 255, 0.1);
}

.nav-link.active {
  color: white;
  background-color: var(--color-primario, #3498db);
}

.nav-link i.bi-chevron-down {
  font-size: 0.75rem;
  transition: transform 0.2s ease;
}

.rotate-180 {
  transform: rotate(180deg);
}

/* Submenú */
.submenu {
  background-color: rgba(0, 0, 0, 0.2);
  padding-left: 0;
}

.submenu .nav-link {
  padding-left: 3rem;
  font-size: 0.9rem;
}

/* Overlay para móvil */
.sidebar-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background-color: rgba(0, 0, 0, 0.5);
  z-index: 999;
}

/* Contenido principal */
.main-content {
  flex: 1;
  margin-left: 260px;
  transition: margin-left 0.3s ease;
  display: flex;
  flex-direction: column;
}

.sidebar-collapsed + .main-content {
  margin-left: 0;
}

/* Navbar */
.navbar {
  background-color: white;
  border-bottom: 1px solid #dee2e6;
  padding: 1rem 1.5rem;
  display: flex;
  justify-content: space-between;
  align-items: center;
  position: sticky;
  top: 0;
  z-index: 100;
}

.navbar-start,
.navbar-end {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.user-dropdown {
  color: #333;
  text-decoration: none;
}

.user-dropdown:hover {
  color: var(--color-primario, #3498db);
}

/* Área de contenido */
.content-area {
  flex: 1;
  padding: 1.5rem;
  background-color: #f8f9fa;
}

/* Responsive */
@media (max-width: 991.98px) {
  .sidebar {
    margin-left: -260px;
  }
  
  .sidebar:not(.sidebar-collapsed) {
    margin-left: 0;
  }
  
  .main-content {
    margin-left: 0;
  }
}
</style>
