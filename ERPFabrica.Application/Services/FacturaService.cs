// FacturaService.cs
using ERPFabrica.Application.DTOs;
using ERPFabrica.Application.Exceptions;
using ERPFabrica.Application.Interfaces;
using ERPFabrica.Domain.Entities;
using ERPFabrica.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace ERPFabrica.Application.Services
{
    public class FacturaService : IFacturaService
    {
        private readonly IApplicationDbContext _context;
        private readonly ITenantProvider _tenantProvider;
        private readonly IGeneradorNumerosService _generadorNumeros;
        private readonly IMovimientoInventarioService _movimientoService;

        public FacturaService(IApplicationDbContext context, ITenantProvider tenantProvider,
                              IGeneradorNumerosService generadorNumeros,
                              IMovimientoInventarioService movimientoService)
        {
            _context = context;
            _tenantProvider = tenantProvider;
            _generadorNumeros = generadorNumeros;
            _movimientoService = movimientoService;
        }

        public async Task<FacturaDto> CrearFacturaAsync(int tenantId, CrearFacturaDto dto, int usuarioId)
        {
            ValidarTenant(tenantId);

            string numero = await _generadorNumeros.GenerarSiguienteNumeroAsync(tenantId, "Factura");
            var factura = new Factura
            {
                TenantId = tenantId,
                NumeroFactura = numero,
                TipoFactura = Enum.Parse<TipoFactura>(dto.TipoFactura),
                SolicitudId = dto.SolicitudId,
                TerceroId = dto.TerceroId,
                Estado = EstadoFactura.Borrador,
                FechaEmision = DateTime.UtcNow,
                FechaVencimiento = dto.FechaVencimiento,
                Notas = dto.Notas,
                UsuarioId = usuarioId
            };

            decimal subTotal = 0, impuestos = 0;
            foreach (var lineaDto in dto.Lineas)
            {
                var linea = new LineaFactura
                {
                    ProductoId = lineaDto.ProductoId,
                    Descripcion = lineaDto.Descripcion,
                    Cantidad = lineaDto.Cantidad,
                    PrecioUnitario = lineaDto.PrecioUnitario,
                    Impuesto = lineaDto.Impuesto,
                    TotalLinea = lineaDto.Cantidad * lineaDto.PrecioUnitario * (1 + lineaDto.Impuesto / 100)
                };
                factura.Lineas.Add(linea);
                subTotal += lineaDto.Cantidad * lineaDto.PrecioUnitario;
                impuestos += lineaDto.Cantidad * lineaDto.PrecioUnitario * (lineaDto.Impuesto / 100);
            }

            factura.SubTotal = subTotal;
            factura.TotalImpuestos = impuestos;
            factura.TotalDescuento = 0;
            factura.Total = subTotal + impuestos;

            _context.Facturas.Add(factura);
            await _context.SaveChangesAsync();
            return await MapearADtoAsync(factura);
        }

        public async Task<FacturaDto> EmitirFacturaAsync(int tenantId, int facturaId, int usuarioId)
        {
            ValidarTenant(tenantId);
            var factura = await _context.Facturas
                .Include(f => f.Lineas)
                .FirstOrDefaultAsync(f => f.Id == facturaId)
                ?? throw new NegocioException("Factura no encontrada.");

            if (factura.Estado != EstadoFactura.Borrador)
                throw new EstadoInvalidoException("Solo facturas en estado 'Borrador' pueden emitirse.");

            // Obtener almacén principal (asumimos el primero marcado como principal)
            var almacenPrincipal = await _context.Almacenes
                .FirstOrDefaultAsync(a => a.TenantId == tenantId && a.EsPrincipal);
            if (almacenPrincipal == null)
                throw new NegocioException("No se encontró un almacén principal para el tenant.");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Generar movimiento de salida por cada línea con producto
                foreach (var linea in factura.Lineas.Where(l => l.ProductoId.HasValue))
                {
                    var dto = new RegistrarMovimientoDto
                    {
                        ProductoId = linea.ProductoId!.Value,
                        AlmacenId = almacenPrincipal.Id,
                        TipoMovimiento = "Salida",
                        Cantidad = linea.Cantidad,
                        PrecioUnitario = linea.PrecioUnitario,
                        DocumentoReferencia = factura.NumeroFactura,
                        Motivo = $"Venta factura {factura.NumeroFactura}"
                    };
                    await _movimientoService.RegistrarSalidaAsync(dto);
                }

                // Actualizar estado y fecha de emisión
                factura.Estado = EstadoFactura.Emitida;
                factura.FechaEmision = DateTime.UtcNow;

                // Actualizar saldo pendiente del tercero
                var tercero = await _context.Terceros.FindAsync(factura.TerceroId);
                if (tercero != null)
                    tercero.SaldoPendiente += factura.Total;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return await MapearADtoAsync(factura);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<FacturaDto> RegistrarPagoAsync(int tenantId, int facturaId, RegistrarPagoDto dto, int usuarioId)
        {
            ValidarTenant(tenantId);
            var factura = await _context.Facturas
                .Include(f => f.Pagos)
                .FirstOrDefaultAsync(f => f.Id == facturaId)
                ?? throw new NegocioException("Factura no encontrada.");

            if (factura.Estado != EstadoFactura.Emitida && factura.Estado != EstadoFactura.PagadaParcial)
                throw new EstadoInvalidoException("Solo se pueden registrar pagos en facturas Emitidas o con Pagos Parciales.");

            var pago = new Pago
            {
                FacturaId = facturaId,
                FechaPago = DateTime.UtcNow,
                Monto = dto.Monto,
                MetodoPago = dto.MetodoPago,
                Referencia = dto.Referencia,
                UsuarioId = usuarioId
            };
            _context.Pagos.Add(pago);

            // Actualizar saldo pendiente del tercero
            var tercero = await _context.Terceros.FindAsync(factura.TerceroId);
            if (tercero != null)
                tercero.SaldoPendiente -= dto.Monto;

            // Calcular total pagado
            decimal totalPagado = factura.Pagos.Sum(p => p.Monto) + dto.Monto;
            if (totalPagado >= factura.Total)
                factura.Estado = EstadoFactura.Pagada;
            else if (totalPagado > 0)
                factura.Estado = EstadoFactura.PagadaParcial;

            await _context.SaveChangesAsync();
            return await MapearADtoAsync(factura);
        }

        public async Task AnularFacturaAsync(int tenantId, int facturaId, int usuarioId)
        {
            ValidarTenant(tenantId);
            var factura = await _context.Facturas
                .Include(f => f.Lineas)
                .FirstOrDefaultAsync(f => f.Id == facturaId)
                ?? throw new NegocioException("Factura no encontrada.");

            if (factura.Estado != EstadoFactura.Emitida && factura.Estado != EstadoFactura.PagadaParcial)
                throw new EstadoInvalidoException("Solo se pueden anular facturas Emitidas o con Pagos Parciales.");

            var almacenPrincipal = await _context.Almacenes
                .FirstOrDefaultAsync(a => a.TenantId == tenantId && a.EsPrincipal)
                ?? throw new NegocioException("Almacén principal no encontrado.");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Reversión de inventario: entrada por cada línea con producto
                foreach (var linea in factura.Lineas.Where(l => l.ProductoId.HasValue))
                {
                    var dto = new RegistrarMovimientoDto
                    {
                        ProductoId = linea.ProductoId!.Value,
                        AlmacenId = almacenPrincipal.Id,
                        TipoMovimiento = "Entrada",
                        Cantidad = linea.Cantidad,
                        PrecioUnitario = linea.PrecioUnitario,
                        DocumentoReferencia = "ANULACION-" + factura.NumeroFactura,
                        Motivo = $"Anulación factura {factura.NumeroFactura}"
                    };
                    await _movimientoService.RegistrarEntradaAsync(dto);
                }

                // Revertir saldo del tercero
                var tercero = await _context.Terceros.FindAsync(factura.TerceroId);
                if (tercero != null)
                    tercero.SaldoPendiente -= factura.Total;

                factura.Estado = EstadoFactura.Anulada;
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<List<FacturaDto>> ObtenerFacturasAsync(int tenantId, EstadoFactura? estado = null)
        {
            ValidarTenant(tenantId);
            var query = _context.Facturas
                .Include(f => f.Lineas).Include(f => f.Pagos).AsQueryable();
            if (estado.HasValue)
                query = query.Where(f => f.Estado == estado.Value);
            var facturas = await query.ToListAsync();
            var dtos = new List<FacturaDto>();
            foreach (var f in facturas)
                dtos.Add(await MapearADtoAsync(f));
            return dtos;
        }

        public async Task<FacturaDto> ObtenerFacturaPorIdAsync(int tenantId, int facturaId)
        {
            ValidarTenant(tenantId);
            var factura = await _context.Facturas
                .Include(f => f.Lineas).Include(f => f.Pagos)
                .FirstOrDefaultAsync(f => f.Id == facturaId);
            return factura == null ? null : await MapearADtoAsync(factura);
        }

        public async Task<decimal> ObtenerSaldoPendienteTerceroAsync(int tenantId, int terceroId)
        {
            ValidarTenant(tenantId);
            var tercero = await _context.Terceros.FirstOrDefaultAsync(t => t.Id == terceroId);
            return tercero?.SaldoPendiente ?? 0;
        }

        private void ValidarTenant(int tenantId)
        {
            if (tenantId != _tenantProvider.TenantId)
                throw new NegocioException("Tenant inválido.");
        }

        private async Task<FacturaDto> MapearADtoAsync(Factura f)
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