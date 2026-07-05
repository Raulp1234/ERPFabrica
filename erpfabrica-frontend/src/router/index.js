import { createRouter, createWebHistory } from 'vue-router';
import { useAuthStore } from '@/stores/auth';

const routes = [
  // Ruta de login (pública)
  {
    path: '/login',
    name: 'Login',
    component: () => import('@/modules/auth/LoginView.vue'),
    meta: { requiresAuth: false },
  },
  // Rutas protegidas con tenantId dinámico
  {
    path: '/:tenantId',
    redirect: to => `/${to.params.tenantId}/dashboard`,
  },
  {
    path: '/:tenantId/dashboard',
    name: 'Dashboard',
    component: () => import('@/modules/dashboard/DashboardView.vue'),
    meta: { requiresAuth: true, modulo: 'Dashboard' },
  },
  // Inventario
  {
    path: '/:tenantId/inventario/productos',
    name: 'Productos',
    component: () => import('@/modules/inventario/ProductosView.vue'),
    meta: { requiresAuth: true, modulo: 'Inventario' },
  },
  {
    path: '/:tenantId/inventario/productos/:id',
    name: 'ProductoDetalle',
    component: () => import('@/modules/inventario/ProductoDetalleView.vue'),
    meta: { requiresAuth: true, modulo: 'Inventario' },
  },
  {
    path: '/:tenantId/inventario/stock',
    name: 'Stock',
    component: () => import('@/modules/inventario/StockView.vue'),
    meta: { requiresAuth: true, modulo: 'Inventario' },
  },
  {
    path: '/:tenantId/inventario/movimientos',
    name: 'Movimientos',
    component: () => import('@/modules/inventario/MovimientosView.vue'),
    meta: { requiresAuth: true, modulo: 'Inventario' },
  },
  {
    path: '/:tenantId/inventario/almacenes',
    name: 'Almacenes',
    component: () => import('@/modules/inventario/AlmacenesView.vue'),
    meta: { requiresAuth: true, modulo: 'Inventario' },
  },
  // Solicitudes
  {
    path: '/:tenantId/solicitudes',
    name: 'Solicitudes',
    component: () => import('@/modules/solicitudes/SolicitudesView.vue'),
    meta: { requiresAuth: true, modulo: 'Pedidos' },
  },
  {
    path: '/:tenantId/solicitudes/:id',
    name: 'SolicitudDetalle',
    component: () => import('@/modules/solicitudes/SolicitudDetalleView.vue'),
    meta: { requiresAuth: true, modulo: 'Pedidos' },
  },
  {
    path: '/:tenantId/solicitudes/nueva',
    name: 'SolicitudNueva',
    component: () => import('@/modules/solicitudes/SolicitudFormView.vue'),
    meta: { requiresAuth: true, modulo: 'Pedidos' },
  },
  // Facturación
  {
    path: '/:tenantId/facturas',
    name: 'Facturas',
    component: () => import('@/modules/facturacion/FacturasView.vue'),
    meta: { requiresAuth: true, modulo: 'Facturacion' },
  },
  {
    path: '/:tenantId/facturas/:id',
    name: 'FacturaDetalle',
    component: () => import('@/modules/facturacion/FacturaDetalleView.vue'),
    meta: { requiresAuth: true, modulo: 'Facturacion' },
  },
  {
    path: '/:tenantId/facturas/nueva',
    name: 'FacturaNueva',
    component: () => import('@/modules/facturacion/FacturaFormView.vue'),
    meta: { requiresAuth: true, modulo: 'Facturacion' },
  },
  // Terceros
  {
    path: '/:tenantId/terceros',
    name: 'Terceros',
    component: () => import('@/modules/terceros/TercerosView.vue'),
    meta: { requiresAuth: true, modulo: 'Terceros' },
  },
  {
    path: '/:tenantId/terceros/:id',
    name: 'TerceroDetalle',
    component: () => import('@/modules/terceros/TerceroDetalleView.vue'),
    meta: { requiresAuth: true, modulo: 'Terceros' },
  },
  // Configuración
  {
    path: '/:tenantId/configuracion',
    name: 'Configuracion',
    component: () => import('@/modules/configuracion/ConfiguracionView.vue'),
    meta: { requiresAuth: true, requiereAdmin: true },
  },
  // Usuarios y Roles (solo admin)
  {
    path: '/:tenantId/usuarios',
    name: 'Usuarios',
    component: () => import('@/modules/admin/UsuariosView.vue'),
    meta: { requiresAuth: true, requiereAdmin: true },
  },
];

const router = createRouter({
  history: createWebHistory(),
  routes,
});

// Guard de navegación para proteger rutas
router.beforeEach(async (to, from, next) => {
  const authStore = useAuthStore();
  
  // Cargar datos desde localStorage si existen
  if (!authStore.token && localStorage.getItem('token')) {
    authStore.loadFromStorage();
  }
  
  // Verificar si la ruta requiere autenticación
  if (to.meta.requiresAuth !== false) {
    if (!authStore.isAuthenticated) {
      // No autenticado, redirigir a login
      return next('/login');
    }
    
    // Verificar si el tenantId coincide
    if (to.params.tenantId && to.params.tenantId !== authStore.tenantId) {
      // Tenant diferente, redirigir al dashboard del tenant correcto
      return next(`/${authStore.tenantId}/dashboard`);
    }
    
    // Verificar si requiere rol de admin
    if (to.meta.requiereAdmin && !authStore.isAdmin) {
      // No es admin, redirigir al dashboard
      return next(`/${authStore.tenantId}/dashboard`);
    }
  }
  
  // Si estamos en login y ya estamos autenticados, redirigir al dashboard
  if (to.path === '/login' && authStore.isAuthenticated) {
    return next(`/${authStore.tenantId}/dashboard`);
  }
  
  next();
});

export default router;
