// IGeneradorNumerosService.cs
namespace ERPFabrica.Application.Interfaces
{
    public interface IGeneradorNumerosService
    {
        Task<string> GenerarSiguienteNumeroAsync(int tenantId, string entidad);
    }
}