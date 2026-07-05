namespace ERPFabrica.Application.DTOs
{
    public class UsuarioDto
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public bool EsAdmin { get; set; }
        public bool EsActivo { get; set; }
        public List<string> Roles { get; set; } = new();
    }

    public class CrearUsuarioDto
    {
        public string Email { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty; // contraseña inicial opcional; si no se envía se genera aleatoria
        public bool EsAdmin { get; set; } = false;
        public List<int>? RolesIds { get; set; }
    }

    public class ActualizarUsuarioDto
    {
        public string NombreCompleto { get; set; } = string.Empty;
        public string? Email { get; set; }
        public bool? EsAdmin { get; set; }
        public bool? EsActivo { get; set; }
    }
}