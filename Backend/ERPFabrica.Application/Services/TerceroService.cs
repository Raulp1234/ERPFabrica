using Microsoft.EntityFrameworkCore;
using ERPFabrica.Application.DTOs;
using ERPFabrica.Application.Exceptions;
using ERPFabrica.Application.Interfaces;
using ERPFabrica.Domain.Entities;
using ERPFabrica.Domain.Enums;

namespace ERPFabrica.Application.Services
{
    public class TerceroService : ITerceroService
    {
        private readonly IApplicationDbContext _context;
        private readonly ITenantProvider _tenantProvider;

        public TerceroService(IApplicationDbContext context, ITenantProvider tenantProvider)
        {
            _context = context;
            _tenantProvider = tenantProvider;
        }

        private void ValidarTenant(int tenantId)
        {
            if (tenantId != _tenantProvider.TenantId)
                throw new NegocioException("Tenant inválido.");
        }

        public async Task<List<TerceroDto>> ObtenerTercerosAsync(int tenantId)
        {
            ValidarTenant(tenantId);
            return await _context.Terceros
                .Include(t => t.Categoria)
                .Select(t => MapToDto(t))
                .ToListAsync();
        }

        public async Task<TerceroDto?> ObtenerTerceroAsync(int tenantId, int id)
        {
            ValidarTenant(tenantId);
            var tercero = await _context.Terceros.Include(t => t.Categoria).FirstOrDefaultAsync(t => t.Id == id);
            return tercero == null ? null : MapToDto(tercero);
        }

        public async Task<TerceroDto> CrearTerceroAsync(int tenantId, CrearTerceroDto dto)
        {
            ValidarTenant(tenantId);
            var tercero = new Tercero
            {
                TenantId = tenantId,
                Codigo = dto.Codigo,
                Nombre = dto.Nombre,
                Tipo = Enum.Parse<TipoTercero>(dto.Tipo),
                Email = dto.Email,
                Telefono = dto.Telefono,
                Direccion = dto.Direccion,
                DocumentoIdentidad = dto.DocumentoIdentidad,
                CategoriaTerceroId = dto.CategoriaTerceroId,
                SaldoPendiente = 0,
                EsActivo = true
            };
            _context.Terceros.Add(tercero);
            await _context.SaveChangesAsync();
            return MapToDto(tercero);
        }

        public async Task<TerceroDto> ActualizarTerceroAsync(int tenantId, int id, ActualizarTerceroDto dto)
        {
            ValidarTenant(tenantId);
            var tercero = await _context.Terceros.FindAsync(id)
                ?? throw new NegocioException("Tercero no encontrado.");
            tercero.Codigo = dto.Codigo;
            tercero.Nombre = dto.Nombre;
            tercero.Tipo = Enum.Parse<TipoTercero>(dto.Tipo);
            tercero.Email = dto.Email;
            tercero.Telefono = dto.Telefono;
            tercero.Direccion = dto.Direccion;
            tercero.DocumentoIdentidad = dto.DocumentoIdentidad;
            tercero.CategoriaTerceroId = dto.CategoriaTerceroId;
            await _context.SaveChangesAsync();
            return MapToDto(tercero);
        }

        public async Task EliminarTerceroAsync(int tenantId, int id)
        {
            ValidarTenant(tenantId);
            var tercero = await _context.Terceros.FindAsync(id)
                ?? throw new NegocioException("Tercero no encontrado.");
            tercero.IsDeleted = true; // soft delete
            await _context.SaveChangesAsync();
        }

        // ----------- Categorías de terceros -----------
        public async Task<List<CategoriaDto>> ObtenerCategoriasAsync(int tenantId)
        {
            ValidarTenant(tenantId);
            return await _context.CategoriaTerceros
                .Select(c => new CategoriaDto { Id = c.Id, Nombre = c.Nombre })
                .ToListAsync();
        }

        public async Task<CategoriaDto> CrearCategoriaAsync(int tenantId, CrearCategoriaDto dto)
        {
            ValidarTenant(tenantId);
            var cat = new CategoriaTercero { TenantId = tenantId, Nombre = dto.Nombre, Descripcion = dto.Descripcion ?? "" };
            _context.CategoriaTerceros.Add(cat);
            await _context.SaveChangesAsync();
            return new CategoriaDto { Id = cat.Id, Nombre = cat.Nombre };
        }

        public async Task<CategoriaDto> ActualizarCategoriaAsync(int tenantId, int id, ActualizarCategoriaDto dto)
        {
            ValidarTenant(tenantId);
            var cat = await _context.CategoriaTerceros.FindAsync(id) ?? throw new NegocioException("Categoría no encontrada.");
            cat.Nombre = dto.Nombre;
            cat.Descripcion = dto.Descripcion ?? cat.Descripcion;
            await _context.SaveChangesAsync();
            return new CategoriaDto { Id = cat.Id, Nombre = cat.Nombre };
        }

        public async Task EliminarCategoriaAsync(int tenantId, int id)
        {
            ValidarTenant(tenantId);
            var cat = await _context.CategoriaTerceros.FindAsync(id) ?? throw new NegocioException("Categoría no encontrada.");
            _context.CategoriaTerceros.Remove(cat);
            await _context.SaveChangesAsync();
        }

        // ----------- Mapeo -----------
        private static TerceroDto MapToDto(Tercero t) => new()
        {
            Id = t.Id,
            Codigo = t.Codigo,
            Nombre = t.Nombre,
            Tipo = t.Tipo.ToString(),
            Email = t.Email,
            Telefono = t.Telefono,
            Direccion = t.Direccion,
            DocumentoIdentidad = t.DocumentoIdentidad,
            CategoriaTerceroId = t.CategoriaTerceroId,
            CategoriaNombre = t.Categoria?.Nombre ?? "",
            SaldoPendiente = t.SaldoPendiente,
            EsActivo = t.EsActivo
        };
    }
}