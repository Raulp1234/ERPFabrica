import { defineStore } from 'pinia';
import apiClient from '../api';

export const useAuthStore = defineStore('auth', {
  state: () => ({
    token: localStorage.getItem('token') || null,
    userId: localStorage.getItem('userId') || null,
    tenantId: localStorage.getItem('tenantId') || null,
    nombreCompleto: localStorage.getItem('nombreCompleto') || null,
    esAdmin: localStorage.getItem('esAdmin') === 'true',
    roles: JSON.parse(localStorage.getItem('roles') || '[]'),
  }),

  getters: {
    isAuthenticated: (state) => !!state.token,
    isAdmin: (state) => state.esAdmin,
    getUserRoles: (state) => state.roles,
  },

  actions: {
    async login(email, password) {
      try {
        const response = await apiClient.post('/auth/login', { email, password });
        const { token, nombreCompleto, tenantId, esAdmin, roles } = response.data;

        this.token = token;
        this.userId = response.data.userId || null;
        this.tenantId = tenantId;
        this.nombreCompleto = nombreCompleto;
        this.esAdmin = esAdmin;
        this.roles = roles || [];

        // Guardar en localStorage
        localStorage.setItem('token', token);
        localStorage.setItem('userId', this.userId || '');
        localStorage.setItem('tenantId', tenantId);
        localStorage.setItem('nombreCompleto', nombreCompleto);
        localStorage.setItem('esAdmin', esAdmin);
        localStorage.setItem('roles', JSON.stringify(this.roles));

        return response.data;
      } catch (error) {
        throw error;
      }
    },

    logout() {
      this.token = null;
      this.userId = null;
      this.tenantId = null;
      this.nombreCompleto = null;
      this.esAdmin = false;
      this.roles = [];

      localStorage.removeItem('token');
      localStorage.removeItem('userId');
      localStorage.removeItem('tenantId');
      localStorage.removeItem('nombreCompleto');
      localStorage.removeItem('esAdmin');
      localStorage.removeItem('roles');
    },

    // Cargar datos desde localStorage (útil al recargar la página)
    loadFromStorage() {
      this.token = localStorage.getItem('token');
      this.userId = localStorage.getItem('userId');
      this.tenantId = localStorage.getItem('tenantId');
      this.nombreCompleto = localStorage.getItem('nombreCompleto');
      this.esAdmin = localStorage.getItem('esAdmin') === 'true';
      this.roles = JSON.parse(localStorage.getItem('roles') || '[]');
    },
  },
});
