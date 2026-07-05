// StockDto.cs
namespace ERPFabrica.Application.DTOs
{
    public class StockDto
    {
        public int ProductoId { get; set; }
        public string ProductoNombre { get; set; } = string.Empty;
        public int AlmacenId { get; set; }
        public string AlmacenNombre { get; set; } = string.Empty;
        public decimal CantidadActual { get; set; }
        public decimal StockMinimo { get; set; }
        public decimal StockMaximo { get; set; }
        public string? UbicacionEstanteria { get; set; }
    }
}