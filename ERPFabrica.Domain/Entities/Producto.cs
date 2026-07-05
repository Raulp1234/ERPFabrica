// Producto.cs
using ERPFabrica.Domain.Enums;

namespace ERPFabrica.Domain.Entities
{
    public class Producto : IHasTenant, IHasSoftDelete
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public string CodigoSKU { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public decimal PrecioCosto { get; set; }
        public decimal PrecioVenta { get; set; }
        public TipoProducto TipoProducto { get; set; }
        public string UnidadMedida { get; set; } = "Unidad";
        public int? CategoriaProductoId { get; set; }
        public bool EsActivo { get; set; }
        public DateTime FechaCreacion { get; set; }
        public bool IsDeleted { get; set; }

        public CategoriaProducto? Categoria { get; set; }
    }
}