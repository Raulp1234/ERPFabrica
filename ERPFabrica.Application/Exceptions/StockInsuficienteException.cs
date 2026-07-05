// StockInsuficienteException.cs
namespace ERPFabrica.Application.Exceptions
{
    public class StockInsuficienteException : Exception
    {
        public StockInsuficienteException(string productoNombre, decimal cantidadRequerida, decimal cantidadDisponible)
            : base($"Stock insuficiente para '{productoNombre}': requerido {cantidadRequerida}, disponible {cantidadDisponible}.") { }
    }
}

// EstadoInvalidoException.cs
namespace ERPFabrica.Application.Exceptions
{
    public class EstadoInvalidoException : Exception
    {
        public EstadoInvalidoException(string mensaje) : base(mensaje) { }
    }
}

// NegocioException.cs
namespace ERPFabrica.Application.Exceptions
{
    public class NegocioException : Exception
    {
        public NegocioException(string mensaje) : base(mensaje) { }
    }
}