// ProductoDto.cs
namespace ERPFabrica.Application.DTOs
{
    public class ProductoDto
    {
        public int Id { get; set; }
        public string CodigoSKU { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public decimal PrecioCosto { get; set; }
        public decimal PrecioVenta { get; set; }
        public string TipoProducto { get; set; } = string.Empty;
        public string UnidadMedida { get; set; } = string.Empty;
        public int? CategoriaProductoId { get; set; }
        public bool EsActivo { get; set; }
    }

    public class CrearProductoDto
    {
        public string CodigoSKU { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public decimal PrecioCosto { get; set; }
        public decimal PrecioVenta { get; set; }
        public string TipoProducto { get; set; } = "Simple";
        public string UnidadMedida { get; set; } = "Unidad";
        public int? CategoriaProductoId { get; set; }
    }
}