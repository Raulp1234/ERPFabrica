// ProductoService.cs
using ERPFabrica.Application.DTOs;
using ERPFabrica.Application.Exceptions;
using ERPFabrica.Application.Interfaces;
using ERPFabrica.Domain.Entities;
using ERPFabrica.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace ERPFabrica.Application.Services
{
    public class ProductoService : IProductoService
    {
        private readonly IApplicationDbContext _context;
        private readonly ITenantService _tenantService;

        public ProductoService(IApplicationDbContext context, ITenantService tenantService)
        {
            _context = context;
            _tenantService = tenantService;
        }

        public async Task<List<ProductoDto>> GetAllAsync()
        {
            // El filtro global ya aplica TenantId y soft delete
            return await _context.Productos
                .Select(p => new ProductoDto
                {
                    Id = p.Id,
                    CodigoSKU = p.CodigoSKU,
                    Nombre = p.Nombre,
                    Descripcion = p.Descripcion,
                    PrecioCosto = p.PrecioCosto,
                    PrecioVenta = p.PrecioVenta,
                    TipoProducto = p.TipoProducto.ToString(),
                    UnidadMedida = p.UnidadMedida,
                    CategoriaProductoId = p.CategoriaProductoId,
                    EsActivo = p.EsActivo
                })
                .ToListAsync();
        }

        public async Task<ProductoDto?> GetByIdAsync(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null) return null;

            return new ProductoDto
            {
                Id = producto.Id,
                CodigoSKU = producto.CodigoSKU,
                Nombre = producto.Nombre,
                Descripcion = producto.Descripcion,
                PrecioCosto = producto.PrecioCosto,
                PrecioVenta = producto.PrecioVenta,
                TipoProducto = producto.TipoProducto.ToString(),
                UnidadMedida = producto.UnidadMedida,
                CategoriaProductoId = producto.CategoriaProductoId,
                EsActivo = producto.EsActivo
            };
        }

        public async Task<ProductoDto> CreateAsync(CrearProductoDto dto)
        {
            var producto = new Producto
            {
                CodigoSKU = dto.CodigoSKU,
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion,
                PrecioCosto = dto.PrecioCosto,
                PrecioVenta = dto.PrecioVenta,
                TipoProducto = Enum.Parse<TipoProducto>(dto.TipoProducto),
                UnidadMedida = dto.UnidadMedida,
                CategoriaProductoId = dto.CategoriaProductoId,
                EsActivo = true,
                FechaCreacion = DateTime.UtcNow
            };

            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();

            return new ProductoDto
            {
                Id = producto.Id,
                CodigoSKU = producto.CodigoSKU,
                Nombre = producto.Nombre,
                Descripcion = producto.Descripcion,
                PrecioCosto = producto.PrecioCosto,
                PrecioVenta = producto.PrecioVenta,
                TipoProducto = producto.TipoProducto.ToString(),
                UnidadMedida = producto.UnidadMedida,
                CategoriaProductoId = producto.CategoriaProductoId,
                EsActivo = producto.EsActivo
            };
        }

        public async Task UpdateAsync(int id, CrearProductoDto dto)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null) throw new KeyNotFoundException("Producto no encontrado.");

            producto.CodigoSKU = dto.CodigoSKU;
            producto.Nombre = dto.Nombre;
            producto.Descripcion = dto.Descripcion;
            producto.PrecioCosto = dto.PrecioCosto;
            producto.PrecioVenta = dto.PrecioVenta;
            producto.TipoProducto = Enum.Parse<Domain.Enums.TipoProducto>(dto.TipoProducto);
            producto.UnidadMedida = dto.UnidadMedida;
            producto.CategoriaProductoId = dto.CategoriaProductoId;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null) throw new KeyNotFoundException("Producto no encontrado.");

            // Soft delete: establece la bandera, el filtro global lo ocultará
            producto.IsDeleted = true;
            await _context.SaveChangesAsync();
        }


        public async Task<List<CategoriaDto>> ObtenerCategoriasAsync(int tenantId)
        {
            _tenantService.ValidarTenant(tenantId);
            return await _context.CategoriaProductos
                .Select(c => new CategoriaDto { Id = c.Id, Nombre = c.Nombre })
                .ToListAsync();
        }

        public async Task<CategoriaDto> CrearCategoriaAsync(int tenantId, CrearCategoriaDto dto)
        {
            _tenantService.ValidarTenant(tenantId);
            var cat = new CategoriaProducto { TenantId = tenantId, Nombre = dto.Nombre, CategoriaPadreId = dto.CategoriaPadreId };
            _context.CategoriaProductos.Add(cat);
            await _context.SaveChangesAsync();
            return new CategoriaDto { Id = cat.Id, Nombre = cat.Nombre };
        }

        public async Task<CategoriaDto> ActualizarCategoriaAsync(int tenantId, int id, ActualizarCategoriaDto dto)
        {
            _tenantService.ValidarTenant(tenantId);
            var cat = await _context.CategoriaProductos.FindAsync(id) ?? throw new NegocioException("Categoría no encontrada.");
            cat.Nombre = dto.Nombre;
            cat.CategoriaPadreId = dto.CategoriaPadreId;
            await _context.SaveChangesAsync();
            return new CategoriaDto { Id = cat.Id, Nombre = cat.Nombre };
        }

        public async Task EliminarCategoriaAsync(int tenantId, int id)
        {
            _tenantService.ValidarTenant(tenantId);
            var cat = await _context.CategoriaProductos.FindAsync(id) ?? throw new NegocioException("Categoría no encontrada.");
            // Verificar que no tenga productos asociados
            if (await _context.Productos.AnyAsync(p => p.CategoriaProductoId == id))
                throw new NegocioException("No se puede eliminar la categoría porque tiene productos asociados.");
            _context.CategoriaProductos.Remove(cat);
            await _context.SaveChangesAsync();
        }
    }
}