using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ERPFabrica.Application.Interfaces;
using ERPFabrica.Application.DTOs;
using ERPFabrica.Application.Exceptions;

namespace ERPFabrica.WebApi.Controllers
{
    [ApiController]
    [Route("api/{tenantId}/campos-extra")]
    [Authorize]
    public class CampoExtraController : ControllerBase
    {
        private readonly ICampoExtraService _campoExtraService;

        public CampoExtraController(ICampoExtraService campoExtraService)
        {
            _campoExtraService = campoExtraService;
        }

        [HttpGet("definiciones/{entidad}")]
        [ProducesResponseType(typeof(List<CampoExtraDefinicionDto>), 200)]
        public async Task<IActionResult> ObtenerDefiniciones(int tenantId, string entidad)
        {
            return Ok(await _campoExtraService.ObtenerDefinicionesAsync(tenantId, entidad));
        }

        [HttpPost("definiciones")]
        [ProducesResponseType(typeof(CampoExtraDefinicionDto), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CrearDefinicion(int tenantId, [FromBody] CrearCampoExtraDefinicionDto dto)
        {
            try
            {
                var creada = await _campoExtraService.CrearDefinicionAsync(tenantId, dto);
                return CreatedAtAction(nameof(ObtenerDefiniciones), new { tenantId, entidad = dto.Entidad }, creada);
            }
            catch (NegocioException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("definiciones/{id}")]
        [ProducesResponseType(typeof(CampoExtraDefinicionDto), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> ActualizarDefinicion(int tenantId, int id, [FromBody] ActualizarCampoExtraDefinicionDto dto)
        {
            try
            {
                var actualizada = await _campoExtraService.ActualizarDefinicionAsync(tenantId, id, dto);
                return Ok(actualizada);
            }
            catch (NegocioException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("definiciones/{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> EliminarDefinicion(int tenantId, int id)
        {
            try
            {
                await _campoExtraService.EliminarDefinicionAsync(tenantId, id);
                return NoContent();
            }
            catch (NegocioException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("valores/{entidad}/{registroId}")]
        [ProducesResponseType(typeof(List<ValorCampoExtraDto>), 200)]
        public async Task<IActionResult> ObtenerValores(int tenantId, string entidad, string registroId)
        {
            return Ok(await _campoExtraService.ObtenerValoresAsync(tenantId, entidad, registroId));
        }

        [HttpPost("valores/{entidad}/{registroId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GuardarValores(int tenantId, string entidad, string registroId, [FromBody] List<ValorCampoExtraDto> valores)
        {
            try
            {
                await _campoExtraService.GuardarValoresAsync(tenantId, entidad, registroId, valores);
                return Ok(new { mensaje = "Valores guardados." });
            }
            catch (NegocioException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}