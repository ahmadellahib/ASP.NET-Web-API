using CourseLibrary.API.Brokers.Logging;
using CourseLibrary.API.Brokers.Storages;
using CourseLibrary.API.Models.Authors;
using CourseLibrary.API.Validators.Authors;

namespace CourseLibrary.API.Services.V1.Authors;

internal sealed class AuthorFoundationService : IAuthorFoundationService
{
    private readonly IStorageBroker _storageBroker;
    private readonly IServicesLogicValidator _servicesLogicValidator;
    private readonly ILoggingBroker<AuthorFoundationService> _loggingBroker;

    public AuthorFoundationService(IStorageBroker storageBroker,
        IServicesLogicValidator servicesLogicValidator,
        ILoggingBroker<AuthorFoundationService> loggingBroker)
    {
        _storageBroker = storageBroker ?? throw new ArgumentNullException(nameof(storageBroker));
        _servicesLogicValidator = servicesLogicValidator ?? throw new ArgumentNullException(nameof(servicesLogicValidator));
        _loggingBroker = loggingBroker ?? throw new ArgumentNullException(nameof(loggingBroker));
    }

    public async Task<Author> CreateAuthorAsync(Author author, CancellationToken cancellationToken)
    {
        _servicesLogicValidator.ValidateEntity(author, new AuthorValidator(true));

        return await _storageBroker.InsertAuthorAsync(author, cancellationToken);
    }

    public async Task<Author> ModifyAuthorAsync(Author author, CancellationToken cancellationToken)
    {
        _servicesLogicValidator.ValidateEntity(author, new AuthorValidator(false));

        return await _storageBroker.UpdateAuthorAsync(author, cancellationToken);
    }

    public async Task RemoveAuthorByIdAsync(Guid authorId, CancellationToken cancellationToken)
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

    public async ValueTask<Author> RetrieveAuthorByIdAsync(Guid authorId, CancellationToken cancellationToken)
    {
        _servicesLogicValidator.ValidateParameter(authorId, nameof(authorId));

        Author? storageAuthor = await _storageBroker.SelectAuthorByIdAsync(authorId, cancellationToken);
        _servicesLogicValidator.ValidateStorageEntity<Author>(storageAuthor, authorId);

        return storageAuthor!;
    }

    public IQueryable<Author> RetrieveAllAuthors()
    {
        IQueryable<Author> storageAuthors = _storageBroker.SelectAllAuthors();

        if (!storageAuthors.Any())
        {
            _loggingBroker.LogWarning(StaticData.WarningMessages.NoEntitiesFoundInStorage);
        }

        return storageAuthors;
    }
}
