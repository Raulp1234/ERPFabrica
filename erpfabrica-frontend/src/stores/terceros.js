import { defineStore } from 'pinia';
import apiClient from '@/api';

export const useTercerosStore = defineStore('terceros', {
  state: () => ({
    terceros: [],
    terceroSeleccionado: null,
    categorias: [],
    loading: false,
    error: null,
  }),

  actions: {
    async obtenerTerceros(filtros = {}) {
      this.loading = true;
      this.error = null;
      
      try {
        const params = new URLSearchParams();
        if (filtros.tipo) params.append('tipo', filtros.tipo);
        if (filtros.categoriaId) params.append('categoriaId', filtros.categoriaId);
        if (filtros.pagina) params.append('pagina', filtros.pagina);
        if (filtros.limite) params.append('limite', filtros.limite);
        
        const response = await apiClient.get(`/terceros?${params.toString()}`);
        this.terceros = response.data.items || response.data;
        return response.data;
      } catch (error) {
        this.error = error.message;
        throw error;
      } finally {
        this.loading = false;
      }
    },

    async obtenerTercero(id) {
      this.loading = true;
      this.error = null;
      
      try {
        const response = await apiClient.get(`/terceros/${id}`);
        this.terceroSeleccionado = response.data;
        return response.data;
      } catch (error) {
        this.error = error.message;
        throw error;
      } finally {
        this.loading = false;
      }
    },

    async crearTercero(terceroData) {
      try {
        const response = await apiClient.post('/terceros', terceroData);
        return response.data;
      } catch (error) {
        throw error;
      }
    },

    async actualizarTercero(id, terceroData) {
      try {
        const response = await apiClient.put(`/terceros/${id}`, terceroData);
        return response.data;
      } catch (error) {
        throw error;
      }
    },

    async eliminarTercero(id) {
      try {
        await apiClient.delete(`/terceros/${id}`);
      } catch (error) {
        throw error;
      }
    },

    // Categorías de terceros
    async obtenerCategoriasTerceros() {
      this.loading = true;
      this.error = null;
      
      try {
        const response = await apiClient.get('/categorias/tercero');
        this.categorias = response.data;
        return response.data;
      } catch (error) {
        this.error = error.message;
        throw error;
      } finally {
        this.loading = false;
      }
    },

    async crearCategoriaTercero(categoriaData) {
      try {
        const response = await apiClient.post('/categorias/tercero', categoriaData);
        return response.data;
      } catch (error) {
        throw error;
      }
    },
  },
});
