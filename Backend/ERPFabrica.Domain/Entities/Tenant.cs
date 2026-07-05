// Tenant.cs
namespace ERPFabrica.Domain.Entities
{
    public class Tenant
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Subdominio { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; }
        public bool EsActivo { get; set; }
        public string PlanId { get; set; } = string.Empty;
    }
}