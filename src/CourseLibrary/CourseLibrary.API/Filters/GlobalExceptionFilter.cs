using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using CourseLibrary.API.Brokers.Logging;
using CourseLibrary.API.Models.Exceptions;

namespace CourseLibrary.API.Filters;

public class GlobalExceptionFilter : IExceptionFilter
{
    private readonly IWebHostEnvironment _env;
    private readonly ILoggingBroker<GlobalExceptionFilter> _logger;

    public GlobalExceptionFilter(IWebHostEnvironment env, ILoggingBroker<GlobalExceptionFilter> logger)
    {
        _env = env;
        _logger = logger;
    }

    public void OnException(ExceptionContext context)
    {
        Exception exception = context.Exception;
        string scheme = context.HttpContext.Request.Scheme;
        string requestMethod = context.HttpContext.Request.Method;
        string requestPath = context.HttpContext.Request.Path;
        string warningMsgTemplate = context.HttpContext.Request.Scheme.ToUpperInvariant() + " {RequestMethod} {RequestPath} {Message}";

        ProblemDetails problemDetails = new()
        {
            Title = "An error occurred.",
            Status = (int)HttpStatusCode.InternalServerError,
            Detail = "An unexpected error occurred.",
            Instance = $"{requestMethod} {requestPath}"
        };

        switch (exception)
        {
            case TaskCanceledException:
            case OperationCanceledException:
                problemDetails.Status = StatusCodes.Status499ClientClosedRequest;
                problemDetails.Title = "Client Closed Request";
                problemDetails.Detail = "The client abruptly terminated the request.";
                break;
            case UnauthorizedAccessException:
                problemDetails.Status = StatusCodes.Status401Unauthorized;
                problemDetails.Title = "Unauthorized";
                problemDetails.Detail = "You are not authorized.";
                _logger.LogWarning(warningMsgTemplate, requestMethod, requestPath, exception.Message);
                break;
            case NotFoundEntityException:
                problemDetails.Status = StatusCodes.Status404NotFound;
                problemDetails.Title = "Not Found";
                problemDetails.Detail = "Resource not found.";
                _logger.LogWarning(warningMsgTemplate, requestMethod, requestPath, exception.Message);
                break;
            case ResourceParametersException:
            case CategoryWithSameNameAlreadyExistsException:
                problemDetails.Status = StatusCodes.Status400BadRequest;
                problemDetails.Title = "Bad Request";
                problemDetails.Detail = exception.Message;
                _logger.LogWarning(warningMsgTemplate, requestMethod, requestPath, exception.Message);
                break;
            case InvalidParameterException:
                problemDetails.Status = StatusCodes.Status400BadRequest;
                problemDetails.Title = "Bad Request";
                problemDetails.Detail = exception.Message;
                _logger.LogError(scheme,requestMethod, requestPath, exception);
                break;
            case InvalidOperationException:
                problemDetails.Status = StatusCodes.Status400BadRequest;
                problemDetails.Title = "Bad Request";
                problemDetails.Detail = "The request is invalid.";
                _logger.LogError(scheme, requestMethod, requestPath, exception);
                break;
            // Add more specific exception handling here
            default:
                if (_env.IsDevelopment())
                {
                    problemDetails.Detail = exception.Message;
                }

                _logger.LogCritical(scheme, requestMethod, requestPath, exception);
                break;
        }

        context.Result = new ObjectResult(problemDetails)
        {
            StatusCode = problemDetails.Status
        };

        context.ExceptionHandled = true;
    }
}
