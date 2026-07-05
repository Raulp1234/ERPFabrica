// CategoriaTercero.cs
namespace ERPFabrica.Domain.Entities
{
    public class CategoriaTercero : IHasTenant
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
    }
}