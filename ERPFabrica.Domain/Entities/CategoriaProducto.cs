// CategoriaProducto.cs
namespace ERPFabrica.Domain.Entities
{
    public class CategoriaProducto : IHasTenant
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int? CategoriaPadreId { get; set; }

        public CategoriaProducto? CategoriaPadre { get; set; }
    }
}