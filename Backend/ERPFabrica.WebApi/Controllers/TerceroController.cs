using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ERPFabrica.Application.Interfaces;
using ERPFabrica.Application.DTOs;
using ERPFabrica.Application.Exceptions;

namespace ERPFabrica.WebApi.Controllers
{
    [ApiController]
    [Route("api/{tenantId}/terceros")]
    [Authorize]
    public class TerceroController : ControllerBase
    {
        private readonly ITerceroService _terceroService;

        public TerceroController(ITerceroService terceroService)
        {
            _terceroService = terceroService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<TerceroDto>), 200)]
        public async Task<IActionResult> ObtenerTerceros(int tenantId)
        {
            return Ok(await _terceroService.ObtenerTercerosAsync(tenantId));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(TerceroDto), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> ObtenerTercero(int tenantId, int id)
        {
            var tercero = await _terceroService.ObtenerTerceroAsync(tenantId, id);
            return tercero == null ? NotFound() : Ok(tercero);
        }

        [HttpPost]
        [ProducesResponseType(typeof(TerceroDto), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CrearTercero(int tenantId, [FromBody] CrearTerceroDto dto)
        {
            try
            {
                var creado = await _terceroService.CrearTerceroAsync(tenantId, dto);
                return CreatedAtAction(nameof(ObtenerTercero), new { tenantId, id = creado.Id }, creado);
            }
            catch (NegocioException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(TerceroDto), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> ActualizarTercero(int tenantId, int id, [FromBody] ActualizarTerceroDto dto)
        {
            try
            {
                var actualizado = await _terceroService.ActualizarTerceroAsync(tenantId, id, dto);
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
        public async Task<IActionResult> EliminarTercero(int tenantId, int id)
        {
            try
            {
                await _terceroService.EliminarTerceroAsync(tenantId, id);
                return NoContent();
            }
            catch (NegocioException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }

   
}