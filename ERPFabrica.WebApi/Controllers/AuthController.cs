using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ERPFabrica.Application.Interfaces;
using ERPFabrica.Application.Exceptions;

namespace ERPFabrica.WebApi.Controllers
{
    [ApiController]
    [Route("api/auth")]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;

        public AuthController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        /// <summary>
        /// Iniciar sesión y obtener token JWT.
        /// </summary>
        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResponseDto), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            try
            {
                var (usuario, token) = await _usuarioService.ValidarCredencialesAsync(request.Email, request.Password);
                return Ok(new LoginResponseDto
                {
                    Token = token,
                    NombreCompleto = usuario.NombreCompleto,
                    TenantId = usuario.TenantId,
                    EsAdmin = usuario.EsAdmin
                });
            }
            catch (NegocioException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Refrescar token JWT.
        /// </summary>
        [HttpPost("refresh-token")]
        [ProducesResponseType(typeof(LoginResponseDto), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto request)
        {
            try
            {
                var (usuario, token) = await _usuarioService.RefrescarTokenAsync(request.Token);
                return Ok(new LoginResponseDto
                {
                    Token = token,
                    NombreCompleto = usuario.NombreCompleto,
                    TenantId = usuario.TenantId,
                    EsAdmin = usuario.EsAdmin
                });
            }
            catch (NegocioException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }

    // DTOs locales (podrían ir en una carpeta DTOs/Requests)
    public class LoginRequestDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class RefreshTokenRequestDto
    {
        public string Token { get; set; } = string.Empty;
    }

    public class LoginResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public int TenantId { get; set; }
        public bool EsAdmin { get; set; }
    }
}