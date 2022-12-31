using AutoMapper;
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
    private readonly IMapper _mapper;
    private readonly IAuthorOrchestrationService _authorOrchestrationService;

    public AuthorsController(IMapper mapper, IAuthorOrchestrationService authorOrchestrationService)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
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

            return Ok(_mapper.Map<AuthorDto>(author));
        }
        catch (CancellationException) { return NoContent(); }
        catch (ValidationException validationException)
           when (validationException.InnerException is NotFoundEntityException<Author>)
        {
            string innerMessage = GetInnerMessage(validationException);

            return NotFound(innerMessage);
        }
        catch (ValidationException validationException)
        {
            SetModelState(ModelState, validationException);

            return apiBehaviorOptions.Value.InvalidModelStateResponseFactory(ControllerContext);
        }
        catch (DependencyException<AuthorFoundationService> dependencyException)
        {
            return Problem(dependencyException.Message);
        }
        catch (ServiceException<AuthorFoundationService> serviceException)
        {
            return Problem(serviceException.Message);
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(AuthorDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> PostAuthorAsync([FromBody] AuthorForCreation authorForCreation, [FromServices] IOptions<ApiBehaviorOptions> apiBehaviorOptions, CancellationToken cancellationToken)
    {
        try
        {
            Author author = _mapper.Map<Author>(authorForCreation);
            Author addedAuthor = await _authorOrchestrationService.CreateAuthorAsync(author, cancellationToken);

            return CreatedAtRoute(nameof(GetAuthorAsync), new { authorId = addedAuthor.Id }, _mapper.Map<AuthorDto>(addedAuthor));
        }
        catch (CancellationException) { return NoContent(); }
        catch (ValidationException validationException)
           when (validationException.InnerException is NotFoundEntityException<Author>)
        {
            string innerMessage = GetInnerMessage(validationException);

            return NotFound(innerMessage);
        }
        catch (ValidationException validationException)
        {
            SetModelState(ModelState, validationException);

            return apiBehaviorOptions.Value.InvalidModelStateResponseFactory(ControllerContext);
        }
        catch (DependencyException<AuthorFoundationService> dependencyException)
            when (dependencyException.InnerException is DbConflictException)
        {
            return Conflict(dependencyException.Message);
        }
        catch (DependencyException<AuthorFoundationService> dependencyException)
        {
            return Problem(dependencyException.Message);
        }
        catch (ServiceException<AuthorFoundationService> serviceException)
        {
            return Problem(serviceException.Message);
        }
    }

    [HttpPut()]
    [ProducesResponseType(typeof(AuthorDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> PutAuthorAsync([FromBody] AuthorForUpdate authorForUpdate, [FromServices] IOptions<ApiBehaviorOptions> apiBehaviorOptions, CancellationToken cancellationToken)
    {
        try
        {
            Author author = _mapper.Map<Author>(authorForUpdate);
            Author storageAuthor = await _authorOrchestrationService.ModifyAuthorAsync(author, cancellationToken);

            return Ok(_mapper.Map<AuthorDto>(storageAuthor));
        }
        catch (CancellationException) { return NoContent(); }
        catch (ValidationException validationException)
           when (validationException.InnerException is NotFoundEntityException<Author>)
        {
            string innerMessage = GetInnerMessage(validationException);

            return NotFound(innerMessage);
        }
        catch (ValidationException validationException)
        {
            SetModelState(ModelState, validationException);

            return apiBehaviorOptions.Value.InvalidModelStateResponseFactory(ControllerContext);
        }
        catch (DependencyException<AuthorFoundationService> dependencyException)
            when (dependencyException.InnerException is DbConflictException)
        {
            return Conflict(dependencyException.Message);
        }
        catch (DependencyException<AuthorFoundationService> dependencyException)
            when (dependencyException.InnerException is LockedEntityException<Author>)
        {
            string innerMessage = GetInnerMessage(dependencyException);

            return Problem(innerMessage);
        }
        catch (DependencyException<AuthorFoundationService> dependencyException)
        {
            return Problem(dependencyException.Message);
        }
        catch (ServiceException<AuthorFoundationService> serviceException)
        {
            return Problem(serviceException.Message);
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
        catch (CancellationException) { return NoContent(); }
        catch (ValidationException validationException)
           when (validationException.InnerException is NotFoundEntityException<Author>)
        {
            string innerMessage = GetInnerMessage(validationException);

            return NotFound(innerMessage);
        }
        catch (ValidationException validationException)
        {
            SetModelState(ModelState, validationException);

            return apiBehaviorOptions.Value.InvalidModelStateResponseFactory(ControllerContext);
        }
        catch (DependencyException<AuthorFoundationService> dependencyException)
            when (dependencyException.InnerException is DbConflictException)
        {
            return Conflict(dependencyException.Message);
        }
        catch (DependencyException<AuthorFoundationService> dependencyException)
            when (dependencyException.InnerException is LockedEntityException<Author>)
        {
            string innerMessage = GetInnerMessage(dependencyException);

            return Problem(innerMessage);
        }
        catch (DependencyException<AuthorFoundationService> dependencyException)
        {
            return Problem(dependencyException.Message);
        }
        catch (ServiceException<AuthorFoundationService> serviceException)
        {
            return Problem(serviceException.Message);
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

            return Ok(_mapper.Map<IEnumerable<AuthorDto>>(storagePagedAuthors));
        }
        catch (ResourceParametersException resourceParametersException)
        {
            return BadRequest(resourceParametersException.Message);
        }
        catch (DependencyException<AuthorFoundationService> dependencyException)
        {
            return Problem(dependencyException.Message);
        }
        catch (ServiceException<AuthorFoundationService> serviceException)
        {
            return Problem(serviceException.Message);
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
}