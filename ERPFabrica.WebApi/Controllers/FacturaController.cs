using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ERPFabrica.Application.DTOs;
using ERPFabrica.Application.Interfaces;
using ERPFabrica.Domain.Enums;
using ERPFabrica.Application.Exceptions;

namespace ERPFabrica.WebApi.Controllers
{
    [ApiController]
    [Route("api/{tenantId}/facturas")]
    [Authorize]
    public class FacturaController : ControllerBase
    {
        private readonly IFacturaService _facturaService;

        public FacturaController(IFacturaService facturaService)
        {
            _facturaService = facturaService;
        }

        private int GetUsuarioId() =>
            int.Parse(User.FindFirst("userId")?.Value ?? throw new NegocioException("Usuario no identificado en token."));

        [HttpGet]
        [ProducesResponseType(typeof(List<FacturaDto>), 200)]
        public async Task<IActionResult> ObtenerFacturas(int tenantId, [FromQuery] string? estado = null)
        {
            EstadoFactura? estadoEnum = null;
            if (!string.IsNullOrWhiteSpace(estado) && Enum.TryParse<EstadoFactura>(estado, out var parsed))
                estadoEnum = parsed;

            return Ok(await _facturaService.ObtenerFacturasAsync(tenantId, estadoEnum));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(FacturaDto), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> ObtenerFacturaPorId(int tenantId, int id)
        {
            var factura = await _facturaService.ObtenerFacturaPorIdAsync(tenantId, id);
            return factura == null ? NotFound() : Ok(factura);
        }

        [HttpPost]
        [ProducesResponseType(typeof(FacturaDto), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CrearFactura(int tenantId, [FromBody] CrearFacturaDto dto)
        {
            try
            {
                var factura = await _facturaService.CrearFacturaAsync(tenantId, dto, GetUsuarioId());
                return CreatedAtAction(nameof(ObtenerFacturaPorId), new { tenantId, id = factura.Id }, factura);
            }
            catch (NegocioException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("{id}/emitir")]
        [ProducesResponseType(typeof(FacturaDto), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> EmitirFactura(int tenantId, int id)
        {
            try
            {
                var factura = await _facturaService.EmitirFacturaAsync(tenantId, id, GetUsuarioId());
                return Ok(factura);
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

        [HttpPost("{id}/pagos")]
        [ProducesResponseType(typeof(FacturaDto), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> RegistrarPago(int tenantId, int id, [FromBody] RegistrarPagoDto dto)
        {
            try
            {
                var factura = await _facturaService.RegistrarPagoAsync(tenantId, id, dto, GetUsuarioId());
                return Ok(factura);
            }
            catch (NegocioException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("{id}/anular")]
        [ProducesResponseType(typeof(FacturaDto), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> AnularFactura(int tenantId, int id)
        {
            try
            {
                await _facturaService.AnularFacturaAsync(tenantId, id, GetUsuarioId());
                return Ok(new { mensaje = "Factura anulada correctamente." });
            }
            catch (NegocioException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("terceros/{terceroId}/saldo-pendiente")]
        [ProducesResponseType(typeof(decimal), 200)]
        public async Task<IActionResult> ObtenerSaldoPendiente(int tenantId, int terceroId)
        {
            var saldo = await _facturaService.ObtenerSaldoPendienteTerceroAsync(tenantId, terceroId);
            return Ok(saldo);
        }
    }
}