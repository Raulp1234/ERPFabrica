// FacturaDto.cs
namespace ERPFabrica.Application.DTOs
{
    public class FacturaDto
    {
        public int Id { get; set; }
        public string NumeroFactura { get; set; } = string.Empty;
        public string TipoFactura { get; set; } = string.Empty;
        public int? SolicitudId { get; set; }
        public int TerceroId { get; set; }
        public string TerceroNombre { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public DateTime FechaEmision { get; set; }
        public DateTime? FechaVencimiento { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TotalImpuestos { get; set; }
        public decimal TotalDescuento { get; set; }
        public decimal Total { get; set; }
        public string? Notas { get; set; }
        public List<LineaFacturaDto> Lineas { get; set; } = new();
        public List<PagoDto> Pagos { get; set; } = new();
    }

    public class LineaFacturaDto
    {
        public int Id { get; set; }
        public int? ProductoId { get; set; }
        public string ProductoNombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public decimal Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Impuesto { get; set; }
        public decimal TotalLinea { get; set; }
    }

    public class PagoDto
    {
        public int Id { get; set; }
        public DateTime FechaPago { get; set; }
        public decimal Monto { get; set; }
        public string MetodoPago { get; set; } = string.Empty;
        public string? Referencia { get; set; }
    }

    public class CrearFacturaDto
    {
        public string TipoFactura { get; set; } = "Venta";
        public int? SolicitudId { get; set; }
        public int TerceroId { get; set; }
        public DateTime? FechaVencimiento { get; set; }
        public string? Notas { get; set; }
        public List<CrearLineaFacturaDto> Lineas { get; set; } = new();
    }

    public class CrearLineaFacturaDto
    {
        public int? ProductoId { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public decimal Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Impuesto { get; set; }
    }

    public class RegistrarPagoDto
    {
        public decimal Monto { get; set; }
        public string MetodoPago { get; set; } = string.Empty;
        public string? Referencia { get; set; }
    }
}