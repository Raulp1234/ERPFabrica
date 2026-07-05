// Application/Interfaces/ITenantService.cs
using ERPFabrica.Application.DTOs;

namespace ERPFabrica.Application.Interfaces
{
    public interface ITenantService
    {
        Task<TenantConfigDto> ObtenerConfiguracionAsync(int tenantId);
        Task<TenantConfigDto> ActualizarConfiguracionAsync(int tenantId, TenantConfigDto dto);
        void ValidarTenant(int tenantId);
    }
}