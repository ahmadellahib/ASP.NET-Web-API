using CourseLibrary.API.Models;
using CourseLibrary.API.Models.Exceptions;
using CourseLibrary.API.Validators;
using FluentValidation.Results;
using ValidationException = FluentValidation.ValidationException;

namespace CourseLibrary.API.Services;

internal sealed class ServicesLogicValidator : IServicesLogicValidator
{
    public void ValidateEntity<T>(T objectT, BaseValidator<T> validator)
    {
        ValidationResult validationResult = validator.Validate(objectT);

        if (!validationResult.IsValid)
        {
            throw new ServiceException(new ValidationException(validationResult.Errors));
        }
    }

    public void ValidateParameter(Guid guidParameter, string parameterName)
    {
        if (guidParameter == Guid.Empty)
        {
            throw new ServiceException(new InvalidParameterException(parameterName));
        }
    }

    public void ValidateParameter(string stringParameter, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(stringParameter))
        {
            throw new ServiceException(new InvalidParameterException(parameterName));
        }
    }

    public void ValidateParameter(IFormFile fileParameter, string parameterName)
    {
        if (fileParameter is null)
        {
            throw new ServiceException(new InvalidParameterException(parameterName));
        }
    }

    public void ValidateStorageEntity<T>(object? storageEntity, params Guid[] ids)
    {
        if (storageEntity == null)
        {
            throw new NotFoundEntityException(typeof(T), ids);
        }
    }

    public void ValidateStorageEntity<T>(object? storageEntity, params int[] ids)
    {
        if (storageEntity == null)
        {
            throw new NotFoundEntityException(typeof(T), ids);
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