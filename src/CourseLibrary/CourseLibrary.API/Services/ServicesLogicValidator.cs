using CourseLibrary.API.Models;
using CourseLibrary.API.Models.Exceptions;
using CourseLibrary.API.Validators;
using FluentValidation.Results;

namespace CourseLibrary.API.Services;

internal sealed class ServicesLogicValidator : IServicesLogicValidator
{
    public void ValidateEntity<T>(T objectT, BaseValidator<T> validator)
    {
        ValidationResult validationResult = validator.Validate(objectT);
        if (!validationResult.IsValid)
        {
            InvalidEntityException<T> invalidEntityException = new();

            foreach (ValidationFailure failure in validationResult.Errors)
            {
                if (invalidEntityException.Data.Contains(failure.PropertyName))
                {
                    (invalidEntityException.Data[failure.PropertyName] as List<string>)?.Add(failure.ErrorMessage);
                }
                else
                {
                    invalidEntityException.Data.Add(failure.PropertyName, new List<string> { failure.ErrorMessage });
                }
            }

            throw invalidEntityException;
        }
    }

    public void ValidateParameter(Guid guidParameter, string parameterName)
    {
        if (guidParameter == Guid.Empty)
        {
            throw new InvalidParameterException(parameterName);
        }
    }

    public void ValidateParameter(string stringParameter, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(stringParameter))
        {
            throw new InvalidParameterException(parameterName);
        }
    }

    public void ValidateParameter(IFormFile fileParameter, string parameterName)
    {
        if (fileParameter is null)
        {
            throw new InvalidParameterException(parameterName);
        }
    }

    public void ValidateStorageEntity<T>(object? storageEntity, params Guid[] ids)
    {
        if (storageEntity == null)
        {
            throw new NotFoundEntityException<T>(ids);
        }
    }

    public void ValidateStorageEntity<T>(object? storageEntity, params int[] ids)
    {
        if (storageEntity == null)
        {
            throw new NotFoundEntityException<T>(ids);
        }
    }

    public void ValidateEntityConcurrency<T>(IConcurrencyAware inputEntity, IConcurrencyAware storageEntity)
    {
        if (inputEntity.ConcurrencyStamp != storageEntity.ConcurrencyStamp)
        {
            throw new EntityConcurrencyException<T>();
        }
    }
}