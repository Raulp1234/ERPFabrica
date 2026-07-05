using Microsoft.EntityFrameworkCore;
using ERPFabrica.Application.DTOs;
using ERPFabrica.Application.Exceptions;
using ERPFabrica.Application.Interfaces;
using ERPFabrica.Domain.Entities;

namespace ERPFabrica.Application.Services
{
    public class TenantService : ITenantService
    {
        private readonly IApplicationDbContext _context;
        private readonly ITenantProvider _tenantProvider;

        public TenantService(IApplicationDbContext context, ITenantProvider tenantProvider)
        {
            _context = context;
            _tenantProvider = tenantProvider;
        }

        public void ValidarTenant(int tenantId)
        {
            if (tenantId != _tenantProvider.TenantId)
                throw new NegocioException("Tenant inválido.");
        }

        public async Task<TenantConfigDto> ObtenerConfiguracionAsync(int tenantId)
        {
            ValidarTenant(tenantId);
            var t = await _context.TenantConfigs.AnyAsync();
            var config = await _context.TenantConfigs.FirstOrDefaultAsync(c => c.TenantId == tenantId);
            if (config == null) throw new NegocioException("Configuración no encontrada.");
            return MapToDto(config);
        }

        public async Task<TenantConfigDto> ActualizarConfiguracionAsync(int tenantId, TenantConfigDto dto)
        {
            ValidarTenant(tenantId);
            var config = await _context.TenantConfigs.FirstOrDefaultAsync(c => c.TenantId == tenantId)
                ?? throw new NegocioException("Configuración no encontrada.");
            config.ColorPrimario = dto.ColorPrimario;
            config.ColorSecundario = dto.ColorSecundario;
            config.LogoUrl = dto.LogoUrl;
            config.NombreSistema = dto.NombreSistema;
            config.MonedaPorDefecto = dto.MonedaPorDefecto;
            config.ImpuestoPorDefecto = dto.ImpuestoPorDefecto;
            config.FormatoFactura = dto.FormatoFactura;
            config.ManejaMultiplesAlmacenes = dto.ManejaMultiplesAlmacenes;
            config.ControlStockEstricto = dto.ControlStockEstricto;
            config.PermiteVentaSinStock = dto.PermiteVentaSinStock;
            config.MetodoCalculoCosto = dto.MetodoCalculoCosto;
            await _context.SaveChangesAsync();
            return MapToDto(config);
        }

        private static TenantConfigDto MapToDto(TenantConfig c) => new()
        {
            ColorPrimario = c.ColorPrimario,
            ColorSecundario = c.ColorSecundario,
            LogoUrl = c.LogoUrl,
            NombreSistema = c.NombreSistema,
            MonedaPorDefecto = c.MonedaPorDefecto,
            ImpuestoPorDefecto = c.ImpuestoPorDefecto,
            FormatoFactura = c.FormatoFactura,
            ManejaMultiplesAlmacenes = c.ManejaMultiplesAlmacenes,
            ControlStockEstricto = c.ControlStockEstricto,
            PermiteVentaSinStock = c.PermiteVentaSinStock,
            MetodoCalculoCosto = c.MetodoCalculoCosto
        };
    }
}