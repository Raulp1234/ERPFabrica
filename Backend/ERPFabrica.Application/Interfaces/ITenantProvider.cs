// ITenantProvider.cs (ubicado en Application/Interfaces)
namespace ERPFabrica.Application.Interfaces
{
    public interface ITenantProvider
    {
        int TenantId { get; }
        void SetTenant(int tenantId);
    }
}