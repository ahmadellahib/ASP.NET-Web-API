using CourseLibrary.API.Brokers.Loggings;
using CourseLibrary.API.Brokers.Storages;
using CourseLibrary.API.Models.Authors;
using CourseLibrary.API.Models.Exceptions;
using CourseLibrary.API.Validators.Authors;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace CourseLibrary.API.Services.V1.Authors;

internal sealed class AuthorFoundationService : IAuthorFoundationService
{
    private readonly IStorageBroker _storageBroker;
    private readonly IServicesLogicValidator _servicesLogicValidator;
    private readonly ILoggingBroker<AuthorFoundationService> _loggingBroker;
    private readonly IServicesExceptionsLogger<AuthorFoundationService> _servicesExceptionsLogger;

    public AuthorFoundationService(IStorageBroker storageBroker,
        IServicesLogicValidator servicesLogicValidator,
        ILoggingBroker<AuthorFoundationService> loggingBroker,
        IServicesExceptionsLogger<AuthorFoundationService> servicesExceptionsLogger)
    {
        _storageBroker = storageBroker ?? throw new ArgumentNullException(nameof(storageBroker));
        _servicesLogicValidator = servicesLogicValidator ?? throw new ArgumentNullException(nameof(servicesLogicValidator));
        _loggingBroker = loggingBroker ?? throw new ArgumentNullException(nameof(loggingBroker));
        _servicesExceptionsLogger = servicesExceptionsLogger ?? throw new ArgumentNullException(nameof(servicesExceptionsLogger));
    }

    public async Task<Author> CreateAuthorAsync(Author author, CancellationToken cancellationToken)
    {
        try
        {
            _servicesLogicValidator.ValidateEntity(author, new AuthorValidator(true));

            return await _storageBroker.InsertAuthorAsync(author, cancellationToken);
        }
        catch (Exception exception)
        {
            throw HandleException(exception);
        }
    }

    public async Task<Author> ModifyAuthorAsync(Author author, CancellationToken cancellationToken)
    {
        try
        {
            _servicesLogicValidator.ValidateEntity(author, new AuthorValidator(false));

            return await _storageBroker.UpdateAuthorAsync(author, cancellationToken);
        }
        catch (Exception exception)
        {
            throw HandleException(exception);
        }
    }

    public async Task RemoveAuthorByIdAsync(Guid authorId, CancellationToken cancellationToken)
    {
        try
        {
            _servicesLogicValidator.ValidateParameter(authorId, nameof(authorId));

            Author? maybeAuthor = await _storageBroker.SelectAuthorByIdAsync(authorId, cancellationToken);
            _servicesLogicValidator.ValidateStorageEntity<Author>(maybeAuthor, authorId);

            bool deleted = await _storageBroker.DeleteAuthorAsync(maybeAuthor!, cancellationToken);

            if (!deleted)
            {
                string exceptionMsg = StaticData.ExceptionMessages.NoRowsWasEffectedByDeleting(nameof(Author), authorId);
                throw new Exception(exceptionMsg);
            }
        }
        catch (Exception exception)
        {
            throw HandleException(exception);
        }
    }

    public async ValueTask<Author> RetrieveAuthorByIdAsync(Guid authorId, CancellationToken cancellationToken)
    {
        try
        {
            _servicesLogicValidator.ValidateParameter(authorId, nameof(authorId));

            Author? storageAuthor = await _storageBroker.SelectAuthorByIdAsync(authorId, cancellationToken);
            _servicesLogicValidator.ValidateStorageEntity<Author>(storageAuthor, authorId);

            return storageAuthor!;
        }
        catch (Exception exception)
        {
            throw HandleException(exception);
        }
    }

    public IQueryable<Author> RetrieveAllAuthors()
    {
        try
        {
            IQueryable<Author> storageAuthors = _storageBroker.SelectAllAuthors();

            if (!storageAuthors.Any())
            {
                _loggingBroker.LogWarning(StaticData.WarningMessages.NoEntitiesFoundInStorage);
            }

            return storageAuthors;
        }
        catch (Exception exception)
        {
            throw HandleException(exception);
        }
    }

    private Exception HandleException(Exception exception)
    {
        switch (exception)
        {
            case InvalidParameterException:
            case NotFoundEntityException<Author>:
                throw _servicesExceptionsLogger.CreateAndLogValidationException(exception);
            case InvalidEntityException<Author>:
                throw _servicesExceptionsLogger.CreateAndLogServiceException(exception);
            case SqlException:
                throw _servicesExceptionsLogger.CreateAndLogCriticalDependencyException(exception);
            case DbUpdateConcurrencyException:
                LockedEntityException<Author> lockedEntityException = new(exception);

                throw _servicesExceptionsLogger.CreateAndLogDependencyException(lockedEntityException);
            case DbUpdateException:
                throw _servicesExceptionsLogger.CreateAndLogDependencyException(exception);
            case TaskCanceledException:
            case OperationCanceledException:
                throw _servicesExceptionsLogger.CreateAndLogCancellationException(exception);
            default:
                throw _servicesExceptionsLogger.CreateAndLogServiceException(exception);
        }
    }
}
