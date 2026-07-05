// HistorialEstadoSolicitud.cs
using ERPFabrica.Domain.Enums;

namespace ERPFabrica.Domain.Entities
{
    public class HistorialEstadoSolicitud
    {
        public int Id { get; set; }
        public int SolicitudId { get; set; }
        public EstadoSolicitud EstadoAnterior { get; set; }
        public EstadoSolicitud EstadoNuevo { get; set; }
        public DateTime FechaCambio { get; set; }
        public int? UsuarioId { get; set; }
        public string? Comentario { get; set; }

        public Solicitud Solicitud { get; set; } = null!;
    }
}