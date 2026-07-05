// Almacen.cs
namespace ERPFabrica.Domain.Entities
{
    public class Almacen : IHasTenant
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Ubicacion { get; set; } = string.Empty;
        public bool EsPrincipal { get; set; }
    }
}