// Application/Interfaces/IUsuarioService.cs
using ERPFabrica.Application.DTOs;

namespace ERPFabrica.Application.Interfaces
{
    public interface IUsuarioService
    {
        Task<(UsuarioDto usuario, string token)> ValidarCredencialesAsync(string email, string password);
        Task<(UsuarioDto usuario, string token)> RefrescarTokenAsync(string tokenExpirado);
        Task<List<UsuarioDto>> ObtenerUsuariosAsync(int tenantId);
        Task<UsuarioDto?> ObtenerUsuarioAsync(int tenantId, int id);
        Task<UsuarioDto> CrearUsuarioAsync(int tenantId, CrearUsuarioDto dto);
        Task<UsuarioDto> ActualizarUsuarioAsync(int tenantId, int id, ActualizarUsuarioDto dto);
        Task EliminarUsuarioAsync(int tenantId, int id);
        Task<UsuarioDto> AsignarRolesAsync(int tenantId, int usuarioId, List<int> rolIds);
    }
}