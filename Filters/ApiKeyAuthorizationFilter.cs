using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ApiProductos.Filters;

public class ApiKeyAuthorizationFilter : Attribute, IAsyncAuthorizationFilter
{
    private readonly string[] _rolesPermitidos;

    public ApiKeyAuthorizationFilter(params string[] rolesPermitidos)
    {
        _rolesPermitidos = rolesPermitidos;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        if (_rolesPermitidos.Length == 0)
        {
            return;
        }

        if (!context.HttpContext.Request.Headers.TryGetValue("X-API-KEY", out var apiKey))
        {
            context.Result = new UnauthorizedObjectResult(new
            {
                mensaje = "Debe enviar la cabecera X-API-KEY para acceder a este recurso."
            });
            return;
        }

        var key = apiKey.ToString();

        if (key == "admin-key")
        {
            if (_rolesPermitidos.Contains("admin"))
            {
                return;
            }

            context.Result = new StatusCodeResult(StatusCodes.Status403Forbidden);
            context.HttpContext.Response.Headers.Append("X-Error-Detalle", "Rol admin requerido.");
            return;
        }

        if (key == "user-key")
        {
            if (_rolesPermitidos.Contains("user"))
            {
                return;
            }

            context.Result = new StatusCodeResult(StatusCodes.Status403Forbidden);
            context.HttpContext.Response.Headers.Append("X-Error-Detalle", "Rol user no tiene permisos suficientes.");
            return;
        }

        context.Result = new UnauthorizedObjectResult(new
        {
            mensaje = "API KEY invalida."
        });
    }
}
