// IProductoService.cs
using ERPFabrica.Application.DTOs;

namespace ERPFabrica.Application.Interfaces
{
    public interface IProductoService
    {
        Task<List<ProductoDto>> GetAllAsync();
        Task<ProductoDto?> GetByIdAsync(int id);
        Task<ProductoDto> CreateAsync(CrearProductoDto dto);
        Task UpdateAsync(int id, CrearProductoDto dto);
        Task DeleteAsync(int id); // soft delete


        Task<List<CategoriaDto>> ObtenerCategoriasAsync(int tenantId);
        Task<CategoriaDto> CrearCategoriaAsync(int tenantId, CrearCategoriaDto dto);
        Task<CategoriaDto> ActualizarCategoriaAsync(int tenantId, int id, ActualizarCategoriaDto dto);
        Task EliminarCategoriaAsync(int tenantId, int id);
    }
}