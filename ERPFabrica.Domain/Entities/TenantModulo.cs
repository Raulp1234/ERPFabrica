// TenantModulo.cs
namespace ERPFabrica.Domain.Entities
{
    public class TenantModulo : IHasTenant
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public int ModuloId { get; set; }
        public DateTime FechaInstalacion { get; set; }
        public bool EsActivo { get; set; }

        public ModuloSistema Modulo { get; set; } = null!;
    }
}