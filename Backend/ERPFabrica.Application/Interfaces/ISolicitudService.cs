// ISolicitudService.cs
using ERPFabrica.Application.DTOs;
using ERPFabrica.Domain.Enums;

namespace ERPFabrica.Application.Interfaces
{
    public interface ISolicitudService
    {
        Task<SolicitudDto> CrearSolicitudAsync(int tenantId, CrearSolicitudDto dto, int usuarioId);
        Task<SolicitudDto> CambiarEstadoAsync(int tenantId, int solicitudId, EstadoSolicitud nuevoEstado, int usuarioId, string? comentario = null);
        Task<FacturaDto> GenerarFacturaDesdeSolicitudAsync(int tenantId, int solicitudId, int usuarioId);
        Task<List<SolicitudDto>> ObtenerSolicitudesAsync(int tenantId, EstadoSolicitud? estado = null);
        Task<SolicitudDto> ObtenerSolicitudPorIdAsync(int tenantId, int solicitudId);
        Task<List<HistorialEstadoDto>> ObtenerHistorialAsync(int tenantId, int solicitudId);
    }
}