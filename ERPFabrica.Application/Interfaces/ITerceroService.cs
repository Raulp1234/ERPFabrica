// Application/Interfaces/ITerceroService.cs
using ERPFabrica.Application.DTOs;

namespace ERPFabrica.Application.Interfaces
{
    public interface ITerceroService
    {
        Task<List<TerceroDto>> ObtenerTercerosAsync(int tenantId);
        Task<TerceroDto?> ObtenerTerceroAsync(int tenantId, int id);
        Task<TerceroDto> CrearTerceroAsync(int tenantId, CrearTerceroDto dto);
        Task<TerceroDto> ActualizarTerceroAsync(int tenantId, int id, ActualizarTerceroDto dto);
        Task EliminarTerceroAsync(int tenantId, int id);

        // Categorías de terceros
        Task<List<CategoriaDto>> ObtenerCategoriasAsync(int tenantId);
        Task<CategoriaDto> CrearCategoriaAsync(int tenantId, CrearCategoriaDto dto);
        Task<CategoriaDto> ActualizarCategoriaAsync(int tenantId, int id, ActualizarCategoriaDto dto);
        Task EliminarCategoriaAsync(int tenantId, int id);
    }
}