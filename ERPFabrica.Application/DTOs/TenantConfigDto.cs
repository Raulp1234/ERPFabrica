public class TenantConfigDto
{
    public string ColorPrimario { get; set; } = "#000000";
    public string ColorSecundario { get; set; } = "#FFFFFF";
    public string LogoUrl { get; set; } = string.Empty;
    public string NombreSistema { get; set; } = "ERP Fábrica";
    public string MonedaPorDefecto { get; set; } = "USD";
    public decimal ImpuestoPorDefecto { get; set; }
    public string FormatoFactura { get; set; } = "FACT-{AÑO}-{SECUENCIAL}";
    public bool ManejaMultiplesAlmacenes { get; set; }
    public bool ControlStockEstricto { get; set; }
    public bool PermiteVentaSinStock { get; set; }
    public string MetodoCalculoCosto { get; set; } = "PMP";
}