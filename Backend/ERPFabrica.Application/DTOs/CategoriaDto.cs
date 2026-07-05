// DTOs locales
public class CategoriaDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
}

public class CrearCategoriaDto
{
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public int? CategoriaPadreId { get; set; } // solo para categorías de producto si aplica
}

public class ActualizarCategoriaDto : CrearCategoriaDto { }