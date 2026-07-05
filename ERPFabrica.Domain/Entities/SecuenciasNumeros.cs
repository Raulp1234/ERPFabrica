namespace ERPFabrica.Domain.Entities
{
    public class SecuenciasNumeros : IHasTenant
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public string Entidad { get; set; } = string.Empty;  // "Solicitud", "Factura"
        public int Año { get; set; }
        public int UltimoNumero { get; set; }
    }
}