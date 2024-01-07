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
        catch (DoctorAlreadyExistsException ex)
        {
            HandleException(context, ex, StatusCodes.Status400BadRequest);
        }

        catch (DoctorNotFoundException ex)
        {
            HandleException(context, ex, StatusCodes.Status404NotFound);
        }
        catch (InvalidValueForAttributeConclusionException ex)
        {
            HandleException(context, ex, StatusCodes.Status400BadRequest);

        }
        catch (ExpiredRefreshTokenException ex)
        {
            HandleException(context, ex, StatusCodes.Status400BadRequest);
        }
        catch (IcdRootNotFoundException ex)
        {
            HandleException(context, ex, StatusCodes.Status400BadRequest);
        }
        catch (InvalidValueForAttributePageException ex)
        {
            HandleException(context, ex, StatusCodes.Status400BadRequest);

        }
        catch (IncorrectPasswordException ex)
        {
            HandleException(context, ex, StatusCodes.Status400BadRequest);
        }
        catch (IncorrectPhoneException ex)
        {
            HandleException(context, ex, StatusCodes.Status400BadRequest);
        }
        catch (IncorrectRegisterDataException ex)
        {
            HandleException(context, ex, StatusCodes.Status400BadRequest);
        }
        catch (IncorrectGenderException ex)
        {
            HandleException(context, ex, StatusCodes.Status400BadRequest);
        }
        catch (InvalidRefreshTokenException ex)
        {
            HandleException(context, ex, StatusCodes.Status400BadRequest);
        }
        catch (InspectionIsNotRootException ex)
        {
            HandleException(context, ex, StatusCodes.Status400BadRequest);
        }
        catch (InspectionNotFoundException ex)
        {
            HandleException(context, ex, StatusCodes.Status404NotFound);
        }
        catch (IncorrectSpecialityException ex)
        {
            HandleException(context, ex, StatusCodes.Status400BadRequest);

        }
        catch (InvalidTokenException ex)
        {
            HandleException(context, ex, StatusCodes.Status401Unauthorized);
        }
        catch (InvalidValueForAttributeDiagnosesException ex)
        {
            HandleException(context, ex, StatusCodes.Status400BadRequest);
        }
        catch (InvalidValueForAttributeDateException ex)
        {
            HandleException(context, ex, StatusCodes.Status400BadRequest);
        }
        catch (NotHavePermissionException ex)
        {
            HandleException(context, ex, StatusCodes.Status403Forbidden);

        }
        catch (NullEmailException ex)
        {
            HandleException(context, ex, StatusCodes.Status400BadRequest);
        }
        catch (NullTokenException ex)
        {
            HandleException(context, ex, StatusCodes.Status400BadRequest);
        }
        catch (PatientNotFoundException ex)
        {
            HandleException(context, ex, StatusCodes.Status400BadRequest);
        }
        catch (UnauthorizedException ex)
        {
            HandleException(context, ex, StatusCodes.Status401Unauthorized);
        }
        catch (ConsultationNotFoundException ex)
        {
            HandleException(context, ex, StatusCodes.Status400BadRequest);
        }
        catch (SpecialityNotFoundException ex)
        {
            HandleException(context, ex, StatusCodes.Status400BadRequest);

        }
        catch (ForbiddenLeaveCommentException ex)
        {
            HandleException(context, ex, StatusCodes.Status403Forbidden);
        }
        catch (CommentNotFoundException ex)
        {
            HandleException(context, ex, StatusCodes.Status400BadRequest);
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
            Message =  exception.Message,
            StatusCode = context.Response.StatusCode
        });
    }

}