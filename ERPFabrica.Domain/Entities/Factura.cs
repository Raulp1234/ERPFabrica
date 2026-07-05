// Factura.cs
using ERPFabrica.Domain.Enums;

namespace ERPFabrica.Domain.Entities
{
    public class Factura : IHasTenant, IHasSoftDelete
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public string NumeroFactura { get; set; } = string.Empty;
        public TipoFactura TipoFactura { get; set; }
        public int? SolicitudId { get; set; }
        public int TerceroId { get; set; }
        public EstadoFactura Estado { get; set; }
        public DateTime FechaEmision { get; set; }
        public DateTime? FechaVencimiento { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TotalImpuestos { get; set; }
        public decimal TotalDescuento { get; set; }
        public decimal Total { get; set; }
        public string? Notas { get; set; }
        public int? UsuarioId { get; set; }
        public bool IsDeleted { get; set; }

        public Solicitud? Solicitud { get; set; }
        public Tercero Tercero { get; set; } = null!;
        public ICollection<LineaFactura> Lineas { get; set; } = new List<LineaFactura>();
        public ICollection<Pago> Pagos { get; set; } = new List<Pago>();
    }
}