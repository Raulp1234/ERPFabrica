// IStockService.cs
using ERPFabrica.Application.DTOs;

namespace ERPFabrica.Application.Interfaces
{
    public interface IStockService
    {
        Task<List<StockDto>> GetStockPorProductoAsync(int productoId);
        Task<StockDto?> GetStockActualAsync(int productoId, int almacenId);
        Task<bool> ValidarDisponibilidadAsync(int productoId, int almacenId, decimal cantidad);
    }
}