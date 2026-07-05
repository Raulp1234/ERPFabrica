using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ERPFabrica.Application.Interfaces;
using ERPFabrica.Application.Exceptions;

namespace ERPFabrica.WebApi.Controllers
{
    [ApiController]
    [Route("api/{tenantId}/categorias")]
    [Authorize]
    public class CategoriaController : ControllerBase
    {
        private readonly ITerceroService _terceroService;    // para categorías de terceros
        private readonly IProductoService _productoService;  // para categorías de productos

        public CategoriaController(ITerceroService terceroService, IProductoService productoService)
        {
            _terceroService = terceroService;
            _productoService = productoService;
        }

        private void ValidarTipo(string tipo)
        {
            if (tipo != "producto" && tipo != "tercero")
                throw new NegocioException("Tipo de categoría inválido. Use 'producto' o 'tercero'.");
        }

        [HttpGet("{tipo}")]
        [ProducesResponseType(typeof(List<CategoriaDto>), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> ObtenerCategorias(int tenantId, string tipo)
        {
            ValidarTipo(tipo);
            if (tipo == "producto")
                return Ok(await _productoService.ObtenerCategoriasAsync(tenantId));
            else
                return Ok(await _terceroService.ObtenerCategoriasAsync(tenantId));
        }

        [HttpPost("{tipo}")]
        [ProducesResponseType(typeof(CategoriaDto), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CrearCategoria(int tenantId, string tipo, [FromBody] CrearCategoriaDto dto)
        {
            ValidarTipo(tipo);
            try
            {
                if (tipo == "producto")
                {
                    var creada = await _productoService.CrearCategoriaAsync(tenantId, dto);
                    return CreatedAtAction(nameof(ObtenerCategorias), new { tenantId, tipo, id = creada.Id }, creada);
                }
                else
                {
                    var creada = await _terceroService.CrearCategoriaAsync(tenantId, dto);
                    return CreatedAtAction(nameof(ObtenerCategorias), new { tenantId, tipo, id = creada.Id }, creada);
                }
            }
            catch (NegocioException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("{tipo}/{id}")]
        [ProducesResponseType(typeof(CategoriaDto), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> ActualizarCategoria(int tenantId, string tipo, int id, [FromBody] ActualizarCategoriaDto dto)
        {
            ValidarTipo(tipo);
            try
            {
                if (tipo == "producto")
                    return Ok(await _productoService.ActualizarCategoriaAsync(tenantId, id, dto));
                else
                    return Ok(await _terceroService.ActualizarCategoriaAsync(tenantId, id, dto));
            }
            catch (NegocioException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("{tipo}/{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> EliminarCategoria(int tenantId, string tipo, int id)
        {
            ValidarTipo(tipo);
            try
            {
                if (tipo == "producto")
                    await _productoService.EliminarCategoriaAsync(tenantId, id);
                else
                    await _terceroService.EliminarCategoriaAsync(tenantId, id);
                return NoContent();
            }
            catch (NegocioException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}