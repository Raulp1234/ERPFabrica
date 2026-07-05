// Enums.cs
namespace ERPFabrica.Domain.Enums
{
    public enum TipoTercero
    {
        Cliente = 1,
        Proveedor = 2,
        Ambos = 3
    }

    public enum TipoProducto
    {
        Simple = 1,
        Compuesto = 2,
        Servicio = 3
    }

    public enum TipoMovimiento
    {
        Entrada = 1,
        Salida = 2,
        Ajuste = 3,
        Reserva = 4,
        CancelacionReserva = 5
    }

    public enum TipoSolicitud
    {
        PedidoCliente = 1,
        OrdenCompra = 2
    }

    public enum EstadoSolicitud
    {
        Borrador = 1,
        Pendiente = 2,
        Aprobado = 3,
        EnProceso = 4,
        Entregado = 5,
        Cancelado = 6
    }

    public enum TipoFactura
    {
        Venta = 1,
        Compra = 2,
        NotaCredito = 3
    }

    public enum EstadoFactura
    {
        Borrador = 1,
        Emitida = 2,
        PagadaParcial = 3,
        Pagada = 4,
        Anulada = 5
    }

    public enum TipoDato
    {
        String = 1,
        Int = 2,
        Decimal = 3,
        Date = 4,
        Bool = 5,
        Select = 6
    }
}