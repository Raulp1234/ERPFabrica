// MovimientoInventario.cs
using ERPFabrica.Domain.Enums;

namespace ERPFabrica.Domain.Entities
{
    public class MovimientoInventario : IHasTenant
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public int ProductoId { get; set; }
        public int AlmacenId { get; set; }
        public TipoMovimiento TipoMovimiento { get; set; }
        public decimal Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public DateTime FechaMovimiento { get; set; }
        public string? DocumentoReferencia { get; set; }
        public string? Motivo { get; set; }
        public int? UsuarioId { get; set; }

        public Producto Producto { get; set; } = null!;
        public Almacen Almacen { get; set; } = null!;
    }
}