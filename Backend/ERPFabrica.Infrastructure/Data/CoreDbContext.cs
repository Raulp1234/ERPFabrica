//// CoreDbContext.cs
//using ERPFabrica.Application.Interfaces;
//using ERPFabrica.Domain.Entities;
//using Microsoft.EntityFrameworkCore;
//using System.Linq.Expressions;

//namespace ERPFabrica.Infrastructure.Data
//{
//    public class CoreDbContext : DbContext, IApplicationDbContext
//    {
//        private readonly ITenantProvider _tenantProvider;

//        public CoreDbContext(DbContextOptions<CoreDbContext> options,
//                            ITenantProvider tenantProvider) : base(options)
//        {
//            _tenantProvider = tenantProvider;
//        }

//        // DbSets
//        public DbSet<Tenant> Tenants => Set<Tenant>();
//        public DbSet<TenantConfig> TenantConfigs => Set<TenantConfig>();
//        public DbSet<TenantModulo> TenantModulos => Set<TenantModulo>();
//        public DbSet<ModuloSistema> ModuloSistemas => Set<ModuloSistema>();
//        public DbSet<Usuario> Usuarios => Set<Usuario>();
//        public DbSet<Rol> Roles => Set<Rol>();
//        public DbSet<Permiso> Permisos => Set<Permiso>();
//        public DbSet<UsuarioRol> UsuarioRoles => Set<UsuarioRol>();
//        public DbSet<RolPermiso> RolPermisos => Set<RolPermiso>();
//        public DbSet<Tercero> Terceros => Set<Tercero>();
//        public DbSet<CategoriaTercero> CategoriaTerceros => Set<CategoriaTercero>();
//        public DbSet<Producto> Productos => Set<Producto>();
//        public DbSet<CategoriaProducto> CategoriaProductos => Set<CategoriaProducto>();
//        public DbSet<Almacen> Almacenes => Set<Almacen>();
//        public DbSet<Stock> Stocks => Set<Stock>();
//        public DbSet<MovimientoInventario> MovimientosInventario => Set<MovimientoInventario>();
//        public DbSet<Solicitud> Solicitudes => Set<Solicitud>();
//        public DbSet<LineaSolicitud> LineasSolicitud => Set<LineaSolicitud>();
//        public DbSet<HistorialEstadoSolicitud> HistorialesEstado => Set<HistorialEstadoSolicitud>();
//        public DbSet<Factura> Facturas => Set<Factura>();
//        public DbSet<LineaFactura> LineasFactura => Set<LineaFactura>();
//        public DbSet<Pago> Pagos => Set<Pago>();
//        public DbSet<CampoExtraDefinicion> CamposExtraDefiniciones => Set<CampoExtraDefinicion>();
//        public DbSet<ValorCampoExtra> ValoresCampoExtra => Set<ValorCampoExtra>();

//        protected override void OnModelCreating(ModelBuilder modelBuilder)
//        {
//            base.OnModelCreating(modelBuilder);

//            // ------------------------
//            // Filtros globales multi-tenant y soft delete
//            // ------------------------
//            var tenantId = _tenantProvider.TenantId;

//            // Aplica filtro TenantId a entidades que implementan IHasTenant (excepto Tenant)
//            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
//            {
//                if (typeof(IHasTenant).IsAssignableFrom(entityType.ClrType) &&
//                    entityType.ClrType != typeof(Tenant))
//                {
//                    var parameter = Expression.Parameter(entityType.ClrType, "e");
//                    var tenantProp = Expression.Property(parameter, nameof(IHasTenant.TenantId));
//                    var tenantConstant = Expression.Constant(tenantId, typeof(int));
//                    var tenantFilter = Expression.Equal(tenantProp, tenantConstant);

//                    // Si además tiene soft delete, añadir filtro de no eliminado
//                    if (typeof(IHasSoftDelete).IsAssignableFrom(entityType.ClrType))
//                    {
//                        var isDeletedProp = Expression.Property(parameter, nameof(IHasSoftDelete.IsDeleted));
//                        var falseConstant = Expression.Constant(false);
//                        var softDeleteFilter = Expression.Equal(isDeletedProp, falseConstant);
//                        tenantFilter = Expression.AndAlso(tenantFilter, softDeleteFilter);
//                    }

//                    var lambda = Expression.Lambda(tenantFilter, parameter);
//                    modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
//                }
//            }

//            // ------------------------
//            // Configuraciones Fluent API
//            // ------------------------

//            // Tenant
//            modelBuilder.Entity<Tenant>(e =>
//            {
//                e.HasIndex(t => t.Subdominio).IsUnique();
//            });

//            // TenantConfig
//            modelBuilder.Entity<TenantConfig>(e =>
//            {
//                e.HasIndex(c => c.TenantId).IsUnique();
//            });

//            // Usuario
//            modelBuilder.Entity<Usuario>(e =>
//            {
//                e.HasIndex(u => new { u.TenantId, u.Email }).IsUnique();
//            });

//            // UsuarioRol (clave compuesta)
//            modelBuilder.Entity<UsuarioRol>(e =>
//            {
//                e.HasKey(ur => new { ur.UsuarioId, ur.RolId });
//            });

//            // RolPermiso (clave compuesta)
//            modelBuilder.Entity<RolPermiso>(e =>
//            {
//                e.HasKey(rp => new { rp.RolId, rp.PermisoId });
//            });

//            // Producto
//            modelBuilder.Entity<Producto>(e =>
//            {
//                e.HasIndex(p => new { p.TenantId, p.CodigoSKU }).IsUnique();
//                e.HasOne(p => p.Categoria)
//                 .WithMany()
//                 .HasForeignKey(p => p.CategoriaProductoId)
//                 .OnDelete(DeleteBehavior.Restrict);
//            });

//            // CategoriaProducto (auto-referencia)
//            modelBuilder.Entity<CategoriaProducto>(e =>
//            {
//                e.HasOne(c => c.CategoriaPadre)
//                 .WithMany()
//                 .HasForeignKey(c => c.CategoriaPadreId)
//                 .OnDelete(DeleteBehavior.Restrict);
//            });

//            // Tercero
//            modelBuilder.Entity<Tercero>(e =>
//            {
//                e.HasOne(t => t.Categoria)
//                 .WithMany()
//                 .HasForeignKey(t => t.CategoriaTerceroId)
//                 .OnDelete(DeleteBehavior.SetNull);
//            });

//            // Stock
//            modelBuilder.Entity<Stock>(e =>
//            {
//                e.HasIndex(s => new { s.TenantId, s.ProductoId, s.AlmacenId }).IsUnique();
//                e.HasOne(s => s.Producto)
//                 .WithMany()
//                 .HasForeignKey(s => s.ProductoId)
//                 .OnDelete(DeleteBehavior.Restrict);
//                e.HasOne(s => s.Almacen)
//                 .WithMany()
//                 .HasForeignKey(s => s.AlmacenId)
//                 .OnDelete(DeleteBehavior.Restrict);
//            });

//            // MovimientoInventario
//            modelBuilder.Entity<MovimientoInventario>(e =>
//            {
//                e.HasOne(m => m.Producto)
//                 .WithMany()
//                 .HasForeignKey(m => m.ProductoId)
//                 .OnDelete(DeleteBehavior.Restrict);
//                e.HasOne(m => m.Almacen)
//                 .WithMany()
//                 .HasForeignKey(m => m.AlmacenId)
//                 .OnDelete(DeleteBehavior.Restrict);
//            });

//            // Solicitud
//            modelBuilder.Entity<Solicitud>(e =>
//            {
//                e.HasIndex(s => new { s.TenantId, s.NumeroSolicitud }).IsUnique();
//                e.HasOne(s => s.Tercero)
//                 .WithMany()
//                 .HasForeignKey(s => s.TerceroId)
//                 .OnDelete(DeleteBehavior.Restrict);
//            });

//            // LineaSolicitud
//            modelBuilder.Entity<LineaSolicitud>(e =>
//            {
//                e.HasOne(l => l.Solicitud)
//                 .WithMany(s => s.Lineas)
//                 .HasForeignKey(l => l.SolicitudId)
//                 .OnDelete(DeleteBehavior.Cascade);
//                e.HasOne(l => l.Producto)
//                 .WithMany()
//                 .HasForeignKey(l => l.ProductoId)
//                 .OnDelete(DeleteBehavior.SetNull);
//            });

//            // HistorialEstadoSolicitud
//            modelBuilder.Entity<HistorialEstadoSolicitud>(e =>
//            {
//                e.HasOne(h => h.Solicitud)
//                 .WithMany()
//                 .HasForeignKey(h => h.SolicitudId)
//                 .OnDelete(DeleteBehavior.Cascade);
//            });

//            // Factura
//            modelBuilder.Entity<Factura>(e =>
//            {
//                e.HasIndex(f => new { f.TenantId, f.NumeroFactura }).IsUnique();
//                e.HasOne(f => f.Tercero)
//                 .WithMany()
//                 .HasForeignKey(f => f.TerceroId)
//                 .OnDelete(DeleteBehavior.Restrict);
//                e.HasOne(f => f.Solicitud)
//                 .WithMany()
//                 .HasForeignKey(f => f.SolicitudId)
//                 .OnDelete(DeleteBehavior.SetNull);
//            });

//            // LineaFactura
//            modelBuilder.Entity<LineaFactura>(e =>
//            {
//                e.HasOne(l => l.Factura)
//                 .WithMany(f => f.Lineas)
//                 .HasForeignKey(l => l.FacturaId)
//                 .OnDelete(DeleteBehavior.Cascade);
//                e.HasOne(l => l.Producto)
//                 .WithMany()
//                 .HasForeignKey(l => l.ProductoId)
//                 .OnDelete(DeleteBehavior.SetNull);
//            });

//            // Pago
//            modelBuilder.Entity<Pago>(e =>
//            {
//                e.HasOne(p => p.Factura)
//                 .WithMany(f => f.Pagos)
//                 .HasForeignKey(p => p.FacturaId)
//                 .OnDelete(DeleteBehavior.Cascade);
//            });

//            // ValorCampoExtra
//            modelBuilder.Entity<ValorCampoExtra>(e =>
//            {
//                e.HasOne(v => v.Definicion)
//                 .WithMany()
//                 .HasForeignKey(v => v.CampoExtraDefinicionId)
//                 .OnDelete(DeleteBehavior.Cascade);
//            });

//            // ------------------------
//            // Datos Semilla
//            // ------------------------
//            modelBuilder.Entity<ModuloSistema>().HasData(
//                new ModuloSistema { Id = 1, Nombre = "Inventario", Descripcion = "Gestión de productos y stock", Version = "1.0", PrecioMensual = 29.99m },
//                new ModuloSistema { Id = 2, Nombre = "Pedidos", Descripcion = "Pedidos de cliente y órdenes de compra", Version = "1.0", PrecioMensual = 19.99m },
//                new ModuloSistema { Id = 3, Nombre = "Facturación", Descripcion = "Facturación y cobros", Version = "1.0", PrecioMensual = 24.99m }
//            );

//            modelBuilder.Entity<Permiso>().HasData(
//                new Permiso { Id = 1, Nombre = "Crear productos", Codigo = "productos.crear" },
//                new Permiso { Id = 2, Nombre = "Editar productos", Codigo = "productos.editar" },
//                new Permiso { Id = 3, Nombre = "Eliminar productos", Codigo = "productos.eliminar" },
//                new Permiso { Id = 4, Nombre = "Ver productos", Codigo = "productos.ver" },
//                new Permiso { Id = 5, Nombre = "Crear facturas", Codigo = "facturas.crear" },
//                new Permiso { Id = 6, Nombre = "Ver facturas", Codigo = "facturas.ver" }
//            );
//        }
//    }
//}

using ERPFabrica.Application.Interfaces;
using ERPFabrica.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ERPFabrica.Infrastructure.Data
{
    public class CoreDbContext : DbContext, IApplicationDbContext
    {
        private readonly ITenantProvider _tenantProvider;

        public CoreDbContext(DbContextOptions<CoreDbContext> options,
                             ITenantProvider tenantProvider) : base(options)
        {
            _tenantProvider = tenantProvider;
        }

        // DbSets obligatorios de la interfaz
        public DbSet<Producto> Productos => Set<Producto>();
        public DbSet<Stock> Stocks => Set<Stock>();
        public DbSet<MovimientoInventario> MovimientosInventario => Set<MovimientoInventario>();
        public DbSet<Almacen> Almacenes => Set<Almacen>();
        public DbSet<Tercero> Terceros => Set<Tercero>();
        public DbSet<Solicitud> Solicitudes => Set<Solicitud>();
        public DbSet<Factura> Facturas => Set<Factura>();
        public DbSet<LineaFactura> LineasFactura => Set<LineaFactura>();
        public DbSet<Pago> Pagos => Set<Pago>();
        public DbSet<CategoriaProducto> CategoriaProductos => Set<CategoriaProducto>();
        public DbSet<CategoriaTercero> CategoriaTerceros => Set<CategoriaTercero>();
        public DbSet<TenantConfig> TenantConfigs => Set<TenantConfig>();
        public DbSet<TenantModulo> TenantModulos => Set<TenantModulo>();

        // DbSets adicionales que no están en la interfaz pero son necesarios para el contexto
        public DbSet<Tenant> Tenants => Set<Tenant>();
        public DbSet<ModuloSistema> ModuloSistemas => Set<ModuloSistema>();
        public DbSet<Usuario> Usuarios => Set<Usuario>();
        public DbSet<Rol> Roles => Set<Rol>();
        public DbSet<Permiso> Permisos => Set<Permiso>();
        public DbSet<UsuarioRol> UsuarioRoles => Set<UsuarioRol>();
        public DbSet<RolPermiso> RolPermisos => Set<RolPermiso>();
        public DbSet<LineaSolicitud> LineasSolicitud => Set<LineaSolicitud>();
        public DbSet<HistorialEstadoSolicitud> HistorialesEstado => Set<HistorialEstadoSolicitud>();
        public DbSet<CampoExtraDefinicion> CamposExtraDefiniciones => Set<CampoExtraDefinicion>();
        public DbSet<ValorCampoExtra> ValoresCampoExtra => Set<ValorCampoExtra>();
        public DbSet<SecuenciasNumeros> SecuenciasNumeros => Set<SecuenciasNumeros>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Expresión que obtiene el TenantId actual desde el AsyncLocal
            var getTenantIdMethod = typeof(TenantContext).GetProperty(nameof(TenantContext.CurrentTenantId))!.GetGetMethod()!;

            // --------------------------------------------------
            // Filtros globales multi-tenant y soft delete (DINÁMICOS)
            // --------------------------------------------------
            // Se usa _tenantProvider.TenantId directamente en la expresión,
            // evaluándose siempre en el momento de ejecutar la consulta.
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                // Solo para entidades que implementan IHasTenant (excepto Tenant en sí)
                //if (typeof(IHasTenant).IsAssignableFrom(entityType.ClrType) &&
                //    entityType.ClrType != typeof(Tenant))
                //{
                //    var parameter = Expression.Parameter(entityType.ClrType, "e");
                //    // Acceso a la propiedad TenantId
                //    var tenantProp = Expression.Property(parameter, nameof(IHasTenant.TenantId));
                //    // Acceso al valor actual de _tenantProvider.TenantId (evaluado en tiempo de consulta)
                //    var currentTenantId = Expression.Property(
                //        Expression.Constant(_tenantProvider),
                //        nameof(ITenantProvider.TenantId)
                //    );
                //    var tenantFilter = Expression.Equal(tenantProp, currentTenantId);

                //    // Si además implementa IHasSoftDelete, añade filtro de no eliminado
                //    if (typeof(IHasSoftDelete).IsAssignableFrom(entityType.ClrType))
                //    {
                //        var isDeletedProp = Expression.Property(parameter, nameof(IHasSoftDelete.IsDeleted));
                //        var falseConstant = Expression.Constant(false);
                //        var softDeleteFilter = Expression.Equal(isDeletedProp, falseConstant);
                //        tenantFilter = Expression.AndAlso(tenantFilter, softDeleteFilter);
                //    }

                //    var lambda = Expression.Lambda(tenantFilter, parameter);
                //    modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
                //}
                if (typeof(IHasTenant).IsAssignableFrom(entityType.ClrType) &&
            entityType.ClrType != typeof(Tenant))
                {
                    var parameter = Expression.Parameter(entityType.ClrType, "e");
                    var tenantProp = Expression.Property(parameter, nameof(IHasTenant.TenantId));

                    // Llamada a TenantContext.CurrentTenantId (evaluado en cada consulta)
                    var getTenantCall = Expression.Call(getTenantIdMethod);
                    var tenantFilter = Expression.Equal(tenantProp, getTenantCall);

                    if (typeof(IHasSoftDelete).IsAssignableFrom(entityType.ClrType))
                    {
                        var isDeletedProp = Expression.Property(parameter, nameof(IHasSoftDelete.IsDeleted));
                        var falseConstant = Expression.Constant(false);
                        var softDeleteFilter = Expression.Equal(isDeletedProp, falseConstant);
                        tenantFilter = Expression.AndAlso(tenantFilter, softDeleteFilter);
                    }

                    var lambda = Expression.Lambda(tenantFilter, parameter);
                    modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
                }
        }

            // --------------------------------------------------
            // Configuraciones Fluent API
            // --------------------------------------------------

            // Tenant
            modelBuilder.Entity<Tenant>(e =>
            {
                e.HasIndex(t => t.Subdominio).IsUnique();
            });

            // TenantConfig
            modelBuilder.Entity<TenantConfig>(e =>
            {
                e.HasIndex(c => c.TenantId).IsUnique();
            });

            // Usuario
            modelBuilder.Entity<Usuario>(e =>
            {
                e.HasIndex(u => new { u.TenantId, u.Email }).IsUnique();
            });

            // UsuarioRol (clave compuesta)
            modelBuilder.Entity<UsuarioRol>(e =>
            {
                e.HasKey(ur => new { ur.UsuarioId, ur.RolId });
            });

            // RolPermiso (clave compuesta)
            modelBuilder.Entity<RolPermiso>(e =>
            {
                e.HasKey(rp => new { rp.RolId, rp.PermisoId });
            });

            // Producto
            modelBuilder.Entity<Producto>(e =>
            {
                e.HasIndex(p => new { p.TenantId, p.CodigoSKU }).IsUnique();
                e.HasOne(p => p.Categoria)
                 .WithMany()
                 .HasForeignKey(p => p.CategoriaProductoId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            // CategoriaProducto (auto-referencia)
            modelBuilder.Entity<CategoriaProducto>(e =>
            {
                e.HasOne(c => c.CategoriaPadre)
                 .WithMany()
                 .HasForeignKey(c => c.CategoriaPadreId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            // Tercero
            modelBuilder.Entity<Tercero>(e =>
            {
                e.HasOne(t => t.Categoria)
                 .WithMany()
                 .HasForeignKey(t => t.CategoriaTerceroId)
                 .OnDelete(DeleteBehavior.SetNull);
            });

            // Stock
            modelBuilder.Entity<Stock>(e =>
            {
                e.HasIndex(s => new { s.TenantId, s.ProductoId, s.AlmacenId }).IsUnique();
                e.HasOne(s => s.Producto)
                 .WithMany()
                 .HasForeignKey(s => s.ProductoId)
                 .OnDelete(DeleteBehavior.Restrict);
                e.HasOne(s => s.Almacen)
                 .WithMany()
                 .HasForeignKey(s => s.AlmacenId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            // MovimientoInventario
            modelBuilder.Entity<MovimientoInventario>(e =>
            {
                e.HasOne(m => m.Producto)
                 .WithMany()
                 .HasForeignKey(m => m.ProductoId)
                 .OnDelete(DeleteBehavior.Restrict);
                e.HasOne(m => m.Almacen)
                 .WithMany()
                 .HasForeignKey(m => m.AlmacenId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            // Solicitud
            modelBuilder.Entity<Solicitud>(e =>
            {
                e.HasIndex(s => new { s.TenantId, s.NumeroSolicitud }).IsUnique();
                e.HasOne(s => s.Tercero)
                 .WithMany()
                 .HasForeignKey(s => s.TerceroId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            // LineaSolicitud
            modelBuilder.Entity<LineaSolicitud>(e =>
            {
                e.HasOne(l => l.Solicitud)
                 .WithMany(s => s.Lineas)
                 .HasForeignKey(l => l.SolicitudId)
                 .OnDelete(DeleteBehavior.Cascade);
                e.HasOne(l => l.Producto)
                 .WithMany()
                 .HasForeignKey(l => l.ProductoId)
                 .OnDelete(DeleteBehavior.SetNull);
            });

            // HistorialEstadoSolicitud
            modelBuilder.Entity<HistorialEstadoSolicitud>(e =>
            {
                e.HasOne(h => h.Solicitud)
                 .WithMany()
                 .HasForeignKey(h => h.SolicitudId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            // Factura
            modelBuilder.Entity<Factura>(e =>
            {
                e.HasIndex(f => new { f.TenantId, f.NumeroFactura }).IsUnique();
                e.HasOne(f => f.Tercero)
                 .WithMany()
                 .HasForeignKey(f => f.TerceroId)
                 .OnDelete(DeleteBehavior.Restrict);
                e.HasOne(f => f.Solicitud)
                 .WithMany()
                 .HasForeignKey(f => f.SolicitudId)
                 .OnDelete(DeleteBehavior.SetNull);
            });

            // LineaFactura
            modelBuilder.Entity<LineaFactura>(e =>
            {
                e.HasOne(l => l.Factura)
                 .WithMany(f => f.Lineas)
                 .HasForeignKey(l => l.FacturaId)
                 .OnDelete(DeleteBehavior.Cascade);
                e.HasOne(l => l.Producto)
                 .WithMany()
                 .HasForeignKey(l => l.ProductoId)
                 .OnDelete(DeleteBehavior.SetNull);
            });

            // Pago
            modelBuilder.Entity<Pago>(e =>
            {
                e.HasOne(p => p.Factura)
                 .WithMany(f => f.Pagos)
                 .HasForeignKey(p => p.FacturaId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            // ValorCampoExtra
            modelBuilder.Entity<ValorCampoExtra>(e =>
            {
                e.HasOne(v => v.Definicion)
                 .WithMany()
                 .HasForeignKey(v => v.CampoExtraDefinicionId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<SecuenciasNumeros>(e =>
            {
                e.HasIndex(s => new { s.TenantId, s.Entidad, s.Año }).IsUnique();
            });

            // --------------------------------------------------
            // Datos Semilla
            // --------------------------------------------------
            modelBuilder.Entity<ModuloSistema>().HasData(
                new ModuloSistema { Id = 1, Nombre = "Inventario", Descripcion = "Gestión de productos y stock", Version = "1.0", PrecioMensual = 29.99m },
                new ModuloSistema { Id = 2, Nombre = "Pedidos", Descripcion = "Pedidos de cliente y órdenes de compra", Version = "1.0", PrecioMensual = 19.99m },
                new ModuloSistema { Id = 3, Nombre = "Facturación", Descripcion = "Facturación y cobros", Version = "1.0", PrecioMensual = 24.99m }
            );

            modelBuilder.Entity<Permiso>().HasData(
                new Permiso { Id = 1, Nombre = "Crear productos", Codigo = "productos.crear" },
                new Permiso { Id = 2, Nombre = "Editar productos", Codigo = "productos.editar" },
                new Permiso { Id = 3, Nombre = "Eliminar productos", Codigo = "productos.eliminar" },
                new Permiso { Id = 4, Nombre = "Ver productos", Codigo = "productos.ver" },
                new Permiso { Id = 5, Nombre = "Crear facturas", Codigo = "facturas.crear" },
                new Permiso { Id = 6, Nombre = "Ver facturas", Codigo = "facturas.ver" }
            );
        }
    }
}