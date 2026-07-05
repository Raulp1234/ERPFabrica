// SolicitudService.cs
using ERPFabrica.Application.DTOs;
using ERPFabrica.Application.Exceptions;
using ERPFabrica.Application.Interfaces;
using ERPFabrica.Domain.Entities;
using ERPFabrica.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace ERPFabrica.Application.Services
{
    public class SolicitudService : ISolicitudService
    {
        private readonly IApplicationDbContext _context;
        private readonly ITenantProvider _tenantProvider;
        private readonly IGeneradorNumerosService _generadorNumeros;

        public SolicitudService(IApplicationDbContext context, ITenantProvider tenantProvider, IGeneradorNumerosService generadorNumeros)
        {
            _context = context;
            _tenantProvider = tenantProvider;
            _generadorNumeros = generadorNumeros;
        }

        public async Task<SolicitudDto> CrearSolicitudAsync(int tenantId, CrearSolicitudDto dto, int usuarioId)
        {
            ValidarTenant(tenantId);

            // Generar número secuencial
            string numero = await _generadorNumeros.GenerarSiguienteNumeroAsync(tenantId, "Solicitud");

            // Mapear a entidad
            var solicitud = new Solicitud
            {
                TenantId = tenantId,
                NumeroSolicitud = numero,
                TipoSolicitud = Enum.Parse<TipoSolicitud>(dto.TipoSolicitud),
                TerceroId = dto.TerceroId,
                Estado = EstadoSolicitud.Borrador,
                FechaCreacion = DateTime.UtcNow,
                FechaLimite = dto.FechaLimite,
                Notas = dto.Notas,
                UsuarioId = usuarioId
            };

            // Agregar líneas
            foreach (var lineaDto in dto.Lineas)
            {
                var linea = new LineaSolicitud
                {
                    ProductoId = lineaDto.ProductoId,
                    Descripcion = lineaDto.Descripcion,
                    CantidadSolicitada = lineaDto.Cantidad,
                    CantidadEntregada = 0,
                    PrecioUnitario = lineaDto.PrecioUnitario,
                    Impuesto = lineaDto.Impuesto,
                    TotalLinea = lineaDto.Cantidad * lineaDto.PrecioUnitario * (1 + lineaDto.Impuesto / 100),
                    Facturada = false
                };
                solicitud.Lineas.Add(linea);
            }

            _context.Solicitudes.Add(solicitud);

            // Registrar historial inicial
            var historial = new HistorialEstadoSolicitud
            {
                Solicitud = solicitud,
                EstadoAnterior = EstadoSolicitud.Borrador,  // no hay anterior
                EstadoNuevo = EstadoSolicitud.Borrador,
                FechaCambio = DateTime.UtcNow,
                UsuarioId = usuarioId,
                Comentario = "Solicitud creada"
            };
            _context.HistorialesEstado.Add(historial);

            await _context.SaveChangesAsync();
            return await MapearADtoAsync(solicitud);
        }

        public async Task<SolicitudDto> CambiarEstadoAsync(int tenantId, int solicitudId, EstadoSolicitud nuevoEstado, int usuarioId, string? comentario = null)
        {
            ValidarTenant(tenantId);

            var solicitud = await _context.Solicitudes
                .Include(s => s.Lineas)
                .FirstOrDefaultAsync(s => s.Id == solicitudId)
                ?? throw new NegocioException("Solicitud no encontrada.");

            var estadoActual = solicitud.Estado;
            ValidarTransicion(estadoActual, nuevoEstado);

            // Validación de stock estricto cuando se aprueba
            if (nuevoEstado == EstadoSolicitud.Aprobado)
            {
                var config = await _context.TenantConfigs.FirstOrDefaultAsync(c => c.TenantId == tenantId);
                if (config?.ControlStockEstricto == true)
                {
                    foreach (var linea in solicitud.Lineas.Where(l => l.ProductoId.HasValue))
                    {
                        var stockTotal = await _context.Stocks
                            .Where(s => s.ProductoId == linea.ProductoId.Value)
                            .SumAsync(s => s.CantidadActual);
                        if (stockTotal < linea.CantidadSolicitada)
                        {
                            var producto = await _context.Productos.FindAsync(linea.ProductoId.Value);
                            throw new StockInsuficienteException(producto?.Nombre ?? "Desconocido",
                                linea.CantidadSolicitada, stockTotal);
                        }
                    }
                }
            }

            // Al entregar, marcar cantidad entregada total
            if (nuevoEstado == EstadoSolicitud.Entregado)
            {
                foreach (var linea in solicitud.Lineas)
                {
                    linea.CantidadEntregada = linea.CantidadSolicitada; // Entrega completa
                }
            }

            // Registrar historial del cambio
            var historial = new HistorialEstadoSolicitud
            {
                SolicitudId = solicitud.Id,
                EstadoAnterior = estadoActual,
                EstadoNuevo = nuevoEstado,
                FechaCambio = DateTime.UtcNow,
                UsuarioId = usuarioId,
                Comentario = comentario
            };
            _context.HistorialesEstado.Add(historial);

            solicitud.Estado = nuevoEstado;
            await _context.SaveChangesAsync();

            return await MapearADtoAsync(solicitud);
        }

        public async Task<FacturaDto> GenerarFacturaDesdeSolicitudAsync(int tenantId, int solicitudId, int usuarioId)
        {
            ValidarTenant(tenantId);

            var solicitud = await _context.Solicitudes
                .Include(s => s.Lineas)
                .FirstOrDefaultAsync(s => s.Id == solicitudId)
                ?? throw new NegocioException("Solicitud no encontrada.");

            if (solicitud.Estado != EstadoSolicitud.Entregado)
                throw new EstadoInvalidoException("Solo se puede generar factura desde una solicitud en estado 'Entregado'.");

            var lineasPendientes = solicitud.Lineas.Where(l => !l.Facturada).ToList();
            if (!lineasPendientes.Any())
                throw new NegocioException("No hay líneas pendientes de facturar.");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Crear factura en borrador
                var factura = new Factura
                {
                    TenantId = tenantId,
                    TipoFactura = TipoFactura.Venta,
                    SolicitudId = solicitud.Id,
                    TerceroId = solicitud.TerceroId,
                    Estado = EstadoFactura.Borrador,
                    FechaEmision = DateTime.UtcNow,
                    FechaVencimiento = null,
                    Notas = $"Generada desde solicitud {solicitud.NumeroSolicitud}",
                    UsuarioId = usuarioId
                };

                decimal subTotal = 0, totalImpuestos = 0;
                foreach (var linea in lineasPendientes)
                {
                    var lineaFactura = new LineaFactura
                    {
                        ProductoId = linea.ProductoId,
                        Descripcion = linea.Descripcion,
                        Cantidad = linea.CantidadSolicitada,  // ya entregada completa
                        PrecioUnitario = linea.PrecioUnitario,
                        Impuesto = linea.Impuesto,
                        TotalLinea = linea.CantidadSolicitada * linea.PrecioUnitario * (1 + linea.Impuesto / 100)
                    };
                    factura.Lineas.Add(lineaFactura);
                    subTotal += linea.CantidadSolicitada * linea.PrecioUnitario;
                    totalImpuestos += linea.CantidadSolicitada * linea.PrecioUnitario * (linea.Impuesto / 100);
                    linea.Facturada = true;
                }

                factura.SubTotal = subTotal;
                factura.TotalImpuestos = totalImpuestos;
                factura.TotalDescuento = 0;
                factura.Total = subTotal + totalImpuestos;
                factura.NumeroFactura = await _generadorNumeros.GenerarSiguienteNumeroAsync(tenantId, "Factura");

                _context.Facturas.Add(factura);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return await MapearFacturaADtoAsync(factura);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<List<SolicitudDto>> ObtenerSolicitudesAsync(int tenantId, EstadoSolicitud? estado = null)
        {
            ValidarTenant(tenantId);
            var query = _context.Solicitudes
                .Include(s => s.Lineas).AsQueryable();
            if (estado.HasValue)
                query = query.Where(s => s.Estado == estado.Value);
            var solicitudes = await query.ToListAsync();
            var dtos = new List<SolicitudDto>();
            foreach (var s in solicitudes)
                dtos.Add(await MapearADtoAsync(s));
            return dtos;
        }

        public async Task<SolicitudDto> ObtenerSolicitudPorIdAsync(int tenantId, int solicitudId)
        {
            ValidarTenant(tenantId);
            var solicitud = await _context.Solicitudes
                .Include(s => s.Lineas)
                .FirstOrDefaultAsync(s => s.Id == solicitudId);
            return solicitud == null ? null : await MapearADtoAsync(solicitud);
        }

        public async Task<List<HistorialEstadoDto>> ObtenerHistorialAsync(int tenantId, int solicitudId)
        {
            ValidarTenant(tenantId);
            return await _context.HistorialesEstado
                .Where(h => h.SolicitudId == solicitudId)
                .OrderBy(h => h.FechaCambio)
                .Select(h => new HistorialEstadoDto
                {
                    Id = h.Id,
                    EstadoAnterior = h.EstadoAnterior.ToString(),
                    EstadoNuevo = h.EstadoNuevo.ToString(),
                    FechaCambio = h.FechaCambio,
                    UsuarioId = h.UsuarioId,
                    Comentario = h.Comentario
                }).ToListAsync();
        }

        // Helpers
        private void ValidarTenant(int tenantId)
        {
            if (tenantId != _tenantProvider.TenantId)
                throw new NegocioException("Tenant inválido.");
        }

        private void ValidarTransicion(EstadoSolicitud actual, EstadoSolicitud nuevo)
        {
            // Transiciones permitidas
            bool permitido = nuevo switch
            {
                EstadoSolicitud.Cancelado => actual != EstadoSolicitud.Cancelado && actual != EstadoSolicitud.Entregado,
                EstadoSolicitud.Pendiente => actual == EstadoSolicitud.Borrador,
                EstadoSolicitud.Aprobado => actual == EstadoSolicitud.Pendiente,
                EstadoSolicitud.EnProceso => actual == EstadoSolicitud.Aprobado,
                EstadoSolicitud.Entregado => actual == EstadoSolicitud.EnProceso,
                _ => false
            };
            if (!permitido)
                throw new EstadoInvalidoException($"Transición de '{actual}' a '{nuevo}' no permitida.");
        }

        private async Task<SolicitudDto> MapearADtoAsync(Solicitud s)
        {
            var tercero = await _context.Terceros.FirstOrDefaultAsync(t => t.Id == s.TerceroId);
            return new SolicitudDto
            {
                Id = s.Id,
                NumeroSolicitud = s.NumeroSolicitud,
                TipoSolicitud = s.TipoSolicitud.ToString(),
                TerceroId = s.TerceroId,
                TerceroNombre = tercero?.Nombre ?? "",
                Estado = s.Estado.ToString(),
                FechaCreacion = s.FechaCreacion,
                FechaLimite = s.FechaLimite,
                Notas = s.Notas,
                Lineas = s.Lineas.Select(l => new LineaSolicitudDto
                {
                    Id = l.Id,
                    ProductoId = l.ProductoId,
                    ProductoNombre = l.Producto?.Nombre ?? "",
                    Descripcion = l.Descripcion,
                    CantidadSolicitada = l.CantidadSolicitada,
                    CantidadEntregada = l.CantidadEntregada,
                    PrecioUnitario = l.PrecioUnitario,
                    Impuesto = l.Impuesto,
                    TotalLinea = l.TotalLinea,
                    Facturada = l.Facturada
                }).ToList()
            };
        }

        private async Task<FacturaDto> MapearFacturaADtoAsync(Factura f)
        {
            var tercero = await _context.Terceros.FirstOrDefaultAsync(t => t.Id == f.TerceroId);
            return new FacturaDto
            {
                Id = f.Id,
                NumeroFactura = f.NumeroFactura,
                TipoFactura = f.TipoFactura.ToString(),
                SolicitudId = f.SolicitudId,
                TerceroId = f.TerceroId,
                TerceroNombre = tercero?.Nombre ?? "",
                Estado = f.Estado.ToString(),
                FechaEmision = f.FechaEmision,
                FechaVencimiento = f.FechaVencimiento,
                SubTotal = f.SubTotal,
                TotalImpuestos = f.TotalImpuestos,
                TotalDescuento = f.TotalDescuento,
                Total = f.Total,
                Notas = f.Notas,
                Lineas = f.Lineas.Select(l => new LineaFacturaDto
                {
                    Id = l.Id,
                    ProductoId = l.ProductoId,
                    ProductoNombre = l.Producto?.Nombre ?? "",
                    Descripcion = l.Descripcion,
                    Cantidad = l.Cantidad,
                    PrecioUnitario = l.PrecioUnitario,
                    Impuesto = l.Impuesto,
                    TotalLinea = l.TotalLinea
                }).ToList(),
                Pagos = f.Pagos.Select(p => new PagoDto
                {
                    Id = p.Id,
                    FechaPago = p.FechaPago,
                    Monto = p.Monto,
                    MetodoPago = p.MetodoPago,
                    Referencia = p.Referencia
                }).ToList()
            };
        }
    }
}