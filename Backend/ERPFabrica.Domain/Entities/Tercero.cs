// Tercero.cs
using ERPFabrica.Domain.Enums;

namespace ERPFabrica.Domain.Entities
{
    public class Tercero : IHasTenant, IHasSoftDelete
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public TipoTercero Tipo { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string DocumentoIdentidad { get; set; } = string.Empty;
        public int? CategoriaTerceroId { get; set; }
        public decimal SaldoPendiente { get; set; }
        public bool EsActivo { get; set; }
        public bool IsDeleted { get; set; }

        public CategoriaTercero? Categoria { get; set; }
    }
}