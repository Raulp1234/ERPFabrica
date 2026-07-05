// StockController.cs
using ERPFabrica.Application.DTOs;
using ERPFabrica.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERPFabrica.WebApi.Controllers
{
    [ApiController]
    [Route("api/{tenantId}/[controller]")]
    [Authorize]
    public class StockController : ControllerBase
    {
        private readonly IStockService _stockService;

        public StockController(IStockService stockService)
        {
            _stockService = stockService;
        }

        [HttpGet("producto/{productoId}")]
        public async Task<ActionResult<List<StockDto>>> GetPorProducto(int productoId)
        {
            return Ok(await _stockService.GetStockPorProductoAsync(productoId));
        }

        [HttpGet("producto/{productoId}/almacen/{almacenId}")]
        public async Task<ActionResult<StockDto>> GetActual(int productoId, int almacenId)
        {
            var stock = await _stockService.GetStockActualAsync(productoId, almacenId);
            if (stock == null) return NotFound();
            return Ok(stock);
        }

        [HttpGet("validar")]
        public async Task<ActionResult<bool>> ValidarDisponibilidad(int productoId, int almacenId, decimal cantidad)
        {
            return Ok(await _stockService.ValidarDisponibilidadAsync(productoId, almacenId, cantidad));
        }
    }
}