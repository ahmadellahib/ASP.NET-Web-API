using CourseLibrary.API.Models;
using CourseLibrary.API.Validators;

namespace CourseLibrary.API.Services;

public interface IServicesLogicValidator
{
    void ValidateEntity<T>(T objectT, BaseValidator<T> validator);

    void ValidateParameter(Guid guidParameter, string parameterName);

    void ValidateParameter(string stringParameter, string parameterName);

    void ValidateStorageEntity<T>(object? storageEntity, params int[] id);

    void ValidateParameter(IFormFile fileParameter, string parameterName);

    void ValidateStorageEntity<T>(object? storageEntity, params Guid[] ids);

    void ValidateEntityConcurrency<T>(IConcurrencyAware inputEntity, IConcurrencyAware storageEntity);
}
