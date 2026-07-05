using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ERPFabrica.Application.Interfaces;
using ERPFabrica.Application.DTOs; // Asumo que IDashboardService devuelve estos DTOs

namespace ERPFabrica.WebApi.Controllers
{
    [ApiController]
    [Route("api/{tenantId}/dashboard")]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(DashboardDto), 200)]
        public async Task<IActionResult> ObtenerMetricas(int tenantId)
        {
            return Ok(await _dashboardService.ObtenerMetricasAsync(tenantId));
        }

        [HttpGet("ventas-por-categoria")]
        [ProducesResponseType(typeof(List<VentaCategoriaDto>), 200)]
        public async Task<IActionResult> ObtenerVentasPorCategoria(int tenantId, [FromQuery] DateTime? desde, [FromQuery] DateTime? hasta)
        {
            return Ok(await _dashboardService.ObtenerVentasPorCategoriaAsync(tenantId, desde, hasta));
        }
    }
}