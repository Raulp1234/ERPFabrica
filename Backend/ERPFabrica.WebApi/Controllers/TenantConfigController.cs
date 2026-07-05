using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ERPFabrica.Application.Interfaces;
using ERPFabrica.Application.DTOs;
using ERPFabrica.Application.Exceptions;

namespace ERPFabrica.WebApi.Controllers
{
    [ApiController]
    [Route("api/{tenantId}/config")]
    [Authorize]
    public class TenantConfigController : ControllerBase
    {
        private readonly ITenantService _tenantService;

        public TenantConfigController(ITenantService tenantService)
        {
            _tenantService = tenantService;
        }

        private void VerificarAdmin()
        {
            var esAdmin = User.FindFirst("EsAdmin")?.Value;
            if (string.IsNullOrEmpty(esAdmin) || !bool.TryParse(esAdmin, out var admin) || !admin)
                throw new NegocioException("Solo administradores pueden modificar la configuración.");
        }

        [HttpGet]
        [ProducesResponseType(typeof(TenantConfigDto), 200)]
        public async Task<IActionResult> ObtenerConfig(int tenantId)
        {
            return Ok(await _tenantService.ObtenerConfiguracionAsync(tenantId));
        }

        [HttpPut]
        [ProducesResponseType(typeof(TenantConfigDto), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> ActualizarConfig(int tenantId, [FromBody] TenantConfigDto dto)
        {
            VerificarAdmin();
            try
            {
                var actualizado = await _tenantService.ActualizarConfiguracionAsync(tenantId, dto);
                return Ok(actualizado);
            }
            catch (NegocioException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}