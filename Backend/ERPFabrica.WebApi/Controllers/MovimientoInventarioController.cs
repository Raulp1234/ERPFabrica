// MovimientoInventarioController.cs
using ERPFabrica.Application.DTOs;
using ERPFabrica.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERPFabrica.WebApi.Controllers
{
    [ApiController]
    [Route("api/{tenantId}/[controller]")]
    [Authorize]
    public class MovimientoInventarioController : ControllerBase
    {
        private readonly IMovimientoInventarioService _movimientoService;

        public MovimientoInventarioController(IMovimientoInventarioService movimientoService)
        {
            _movimientoService = movimientoService;
        }

        [HttpGet("historial/{productoId}")]
        public async Task<ActionResult<List<MovimientoInventarioDto>>> GetHistorial(int productoId, int? almacenId = null)
        {
            return Ok(await _movimientoService.GetHistorialAsync(productoId, almacenId));
        }

        [HttpPost("entrada")]
        public async Task<ActionResult<MovimientoInventarioDto>> RegistrarEntrada(RegistrarMovimientoDto dto)
        {
            var resultado = await _movimientoService.RegistrarEntradaAsync(dto);
            return CreatedAtAction(nameof(GetHistorial), new { tenantId = RouteData.Values["tenantId"], productoId = dto.ProductoId }, resultado);
        }

        [HttpPost("salida")]
        public async Task<ActionResult<MovimientoInventarioDto>> RegistrarSalida(RegistrarMovimientoDto dto)
        {
            var resultado = await _movimientoService.RegistrarSalidaAsync(dto);
            return CreatedAtAction(nameof(GetHistorial), new { tenantId = RouteData.Values["tenantId"], productoId = dto.ProductoId }, resultado);
        }
    }
}