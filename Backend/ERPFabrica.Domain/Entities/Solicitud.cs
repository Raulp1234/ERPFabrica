// Solicitud.cs
using ERPFabrica.Domain.Enums;

namespace ERPFabrica.Domain.Entities
{
    public class Solicitud : IHasTenant, IHasSoftDelete
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public string NumeroSolicitud { get; set; } = string.Empty;
        public TipoSolicitud TipoSolicitud { get; set; }
        public int TerceroId { get; set; }
        public EstadoSolicitud Estado { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaLimite { get; set; }
        public string? Notas { get; set; }
        public int? UsuarioId { get; set; }
        public bool IsDeleted { get; set; }

        public Tercero Tercero { get; set; } = null!;
        public ICollection<LineaSolicitud> Lineas { get; set; } = new List<LineaSolicitud>();
    }
}