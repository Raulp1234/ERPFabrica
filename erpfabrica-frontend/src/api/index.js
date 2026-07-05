import axios from 'axios';
import { useAuthStore } from '@/stores/auth';
import router from '@/router';

// Crear instancia de axios con configuración base
const apiClient = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000/api',
  headers: {
    'Content-Type': 'application/json',
  },
});

// Interceptor para agregar el token JWT en cada petición
apiClient.interceptors.request.use(
  (config) => {
    const authStore = useAuthStore();
    const token = authStore.token;
    
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    
    // Agregar tenantId a la URL si no está presente y tenemos uno en el store
    // Solo si la URL no comienza ya con el tenantId
    if (authStore.tenantId && config.url) {
      // Verificar si la URL ya comienza con el tenantId (ej: /abc123/dashboard)
      const yaTieneTenantId = /^\/[a-f0-9-]+(?:\/|$)/i.test(config.url);
      
      if (!yaTieneTenantId) {
        // Agregar tenantId al inicio de la URL
        config.url = `/${authStore.tenantId}${config.url}`;
      }
    }
    
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// Interceptor para manejar respuestas y errores
apiClient.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response) {
      const { status } = error.response;
      
      // Manejar error 401 - No autorizado
      if (status === 401) {
        const authStore = useAuthStore();
        authStore.logout();
        router.push('/login');
      }
      
      // Manejar error 403 - Prohibido
      if (status === 403) {
        console.error('Acceso denegado: No tienes permisos para esta acción');
      }
      
      // Manejar error 404 - No encontrado
      if (status === 404) {
        console.error('Recurso no encontrado');
      }
      
      // Manejar error 500 - Error del servidor
      if (status === 500) {
        console.error('Error del servidor');
      }
    }
    
    return Promise.reject(error);
  }
);

export default apiClient;
