using ERPFabrica.Application.DTOs;

namespace ERPFabrica.Application.Interfaces
{
    public interface ICampoExtraService
    {
        Task<List<CampoExtraDefinicionDto>> ObtenerDefinicionesAsync(int tenantId, string entidad);
        Task<CampoExtraDefinicionDto> CrearDefinicionAsync(int tenantId, CrearCampoExtraDefinicionDto dto);
        Task<CampoExtraDefinicionDto> ActualizarDefinicionAsync(int tenantId, int id, ActualizarCampoExtraDefinicionDto dto);
        Task EliminarDefinicionAsync(int tenantId, int id);
        Task<List<ValorCampoExtraDto>> ObtenerValoresAsync(int tenantId, string entidad, string registroId);
        Task GuardarValoresAsync(int tenantId, string entidad, string registroId, List<ValorCampoExtraDto> valores);
    }
}