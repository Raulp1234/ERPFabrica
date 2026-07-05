using Microsoft.EntityFrameworkCore;
using ERPFabrica.Application.DTOs;
using ERPFabrica.Application.Interfaces;
using ERPFabrica.Domain.Enums;

namespace ERPFabrica.Application.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IApplicationDbContext _context;
        private readonly ITenantProvider _tenantProvider;

        public DashboardService(IApplicationDbContext context, ITenantProvider tenantProvider)
        {
            _context = context;
            _tenantProvider = tenantProvider;
        }

        public async Task<DashboardDto> ObtenerMetricasAsync(int tenantId)
        {
            // Productos bajo stock mínimo
            var bajoStock = await _context.Stocks
                .Where(s => s.TenantId == tenantId && s.CantidadActual <= s.StockMinimo)
                .Select(s => new ProductoStockBajoDto
                {
                    ProductoId = s.ProductoId,
                    Nombre = s.Producto.Nombre,
                    CodigoSKU = s.Producto.CodigoSKU,
                    StockActual = s.CantidadActual,
                    StockMinimo = s.StockMinimo
                }).ToListAsync();

            // Facturas emitidas/pagadas parciales (pendientes de cobro)
            var facturasPendientes = await _context.Facturas
                .Where(f => f.TenantId == tenantId && (f.Estado == EstadoFactura.Emitida || f.Estado == EstadoFactura.PagadaParcial))
                .Select(f => new FacturaResumenDto
                {
                    FacturaId = f.Id,
                    NumeroFactura = f.NumeroFactura,
                    TerceroNombre = f.Tercero.Nombre,
                    Total = f.Total,
                    FechaEmision = f.FechaEmision,
                    SaldoPendiente = f.Total - f.Pagos.Sum(p => p.Monto)
                }).ToListAsync();

            // Ventas del mes (facturas emitidas en el mes actual)
            var inicioMes = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
            var ventasMes = await _context.Facturas
                .Where(f => f.TenantId == tenantId && f.Estado == EstadoFactura.Emitida && f.FechaEmision >= inicioMes)
                .SumAsync(f => f.Total);

            // Compras del mes (si usamos TipoFactura.Compra)
            var comprasMes = await _context.Facturas
                .Where(f => f.TenantId == tenantId && f.TipoFactura == TipoFactura.Compra && f.Estado == EstadoFactura.Emitida && f.FechaEmision >= inicioMes)
                .SumAsync(f => f.Total);

            // Solicitudes pendientes (estado Pendiente o Aprobado)
            var solicitudesPendientes = await _context.Solicitudes
                .CountAsync(s => s.TenantId == tenantId && (s.Estado == EstadoSolicitud.Pendiente || s.Estado == EstadoSolicitud.Aprobado));

            // Últimos movimientos de inventario (5 más recientes)
            var ultimosMov = await _context.MovimientosInventario
                .Where(m => m.TenantId == tenantId)
                .OrderByDescending(m => m.FechaMovimiento)
                .Take(5)
                .Select(m => new MovimientoResumenDto
                {
                    TipoMovimiento = m.TipoMovimiento.ToString(),
                    ProductoNombre = m.Producto.Nombre,
                    Cantidad = m.Cantidad,
                    Fecha = m.FechaMovimiento
                }).ToListAsync();

            return new DashboardDto
            {
                ProductosBajoStockMinimo = bajoStock,
                FacturasPendientesCobro = facturasPendientes,
                VentasDelMes = ventasMes,
                ComprasDelMes = comprasMes,
                SolicitudesPendientes = solicitudesPendientes,
                UltimosMovimientos = ultimosMov
            };
        }

        public async Task<List<VentaCategoriaDto>> ObtenerVentasPorCategoriaAsync(int tenantId, DateTime? desde, DateTime? hasta)
        {
            var query = _context.Facturas
                .Where(f => f.TenantId == tenantId && f.Estado == EstadoFactura.Emitida);

            if (desde.HasValue) query = query.Where(f => f.FechaEmision >= desde.Value);
            if (hasta.HasValue) query = query.Where(f => f.FechaEmision <= hasta.Value);

            var ventasPorCategoria = await query
                .SelectMany(f => f.Lineas.Where(l => l.Producto != null)
                    .GroupBy(l => l.Producto!.Categoria!.Nombre)
                    .Select(g => new VentaCategoriaDto
                    {
                        CategoriaNombre = g.Key,
                        TotalVentas = g.Sum(l => l.TotalLinea)
                    }))
                .GroupBy(vc => vc.CategoriaNombre)
                .Select(g => new VentaCategoriaDto
                {
                    CategoriaNombre = g.Key,
                    TotalVentas = g.Sum(vc => vc.TotalVentas)
                })
                .ToListAsync();

            return ventasPorCategoria;
        }
    }
}