using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace Web.Middlewares
{
    // Este middleware agarra los errores que ocurran en cualquier parte de la API
    public class GlobalExceptionHandlingMiddleware : IMiddleware
    {
        private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

        public GlobalExceptionHandlingMiddleware(ILogger<GlobalExceptionHandlingMiddleware> logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                // Deja que la petición siga su curso normal hacia el controlador
                await next(context);
            }
            catch (KeyNotFoundException ex)
            {
                // Si no se encuentra un recurso , devuelve un error 404
                _logger.LogError(ex, ex.Message);

                int statusCode = (int)HttpStatusCode.NotFound;
                context.Response.StatusCode = statusCode;

                ProblemDetails problem = new()
                {
                    Status = statusCode,
                    Type = "Not Found",
                    Title = "Recurso no encontrado",
                    Detail = ex.Message
                };

                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(JsonSerializer.Serialize(problem));
            }
            catch (ArgumentException ex)
            {
                // Si los datos enviados son inválidos o fallan las validaciones, devuelve un error 400
                _logger.LogError(ex, ex.Message);

                int statusCode = (int)HttpStatusCode.BadRequest;
                context.Response.StatusCode = statusCode;

                ProblemDetails problem = new()
                {
                    Status = statusCode,
                    Type = "Bad Request",
                    Title = "Datos de entrada no validos",
                    Detail = ex.Message
                };

                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(JsonSerializer.Serialize(problem));
            }
            catch (UnauthorizedAccessException ex)
            {
                // Si las credenciales sin invalidas o el usuario no esta autorizado devuelve un error 401
                _logger.LogError(ex, ex.Message);

                int statusCode = (int)HttpStatusCode.Unauthorized;
                context.Response.StatusCode = statusCode;

                ProblemDetails problem = new()
                {
                    Status = statusCode,
                    Type = "Unauthorized",
                    Title = "No autorizado",
                    Detail = ex.Message
                };
            
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(JsonSerializer.Serialize(problem));
            }

            catch (Exception ex)
            {
                // Para cualquier otro error inesperado en el servidor, devuelve un error 500
                _logger.LogError(ex, ex.Message);

                int statusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.StatusCode = statusCode;

                ProblemDetails problem = new()
                {
                    Status = statusCode,
                    Type = "Server Error",
                    Title = "Error interno del servidor",
                    Detail = ex.Message
                };

                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(JsonSerializer.Serialize(problem));
            }
        }
    }
}