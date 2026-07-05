// TenantMiddleware.cs
using ERPFabrica.Application.Interfaces;
using ERPFabrica.Infrastructure.Data;

namespace ERPFabrica.WebApi.Middleware
{
    public class TenantMiddleware
    {
        private readonly RequestDelegate _next;

        public TenantMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ITenantProvider tenantProvider)
        {
            // Extrae el tenantId de la ruta
            if (context.Request.RouteValues.TryGetValue("tenantId", out var routeTenantObj) &&
                int.TryParse(routeTenantObj?.ToString(), out int routeTenantId))
            {
                // Verifica que el claim del token coincida con el de la ruta (si el usuario está autenticado)
                if (context.User.Identity?.IsAuthenticated == true)
                {
                    var claimTenantId = context.User.FindFirst("TenantId")?.Value;
                    if (claimTenantId != null && int.TryParse(claimTenantId, out int tokenTenantId))
                    {
                        if (routeTenantId != tokenTenantId)
                        {
                            context.Response.StatusCode = StatusCodes.Status403Forbidden;
                            await context.Response.WriteAsync("El tenant de la ruta no coincide con el token.");
                            return;
                        }
                    }
                }

                // Establece el tenant actual en el proveedor para los filtros globales
                tenantProvider.SetTenant(routeTenantId);
                TenantContext.CurrentTenantId = routeTenantId; // ← nuevo
            }

            await _next(context);
        }
    }
}