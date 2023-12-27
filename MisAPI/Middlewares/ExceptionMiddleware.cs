using MisAPI.Configurations;
using MisAPI.DTOs;

namespace MisAPI.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    
    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        // catch (InvalidRefreshToken ex)
        // {
        //     HandleException(context, ex, StatusCodes.Status400BadRequest);
        // }
        catch (Exception ex)
        {
            HandleException(context, ex, StatusCodes.Status500InternalServerError);
        }
    }
    
    private static void HandleException(HttpContext context, Exception exception, int statusCode)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;
                                       
        context.Response.WriteAsJsonAsync(new ErrorDto
        {
            Message = exception.Message,
            StatusCode = context.Response.StatusCode
        });
    }
}