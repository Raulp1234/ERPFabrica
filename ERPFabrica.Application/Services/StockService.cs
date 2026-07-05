// StockService.cs
using ERPFabrica.Application.DTOs;
using ERPFabrica.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ERPFabrica.Application.Services
{
    public class StockService : IStockService
    {
        private readonly IApplicationDbContext _context;

        public StockService(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<StockDto>> GetStockPorProductoAsync(int productoId)
        {
            return await _context.Stocks
                .Where(s => s.ProductoId == productoId)
                .Select(s => new StockDto
                {
                    ProductoId = s.ProductoId,
                    ProductoNombre = s.Producto.Nombre,
                    AlmacenId = s.AlmacenId,
                    AlmacenNombre = s.Almacen.Nombre,
                    CantidadActual = s.CantidadActual,
                    StockMinimo = s.StockMinimo,
                    StockMaximo = s.StockMaximo,
                    UbicacionEstanteria = s.UbicacionEstanteria
                })
                .ToListAsync();
        }

        public async Task<StockDto?> GetStockActualAsync(int productoId, int almacenId)
        {
            var stock = await _context.Stocks
                .FirstOrDefaultAsync(s => s.ProductoId == productoId && s.AlmacenId == almacenId);

            if (stock == null) return null;

            return new StockDto
            {
                ProductoId = stock.ProductoId,
                ProductoNombre = stock.Producto.Nombre,
                AlmacenId = stock.AlmacenId,
                AlmacenNombre = stock.Almacen.Nombre,
                CantidadActual = stock.CantidadActual,
                StockMinimo = stock.StockMinimo,
                StockMaximo = stock.StockMaximo,
                UbicacionEstanteria = stock.UbicacionEstanteria
            };
        }

        public async Task<bool> ValidarDisponibilidadAsync(int productoId, int almacenId, decimal cantidad)
        {
            var stock = await _context.Stocks
                .FirstOrDefaultAsync(s => s.ProductoId == productoId && s.AlmacenId == almacenId);
            return stock != null && stock.CantidadActual >= cantidad;
        }
    }
}