// ModuloSistema.cs
namespace ERPFabrica.Domain.Entities
{
    public class ModuloSistema
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string Version { get; set; } = "1.0";
        public decimal PrecioMensual { get; set; }
    }
}