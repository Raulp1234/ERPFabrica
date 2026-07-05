import { defineStore } from 'pinia';
import apiClient from '@/api';

export const useInventarioStore = defineStore('inventario', {
  state: () => ({
    productos: [],
    productoSeleccionado: null,
    almacenes: [],
    stock: [],
    movimientos: [],
    categorias: [],
    loading: false,
    error: null,
    pagination: {
      pagina: 1,
      limite: 10,
      total: 0,
    },
  }),

  actions: {
    // Productos
    async obtenerProductos(filtros = {}) {
      this.loading = true;
      this.error = null;
      
      try {
        const params = new URLSearchParams();
        if (filtros.pagina) params.append('pagina', filtros.pagina);
        if (filtros.limite) params.append('limite', filtros.limite);
        if (filtros.busqueda) params.append('busqueda', filtros.busqueda);
        if (filtros.categoriaId) params.append('categoriaId', filtros.categoriaId);
        
        const response = await apiClient.get(`/productos?${params.toString()}`);
        this.productos = response.data.items || response.data;
        this.pagination.total = response.data.total || response.data.length;
        
        return response.data;
      } catch (error) {
        this.error = error.message;
        throw error;
      } finally {
        this.loading = false;
      }
    },

    async obtenerProducto(id) {
      this.loading = true;
      this.error = null;
      
      try {
        const response = await apiClient.get(`/productos/${id}`);
        this.productoSeleccionado = response.data;
        return response.data;
      } catch (error) {
        this.error = error.message;
        throw error;
      } finally {
        this.loading = false;
      }
    },

    async crearProducto(productoData) {
      try {
        const response = await apiClient.post('/productos', productoData);
        return response.data;
      } catch (error) {
        throw error;
      }
    },

    async actualizarProducto(id, productoData) {
      try {
        const response = await apiClient.put(`/productos/${id}`, productoData);
        return response.data;
      } catch (error) {
        throw error;
      }
    },

    async eliminarProducto(id) {
      try {
        await apiClient.delete(`/productos/${id}`);
      } catch (error) {
        throw error;
      }
    },

    // Almacenes
    async obtenerAlmacenes() {
      this.loading = true;
      this.error = null;
      
      try {
        const response = await apiClient.get('/almacenes');
        this.almacenes = response.data;
        return response.data;
      } catch (error) {
        this.error = error.message;
        throw error;
      } finally {
        this.loading = false;
      }
    },

    async crearAlmacen(almacenData) {
      try {
        const response = await apiClient.post('/almacenes', almacenData);
        return response.data;
      } catch (error) {
        throw error;
      }
    },

    async actualizarAlmacen(id, almacenData) {
      try {
        const response = await apiClient.put(`/almacenes/${id}`, almacenData);
        return response.data;
      } catch (error) {
        throw error;
      }
    },

    async eliminarAlmacen(id) {
      try {
        await apiClient.delete(`/almacenes/${id}`);
      } catch (error) {
        throw error;
      }
    },

    // Stock
    async obtenerStock(filtros = {}) {
      this.loading = true;
      this.error = null;
      
      try {
        const params = new URLSearchParams();
        if (filtros.productoId) params.append('productoId', filtros.productoId);
        if (filtros.almacenId) params.append('almacenId', filtros.almacenId);
        
        const response = await apiClient.get(`/stock?${params.toString()}`);
        this.stock = response.data;
        return response.data;
      } catch (error) {
        this.error = error.message;
        throw error;
      } finally {
        this.loading = false;
      }
    },

    // Movimientos
    async obtenerMovimientos(filtros = {}) {
      this.loading = true;
      this.error = null;
      
      try {
        const params = new URLSearchParams();
        if (filtros.productoId) params.append('productoId', filtros.productoId);
        if (filtros.almacenId) params.append('almacenId', filtros.almacenId);
        
        const response = await apiClient.get(`/movimientos-inventario?${params.toString()}`);
        this.movimientos = response.data;
        return response.data;
      } catch (error) {
        this.error = error.message;
        throw error;
      } finally {
        this.loading = false;
      }
    },

    async registrarMovimiento(movimientoData) {
      try {
        const response = await apiClient.post('/movimientos-inventario', movimientoData);
        return response.data;
      } catch (error) {
        throw error;
      }
    },

    // Categorías
    async obtenerCategorias(tipo = 'producto') {
      this.loading = true;
      this.error = null;
      
      try {
        const response = await apiClient.get(`/categorias/${tipo}`);
        if (tipo === 'producto') {
          this.categorias = response.data;
        }
        return response.data;
      } catch (error) {
        this.error = error.message;
        throw error;
      } finally {
        this.loading = false;
      }
    },

    async crearCategoria(tipo, categoriaData) {
      try {
        const response = await apiClient.post(`/categorias/${tipo}`, categoriaData);
        return response.data;
      } catch (error) {
        throw error;
      }
    },

    async actualizarCategoria(tipo, id, categoriaData) {
      try {
        const response = await apiClient.put(`/categorias/${tipo}/${id}`, categoriaData);
        return response.data;
      } catch (error) {
        throw error;
      }
    },

    async eliminarCategoria(tipo, id) {
      try {
        await apiClient.delete(`/categorias/${tipo}/${id}`);
      } catch (error) {
        throw error;
      }
    },
  },
});
