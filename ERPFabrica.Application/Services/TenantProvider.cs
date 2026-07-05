using ERPFabrica.Application.Interfaces;

namespace ERPFabrica.Application.Services
{
    public class TenantProvider : ITenantProvider
    {
        public int TenantId { get; private set; }
        public void SetTenant(int tenantId) => TenantId = tenantId;
    }
}
