// GeneradorNumerosService.cs
using ERPFabrica.Application.Exceptions;
using ERPFabrica.Application.Interfaces;
using ERPFabrica.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ERPFabrica.Application.Services
{
    public class GeneradorNumerosService : IGeneradorNumerosService
    {
        private readonly IApplicationDbContext _context;
        private readonly ITenantProvider _tenantProvider;

        public GeneradorNumerosService(IApplicationDbContext context, ITenantProvider tenantProvider)
        {
            _context = context;
            _tenantProvider = tenantProvider;
        }

        public async Task<string> GenerarSiguienteNumeroAsync(int tenantId, string entidad)
        {
            // Validación de tenant
            if (tenantId != _tenantProvider.TenantId)
                throw new NegocioException("Tenant inválido.");

            int año = DateTime.UtcNow.Year;
            var secuencia = await _context.SecuenciasNumeros
                .FirstOrDefaultAsync(s => s.TenantId == tenantId && s.Entidad == entidad && s.Año == año);

            if (secuencia == null)
            {
                secuencia = new SecuenciasNumeros
                {
                    TenantId = tenantId,
                    Entidad = entidad,
                    Año = año,
                    UltimoNumero = 1
                };
                _context.SecuenciasNumeros.Add(secuencia);
            }
            else
            {
                secuencia.UltimoNumero++;
            }

            await _context.SaveChangesAsync();

            string prefijo = entidad switch
            {
                "Solicitud" => "SOL",
                "Factura" => "FACT",
                _ => entidad.ToUpper()
            };
            return $"{prefijo}-{año}-{secuencia.UltimoNumero:D6}";
        }
    }
}