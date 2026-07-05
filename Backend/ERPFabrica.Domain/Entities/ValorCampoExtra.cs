// ValorCampoExtra.cs
namespace ERPFabrica.Domain.Entities
{
    public class ValorCampoExtra
    {
        public int Id { get; set; }
        public string RegistroId { get; set; } = string.Empty; // GUID o ID del registro padre
        public int CampoExtraDefinicionId { get; set; }
        public string? ValorString { get; set; }
        public decimal? ValorDecimal { get; set; }
        public DateTime? ValorDate { get; set; }
        public bool? ValorBool { get; set; }

        public CampoExtraDefinicion Definicion { get; set; } = null!;
    }
}