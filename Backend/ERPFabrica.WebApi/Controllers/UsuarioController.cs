using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ERPFabrica.Application.Interfaces;
using ERPFabrica.Application.DTOs;
using ERPFabrica.Application.Exceptions;

namespace ERPFabrica.WebApi.Controllers
{
    [ApiController]
    [Route("api/{tenantId}/usuarios")]
    [Authorize]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;

        public UsuarioController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        private void VerificarAdmin()
        {
            var esAdmin = User.FindFirst("EsAdmin")?.Value;
            if (string.IsNullOrEmpty(esAdmin) || !bool.TryParse(esAdmin, out var admin) || !admin)
                throw new NegocioException("Se requiere rol de administrador.");
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<UsuarioDto>), 200)]
        public async Task<IActionResult> ObtenerUsuarios(int tenantId)
        {
            VerificarAdmin();
            return Ok(await _usuarioService.ObtenerUsuariosAsync(tenantId));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UsuarioDto), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> ObtenerUsuario(int tenantId, int id)
        {
            VerificarAdmin();
            var usuario = await _usuarioService.ObtenerUsuarioAsync(tenantId, id);
            return usuario == null ? NotFound() : Ok(usuario);
        }

        [HttpPost]
        [ProducesResponseType(typeof(UsuarioDto), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CrearUsuario(int tenantId, [FromBody] CrearUsuarioDto dto)
        {
            VerificarAdmin();
            try
            {
                var creado = await _usuarioService.CrearUsuarioAsync(tenantId, dto);
                return CreatedAtAction(nameof(ObtenerUsuario), new { tenantId, id = creado.Id }, creado);
            }
            catch (NegocioException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(UsuarioDto), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> ActualizarUsuario(int tenantId, int id, [FromBody] ActualizarUsuarioDto dto)
        {
            VerificarAdmin();
            try
            {
                var actualizado = await _usuarioService.ActualizarUsuarioAsync(tenantId, id, dto);
                return Ok(actualizado);
            }
            catch (NegocioException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> EliminarUsuario(int tenantId, int id)
        {
            VerificarAdmin();
            try
            {
                await _usuarioService.EliminarUsuarioAsync(tenantId, id);
                return NoContent();
            }
            catch (NegocioException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("{id}/roles")]
        [ProducesResponseType(typeof(UsuarioDto), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> AsignarRoles(int tenantId, int id, [FromBody] List<int> rolIds)
        {
            VerificarAdmin();
            try
            {
                var actualizado = await _usuarioService.AsignarRolesAsync(tenantId, id, rolIds);
                return Ok(actualizado);
            }
            catch (NegocioException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }

}