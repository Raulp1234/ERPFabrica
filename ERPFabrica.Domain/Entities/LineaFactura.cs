// LineaFactura.cs
namespace ERPFabrica.Domain.Entities
{
    public class LineaFactura
    {
        public int Id { get; set; }
        public int FacturaId { get; set; }
        public int? ProductoId { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public decimal Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Impuesto { get; set; }
        public decimal TotalLinea { get; set; }

        public Factura Factura { get; set; } = null!;
        public Producto? Producto { get; set; }
    }
}