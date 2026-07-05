//using ERPFabrica.Domain.Entities;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Infrastructure;

//namespace ERPFabrica.Application.Interfaces
//{
//    public interface IApplicationDbContext
//    {
//        DbSet<Producto> Productos { get; set; }
//        DbSet<Stock> Stocks { get; set; }
//        DbSet<MovimientoInventario> MovimientosInventario { get; set; }
//        // Agrega aquí todos los DbSet que necesiten los servicios de Application.
//        // Por simplicidad, se incluyen los más usados; en un proyecto real puedes mapear todos.
//        DbSet<Almacen> Almacenes { get; set; }
//        DbSet<Tercero> Terceros { get; set; }
//        DbSet<Solicitud> Solicitudes { get; set; }
//        DbSet<Factura> Facturas { get; set; }
//        // ...

//        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
//        DatabaseFacade Database { get; }  // Para transacciones
//    }
//}

using ERPFabrica.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace ERPFabrica.Application.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<Producto> Productos { get; }
        DbSet<Stock> Stocks { get; }
        DbSet<MovimientoInventario> MovimientosInventario { get; }
        DbSet<Almacen> Almacenes { get; }
        DbSet<Tercero> Terceros { get; }
        DbSet<Solicitud> Solicitudes { get; }
        DbSet<Factura> Facturas { get; }
        DbSet<LineaFactura> LineasFactura { get; }
        DbSet<Pago> Pagos { get; }
        DbSet<CategoriaProducto> CategoriaProductos { get; }
        DbSet<CategoriaTercero> CategoriaTerceros { get; }
        DbSet<TenantConfig> TenantConfigs { get; }
        DbSet<TenantModulo> TenantModulos { get; }
        DbSet<SecuenciasNumeros> SecuenciasNumeros { get; }
        DbSet<HistorialEstadoSolicitud> HistorialesEstado { get; }
        DbSet<ValorCampoExtra> ValoresCampoExtra { get; }
        DbSet<UsuarioRol> UsuarioRoles { get; }
        DbSet<Rol> Roles { get; }
        DbSet<Usuario> Usuarios { get; }
        DbSet<CampoExtraDefinicion> CamposExtraDefiniciones { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        DatabaseFacade Database { get; }
    }
}