// SolicitudDto.cs
namespace ERPFabrica.Application.DTOs
{
    public class SolicitudDto
    {
        public int Id { get; set; }
        public string NumeroSolicitud { get; set; } = string.Empty;
        public string TipoSolicitud { get; set; } = string.Empty;
        public int TerceroId { get; set; }
        public string TerceroNombre { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaLimite { get; set; }
        public string? Notas { get; set; }
        public List<LineaSolicitudDto> Lineas { get; set; } = new();
    }

    public class LineaSolicitudDto
    {
        public int Id { get; set; }
        public int? ProductoId { get; set; }
        public string ProductoNombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public decimal CantidadSolicitada { get; set; }
        public decimal CantidadEntregada { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Impuesto { get; set; }
        public decimal TotalLinea { get; set; }
        public bool Facturada { get; set; }
    }

    public class CrearSolicitudDto
    {
        public string TipoSolicitud { get; set; } = "PedidoCliente";
        public int TerceroId { get; set; }
        public DateTime? FechaLimite { get; set; }
        public string? Notas { get; set; }
        public List<CrearLineaSolicitudDto> Lineas { get; set; } = new();
    }

    public class CrearLineaSolicitudDto
    {
        public int? ProductoId { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public decimal Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Impuesto { get; set; }
    }
}