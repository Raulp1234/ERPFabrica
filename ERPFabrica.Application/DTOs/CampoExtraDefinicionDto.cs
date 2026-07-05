// DTOs locales (pueden ser los ya existentes en Application)
public class CampoExtraDefinicionDto
{
    public int Id { get; set; }
    public string Entidad { get; set; } = string.Empty;
    public string NombreCampo { get; set; } = string.Empty;
    public string TipoDato { get; set; } = string.Empty;
    public bool EsRequerido { get; set; }
    public string? OpcionesJson { get; set; }
    public int Orden { get; set; }
}

public class CrearCampoExtraDefinicionDto
{
    public string Entidad { get; set; } = string.Empty;
    public string NombreCampo { get; set; } = string.Empty;
    public string TipoDato { get; set; } = string.Empty;
    public bool EsRequerido { get; set; }
    public string? OpcionesJson { get; set; }
    public int Orden { get; set; }
}

public class ActualizarCampoExtraDefinicionDto : CrearCampoExtraDefinicionDto { }

public class ValorCampoExtraDto
{
    public int Id { get; set; }
    public int CampoExtraDefinicionId { get; set; }
    public string? ValorString { get; set; }
    public decimal? ValorDecimal { get; set; }
    public DateTime? ValorDate { get; set; }
    public bool? ValorBool { get; set; }
}