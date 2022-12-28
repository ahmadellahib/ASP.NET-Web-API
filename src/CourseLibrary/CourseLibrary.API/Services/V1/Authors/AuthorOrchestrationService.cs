using CourseLibrary.API.Models.Authors;
using CourseLibrary.API.Models.Exceptions;
using CourseLibrary.API.Pagination;
using CourseLibrary.API.Services.V1.Courses;

namespace CourseLibrary.API.Services.V1.Authors;

internal sealed class AuthorOrchestrationService : IAuthorOrchestrationService
{
    private readonly IAuthorProcessingService _authorProcessingService;
    private readonly IServicesLogicValidator _servicesLogicValidator;
    private readonly ICourseOrchestrationService _courseOrchestrationService;
    private readonly IServicesExceptionsLogger<AuthorOrchestrationService> _servicesExceptionsLogger;

    public AuthorOrchestrationService(IAuthorProcessingService authorProcessingService,
        IServicesLogicValidator servicesLogicValidator,
        ICourseOrchestrationService courseOrchestrationService,
        IServicesExceptionsLogger<AuthorOrchestrationService> servicesExceptionsLogger)
    {
        _authorProcessingService = authorProcessingService ?? throw new ArgumentNullException(nameof(authorProcessingService));
        _servicesLogicValidator = servicesLogicValidator ?? throw new ArgumentNullException(nameof(servicesLogicValidator));
        _courseOrchestrationService = courseOrchestrationService ?? throw new ArgumentNullException(nameof(courseOrchestrationService));
        _servicesExceptionsLogger = servicesExceptionsLogger ?? throw new ArgumentNullException(nameof(servicesExceptionsLogger));
    }

    public async Task<Author> CreateAuthorAsync(Author author, CancellationToken cancellationToken) =>
        await _authorProcessingService.CreateAuthorAsync(author, cancellationToken);

    public async Task<Author> ModifyAuthorAsync(Author author, CancellationToken cancellationToken)
    {
        try
        {
            Author storageAuthor = await _authorProcessingService.RetrieveAuthorByIdAsync(author.Id, cancellationToken);
            _servicesLogicValidator.ValidateEntityConcurrency<Author>(author, storageAuthor);

            author.UserId = storageAuthor.UserId;

            return await _authorProcessingService.ModifyAuthorAsync(author, cancellationToken);
        }
        catch (CancellationException) { throw; }
        catch (ResourceParametersException) { throw; }
        catch (ValidationException) { throw; }
        catch (DependencyException<AuthorFoundationService>) { throw; }
        catch (ServiceException<AuthorFoundationService>) { throw; }
        catch (EntityConcurrencyException<Author> entityConcurrencyException)
        {
            throw _servicesExceptionsLogger.CreateAndLogValidationException(entityConcurrencyException);
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
            await _courseOrchestrationService.RemoveCoursesByAuthorIdAsync(authorId, cancellationToken);
            await _authorProcessingService.RemoveAuthorByIdAsync(authorId, cancellationToken);
        }
        catch (CancellationException) { throw; }
        catch (ResourceParametersException) { throw; }
        catch (ValidationException) { throw; }
        catch (DependencyException<AuthorFoundationService>) { throw; }
        catch (ServiceException<AuthorFoundationService>) { throw; }
        catch (Exception exception)
        {
            throw _servicesExceptionsLogger.CreateAndLogServiceException(exception);
        }
    }

    public async ValueTask<Author> RetrieveAuthorByIdAsync(Guid authorId, CancellationToken cancellationToken) =>
        await _authorProcessingService.RetrieveAuthorByIdAsync(authorId, cancellationToken);

    public IEnumerable<Author> RetrieveAllAuthors() =>
        _authorProcessingService.RetrieveAllAuthors();

    public PagedList<Author> SearchAuthors(AuthorResourceParameters authorResourceParameters)
    {
        try
        {
            IQueryable<Author> authors = _authorProcessingService.SearchAuthors(authorResourceParameters);

            return PagedList<Author>.Create(authors, authorResourceParameters.PageNumber, authorResourceParameters.PageSize);
        }
        catch (Exception exception)
        {
            throw _servicesExceptionsLogger.CreateAndLogServiceException(exception);
        }
    }
}