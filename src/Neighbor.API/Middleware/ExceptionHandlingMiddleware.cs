using System.Text.Json;
using Neighbor.Domain.Exceptions;

namespace Neighbor.API.Middleware;
public class ExceptionHandlingMiddleware : IMiddleware
{
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger)
        => _logger = logger;

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);

            await HandleExceptionAsync(context, e);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext httpContext, Exception exception)
    {
        var statusCode = GetStatusCode(exception);

        var response = new
        {
            title = GetTitle(exception),
            status = statusCode,
            detail = exception.Message,
            errorCode = GetErrorCode(exception),
            errors = GetErrors(exception),
        };

        httpContext.Response.ContentType = "application/json";

        httpContext.Response.StatusCode = statusCode;

        await httpContext.Response.WriteAsync(JsonSerializer.Serialize(response));
    }

    private static int GetStatusCode(Exception exception) =>
        exception switch
        {
            BadRequestException => StatusCodes.Status400BadRequest,
            NotFoundException => StatusCodes.Status404NotFound,
            AuthorizeException => StatusCodes.Status401Unauthorized,
            //Application.Exceptions.ValidationException => StatusCodes.Status422UnprocessableEntity,
            FluentValidation.ValidationException => StatusCodes.Status400BadRequest,
            FormatException => StatusCodes.Status422UnprocessableEntity,
            _ => StatusCodes.Status500InternalServerError
        };

    private static string GetErrorCode(Exception exception) =>
        exception switch
        {
            DomainException applicationException => applicationException.ErrorCode,
            _ => "Server Error"
        };

    private static string GetTitle(Exception exception) =>
        exception switch
        {
            DomainException applicationException => applicationException.Title,
            _ => "Server Error"
        };

    private static IReadOnlyCollection<Application.Exceptions.ValidationError> GetErrors(Exception exception)
    {
        IReadOnlyCollection<Application.Exceptions.ValidationError> errors = null;

        if (exception is Application.Exceptions.ValidationException validationException)
        {
            errors = validationException.Errors;
        }
        return errors;
    }
}
