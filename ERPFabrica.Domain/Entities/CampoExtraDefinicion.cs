// CampoExtraDefinicion.cs
using ERPFabrica.Domain.Enums;

namespace ERPFabrica.Domain.Entities
{
    public class CampoExtraDefinicion : IHasTenant
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public string Entidad { get; set; } = string.Empty; // "Producto", "Tercero", "Factura", "Solicitud"
        public string NombreCampo { get; set; } = string.Empty;
        public TipoDato TipoDato { get; set; }
        public bool EsRequerido { get; set; }
        public string? OpcionesJson { get; set; } // JSON para opciones de dropdown
        public int Orden { get; set; }
    }
}