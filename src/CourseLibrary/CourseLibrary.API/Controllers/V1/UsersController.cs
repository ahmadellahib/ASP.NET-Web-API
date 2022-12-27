using AutoMapper;
using CourseLibrary.API.Contracts.Users;
using CourseLibrary.API.Filters;
using CourseLibrary.API.Models.Exceptions;
using CourseLibrary.API.Models.Users;
using CourseLibrary.API.Pagination;
using CourseLibrary.API.Services.V1.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace CourseLibrary.API.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ServiceFilter(typeof(EndpointElapsedTimeFilter))]
public class UsersController : BaseController
{
    private readonly IMapper _mapper;
    private readonly IUserOrchestrationService _userOrchestrationService;

    public UsersController(IMapper mapper, IUserOrchestrationService userOrchestrationService)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _userOrchestrationService = userOrchestrationService ?? throw new ArgumentNullException(nameof(userOrchestrationService));
    }

    [HttpGet("{userId}", Name = nameof(GetUserAsync))]
    [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async ValueTask<IActionResult> GetUserAsync([FromRoute] Guid userId, [FromServices] IOptions<ApiBehaviorOptions> apiBehaviorOptions, CancellationToken cancellationToken)
    {
        try
        {
            User user = await _userOrchestrationService.RetrieveUserByIdAsync(userId, cancellationToken);

            return Ok(_mapper.Map<UserDto>(user));
        }
        catch (CancellationException) { return NoContent(); }
        catch (ValidationException validationException)
           when (validationException.InnerException is NotFoundEntityException<User>)
        {
            string innerMessage = GetInnerMessage(validationException);

            return NotFound(innerMessage);
        }
        catch (ValidationException validationException)
        {
            SetModelState(ModelState, validationException);

            return apiBehaviorOptions.Value.InvalidModelStateResponseFactory(ControllerContext);
        }
        catch (DependencyException<UserFoundationService> dependencyException)
        {
            return Problem(dependencyException.Message);
        }
        catch (ServiceException<UserFoundationService> serviceException)
        {
            return Problem(serviceException.Message);
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(User), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> PostUserAsync([FromBody] UserForCreation userForCreation, [FromServices] IOptions<ApiBehaviorOptions> apiBehaviorOptions, CancellationToken cancellationToken)
    {
        try
        {
            User user = _mapper.Map<User>(userForCreation);
            User addedUser = await _userOrchestrationService.CreateUserAsync(user, cancellationToken);

            return CreatedAtRoute(nameof(GetUserAsync), new { userId = addedUser.Id }, _mapper.Map<UserDto>(addedUser));
        }
        catch (CancellationException) { return NoContent(); }
        catch (ValidationException validationException)
           when (validationException.InnerException is NotFoundEntityException<User>)
        {
            string innerMessage = GetInnerMessage(validationException);

            return NotFound(innerMessage);
        }
        catch (ValidationException validationException)
        {
            SetModelState(ModelState, validationException);

            return apiBehaviorOptions.Value.InvalidModelStateResponseFactory(ControllerContext);
        }
        catch (DependencyException<UserFoundationService> dependencyException)
            when (dependencyException.InnerException is DbConflictException)
        {
            return Conflict(dependencyException.Message);
        }
        catch (DependencyException<UserFoundationService> dependencyException)
        {
            return Problem(dependencyException.Message);
        }
        catch (ServiceException<UserFoundationService> serviceException)
        {
            return Problem(serviceException.Message);
        }
    }

    [HttpPut()]
    [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> PutUserAsync([FromBody] UserForUpdate userForUpdate, [FromServices] IOptions<ApiBehaviorOptions> apiBehaviorOptions, CancellationToken cancellationToken)
    {
        try
        {
            User user = _mapper.Map<User>(userForUpdate);
            User storageUser = await _userOrchestrationService.ModifyUserAsync(user, cancellationToken);

            return Ok(_mapper.Map<UserDto>(storageUser));
        }
        catch (CancellationException) { return NoContent(); }
        catch (ValidationException validationException)
           when (validationException.InnerException is NotFoundEntityException<User>)
        {
            string innerMessage = GetInnerMessage(validationException);

            return NotFound(innerMessage);
        }
        catch (ValidationException validationException)
        {
            SetModelState(ModelState, validationException);

            return apiBehaviorOptions.Value.InvalidModelStateResponseFactory(ControllerContext);
        }
        catch (DependencyException<UserFoundationService> dependencyException)
            when (dependencyException.InnerException is DbConflictException)
        {
            return Conflict(dependencyException.Message);
        }
        catch (DependencyException<UserFoundationService> dependencyException)
            when (dependencyException.InnerException is LockedEntityException<User>)
        {
            string innerMessage = GetInnerMessage(dependencyException);

            return Problem(innerMessage);
        }
        catch (DependencyException<UserFoundationService> dependencyException)
        {
            return Problem(dependencyException.Message);
        }
        catch (ServiceException<UserFoundationService> serviceException)
        {
            return Problem(serviceException.Message);
        }
    }

    [HttpGet(Name = nameof(SearchUsersAsync))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public ActionResult<IEnumerable<UserDto>> SearchUsersAsync([FromQuery] UserResourceParameters userResourceParameters)
    {
        try
        {
            PagedList<User> storagePagedUsers = _userOrchestrationService.SearchUsers(userResourceParameters);

            PaginationMetaData PaginationMetaData = new()
            {
                TotalCount = storagePagedUsers.TotalCount,
                PageSize = storagePagedUsers.PageSize,
                CurrentPage = storagePagedUsers.CurrentPage,
                TotalPages = storagePagedUsers.TotalPages,
                HasPrevious = storagePagedUsers.HasPrevious,
                HasNext = storagePagedUsers.HasNext,
                PreviousPageLink = storagePagedUsers.HasPrevious ? CreateUserResourceUri(userResourceParameters, ResourceUriType.PreviousPage) : string.Empty,
                NextPageLink = storagePagedUsers.HasNext ? CreateUserResourceUri(userResourceParameters, ResourceUriType.NextPage) : string.Empty
            };

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(PaginationMetaData));

            return Ok(_mapper.Map<IEnumerable<UserDto>>(storagePagedUsers));
        }
        catch (ResourceParametersException resourceParametersException)
        {
            return BadRequest(resourceParametersException.Message);
        }
        catch (DependencyException<UserFoundationService> dependencyException)
        {
            return Problem(dependencyException.Message);
        }
        catch (ServiceException<UserFoundationService> serviceException)
        {
            return Problem(serviceException.Message);
        }
    }

    private string CreateUserResourceUri(UserResourceParameters userResourceParameters, ResourceUriType type)
    {
        string? resourceUri;

        switch (type)
        {
            case ResourceUriType.PreviousPage:
                resourceUri = Url.Link(nameof(SearchUsersAsync),
                    new
                    {
                        orderBy = userResourceParameters.OrderBy,
                        pageNumber = userResourceParameters.PageNumber - 1,
                        pageSize = userResourceParameters.PageSize,
                        searchQuery = userResourceParameters.SearchQuery
                    });
                break;

            case ResourceUriType.NextPage:
                resourceUri = Url.Link(nameof(SearchUsersAsync),
                    new
                    {
                        orderBy = userResourceParameters.OrderBy,
                        pageNumber = userResourceParameters.PageNumber + 1,
                        pageSize = userResourceParameters.PageSize,
                        searchQuery = userResourceParameters.SearchQuery
                    });
                break;

            default:
                resourceUri = Url.Link(nameof(SearchUsersAsync),
                    new
                    {
                        orderBy = userResourceParameters.OrderBy,
                        pageNumber = userResourceParameters.PageNumber,
                        pageSize = userResourceParameters.PageSize,
                        searchQuery = userResourceParameters.SearchQuery
                    });
                break;
        }

        return resourceUri is null ? string.Empty : resourceUri.Replace("http://", "https://");
    }
}