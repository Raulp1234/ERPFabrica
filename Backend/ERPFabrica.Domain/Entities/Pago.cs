// Pago.cs
namespace ERPFabrica.Domain.Entities
{
    public class Pago
    {
        public int Id { get; set; }
        public int FacturaId { get; set; }
        public DateTime FechaPago { get; set; }
        public decimal Monto { get; set; }
        public string MetodoPago { get; set; } = string.Empty;
        public string? Referencia { get; set; }
        public int? UsuarioId { get; set; }

        public Factura Factura { get; set; } = null!;
    }
}