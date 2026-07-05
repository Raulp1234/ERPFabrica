// ProductoController.cs
using ERPFabrica.Application.DTOs;
using ERPFabrica.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERPFabrica.WebApi.Controllers
{
    [ApiController]
    [Route("api/{tenantId}/[controller]")]
    [Authorize]
    public class ProductoController : ControllerBase
    {
        private readonly IProductoService _productoService;

        public ProductoController(IProductoService productoService)
        {
            _productoService = productoService;
        }

        [HttpGet]
        public async Task<ActionResult<List<ProductoDto>>> GetAll()
        {
            return Ok(await _productoService.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductoDto>> GetById(int id)
        {
            var producto = await _productoService.GetByIdAsync(id);
            if (producto == null) return NotFound();
            return Ok(producto);
        }

        [HttpPost]
        public async Task<ActionResult<ProductoDto>> Create(CrearProductoDto dto)
        {
            var creado = await _productoService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { tenantId = RouteData.Values["tenantId"], id = creado.Id }, creado);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, CrearProductoDto dto)
        {
            await _productoService.UpdateAsync(id, dto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _productoService.DeleteAsync(id);
            return NoContent();
        }
    }
}