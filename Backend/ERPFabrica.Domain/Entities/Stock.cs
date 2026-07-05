// Stock.cs
namespace ERPFabrica.Domain.Entities
{
    public class Stock : IHasTenant
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public int ProductoId { get; set; }
        public int AlmacenId { get; set; }
        public decimal CantidadActual { get; set; }
        public decimal StockMinimo { get; set; }
        public decimal StockMaximo { get; set; }
        public string? UbicacionEstanteria { get; set; }
        public DateTime UltimaActualizacion { get; set; }

        public Producto Producto { get; set; } = null!;
        public Almacen Almacen { get; set; } = null!;
    }
}