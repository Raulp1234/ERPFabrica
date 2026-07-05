import { defineStore } from 'pinia';
import apiClient from 'src/api';

export const useCamposExtraStore = defineStore('camposExtra', {
  state: () => ({
    definiciones: {}, // Por entidad: { Producto: [], Tercero: [], Factura: [] }
    valores: {}, // Por registro: { productoId: [], terceroId: [] }
    loading: false,
    error: null,
  }),

  actions: {
    async obtenerDefiniciones(entidad) {
      this.loading = true;
      this.error = null;
      
      try {
        const response = await apiClient.get(`/campos-extra/definiciones/${entidad}`);
        this.definiciones[entidad] = response.data;
        return response.data;
      } catch (error) {
        this.error = error.message;
        throw error;
      } finally {
        this.loading = false;
      }
    },

    async crearDefinicion(entidad, definicionData) {
      try {
        const response = await apiClient.post('/campos-extra/definiciones', {
          entidad,
          ...definicionData,
        });
        
        // Actualizar cache local
        if (!this.definiciones[entidad]) {
          this.definiciones[entidad] = [];
        }
        this.definiciones[entidad].push(response.data);
        
        return response.data;
      } catch (error) {
        throw error;
      }
    },

    async actualizarDefinicion(id, definicionData) {
      try {
        const response = await apiClient.put(`/campos-extra/definiciones/${id}`, definicionData);
        
        // Actualizar cache local (buscar y reemplazar)
        for (const entidad in this.definiciones) {
          const index = this.definiciones[entidad].findIndex(d => d.id === id);
          if (index !== -1) {
            this.definiciones[entidad][index] = response.data;
            break;
          }
        }
        
        return response.data;
      } catch (error) {
        throw error;
      }
    },

    async eliminarDefinicion(id) {
      try {
        await apiClient.delete(`/campos-extra/definiciones/${id}`);
        
        // Eliminar de cache local
        for (const entidad in this.definiciones) {
          this.definiciones[entidad] = this.definiciones[entidad].filter(d => d.id !== id);
        }
      } catch (error) {
        throw error;
      }
    },

    async obtenerValores(entidad, registroId) {
      this.loading = true;
      this.error = null;
      
      try {
        const response = await apiClient.get(`/campos-extra/valores/${entidad}/${registroId}`);
        const key = `${entidad}_${registroId}`;
        this.valores[key] = response.data;
        return response.data;
      } catch (error) {
        this.error = error.message;
        throw error;
      } finally {
        this.loading = false;
      }
    },

    async guardarValores(entidad, registroId, valores) {
      try {
        const response = await apiClient.post(`/campos-extra/valores/${entidad}/${registroId}`, valores);
        
        // Actualizar cache local
        const key = `${entidad}_${registroId}`;
        this.valores[key] = response.data;
        
        return response.data;
      } catch (error) {
        throw error;
      }
    },

    // Helper para obtener definiciones de una entidad desde el cache
    getDefinicionesPorEntidad(entidad) {
      return this.definiciones[entidad] || [];
    },

    // Helper para obtener valores de un registro desde el cache
    getValoresPorRegistro(entidad, registroId) {
      const key = `${entidad}_${registroId}`;
      return this.valores[key] || [];
    },
  },
});
