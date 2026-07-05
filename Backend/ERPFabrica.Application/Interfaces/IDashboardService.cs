using ERPFabrica.Application.DTOs;

namespace ERPFabrica.Application.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardDto> ObtenerMetricasAsync(int tenantId);
        Task<List<VentaCategoriaDto>> ObtenerVentasPorCategoriaAsync(int tenantId, DateTime? desde, DateTime? hasta);
    }
}