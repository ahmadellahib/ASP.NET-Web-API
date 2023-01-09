using CourseLibrary.API.Brokers.Loggings;
using CourseLibrary.API.Contracts.Authors;
using CourseLibrary.API.Filters;
using CourseLibrary.API.Models.Authors;
using CourseLibrary.API.Models.Exceptions;
using CourseLibrary.API.Pagination;
using CourseLibrary.API.Services.V1.Authors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace CourseLibrary.API.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ServiceFilter(typeof(EndpointElapsedTimeFilter))]
public class AuthorsController : BaseController
{
    private readonly ILoggingBroker<AuthorsController> _loggingBroker;
    private readonly IAuthorOrchestrationService _authorOrchestrationService;

    public AuthorsController(ILoggingBroker<AuthorsController> loggingBroker, IAuthorOrchestrationService authorOrchestrationService)
    {
        _loggingBroker = loggingBroker ?? throw new ArgumentNullException(nameof(loggingBroker));
        _authorOrchestrationService = authorOrchestrationService ?? throw new ArgumentNullException(nameof(authorOrchestrationService));
    }

    [HttpGet("{authorId}", Name = nameof(GetAuthorAsync))]
    [ProducesResponseType(typeof(AuthorDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async ValueTask<IActionResult> GetAuthorAsync([FromRoute] Guid authorId, [FromServices] IOptions<ApiBehaviorOptions> apiBehaviorOptions, CancellationToken cancellationToken)
    {
        try
        {
            Author author = await _authorOrchestrationService.RetrieveAuthorByIdAsync(authorId, cancellationToken);

            return Ok((AuthorDto)author);
        }
        catch (Exception exception)
        {
            return HandleException(exception, apiBehaviorOptions, ControllerContext);
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(AuthorCreatedDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> PostAuthorAsync([FromBody] AuthorForCreation authorForCreation, [FromServices] IOptions<ApiBehaviorOptions> apiBehaviorOptions, CancellationToken cancellationToken)
    {
        try
        {
            Author addedAuthor = await _authorOrchestrationService.CreateAuthorAsync((Author)authorForCreation, cancellationToken);

            return CreatedAtRoute(nameof(GetAuthorAsync), new { authorId = addedAuthor.Id }, (AuthorCreatedDto)addedAuthor);
        }
        catch (Exception exception)
        {
            return HandleException(exception, apiBehaviorOptions, ControllerContext);
        }
    }

    [HttpPut()]
    [ProducesResponseType(typeof(AuthorUpdatedDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> PutAuthorAsync([FromBody] AuthorForUpdate authorForUpdate, [FromServices] IOptions<ApiBehaviorOptions> apiBehaviorOptions, CancellationToken cancellationToken)
    {
        try
        {
            Author storageAuthor = await _authorOrchestrationService.ModifyAuthorAsync((Author)authorForUpdate, cancellationToken);

            return Ok((AuthorUpdatedDto)storageAuthor);
        }
        catch (Exception exception)
        {
            return HandleException(exception, apiBehaviorOptions, ControllerContext);
        }
    }

    [HttpDelete("{authorId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteAuthorAsync(Guid authorId, [FromServices] IOptions<ApiBehaviorOptions> apiBehaviorOptions, CancellationToken cancellationToken)
    {
        try
        {
            await _authorOrchestrationService.RemoveAuthorByIdAsync(authorId, cancellationToken);

            return NoContent();
        }
        catch (Exception exception)
        {
            return HandleException(exception, apiBehaviorOptions, ControllerContext);
        }
    }

    [HttpGet(Name = nameof(SearchAuthorsAsync))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public ActionResult<IEnumerable<AuthorDto>> SearchAuthorsAsync([FromQuery] AuthorResourceParameters authorResourceParameters)
    {
        try
        {
            PagedList<Author> storagePagedAuthors = _authorOrchestrationService.SearchAuthors(authorResourceParameters);

            PaginationMetaData PaginationMetaData = new()
            {
                TotalCount = storagePagedAuthors.TotalCount,
                PageSize = storagePagedAuthors.PageSize,
                CurrentPage = storagePagedAuthors.CurrentPage,
                TotalPages = storagePagedAuthors.TotalPages,
                HasPrevious = storagePagedAuthors.HasPrevious,
                HasNext = storagePagedAuthors.HasNext,
                PreviousPageLink = storagePagedAuthors.HasPrevious ? CreateAuthorResourceUri(authorResourceParameters, ResourceUriType.PreviousPage) : string.Empty,
                NextPageLink = storagePagedAuthors.HasNext ? CreateAuthorResourceUri(authorResourceParameters, ResourceUriType.NextPage) : string.Empty
            };

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(PaginationMetaData));

            List<AuthorDto> authorsDtos = new();

            foreach (Author author in storagePagedAuthors)
            {
                authorsDtos.Add((AuthorDto)author);
            }

            return Ok(authorsDtos);
        }
        catch (Exception exception)
        {
            return (ActionResult)HandleException(exception);
        }
    }

    private string CreateAuthorResourceUri(AuthorResourceParameters authorResourceParameters, ResourceUriType type)
    {
        string? resourceUri;

        switch (type)
        {
            case ResourceUriType.PreviousPage:
                resourceUri = Url.Link(nameof(SearchAuthorsAsync),
                    new
                    {
                        orderBy = authorResourceParameters.OrderBy,
                        pageNumber = authorResourceParameters.PageNumber - 1,
                        pageSize = authorResourceParameters.PageSize,
                        searchQuery = authorResourceParameters.SearchQuery
                    });
                break;

            case ResourceUriType.NextPage:
                resourceUri = Url.Link(nameof(SearchAuthorsAsync),
                    new
                    {
                        orderBy = authorResourceParameters.OrderBy,
                        pageNumber = authorResourceParameters.PageNumber + 1,
                        pageSize = authorResourceParameters.PageSize,
                        searchQuery = authorResourceParameters.SearchQuery
                    });
                break;

            default:
                resourceUri = Url.Link(nameof(SearchAuthorsAsync),
                    new
                    {
                        orderBy = authorResourceParameters.OrderBy,
                        pageNumber = authorResourceParameters.PageNumber,
                        pageSize = authorResourceParameters.PageSize,
                        searchQuery = authorResourceParameters.SearchQuery
                    });
                break;
        }

        return resourceUri is null ? string.Empty : resourceUri.Replace("http://", "https://");
    }

    private IActionResult HandleException(Exception exception, IOptions<ApiBehaviorOptions>? apiBehaviorOptions = null, ActionContext? actionContext = null)
    {
        switch (exception)
        {
            case ResourceParametersException:
                return BadRequest(exception.Message);
            case CancellationException:
                return NoContent();
            case ValidationException when exception.InnerException is NotFoundEntityException<Author>:
                return NotFound(GetInnerMessage(exception));
            case ValidationException:
                if (apiBehaviorOptions is null || actionContext is null)
                {
                    throw new ArgumentNullException(nameof(apiBehaviorOptions));
                }

                SetModelState(ModelState, (ValidationException)exception);

                return apiBehaviorOptions!.Value.InvalidModelStateResponseFactory(actionContext!);
            case IDependencyException when (exception.InnerException is DbConflictException):
                return Conflict(exception.Message);
            case IDependencyException when (exception.InnerException is LockedEntityException<Author>):
                return Problem(GetInnerMessage(exception));
            case IServiceException:
                return Problem(StaticData.ControllerMessages.InternalServerError);
            default:
                _loggingBroker.LogError(exception);
                return Problem(StaticData.ControllerMessages.InternalServerError);
        }
    }
}