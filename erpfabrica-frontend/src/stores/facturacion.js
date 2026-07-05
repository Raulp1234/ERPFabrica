import { defineStore } from 'pinia';
import apiClient from '../api';

export const useFacturacionStore = defineStore('facturacion', {
  state: () => ({
    facturas: [],
    facturaSeleccionada: null,
    loading: false,
    error: null,
  }),

  actions: {
    async obtenerFacturas(filtros = {}) {
      this.loading = true;
      this.error = null;
      
      try {
        const params = new URLSearchParams();
        if (filtros.estado) params.append('estado', filtros.estado);
        if (filtros.pagina) params.append('pagina', filtros.pagina);
        if (filtros.limite) params.append('limite', filtros.limite);
        
        const response = await apiClient.get(`/facturas?${params.toString()}`);
        this.facturas = response.data.items || response.data;
        return response.data;
      } catch (error) {
        this.error = error.message;
        throw error;
      } finally {
        this.loading = false;
      }
    },

    async obtenerFactura(id) {
      this.loading = true;
      this.error = null;
      
      try {
        const response = await apiClient.get(`/facturas/${id}`);
        this.facturaSeleccionada = response.data;
        return response.data;
      } catch (error) {
        this.error = error.message;
        throw error;
      } finally {
        this.loading = false;
      }
    },

    async crearFactura(facturaData) {
      try {
        const response = await apiClient.post('/facturas', facturaData);
        return response.data;
      } catch (error) {
        throw error;
      }
    },

    async emitirFactura(id) {
      try {
        const response = await apiClient.put(`/facturas/${id}/emitir`);
        return response.data;
      } catch (error) {
        throw error;
      }
    },

    async registrarPago(id, pagoData) {
      try {
        const response = await apiClient.post(`/facturas/${id}/pagos`, pagoData);
        return response.data;
      } catch (error) {
        throw error;
      }
    },

    async anularFactura(id) {
      try {
        const response = await apiClient.put(`/facturas/${id}/anular`);
        return response.data;
      } catch (error) {
        throw error;
      }
    },

    async obtenerSaldoPendiente(terceroId) {
      try {
        const response = await apiClient.get(`/terceros/${terceroId}/saldo-pendiente`);
        return response.data;
      } catch (error) {
        throw error;
      }
    },
  },
});
