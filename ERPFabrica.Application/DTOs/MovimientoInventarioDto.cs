// MovimientoInventarioDto.cs
namespace ERPFabrica.Application.DTOs
{
    public class MovimientoInventarioDto
    {
        public int Id { get; set; }
        public int ProductoId { get; set; }
        public int AlmacenId { get; set; }
        public string TipoMovimiento { get; set; } = string.Empty;
        public decimal Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public DateTime FechaMovimiento { get; set; }
        public string? DocumentoReferencia { get; set; }
        public string? Motivo { get; set; }
    }

    public class RegistrarMovimientoDto
    {
        public int ProductoId { get; set; }
        public int AlmacenId { get; set; }
        public string TipoMovimiento { get; set; } = string.Empty; // "Entrada" o "Salida"
        public decimal Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public string? DocumentoReferencia { get; set; }
        public string? Motivo { get; set; }
    }
}