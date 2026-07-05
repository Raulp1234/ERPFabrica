// IHasTenant.cs
namespace ERPFabrica.Domain.Entities
{
    public interface IHasTenant
    {
        int TenantId { get; set; }
    }

    public interface IHasSoftDelete
    {
        bool IsDeleted { get; set; }
    }
}