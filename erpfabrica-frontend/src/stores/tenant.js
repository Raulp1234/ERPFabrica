import { defineStore } from 'pinia';
import apiClient from '../api';

export const useTenantConfigStore = defineStore('tenantConfig', {
  state: () => ({
    config: null,
    loading: false,
    error: null,
  }),

  getters: {
    colorPrimario: (state) => state.config?.colorPrimario || '#3498db',
    colorSecundario: (state) => state.config?.colorSecundario || '#2c3e50',
    logoUrl: (state) => state.config?.logoUrl || '',
    nombreSistema: (state) => state.config?.nombreSistema || 'ERPFabrica',
    monedaPorDefecto: (state) => state.config?.monedaPorDefecto || 'USD',
    impuestoPorDefecto: (state) => state.config?.impuestoPorDefecto || 0.16,
    formatoFactura: (state) => state.config?.formatoFactura || 'standard',
    manejaMultiplesAlmacenes: (state) => state.config?.manejaMultiplesAlmacenes || false,
    controlStockEstricto: (state) => state.config?.controlStockEstricto || false,
    permiteVentaSinStock: (state) => state.config?.permiteVentaSinStock || false,
    moduloActivos: (state) => state.config?.moduloActivos || [],
    
    // Verificar si un módulo está activo
    isModuloActivo: (state) => (moduloNombre) => {
      return state.config?.moduloActivos?.includes(moduloNombre) || false;
    },
  },

  actions: {
    async cargarConfig(tenantId) {
      this.loading = true;
      this.error = null;
      
      try {
    /*    const response = await apiClient.get(`/${tenantId}/config`);   */
          const response = await apiClient.get(`/config`);  
        this.config = response.data;
        
        // Aplicar variables CSS dinámicas
        this.aplicarEstilosDinamicos();
        
        return response.data;
      } catch (error) {
        this.error = error.message;
        throw error;
      } finally {
        this.loading = false;
      }
    },

    async actualizarConfig(configData) {
      this.loading = true;
      this.error = null;
      
      try {
        /* const response = await apiClient.put(`/${this.config.tenantId}/config`, configData); */
        const response = await apiClient.put(`/config`, configData);
        this.config = response.data;
        
        // Aplicar nuevos estilos
        this.aplicarEstilosDinamicos();
        
        return response.data;
      } catch (error) {
        this.error = error.message;
        throw error;
      } finally {
        this.loading = false;
      }
    },

    aplicarEstilosDinamicos() {
      if (!this.config) return;
      
      const root = document.documentElement;
      
      // Establecer variables CSS para colores
      root.style.setProperty('--color-primario', this.colorPrimario);
      root.style.setProperty('--color-secundario', this.colorSecundario);
      
      // Actualizar título de la página
      document.title = this.nombreSistema;
    },
  },
});
