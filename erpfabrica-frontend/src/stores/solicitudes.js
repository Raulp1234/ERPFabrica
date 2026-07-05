import { defineStore } from 'pinia';
import apiClient from '@/api';

export const useSolicitudesStore = defineStore('solicitudes', {
  state: () => ({
    solicitudes: [],
    solicitudSeleccionada: null,
    loading: false,
    error: null,
  }),

  actions: {
    async obtenerSolicitudes(filtros = {}) {
      this.loading = true;
      this.error = null;
      
      try {
        const params = new URLSearchParams();
        if (filtros.estado) params.append('estado', filtros.estado);
        if (filtros.pagina) params.append('pagina', filtros.pagina);
        if (filtros.limite) params.append('limite', filtros.limite);
        
        const response = await apiClient.get(`/solicitudes?${params.toString()}`);
        this.solicitudes = response.data.items || response.data;
        return response.data;
      } catch (error) {
        this.error = error.message;
        throw error;
      } finally {
        this.loading = false;
      }
    },

    async obtenerSolicitud(id) {
      this.loading = true;
      this.error = null;
      
      try {
        const response = await apiClient.get(`/solicitudes/${id}`);
        this.solicitudSeleccionada = response.data;
        return response.data;
      } catch (error) {
        this.error = error.message;
        throw error;
      } finally {
        this.loading = false;
      }
    },

    async crearSolicitud(solicitudData) {
      try {
        const response = await apiClient.post('/solicitudes', solicitudData);
        return response.data;
      } catch (error) {
        throw error;
      }
    },

    async actualizarEstado(id, estadoNuevo, comentario = '') {
      try {
        const response = await apiClient.put(`/solicitudes/${id}/estado`, { 
          estadoNuevo, 
          comentario 
        });
        return response.data;
      } catch (error) {
        throw error;
      }
    },

    async generarFactura(id) {
      try {
        const response = await apiClient.post(`/solicitudes/${id}/generar-factura`);
        return response.data;
      } catch (error) {
        throw error;
      }
    },
  },
});
