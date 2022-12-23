using CourseLibrary.API.Brokers.Loggings;
using CourseLibrary.API.Brokers.Storages;
using CourseLibrary.API.Models.Authors;
using CourseLibrary.API.Models.Exceptions;
using CourseLibrary.API.Validators.Authors;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace CourseLibrary.API.Services.V1.Authors;

public class AuthorFoundationService : IAuthorFoundationService
{
    private readonly IStorageBroker _storageBroker;
    private readonly IServicesLogicValidator _servicesLogicValidator;
    private readonly ILoggingBroker<AuthorFoundationService> _loggingBroker;
    private readonly ServicesExceptionsLogger<AuthorFoundationService> _servicesExceptionsLogger;

    public AuthorFoundationService(IStorageBroker storageBroker,
        IServicesLogicValidator servicesLogicValidator,
        ILoggingBroker<AuthorFoundationService> loggingBroker,
        ServicesExceptionsLogger<AuthorFoundationService> servicesExceptionsLogger)
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

            author.ConcurrencyStamp = Guid.NewGuid().ToString();

            return await _storageBroker.InsertAuthorAsync(author, cancellationToken);
        }
        catch (InvalidEntityException<Author> invalidEntityException)
        {
            throw _servicesExceptionsLogger.CreateAndLogServiceException(invalidEntityException);
        }
        catch (SqlException sqlException)
        {
            throw _servicesExceptionsLogger.CreateAndLogCriticalDependencyException(sqlException);
        }
        catch (DbUpdateConcurrencyException dbUpdateConcurrencyException)
        {
            LockedEntityException<Author> lockedEntityException = new(dbUpdateConcurrencyException);

            throw _servicesExceptionsLogger.CreateAndLogDependencyException(lockedEntityException);
        }
        catch (DbUpdateException dbUpdateException)
        {
            throw _servicesExceptionsLogger.CreateAndLogDependencyException(dbUpdateException);
        }
        catch (Exception exception) when (exception is OperationCanceledException || exception is TaskCanceledException)
        {
            throw _servicesExceptionsLogger.CreateAndLogCancellationException(exception);
        }
        catch (Exception exception)
        {
            throw _servicesExceptionsLogger.CreateAndLogServiceException(exception);
        }
    }

    public async Task<Author> ModifyAuthorAsync(Author author, CancellationToken cancellationToken)
    {
        try
        {
            _servicesLogicValidator.ValidateEntity(author, new AuthorValidator(false));

            Author? maybeAuthor = await _storageBroker.SelectAuthorByIdAsync(author.Id, cancellationToken);
            _servicesLogicValidator.ValidateStorageEntity<Author>(maybeAuthor, author.Id);
            _servicesLogicValidator.ValidateEntityConcurrency<Author>(author, maybeAuthor!);

            author.ConcurrencyStamp = Guid.NewGuid().ToString();

            return await _storageBroker.UpdateAuthorAsync(author, cancellationToken);
        }
        catch (InvalidEntityException<Author> invalidEntityException)
        {
            throw _servicesExceptionsLogger.CreateAndLogServiceException(invalidEntityException);
        }
        catch (NotFoundEntityException<Author> notFoundEntityException)
        {
            throw _servicesExceptionsLogger.CreateAndLogValidationException(notFoundEntityException);
        }
        catch (EntityConcurrencyException<Author> entityConcurrencyException)
        {
            throw _servicesExceptionsLogger.CreateAndLogValidationException(entityConcurrencyException);
        }
        catch (SqlException sqlException)
        {
            throw _servicesExceptionsLogger.CreateAndLogCriticalDependencyException(sqlException);
        }
        catch (DbUpdateConcurrencyException dbUpdateConcurrencyException)
        {
            LockedEntityException<Author> lockedEntityException = new(dbUpdateConcurrencyException);

            throw _servicesExceptionsLogger.CreateAndLogDependencyException(lockedEntityException);
        }
        catch (DbUpdateException dbUpdateException)
        {
            throw _servicesExceptionsLogger.CreateAndLogDependencyException(dbUpdateException);
        }
        catch (Exception exception) when (exception is OperationCanceledException || exception is TaskCanceledException)
        {
            throw _servicesExceptionsLogger.CreateAndLogCancellationException(exception);
        }
        catch (Exception exception)
        {
            throw _servicesExceptionsLogger.CreateAndLogServiceException(exception);
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
        catch (InvalidParameterException invalidIdException)
        {
            throw _servicesExceptionsLogger.CreateAndLogValidationException(invalidIdException);
        }
        catch (NotFoundEntityException<Author> notFoundEntityException)
        {
            throw _servicesExceptionsLogger.CreateAndLogValidationException(notFoundEntityException);
        }
        catch (Exception exception) when (exception is OperationCanceledException || exception is TaskCanceledException)
        {
            throw _servicesExceptionsLogger.CreateAndLogCancellationException(exception);
        }
        catch (Exception exception)
        {
            throw _servicesExceptionsLogger.CreateAndLogServiceException(exception);
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
        catch (SqlException sqlException)
        {
            throw _servicesExceptionsLogger.CreateAndLogCriticalDependencyException(sqlException);
        }
        catch (Exception exception)
        {
            throw _servicesExceptionsLogger.CreateAndLogServiceException(exception);
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
        catch (InvalidParameterException invalidIdException)
        {
            throw _servicesExceptionsLogger.CreateAndLogValidationException(invalidIdException);
        }
        catch (NotFoundEntityException<Author> notFoundEntityException)
        {
            throw _servicesExceptionsLogger.CreateAndLogValidationException(notFoundEntityException);
        }
        catch (Exception exception) when (exception is OperationCanceledException || exception is TaskCanceledException)
        {
            throw _servicesExceptionsLogger.CreateAndLogCancellationException(exception);
        }
        catch (Exception exception)
        {
            throw _servicesExceptionsLogger.CreateAndLogServiceException(exception);
        }
    }
}
