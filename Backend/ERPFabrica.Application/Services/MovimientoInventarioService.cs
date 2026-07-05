// MovimientoInventarioService.cs
using ERPFabrica.Application.DTOs;
using ERPFabrica.Application.Interfaces;
using ERPFabrica.Domain.Entities;
using ERPFabrica.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace ERPFabrica.Application.Services
{
    public class MovimientoInventarioService : IMovimientoInventarioService
    {
        private readonly IApplicationDbContext _context;
        private readonly IStockService _stockService;

        public MovimientoInventarioService(IApplicationDbContext context, IStockService stockService)
        {
            _context = context;
            _stockService = stockService;
        }

        public async Task<List<MovimientoInventarioDto>> GetHistorialAsync(int productoId, int? almacenId = null)
        {
            var query = _context.MovimientosInventario
                .Where(m => m.ProductoId == productoId);

            if (almacenId.HasValue)
                query = query.Where(m => m.AlmacenId == almacenId.Value);

            return await query
                .OrderByDescending(m => m.FechaMovimiento)
                .Select(m => new MovimientoInventarioDto
                {
                    Id = m.Id,
                    ProductoId = m.ProductoId,
                    AlmacenId = m.AlmacenId,
                    TipoMovimiento = m.TipoMovimiento.ToString(),
                    Cantidad = m.Cantidad,
                    PrecioUnitario = m.PrecioUnitario,
                    FechaMovimiento = m.FechaMovimiento,
                    DocumentoReferencia = m.DocumentoReferencia,
                    Motivo = m.Motivo
                })
                .ToListAsync();
        }

        public async Task<MovimientoInventarioDto> RegistrarEntradaAsync(RegistrarMovimientoDto dto)
        {
            return await RegistrarMovimiento(dto, TipoMovimiento.Entrada);
        }

        public async Task<MovimientoInventarioDto> RegistrarSalidaAsync(RegistrarMovimientoDto dto)
        {
            // Validar stock suficiente para salida
            bool disponible = await _stockService.ValidarDisponibilidadAsync(dto.ProductoId, dto.AlmacenId, dto.Cantidad);
            if (!disponible)
                throw new InvalidOperationException("Stock insuficiente para realizar la salida.");

            return await RegistrarMovimiento(dto, TipoMovimiento.Salida);
        }

        private async Task<MovimientoInventarioDto> RegistrarMovimiento(RegistrarMovimientoDto dto, TipoMovimiento tipo)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. Crear el movimiento
                var movimiento = new MovimientoInventario
                {
                    ProductoId = dto.ProductoId,
                    AlmacenId = dto.AlmacenId,
                    TipoMovimiento = tipo,
                    Cantidad = dto.Cantidad,
                    PrecioUnitario = dto.PrecioUnitario,
                    FechaMovimiento = DateTime.UtcNow,
                    DocumentoReferencia = dto.DocumentoReferencia,
                    Motivo = dto.Motivo
                };
                _context.MovimientosInventario.Add(movimiento);

                // 2. Actualizar Stock.CantidadActual
                var stock = await _context.Stocks
                    .FirstOrDefaultAsync(s => s.ProductoId == dto.ProductoId && s.AlmacenId == dto.AlmacenId);

                if (stock == null)
                {
                    stock = new Stock
                    {
                        ProductoId = dto.ProductoId,
                        AlmacenId = dto.AlmacenId,
                        CantidadActual = 0,
                        StockMinimo = 0,
                        StockMaximo = 0,
                        UltimaActualizacion = DateTime.UtcNow
                    };
                    _context.Stocks.Add(stock);
                }

                if (tipo == TipoMovimiento.Entrada)
                    stock.CantidadActual += dto.Cantidad;
                else if (tipo == TipoMovimiento.Salida)
                    stock.CantidadActual -= dto.Cantidad;

                stock.UltimaActualizacion = DateTime.UtcNow;

                // 3. Recalcular PrecioCosto del producto usando PMP (Precio Medio Ponderado)
                await RecalcularCostoPMPAsync(dto.ProductoId, dto.PrecioUnitario, dto.Cantidad, tipo);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return new MovimientoInventarioDto
                {
                    Id = movimiento.Id,
                    ProductoId = movimiento.ProductoId,
                    AlmacenId = movimiento.AlmacenId,
                    TipoMovimiento = movimiento.TipoMovimiento.ToString(),
                    Cantidad = movimiento.Cantidad,
                    PrecioUnitario = movimiento.PrecioUnitario,
                    FechaMovimiento = movimiento.FechaMovimiento,
                    DocumentoReferencia = movimiento.DocumentoReferencia,
                    Motivo = movimiento.Motivo
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        private async Task RecalcularCostoPMPAsync(int productoId, decimal precioUnitario, decimal cantidad, TipoMovimiento tipo)
        {
            // Solo entradas afectan el costo PMP; las salidas no modifican el costo
            if (tipo != TipoMovimiento.Entrada) return;

            var producto = await _context.Productos.FindAsync(productoId);
            if (producto == null) return;

            // Stock total actual del producto en todos los almacenes
            var stockTotal = await _context.Stocks
                .Where(s => s.ProductoId == productoId)
                .SumAsync(s => s.CantidadActual);

            if (stockTotal <= 0)
            {
                // Si no hay stock, el nuevo costo es el precio de la entrada
                producto.PrecioCosto = precioUnitario;
            }
            else
            {
                // Fórmula PMP: ((StockAnterior * CostoAnterior) + (CantidadEntrada * PrecioEntrada)) / (StockAnterior + CantidadEntrada)
                decimal costoAnteriorTotal = (stockTotal - cantidad) * producto.PrecioCosto;
                decimal costoEntrada = cantidad * precioUnitario;
                producto.PrecioCosto = (costoAnteriorTotal + costoEntrada) / stockTotal;
            }
        }
    }
}