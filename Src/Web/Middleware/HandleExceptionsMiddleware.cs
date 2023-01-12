using Domain.Exceptions;
using System.Text.Json;

namespace Web.Middleware;

public sealed class HandleExceptionsMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleAsync(context, ex);
        }
    }

    private static async Task HandleAsync(HttpContext httpContext, Exception exception)
    {
        httpContext.Response.ContentType = "application/json";

        httpContext.Response.StatusCode = exception switch
        {
            NotFoundException => StatusCodes.Status404NotFound,
            _ => StatusCodes.Status500InternalServerError
        };

        await httpContext.Response.WriteAsync(JsonSerializer.Serialize(new
        {
            error = exception.Message
        }));
    }
}
