// Usuario.cs
namespace ERPFabrica.Domain.Entities
{
    public class Usuario : IHasTenant
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public bool EsAdmin { get; set; }
        public DateTime FechaCreacion { get; set; }
        public bool EsActivo { get; set; }
    }
}