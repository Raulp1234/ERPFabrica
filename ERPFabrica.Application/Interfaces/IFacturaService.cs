// IFacturaService.cs
using ERPFabrica.Application.DTOs;
using ERPFabrica.Domain.Enums;

namespace ERPFabrica.Application.Interfaces
{
    public interface IFacturaService
    {
        Task<FacturaDto> CrearFacturaAsync(int tenantId, CrearFacturaDto dto, int usuarioId);
        Task<FacturaDto> EmitirFacturaAsync(int tenantId, int facturaId, int usuarioId);
        Task<FacturaDto> RegistrarPagoAsync(int tenantId, int facturaId, RegistrarPagoDto dto, int usuarioId);
        Task AnularFacturaAsync(int tenantId, int facturaId, int usuarioId);
        Task<List<FacturaDto>> ObtenerFacturasAsync(int tenantId, EstadoFactura? estado = null);
        Task<FacturaDto> ObtenerFacturaPorIdAsync(int tenantId, int facturaId);
        Task<decimal> ObtenerSaldoPendienteTerceroAsync(int tenantId, int terceroId);
    }
}