// Application/DTOs/DashboardDtos.cs
namespace ERPFabrica.Application.DTOs
{
    public class DashboardDto
    {
        public List<ProductoStockBajoDto> ProductosBajoStockMinimo { get; set; } = new();
        public List<FacturaResumenDto> FacturasPendientesCobro { get; set; } = new();
        public decimal VentasDelMes { get; set; }
        public decimal ComprasDelMes { get; set; }
        public int SolicitudesPendientes { get; set; }
        public List<MovimientoResumenDto> UltimosMovimientos { get; set; } = new();
    }

    public class ProductoStockBajoDto
    {
        public int ProductoId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string CodigoSKU { get; set; } = string.Empty;
        public decimal StockActual { get; set; }
        public decimal StockMinimo { get; set; }
    }

    public class FacturaResumenDto
    {
        public int FacturaId { get; set; }
        public string NumeroFactura { get; set; } = string.Empty;
        public string TerceroNombre { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public DateTime FechaEmision { get; set; }
        public decimal SaldoPendiente { get; set; }
    }

    public class MovimientoResumenDto
    {
        public string TipoMovimiento { get; set; } = string.Empty;
        public string ProductoNombre { get; set; } = string.Empty;
        public decimal Cantidad { get; set; }
        public DateTime Fecha { get; set; }
    }

    public class VentaCategoriaDto
    {
        public string CategoriaNombre { get; set; } = string.Empty;
        public decimal TotalVentas { get; set; }
    }
}