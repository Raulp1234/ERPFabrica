namespace ERPFabrica.Application.DTOs
{
    public class TerceroDto
    {
        public int Id { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string DocumentoIdentidad { get; set; } = string.Empty;
        public int? CategoriaTerceroId { get; set; }
        public string CategoriaNombre { get; set; } = string.Empty;
        public decimal SaldoPendiente { get; set; }
        public bool EsActivo { get; set; }
    }

    public class CrearTerceroDto
    {
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Tipo { get; set; } = "Cliente"; // Cliente, Proveedor, Ambos
        public string Email { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string DocumentoIdentidad { get; set; } = string.Empty;
        public int? CategoriaTerceroId { get; set; }
    }

    public class ActualizarTerceroDto : CrearTerceroDto { }
}