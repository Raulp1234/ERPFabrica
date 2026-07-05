// IMovimientoInventarioService.cs
using ERPFabrica.Application.DTOs;

namespace ERPFabrica.Application.Interfaces
{
    public interface IMovimientoInventarioService
    {
        Task<List<MovimientoInventarioDto>> GetHistorialAsync(int productoId, int? almacenId = null);
        Task<MovimientoInventarioDto> RegistrarEntradaAsync(RegistrarMovimientoDto dto);
        Task<MovimientoInventarioDto> RegistrarSalidaAsync(RegistrarMovimientoDto dto);
    }
}