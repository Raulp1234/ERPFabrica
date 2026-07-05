// LineaSolicitud.cs
namespace ERPFabrica.Domain.Entities
{
    public class LineaSolicitud
    {
        public int Id { get; set; }
        public int SolicitudId { get; set; }
        public int? ProductoId { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public decimal CantidadSolicitada { get; set; }
        public decimal CantidadEntregada { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Impuesto { get; set; }
        public decimal TotalLinea { get; set; }
        public bool Facturada { get; set; }

        public Solicitud Solicitud { get; set; } = null!;
        public Producto? Producto { get; set; }
    }
}