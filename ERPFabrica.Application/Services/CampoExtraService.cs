using Microsoft.EntityFrameworkCore;
using ERPFabrica.Application.DTOs;
using ERPFabrica.Application.Exceptions;
using ERPFabrica.Application.Interfaces;
using ERPFabrica.Domain.Entities;
using ERPFabrica.Domain.Enums;

namespace ERPFabrica.Application.Services
{
    public class CampoExtraService : ICampoExtraService
    {
        private readonly IApplicationDbContext _context;
        private readonly ITenantProvider _tenantProvider;

        public CampoExtraService(IApplicationDbContext context, ITenantProvider tenantProvider)
        {
            _context = context;
            _tenantProvider = tenantProvider;
        }

        private void ValidarTenant(int tenantId)
        {
            if (tenantId != _tenantProvider.TenantId)
                throw new NegocioException("Tenant inválido.");
        }

        public async Task<List<CampoExtraDefinicionDto>> ObtenerDefinicionesAsync(int tenantId, string entidad)
        {
            ValidarTenant(tenantId);
            return await _context.CamposExtraDefiniciones
                .Where(d => d.TenantId == tenantId && d.Entidad == entidad)
                .OrderBy(d => d.Orden)
                .Select(d => new CampoExtraDefinicionDto
                {
                    Id = d.Id,
                    Entidad = d.Entidad,
                    NombreCampo = d.NombreCampo,
                    TipoDato = d.TipoDato.ToString(),
                    EsRequerido = d.EsRequerido,
                    OpcionesJson = d.OpcionesJson,
                    Orden = d.Orden
                }).ToListAsync();
        }

        public async Task<CampoExtraDefinicionDto> CrearDefinicionAsync(int tenantId, CrearCampoExtraDefinicionDto dto)
        {
            ValidarTenant(tenantId);
            var def = new CampoExtraDefinicion
            {
                TenantId = tenantId,
                Entidad = dto.Entidad,
                NombreCampo = dto.NombreCampo,
                TipoDato = Enum.Parse<TipoDato>(dto.TipoDato),
                EsRequerido = dto.EsRequerido,
                OpcionesJson = dto.OpcionesJson,
                Orden = dto.Orden
            };
            _context.CamposExtraDefiniciones.Add(def);
            await _context.SaveChangesAsync();
            return new CampoExtraDefinicionDto
            {
                Id = def.Id,
                Entidad = def.Entidad,
                NombreCampo = def.NombreCampo,
                TipoDato = def.TipoDato.ToString(),
                EsRequerido = def.EsRequerido,
                OpcionesJson = def.OpcionesJson,
                Orden = def.Orden
            };
        }

        public async Task<CampoExtraDefinicionDto> ActualizarDefinicionAsync(int tenantId, int id, ActualizarCampoExtraDefinicionDto dto)
        {
            ValidarTenant(tenantId);
            var def = await _context.CamposExtraDefiniciones.FindAsync(id)
                ?? throw new NegocioException("Definición no encontrada.");
            def.NombreCampo = dto.NombreCampo;
            def.TipoDato = Enum.Parse<TipoDato>(dto.TipoDato);
            def.EsRequerido = dto.EsRequerido;
            def.OpcionesJson = dto.OpcionesJson;
            def.Orden = dto.Orden;
            await _context.SaveChangesAsync();
            return new CampoExtraDefinicionDto
            {
                Id = def.Id,
                Entidad = def.Entidad,
                NombreCampo = def.NombreCampo,
                TipoDato = def.TipoDato.ToString(),
                EsRequerido = def.EsRequerido,
                OpcionesJson = def.OpcionesJson,
                Orden = def.Orden
            };
        }

        public async Task EliminarDefinicionAsync(int tenantId, int id)
        {
            ValidarTenant(tenantId);
            var def = await _context.CamposExtraDefiniciones.FindAsync(id)
                ?? throw new NegocioException("Definición no encontrada.");
            // Eliminar valores asociados (cascade en BD, pero por si acaso)
            _context.ValoresCampoExtra.RemoveRange(
                _context.ValoresCampoExtra.Where(v => v.CampoExtraDefinicionId == id)
            );
            _context.CamposExtraDefiniciones.Remove(def);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ValorCampoExtraDto>> ObtenerValoresAsync(int tenantId, string entidad, string registroId)
        {
            ValidarTenant(tenantId);
            return await _context.ValoresCampoExtra
                .Where(v => v.Definicion.TenantId == tenantId && v.Definicion.Entidad == entidad && v.RegistroId == registroId)
                .Select(v => new ValorCampoExtraDto
                {
                    Id = v.Id,
                    CampoExtraDefinicionId = v.CampoExtraDefinicionId,
                    ValorString = v.ValorString,
                    ValorDecimal = v.ValorDecimal,
                    ValorDate = v.ValorDate,
                    ValorBool = v.ValorBool
                }).ToListAsync();
        }

        public async Task GuardarValoresAsync(int tenantId, string entidad, string registroId, List<ValorCampoExtraDto> valores)
        {
            ValidarTenant(tenantId);
            // Eliminar valores existentes para ese registro y entidad
            var existentes = await _context.ValoresCampoExtra
                .Where(v => v.Definicion.TenantId == tenantId && v.Definicion.Entidad == entidad && v.RegistroId == registroId)
                .ToListAsync();
            _context.ValoresCampoExtra.RemoveRange(existentes);

            // Insertar los nuevos valores
            foreach (var val in valores)
            {
                var nuevo = new ValorCampoExtra
                {
                    RegistroId = registroId,
                    CampoExtraDefinicionId = val.CampoExtraDefinicionId,
                    ValorString = val.ValorString,
                    ValorDecimal = val.ValorDecimal,
                    ValorDate = val.ValorDate,
                    ValorBool = val.ValorBool
                };
                _context.ValoresCampoExtra.Add(nuevo);
            }
            await _context.SaveChangesAsync();
        }
    }
}