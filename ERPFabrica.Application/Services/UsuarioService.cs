using ERPFabrica.Application.DTOs;
using ERPFabrica.Application.Exceptions;
using ERPFabrica.Application.Interfaces;
using ERPFabrica.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ERPFabrica.Application.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IApplicationDbContext _context;
        private readonly ITenantProvider _tenantProvider;
        private readonly IConfiguration _configuration;

        public UsuarioService(IApplicationDbContext context, ITenantProvider tenantProvider, IConfiguration configuration)
        {
            _context = context;
            _tenantProvider = tenantProvider;
            _configuration = configuration;
        }

        // 🔐 Login
        public async Task<(UsuarioDto usuario, string token)> ValidarCredencialesAsync(string email, string password)
        {
            var usuario = await _context.Usuarios
                .IgnoreQueryFilters() // para permitir login sin tenant (aún no tenemos tenantId)
                .FirstOrDefaultAsync(u => u.Email == email && u.EsActivo);

            if (usuario == null || !BCrypt.Net.BCrypt.Verify(password, usuario.PasswordHash))
                throw new NegocioException("Credenciales inválidas.");

            var token = GenerarToken(usuario);
            return (await MapToDto(usuario), token);
        }

        // 🔄 Refresh token
        public async Task<(UsuarioDto usuario, string token)> RefrescarTokenAsync(string tokenExpirado)
        {
            var principal = ValidarToken(tokenExpirado);
            var userId = int.Parse(principal.FindFirst("userId")!.Value);
            var usuario = await _context.Usuarios
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(u => u.Id == userId && u.EsActivo)
                ?? throw new NegocioException("Token inválido o usuario inactivo.");

            var newToken = GenerarToken(usuario);
            return (await MapToDto(usuario), newToken);
        }

        public async Task<List<UsuarioDto>> ObtenerUsuariosAsync(int tenantId)
        {
            ValidarTenant(tenantId);
            var usuarios = await _context.Usuarios
                .Where(u => u.TenantId == tenantId)
                .ToListAsync();
            var lista = new List<UsuarioDto>();
            foreach (var u in usuarios) lista.Add(await MapToDto(u));
            return lista;
        }

        public async Task<UsuarioDto?> ObtenerUsuarioAsync(int tenantId, int id)
        {
            ValidarTenant(tenantId);
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == id && u.TenantId == tenantId);
            return usuario == null ? null : await MapToDto(usuario);
        }

        public async Task<UsuarioDto> CrearUsuarioAsync(int tenantId, CrearUsuarioDto dto)
        {
            ValidarTenant(tenantId);
            if (await _context.Usuarios.AnyAsync(u => u.Email == dto.Email && u.TenantId == tenantId))
                throw new NegocioException("El email ya está registrado en este tenant.");

            var usuario = new Usuario
            {
                TenantId = tenantId,
                Email = dto.Email,
                NombreCompleto = dto.NombreCompleto,
                EsAdmin = dto.EsAdmin,
                EsActivo = true,
                FechaCreacion = DateTime.UtcNow
            };

            // Generar hash de contraseña (usar BCrypt.Net)
            string password = string.IsNullOrWhiteSpace(dto.Password) ? GenerarPasswordAleatorio() : dto.Password;
            usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            // Asignar roles si se proporcionan
            if (dto.RolesIds?.Any() == true)
            {
                foreach (var rolId in dto.RolesIds)
                {
                    _context.UsuarioRoles.Add(new UsuarioRol { UsuarioId = usuario.Id, RolId = rolId });
                }
                await _context.SaveChangesAsync();
            }

            // Podríamos enviar un email con la contraseña inicial aquí...
            return await MapToDto(usuario);
        }

        public async Task<UsuarioDto> ActualizarUsuarioAsync(int tenantId, int id, ActualizarUsuarioDto dto)
        {
            ValidarTenant(tenantId);
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == id && u.TenantId == tenantId)
                ?? throw new NegocioException("Usuario no encontrado.");

            usuario.NombreCompleto = dto.NombreCompleto ?? usuario.NombreCompleto;
            if (dto.Email != null) usuario.Email = dto.Email;
            if (dto.EsAdmin.HasValue) usuario.EsAdmin = dto.EsAdmin.Value;
            if (dto.EsActivo.HasValue) usuario.EsActivo = dto.EsActivo.Value;

            await _context.SaveChangesAsync();
            return await MapToDto(usuario);
        }

        public async Task EliminarUsuarioAsync(int tenantId, int id)
        {
            ValidarTenant(tenantId);
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == id && u.TenantId == tenantId)
                ?? throw new NegocioException("Usuario no encontrado.");
            usuario.EsActivo = false; // soft delete
            await _context.SaveChangesAsync();
        }

        public async Task<UsuarioDto> AsignarRolesAsync(int tenantId, int usuarioId, List<int> rolIds)
        {
            ValidarTenant(tenantId);
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == usuarioId && u.TenantId == tenantId)
                ?? throw new NegocioException("Usuario no encontrado.");

            // Remover roles existentes y asignar nuevos
            var rolesExistentes = await _context.UsuarioRoles.Where(ur => ur.UsuarioId == usuarioId).ToListAsync();
            _context.UsuarioRoles.RemoveRange(rolesExistentes);
            foreach (var rolId in rolIds)
            {
                _context.UsuarioRoles.Add(new UsuarioRol { UsuarioId = usuarioId, RolId = rolId });
            }
            await _context.SaveChangesAsync();
            return await MapToDto(usuario);
        }

        // ------------------------------
        // Métodos privados
        // ------------------------------
        private void ValidarTenant(int tenantId)
        {
            if (tenantId != _tenantProvider.TenantId)
                throw new NegocioException("Tenant inválido.");
        }

        private string GenerarToken(Usuario usuario)
        {
            var claims = new List<Claim>
            {
                new Claim("userId", usuario.Id.ToString()),
                new Claim("TenantId", usuario.TenantId.ToString()),
                new Claim("EsAdmin", usuario.EsAdmin.ToString()),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim(ClaimTypes.Name, usuario.NombreCompleto)
            };

            // Agregar roles desde la tabla UsuarioRol + Rol
            var roles = _context.UsuarioRoles
                .Where(ur => ur.UsuarioId == usuario.Id)
                .Join(_context.Roles, ur => ur.RolId, r => r.Id, (ur, r) => r.Nombre)
                .ToList();
            foreach (var rol in roles)
                claims.Add(new Claim(ClaimTypes.Role, rol));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:DurationInMinutes"] ?? "1440")),
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private ClaimsPrincipal ValidarToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!);
            var parameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = false, // para refresh, no validar expiración
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidAudience = _configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(key)
            };
            return tokenHandler.ValidateToken(token, parameters, out _);
        }

        private string GenerarPasswordAleatorio() => Guid.NewGuid().ToString().Substring(0, 12);

        private async Task<UsuarioDto> MapToDto(Usuario u)
        {
            var roles = await _context.UsuarioRoles
                .Where(ur => ur.UsuarioId == u.Id)
                .Join(_context.Roles, ur => ur.RolId, r => r.Id, (ur, r) => r.Nombre)
                .ToListAsync();

            return new UsuarioDto
            {
                Id = u.Id,
                TenantId = u.TenantId,
                Email = u.Email,
                NombreCompleto = u.NombreCompleto,
                EsAdmin = u.EsAdmin,
                EsActivo = u.EsActivo,
                Roles = roles
            };
        }
    }
}