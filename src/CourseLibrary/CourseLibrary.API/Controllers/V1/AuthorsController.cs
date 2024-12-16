using CourseLibrary.API.Contracts.Authors;
using CourseLibrary.API.Filters;
using CourseLibrary.API.Models.Authors;
using CourseLibrary.API.Pagination;
using CourseLibrary.API.Services.V1.Authors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Text.Json;
using Asp.Versioning;

namespace CourseLibrary.API.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ServiceFilter(typeof(EndpointElapsedTimeFilter))]
public class AuthorsController : BaseController<AuthorsController>
{
    private readonly IAuthorOrchestrationService _authorOrchestrationService;

    public AuthorsController(IAuthorOrchestrationService authorOrchestrationService)
    {
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
        Author author = await _authorOrchestrationService.RetrieveAuthorByIdAsync(authorId, cancellationToken);

        return Ok((AuthorDto)author);
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
        Author addedAuthor = await _authorOrchestrationService.CreateAuthorAsync((Author)authorForCreation, cancellationToken);

        return CreatedAtRoute(nameof(GetAuthorAsync), new { authorId = addedAuthor.Id }, (AuthorCreatedDto)addedAuthor);
    }

    [HttpPatch("{authorId}")]
    [ProducesResponseType(typeof(AuthorUpdatedDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> PatchAuthorAsync([FromRoute] Guid authorId, [FromBody] AuthorForUpdate authorForUpdate, [FromServices] IOptions<ApiBehaviorOptions> apiBehaviorOptions, CancellationToken cancellationToken)
    {
        Author author = (Author)authorForUpdate;
        author.Id = authorId;
        Author storageAuthor = await _authorOrchestrationService.ModifyAuthorAsync(author, cancellationToken);

        return Ok((AuthorUpdatedDto)storageAuthor);
    }

    [HttpDelete("{authorId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteAuthorAsync(Guid authorId, [FromServices] IOptions<ApiBehaviorOptions> apiBehaviorOptions, CancellationToken cancellationToken)
    {
        await _authorOrchestrationService.RemoveAuthorByIdAsync(authorId, cancellationToken);

        return NoContent();
    }

    [HttpGet(Name = nameof(SearchAuthorsAsync))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public ActionResult<IEnumerable<AuthorDto>> SearchAuthorsAsync([FromQuery] AuthorResourceParameters authorResourceParameters)
    {
        PagedList<Author> storagePagedAuthors = _authorOrchestrationService.SearchAuthors(authorResourceParameters);

        PaginationMetaData paginationMetaData = new()
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

        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(paginationMetaData));

        List<AuthorDto> authorsDto = storagePagedAuthors.Select(author => (AuthorDto)author).ToList();

        return Ok(authorsDto);
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
}