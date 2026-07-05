using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ERPFabrica.Application.DTOs;
using ERPFabrica.Application.Interfaces;
using ERPFabrica.Domain.Enums;
using ERPFabrica.Application.Exceptions;

namespace ERPFabrica.WebApi.Controllers
{
    [ApiController]
    [Route("api/{tenantId}/solicitudes")]
    [Authorize]
    public class SolicitudController : ControllerBase
    {
        private readonly ISolicitudService _solicitudService;

        public SolicitudController(ISolicitudService solicitudService)
        {
            _solicitudService = solicitudService;
        }

        private int GetUsuarioId() =>
            int.Parse(User.FindFirst("userId")?.Value ?? throw new NegocioException("Usuario no identificado en token."));

        [HttpGet]
        [ProducesResponseType(typeof(List<SolicitudDto>), 200)]
        public async Task<IActionResult> ObtenerSolicitudes(int tenantId, [FromQuery] string? estado = null)
        {
            EstadoSolicitud? estadoEnum = null;
            if (!string.IsNullOrWhiteSpace(estado) && Enum.TryParse<EstadoSolicitud>(estado, out var parsed))
                estadoEnum = parsed;

            var resultado = await _solicitudService.ObtenerSolicitudesAsync(tenantId, estadoEnum);
            return Ok(resultado);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(SolicitudDto), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> ObtenerSolicitudPorId(int tenantId, int id)
        {
            var solicitud = await _solicitudService.ObtenerSolicitudPorIdAsync(tenantId, id);
            return solicitud == null ? NotFound() : Ok(solicitud);
        }

        [HttpPost]
        [ProducesResponseType(typeof(SolicitudDto), 201)]
        public async Task<IActionResult> CrearSolicitud(int tenantId, [FromBody] CrearSolicitudDto dto)
        {
            try
            {
                var creada = await _solicitudService.CrearSolicitudAsync(tenantId, dto, GetUsuarioId());
                return CreatedAtAction(nameof(ObtenerSolicitudPorId), new { tenantId, id = creada.Id }, creada);
            }
            catch (NegocioException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("{id}/estado")]
        [ProducesResponseType(typeof(SolicitudDto), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CambiarEstado(int tenantId, int id, [FromBody] CambiarEstadoDto dto)
        {
            if (!Enum.TryParse<EstadoSolicitud>(dto.EstadoNuevo, out var nuevoEstado))
                return BadRequest(new { error = $"Estado '{dto.EstadoNuevo}' no válido." });

            try
            {
                var solicitud = await _solicitudService.CambiarEstadoAsync(tenantId, id, nuevoEstado, GetUsuarioId(), dto.Comentario);
                return Ok(solicitud);
            }
            catch (NegocioException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (EstadoInvalidoException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (StockInsuficienteException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("{id}/generar-factura")]
        [ProducesResponseType(typeof(FacturaDto), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GenerarFactura(int tenantId, int id)
        {
            try
            {
                var factura = await _solicitudService.GenerarFacturaDesdeSolicitudAsync(tenantId, id, GetUsuarioId());
                return CreatedAtAction(nameof(FacturaController.ObtenerFacturaPorId), "Factura", new { tenantId, id = factura.Id }, factura);
            }
            catch (NegocioException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }

    // DTO local para cambiar estado
    public class CambiarEstadoDto
    {
        public string EstadoNuevo { get; set; } = string.Empty;
        public string? Comentario { get; set; }
    }
}