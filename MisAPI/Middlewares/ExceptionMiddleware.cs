using MisAPI.DTOs;
using MisAPI.Exceptions;

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
        catch (InvalidRefreshTokenException ex)
        {
            HandleException(context, ex, StatusCodes.Status400BadRequest);
        }
        catch (IncorrectRegisterDataException ex)
        {
            HandleException(context, ex, StatusCodes.Status400BadRequest);
        }
        catch (InvalidTokenException ex)
        {
            HandleException(context, ex, StatusCodes.Status401Unauthorized);
        }
        catch (NullTokenException ex)
        {
            HandleException(context, ex, StatusCodes.Status400BadRequest);
        }
        catch (IncorrectPhoneException ex)
        {
            HandleException(context, ex, StatusCodes.Status400BadRequest);
        }
        catch (UnauthorizedException ex)
        {
            HandleException(context, ex, StatusCodes.Status401Unauthorized);
        }
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
            Message = statusCode != 500 ? exception.Message : "Unexpected exception occurred",
            StatusCode = context.Response.StatusCode
        });
    }

}