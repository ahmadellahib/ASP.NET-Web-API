using CourseLibrary.API.Brokers.Logging;
using CourseLibrary.API.Models.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

public abstract class BaseController<T> : ControllerBase where T : BaseController<T>
{
    private ILoggingBroker<T>? _loggingBroker;

    protected ILoggingBroker<T> LoggingBroker =>
        _loggingBroker ??= HttpContext.RequestServices.GetRequiredService<ILoggingBroker<T>>();

    protected static string GetInnerMessage(Exception exception)
    {
        if (exception is not null && exception.InnerException is not null)
        {
            return exception.InnerException.Message;
        }

        return string.Empty;
    }

    protected static void SetModelState(ModelStateDictionary modelState, ValidationException validationException)
    {
        string innerMessage = GetInnerMessage(validationException);
        string propertyName = validationException.InnerException is InvalidParameterException exception ? exception.PropertyName : "Model";

        modelState.AddModelError(propertyName, innerMessage);

        if (validationException.InnerException is null)
        {
            return;
        }

        foreach (object? key in validationException.InnerException.Data.Keys)
        {
            if (validationException.InnerException.Data[key] is not List<string> values)
            {
                continue;
            }

            foreach (string value in values)
            {
                modelState.AddModelError(key.ToString() ?? string.Empty, value);
            }
        }
    }
}